using TodoList.Domain.Enums;     // Backend görev bilgisini kullanıcıya gönderir.

namespace TodoList.Application.DTOs.Todos;

public class TodoResponseDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TodoPriority Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int? CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? CategoryColor { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}