using TodoList.Domain.Enums;        // kullanıcının göndermesine izin verilen alanlar burası
                                    //  Kullanıcı yeni görev oluştururken backend’e gönderir.

namespace TodoList.Application.DTOs.Todos;

public class CreateTodoRequestDto
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TodoPriority Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public int? CategoryId { get; set; }
}