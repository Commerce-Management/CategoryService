using CategoryService.Core.Interfaces;
using CategoryService.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CategoryService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{

    [HttpGet]
    // [Authorize(Roles = "AppAdmin")]
    public async Task<ActionResult<IEnumerable<GetCategoryDto>>> GetCategories()
    {
        try
        {
            var result = await categoryService.GetAllCategories();
            if (result.Any()) return Ok(result);

            return NotFound("Categories not found.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Internal server error occurred");
            return StatusCode(500, new { Error = "An unexpected error occurred. Please try again later.", Details = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetCategoryDto>> GetCategoryById(Guid id)
    {
        try
        {
            var category = await categoryService.GetCategoryById(id);

            if (category is null)
                return NotFound($"Category with ID: {id} not found.");

            return Ok(category);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Internal server error occurred");
            return StatusCode(500, new { Error = "An unexpected error occurred. Please try again later.", Details = ex.Message });
        }
    }

    [HttpGet("name/{name}")]
    public async Task<ActionResult<GetCategoryDto>> GetCategoryByName(string name)
    {
        try
        {
            var category = await categoryService.GetCategoryByName(name);

            if (category is null)
                return NotFound($"Category with name: {name} not found.");

            return Ok(category);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Internal server error occurred");
            return StatusCode(500, new { Error = "An unexpected error occurred. Please try again later.", Details = ex.Message });
        }
    }

    [HttpPost]
    // [Authorize(Roles = "AppAdmin")]
    public async Task<ActionResult<GetCategoryDto>> CreateCategory([FromBody] CreateCategoryDto categoryDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { Error = "Model not valid" });
        
        try
        {
            var result = await categoryService.CreateCategory(categoryDto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while creating order.");
            var innerExceptionMessage = ex.InnerException?.Message ?? "No inner exception";
            return StatusCode(500, new
            {
                Error = "An unexpected error occurred. Please try again later.",
                Details = ex.Message,  
                InnerException = innerExceptionMessage,
                StackTrace = ex.StackTrace 
            });
        }
    }

    [HttpPut("{id:guid}")]
    // [Authorize(Roles = "AppAdmin")]
    public async Task<ActionResult> UpdateCategory(Guid id, CreateCategoryDto categoryDto)
    {
        if (!ModelState.IsValid)
            return BadRequest("Model not valid");

        var result = await categoryService.UpdateCategory(id, categoryDto);
        if (result) return Ok();

        return BadRequest($"Category with ID: {id} could not be updated.");
    }

    [HttpDelete("{id:guid}")]
    // [Authorize(Roles = "AppAdmin")]
    public async Task<ActionResult> DeleteCategory(Guid id)
    {
        if (!ModelState.IsValid)
            return BadRequest("Model not valid");
        
        var result = await categoryService.DeleteCategory(id);
        if (result) return Ok();
        
        return BadRequest($"Category with ID: {id} could not be deleted.");
    }
}