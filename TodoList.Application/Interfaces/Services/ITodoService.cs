using TodoList.Application.Common.Responses;
using TodoList.Application.DTOs.Todos;

namespace TodoList.Application.Interfaces.Services;
// Todo Service hangi işlemleri yapabilmeli bunu yazdık işlemleri yazmadık
public interface ITodoService
{
    Task<PagedResult<TodoResponseDto>> GetAllAsync(    // Filtreleri alacak ve görevleri sayfalı şekilde döndürecek.
        TodoFilterRequestDto filter);

    Task<TodoResponseDto?> GetByIdAsync(int id);      // Id’ye göre tek görevi getirecek

    Task<TodoResponseDto> CreateAsync(
        CreateTodoRequestDto request);     // Yeni görev oluşturacak. Burada kullanıcıdan gelen: CreateTodoRequestDto  alınacak. Service bunu: TodoItem entity’sine çevirecek ve repository ile kaydedecek

    Task<TodoResponseDto?> UpdateAsync(              // Var olan görevi güncelleyecek
        UpdateTodoRequestDto request);

    Task<bool> ChangeStatusAsync(                     // Görevin tamamlanma durumunu değiştirecek.
        int id,
        ChangeTodoStatusRequestDto request);

    Task<bool> DeleteAsync(int id);   // Görevi silecek.
}