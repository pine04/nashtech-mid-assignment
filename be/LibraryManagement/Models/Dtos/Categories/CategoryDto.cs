using LibraryManagement.Models.Entities;

namespace LibraryManagement.Models.Dtos.Categories
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int BookCount { get; set; }
    }

    public static class CategoryExtensions
    {
        public static CategoryDto ToCategoryDto(this Category category)
        {
            if (category.Books == null)
            {
                throw new InvalidOperationException("Books must be included when building CategoryDto.");
            }

            return new CategoryDto()
            {
                Id = category.Id,
                Name = category.Name,
                BookCount = category.Books.Count
            };
        }
    }
}