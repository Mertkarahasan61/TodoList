using TodoList.Application.DTOs.Categories;
using TodoList.Application.Interfaces.Repositories;
using TodoList.Application.Interfaces.Services;
using TodoList.Domain.Entities;

namespace TodoList.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository; // CategoryService'in veritabanıyla direkt konuşmasını istemiyoruz.

    public CategoryService(         // dışarıdan gelen repository'yi sınıfın içine kaydediyor.
        ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<CategoryResponseDto>> GetAllAsync()   
    {
        var categories =
            await _categoryRepository.GetAllAsync();  // Bütün kategorileri getir.diyoruz repositoriye

        var result = new List<CategoryResponseDto>();  // boş DTO listesi oluşturuyoruz.

        foreach (var category in categories)    // kategorileri tek tek dolaşıyoruz.
        {
            result.Add(new CategoryResponseDto  // ile DTO'ya çeviriyoruz.
            {
                Id = category.Id,
                Name = category.Name,
                Color = category.Color,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            });
        }

        return result;  // döndürüyoruz
    }

    public async Task<CategoryResponseDto> CreateAsync(   // Kullanıcıdan:CreateCategoryRequestDto geliyor 
        CreateCategoryRequestDto request)
    {
        var category = new Category        // ile gerçek entity oluşturuyoruz
        {
            Name = request.Name,
            Color = request.Color,
            CreatedAt = DateTime.UtcNow
        };

        await _categoryRepository.AddAsync(category);  // eklenmek üzere repository'ye ver.

        await _categoryRepository.SaveChangesAsync();  // → SQL Server'a gerçekten kaydet

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Color = category.Color,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    public async Task<CategoryResponseDto?> UpdateAsync(
        UpdateCategoryRequestDto request)
    {
        var category =
            await _categoryRepository.GetByIdAsync(request.Id);   // güncelleyeceğimiz mevcut kategoriyi buluyoruz.

        if (category is null)  
        {
            return null;
        }

        category.Name = request.Name;     // Sonra asıl güncelleme:
        category.Color = request.Color;
        category.UpdatedAt = DateTime.UtcNow;

        _categoryRepository.Update(category);

        await _categoryRepository.SaveChangesAsync();

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Color = category.Color,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category =
            await _categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            return false;
        }

        category.IsDeleted = true;
        category.UpdatedAt = DateTime.UtcNow;

        _categoryRepository.Update(category);

        await _categoryRepository.SaveChangesAsync();

        return true;
    }
}