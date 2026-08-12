using TodoList.Domain.Enums;     // Kullanıcı var olan bir görevi düzenlemek istediğinde UpdateTodoRequestDto nesnesine çevirecek.
                                 // Doğrudan TodoItem gönderseydik kullanıcı diğer sistem alanlarınıda düzenleyeme çalışırdı IsDeleted gibi
                                 // Kullanıcının hangi alanları gönderebileceğini daha Controller’a girerken sınırlandırırız ve yanlışlık yapma riskini azaltırız.
namespace TodoList.Application.DTOs.Todos;   // burada kullanıcıdan veri alcaz

public class UpdateTodoRequestDto
{
    public int Id { get; set; }   //  Hangi kayıt güncellenecek bilinmeli, bu yüzden Id var. create de daha kayıt olmadıgı için ıd yok

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TodoPriority Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public int? CategoryId { get; set; }
}