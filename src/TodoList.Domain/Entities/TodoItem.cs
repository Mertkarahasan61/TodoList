using TodoList.Domain.Common;
using TodoList.Domain.Enums;

namespace TodoList.Domain.Entities;

public class TodoItem : BaseEntity   // Id CreatedAt UpdatedAt  IsDeleted de dahil edildi 
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TodoPriority Priority { get; set; }  //todopriority enumdu sadece  TodoPriority.Low gibi değerleri alır  Priority = TodoPriority.High;

    public DateTime? DueDate { get; set; }   // bitis tarihi  ? oldugu için DueDate = null; olabilir 

    public bool IsCompleted { get; set; }  // tamamlandı mı diye bakar 

    public DateTime? CompletedAt { get; set; }  // tamamlanma tarihi 

    public int? CategoryId { get; set; }  // görev kategorisizde oluşturulabilir o yüzdeen ? var

    public Category? Category { get; set; }  // category.cs nin tamamını kullanmamızı sağlar  todo.Category.Name gibi 
}
