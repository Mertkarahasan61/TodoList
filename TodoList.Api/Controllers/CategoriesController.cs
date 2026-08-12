using Microsoft.AspNetCore.Mvc;
using TodoList.Application.Common.Responses;
using TodoList.Application.DTOs.Categories;
using TodoList.Application.Interfaces.Services;
using FluentValidation;
namespace TodoList.Api.Controllers;

[ApiController]  
[Route("api/categories")]  // Bu Controller'ın ana adresi:/api/categories
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IValidator<CreateCategoryRequestDto> _createCategoryValidator;
    private readonly IValidator<UpdateCategoryRequestDto> _updateCategoryValidator;

    public CategoriesController(ICategoryService categoryService,
    IValidator<CreateCategoryRequestDto> createCategoryValidator,
    IValidator<UpdateCategoryRequestDto> updateCategoryValidator)
    {
        _categoryService = categoryService;
        _createCategoryValidator = createCategoryValidator;
        _updateCategoryValidator = updateCategoryValidator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CategoryResponseDto>>>> GetAll()
    {
        var result = await _categoryService.GetAllAsync();

        return Ok(new ApiResponse<List<CategoryResponseDto>>
        {
            Success = true,
            Data = result
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> Create(
        [FromBody] CreateCategoryRequestDto request)
    {
        var validationResult =
    await _createCategoryValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(new ApiResponse<CategoryResponseDto>
            {
                Success = false,
                Message = "Gönderilen bilgiler geçersiz.",
                Errors = validationResult.Errors
                    .Select(x => x.ErrorMessage)
            });
        }

        var result = await _categoryService.CreateAsync(request);

        return Ok(new ApiResponse<CategoryResponseDto>
        {
            Success = true,
            Message = "Kategori başarıyla oluşturuldu.",
            Data = result
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> Update(
    int id,
    [FromBody] UpdateCategoryRequestDto request)
    {
        request.Id = id;   // URL'deki Id ile body'deki yeni bilgileri tek request nesnesinde birleştirmek için
        var validationResult =
    await _updateCategoryValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(new ApiResponse<CategoryResponseDto>
            {
                Success = false,
                Message = "Gönderilen bilgiler geçersiz.",
                Errors = validationResult.Errors
                    .Select(x => x.ErrorMessage)
            });
        }

        var result =
            await _categoryService.UpdateAsync(request);

        if (result is null)
        {
            return NotFound(new ApiResponse<CategoryResponseDto>
            {
                Success = false,
                Message = "Kategori bulunamadı."
            });
        }

        return Ok(new ApiResponse<CategoryResponseDto>
        {
            Success = true,
            Message = "Kategori başarıyla güncellendi.",
            Data = result
        });
    }
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)  // ApiResponse<T> generic bir sınıf ve T yerine bir tip yazmak zorundayız.o yüzden object
    {
        bool success =
            await _categoryService.DeleteAsync(id);

        if (!success)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Kategori bulunamadı."
            });
        }

        return Ok(new ApiResponse<object>
        {   
            Success = true,
            Message = "Kategori başarıyla silindi."
        });
    }
}
