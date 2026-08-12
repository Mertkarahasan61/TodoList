namespace TodoList.Application.DTOs.Categories;
// Mevcut bir kategorinin adını veya rengini değiştirmek için kullanılacak.
public class UpdateCategoryRequestDto
{
    public int Id { get; set; }   // Var olan kategoriyi değiştirir. ıd vardır o yuzden

    public string Name { get; set; } = string.Empty;

    public string? Color { get; set; }
}