namespace TodoList.Domain.Common;

public abstract class BaseEntity  // abstract: bu sınıftan dogrudan nesne olusturmayı engeller var entity = new BaseEntity(); yapamayız
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }
}