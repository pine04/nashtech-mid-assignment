using LibraryManagement.Data;
using LibraryManagement.Exceptions;
using LibraryManagement.Models.Dtos.Auth;
using LibraryManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services.Auth
{
    public class AuthService : IAuthService
    {
        private ApplicationDbContext _dbContext;
        private IPasswordHasher _passwordHasher;
        private IJwtIssuer _jwtIssuer;

        public AuthService(ApplicationDbContext dbContext, IPasswordHasher passwordHasher, IJwtIssuer jwtIssuer)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _jwtIssuer = jwtIssuer;
        }

        public async Task<UserDto> GetMyProfileAsync(int userId, CancellationToken cancellationToken)
        {
            User? user = await _dbContext.Users.FindAsync(userId, cancellationToken);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            return user.ToUserDto();
        }

        public async Task<AuthDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // Check if the email is already used.
                bool emailExists = await _dbContext.Users.AnyAsync(u => u.Email == registerDto.Email, cancellationToken);
                if (emailExists)
                {
                    throw new EmailAlreadyUsedException();
                }

                // Create a new user and save them to the DB.
                User user = new User()
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Email = registerDto.Email,
                    Password = _passwordHasher.Hash(registerDto.Password),
                    Role = registerDto.Role
                };

                await _dbContext.Users.AddAsync(user, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                // Create a new pair of access-refresh tokens.
                string accessToken = _jwtIssuer.IssueAccessToken(user);
                string refreshToken = _jwtIssuer.IssueRefreshToken(user);

                // Add these tokens to the DB table.
                await _dbContext.Tokens.AddRangeAsync(
                    new Token()
                    {
                        UserId = user.Id,
                        TokenValue = accessToken,
                        TokenType = TokenType.Access,
                        Expires = DateTime.UtcNow.AddMinutes(_jwtIssuer.AccessTokenExpirationTime)
                    },
                    new Token()
                    {
                        UserId = user.Id,
                        TokenValue = refreshToken,
                        TokenType = TokenType.Refresh,
                        Expires = DateTime.UtcNow.AddMinutes(_jwtIssuer.RefreshTokenExpirationTime)
                    }
                );

                // Save changes to DB.
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                // Return the tokens.
                return new AuthDto()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }

        }

        public async Task<AuthDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email, cancellationToken);
            if (user == null)
            {
                throw new UnauthorizedException("User does not exist.");
            }

            bool passwordMatches = _passwordHasher.Verify(loginDto.Password, user.Password);
            if (!passwordMatches)
            {
                throw new UnauthorizedException("Incorrect password.");
            }

            // Remove all existing access/refresh tokens associated with this user.
            await _dbContext.Tokens.Where(t => t.UserId == user.Id).ExecuteDeleteAsync(cancellationToken);

            // Create a new pair of access-refresh tokens.
            string accessToken = _jwtIssuer.IssueAccessToken(user);
            string refreshToken = _jwtIssuer.IssueRefreshToken(user);

            // Add these tokens to the DB table.
            await _dbContext.Tokens.AddRangeAsync(
                new Token()
                {
                    UserId = user.Id,
                    TokenValue = accessToken,
                    TokenType = TokenType.Access,
                    Expires = DateTime.UtcNow.AddMinutes(_jwtIssuer.AccessTokenExpirationTime)
                },
                new Token()
                {
                    UserId = user.Id,
                    TokenValue = refreshToken,
                    TokenType = TokenType.Refresh,
                    Expires = DateTime.UtcNow.AddMinutes(_jwtIssuer.RefreshTokenExpirationTime)
                }
            );
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Return the tokens.
            return new AuthDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }

        public async Task LogoutAsync(int userId, CancellationToken cancellationToken = default)
        {
            // Remove all existing access/refresh tokens associated with this user.
            await _dbContext.Tokens.Where(t => t.UserId == userId).ExecuteDeleteAsync(cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<AuthDto> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            // Check if the provided refresh token is valid.
            Token? token = await _dbContext.Tokens
                .FirstOrDefaultAsync(t => t.TokenValue == refreshToken && t.Expires > DateTime.UtcNow && t.TokenType == TokenType.Refresh, cancellationToken);

            if (token == null)
            {
                throw new UnauthorizedException("Refresh token invalid.");
            }

            // Remove all existing access/refresh tokens associated with this user.
            await _dbContext.Tokens.Where(t => t.UserId == token.UserId).ExecuteDeleteAsync(cancellationToken);

            // Get this user.
            User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == token.UserId, cancellationToken);

            if (user == null)
            {
                throw new NonExistentUserException();
            }

            // Create a new pair of access-refresh tokens.
            string newAccessToken = _jwtIssuer.IssueAccessToken(user);
            string newRefreshToken = _jwtIssuer.IssueRefreshToken(user);

            // Add these tokens to the DB table.
            await _dbContext.Tokens.AddRangeAsync(
                new Token()
                {
                    UserId = user.Id,
                    TokenValue = newAccessToken,
                    TokenType = TokenType.Access,
                    Expires = DateTime.UtcNow.AddMinutes(_jwtIssuer.AccessTokenExpirationTime)
                },
                new Token()
                {
                    UserId = user.Id,
                    TokenValue = newRefreshToken,
                    TokenType = TokenType.Refresh,
                    Expires = DateTime.UtcNow.AddMinutes(_jwtIssuer.RefreshTokenExpirationTime)
                }
            );
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Return the tokens.
            return new AuthDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };
        }
    }
}