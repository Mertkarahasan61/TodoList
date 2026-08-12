using TodoList.Application.DTOs.Categories;

namespace TodoList.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<List<CategoryResponseDto>> GetAllAsync();  // kategorileri listele.

    Task<CategoryResponseDto> CreateAsync(     // kullanıcının yazdıgı kategori oluştur.
        CreateCategoryRequestDto request);

    Task<CategoryResponseDto?> UpdateAsync(   // kullanıcının yazdıgı kategori güncelle.
        UpdateCategoryRequestDto request);

    Task<bool> DeleteAsync(int id);   // kategori sil.
}