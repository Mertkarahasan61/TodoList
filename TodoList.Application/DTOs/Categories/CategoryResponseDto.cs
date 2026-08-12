namespace TodoList.Application.DTOs.Categories;
// Kategori bilgisini frontend’e gönderir  yani kullanıcıya 
public class CategoryResponseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Color { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}