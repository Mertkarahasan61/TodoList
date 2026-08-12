using TodoList.Application.DTOs.Todos;
using TodoList.Domain.Entities;

namespace TodoList.Application.Interfaces.Repositories;
// Bu dosya sadece kuralları belirliyor

public interface ITodoRepository
    : IGenericRepository<TodoItem>                                               /*  Id ile getir
Hepsini getir
Ekle
Güncelle
Kayıt var mı kontrol et
Kaydet    bunları ekledik  */  //  kategoriyi de sorguya dahil eden ayrı bir metot kullanacağız:
{
    Task<TodoItem?> GetByIdWithCategoryAsync(int id);    

    Task<List<TodoItem>> GetFilteredAsync(
        TodoFilterRequestDto filter);

    Task<int> CountFilteredAsync(
        TodoFilterRequestDto filter);
}