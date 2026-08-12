using TodoList.Domain.Common;

namespace TodoList.Domain.Entities;

public class Category : BaseEntity  // kalıtım yaptı miras aldı baseentitynin özelliklerini 
{
    public string Name { get; set; } = string.Empty;   // string.empty " " anlamına gelir  null olmasını engelliyo

    public string? Color { get; set; }   // ? null olabilir 

    public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>(); 
    // ICollection birden fazla eleman tutar todoitemleri tutuyo add remove count yapmamızı sağlar 
    // new List<TodoItem>(); boş liste oluşturur
}