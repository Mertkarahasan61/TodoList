using TodoList.Domain.Enums;

namespace TodoList.Application.DTOs.Todos;

public class TodoFilterRequestDto
{
    public string? Search { get; set; }  // Görev başlığında veya açıklamasında aranacak metni tutar

    public string? SearchField { get; set; } = "title";  // Aramanın hangi alanda yapılacağını belirtir: title = sadece başlık, description = sadece açıklama

    public string? Status { get; set; }  // Görevin tamamlanma durumuna göre filtreleme yapmak için kullanılacak.

    public TodoPriority? Priority { get; set; }

    public int? CategoryId { get; set; }

    public string SortBy { get; set; } = "createdAt";   // Görevlerin hangi alana göre sıralanacağını belirtir varsayılan createdata göre 

    public string SortDirection { get; set; } = "desc";   // Sıralamanın yönünü belirtir.

    public int PageNumber { get; set; } = 1;   // Kaçıncı sayfanın getirileceğini belirtir. varsayılan 1

    public int PageSize { get; set; } = 10;   // Bir sayfada kaç görev gösterileceğini belirtir. 10 görev 
}