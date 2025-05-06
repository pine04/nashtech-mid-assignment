using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using LibraryManagement.Models.Dtos;
using LibraryManagement.Models.Dtos.Categories;
using LibraryManagement.Services.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("/api/categories")]
    [Produces(MediaTypeNames.Application.Json)]
    public class CategoriesController : ControllerBase
    {
        private ICategoriesService _categoriesService;

        public CategoriesController(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<CategoryDto>>> GetCategoriesAsync(
            [Range(1, int.MaxValue)] int pageNumber = 1,
            [Range(1, 100)] int pageSize = 10,
            string searchQuery = "",
            CancellationToken cancellationToken = default)
        {
            PagedResult<CategoryDto> result = await _categoriesService.GetCategoriesAsync(pageNumber, pageSize, searchQuery, cancellationToken);
            return result;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDto>> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            CategoryDto categoryDto = await _categoriesService.GetCategoryByIdAsync(id, cancellationToken);
            return categoryDto;
        }

        [HttpPost]
        [Authorize("SuperUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryDto>> CreateCategoryAsync(CreateCategoryDto createCategoryDto, CancellationToken cancellationToken = default)
        {
            CategoryDto categoryDto = await _categoriesService.CreateCategoryAsync(createCategoryDto, cancellationToken);
            return CreatedAtAction(nameof(GetCategoryByIdAsync), new { id = categoryDto.Id }, categoryDto);
        }

        [HttpPut("{id}")]
        [Authorize("SuperUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto, CancellationToken cancellationToken = default)
        {
            return await _categoriesService.UpdateCategoryAsync(id, updateCategoryDto, cancellationToken);
        }

        [HttpDelete("{id}")]
        [Authorize("SuperUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
        {
            await _categoriesService.DeleteCategoryAsync(id, cancellationToken);
            return NoContent();
        }
    }
}