using LibraryManagement.Models.Dtos;
using LibraryManagement.Models.Dtos.Categories;

namespace LibraryManagement.Services.Categories
{
    public interface ICategoriesService
    {
        public Task<PagedResult<CategoryDto>> GetCategoriesAsync(int pageNumber, int pageSize, string searchQuery, CancellationToken cancellationToken);
        public Task<CategoryDto> GetCategoryByIdAsync(int id, CancellationToken cancellationToken);
        public Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto, CancellationToken cancellationToken);
        public Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto, CancellationToken cancellationToken);
        public Task DeleteCategoryAsync(int id, CancellationToken cancellationToken);
    }
}