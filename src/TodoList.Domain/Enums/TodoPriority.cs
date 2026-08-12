namespace TodoList.Domain.Enums;  

public enum TodoPriority  // Bir alan yalnızca önceden belirlenmiş birkaç seçenekten birini alabilecekse enum kullanılır farklı bir sey alamaz
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}   // enumlar miras alınamaz