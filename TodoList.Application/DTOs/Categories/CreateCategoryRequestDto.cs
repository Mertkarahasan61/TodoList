namespace TodoList.Application.DTOs.Categories;
// kullanıcıdan alacağımız kategori bilgileri yeni kategori oluşturulurken
public class CreateCategoryRequestDto
{    // Yeni kategori henüz veritabanında bulunmadığı için bir Id değeri yoktur.
    public string Name { get; set; } = string.Empty;

    public string? Color { get; set; }
}