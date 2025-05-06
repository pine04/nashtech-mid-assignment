using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LibraryManagement.Data;
using LibraryManagement.Middlewares;
using LibraryManagement.Models.Entities;
using LibraryManagement.Services.Auth;
using LibraryManagement.Services.Books;
using LibraryManagement.Services.Categories;
using LibraryManagement.Services.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace LibraryManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IJwtIssuer, JwtIssuer>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ICategoriesService, CategoriesService>();
            builder.Services.AddScoped<IBooksService, BooksService>();
            builder.Services.AddScoped<IRequestsService, RequestsService>();

            builder.Services.AddTransient<IApplicationSeeder, ApplicationSeeder>();
            builder.Services.AddScoped<ApplicationDbContext>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var seeder = provider.GetRequiredService<IApplicationSeeder>();

                return new ApplicationDbContext(configuration, seeder);
            });

            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<ApplicationExceptionHandler>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("MIICWwIBAAKBgHZO8IQouqjDyY47ZDGdw9jPDVHadgfT1kP3igz5xamdVaYPHaN24UZMeSXjW9sWZzwFVbhOAGrjR0MM6APrlvv5mpy67S/K4q4D7Dvf6QySKFzwMZ99Qk10fK8tLoUlHG3qfk9+85LhL/Rnmd9FD7nz8+cYXFmz5LIaLEQATdyNAgMBAAECgYA9ng2Md34IKbiPGIWthcKb5/LC/+nbV8xPp9xBt9Dn7ybNjy/blC3uJCQwxIJxz/BChXDIxe9XvDnARTeN2yTOKrV6mUfI+VmON5gTD5hMGtWmxEsmTfu3JL0LjDe8Rfdu46w5qjX5jyDwU0ygJPqXJPRmHOQW0WN8oLIaDBxIQQJBAN66qMS2GtcgTqECjnZuuP+qrTKL4JzG+yLLNoyWJbMlF0/HatsmrFq/CkYwA806OTmCkUSm9x6mpX1wHKi4jbECQQCH+yVb67gdghmoNhc5vLgnm/efNnhUh7u07OCL3tE9EBbxZFRs17HftfEcfmtOtoyTBpf9jrOvaGjYxmxXWSedAkByZrHVCCxVHxUEAoomLsz7FTGM6ufd3x6TSomkQGLw1zZYFfe+xOh2W/XtAzCQsz09WuE+v/viVHpgKbuutcyhAkB8o8hXnBVz/rdTxti9FG1b6QstBXmASbXVHbaonkD+DoxpEMSNy5t/6b4qlvn2+T6a2VVhlXbAFhzcbewKmG7FAkEAs8z4Y1uI0Bf6ge4foXZ/2B9/pJpODnp2cbQjHomnXM861B/C+jPW3TJJN2cfbAxhCQT2NhzewaqoYzy7dpYsIQ==")),
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async (context) =>
                        {
                            JsonWebToken? token = context.SecurityToken as JsonWebToken;
                            string? jwt = token?.EncodedToken;

                            if (jwt == null)
                            {
                                context.Fail("Token is missing.");
                                return;
                            }

                            var db = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
                            var tokenInDb = await db.Tokens
                                .FirstOrDefaultAsync(t => t.TokenValue == jwt && t.Expires > DateTime.UtcNow && t.TokenType == TokenType.Access);

                            if (tokenInDb == null)
                            {
                                context.Fail("Token is invalid.");
                            }
                        }
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("NormalUser", p => p.RequireClaim(ClaimTypes.Role, "NormalUser"));
                options.AddPolicy("SuperUser", p => p.RequireClaim(ClaimTypes.Role, "SuperUser"));
            });

            builder.Services.AddControllers();

            builder.Services.AddMvc(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    name: "AllowSpecificOrigin",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:5173")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    }
                );
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowSpecificOrigin");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseExceptionHandler();

            app.MapControllers();

            app.Run();
        }
    }
}
