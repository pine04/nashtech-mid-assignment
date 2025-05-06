using LibraryManagement.Data;
using LibraryManagement.Exceptions;
using LibraryManagement.Models.Dtos;
using LibraryManagement.Models.Dtos.Categories;
using LibraryManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services.Categories
{
    public class CategoriesService : ICategoriesService
    {
        private ApplicationDbContext _dbContext;

        public CategoriesService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<CategoryDto>> GetCategoriesAsync(int pageNumber = 1, int pageSize = 10, string searchQuery = "", CancellationToken cancellationToken = default)
        {
            int skip = (pageNumber - 1) * pageSize;

            IQueryable<Category> query = _dbContext.Categories
                .Include(category => category.Books);

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(c => EF.Functions.Like(c.Name, $"%{searchQuery}%"));
            }

            List<CategoryDto> categories = await query
                .Skip(skip)
                .Take(pageSize)
                .Select(category => category.ToCategoryDto())
                .ToListAsync(cancellationToken);

            int totalRecordCount = await query.CountAsync(cancellationToken);

            return new PagedResult<CategoryDto>()
            {
                Results = categories,
                TotalRecordCount = totalRecordCount
            };
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Category? category = await _dbContext.Categories.Include(category => category.Books).FirstOrDefaultAsync(category => category.Id == id, cancellationToken);

            if (category == null)
            {
                throw new NotFoundException("Category not found.");
            }

            return category.ToCategoryDto();
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto, CancellationToken cancellationToken = default)
        {
            Category category = new Category()
            {
                Name = createCategoryDto.Name
            };

            await _dbContext.Categories.AddAsync(category, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _dbContext.Entry(category).Collection(c => c.Books).LoadAsync(cancellationToken);

            return category.ToCategoryDto();
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto, CancellationToken cancellationToken = default)
        {
            Category? category = await _dbContext.Categories.FindAsync(id, cancellationToken);

            if (category == null)
            {
                throw new NotFoundException("Category not found.");
            }

            category.Name = updateCategoryDto.Name;
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _dbContext.Entry(category).Collection(c => c.Books).LoadAsync(cancellationToken);

            return category.ToCategoryDto();
        }

        public async Task DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
        {
            Category? category = await _dbContext.Categories.FindAsync(id, cancellationToken);

            if (category == null)
            {
                throw new NotFoundException("Category not found.");
            }

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}