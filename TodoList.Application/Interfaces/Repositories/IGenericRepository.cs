using TodoList.Domain.Common;
// Repository’nin hangi işlemleri yapabilmesi gerektiğini belirliyoruz.
namespace TodoList.Application.Interfaces.Repositories;

public interface IGenericRepository<T>   // T yerine herhangi bir tür gelemez. T, mutlaka BaseEntity veya BaseEntityden türemiş bir sınıf olmalıdır.
    where T : BaseEntity                  // public class TodoItem : BaseEntity
                                          // public class Category : BaseEntity bunlara örnek where ve sonrası olmasaydı her şey alınabilirdi
{                                 // Bu bekleme sırasında uygulamanın diğer işleri gereksiz yere engellenmesin diye asenkron yapı kullanıyoru
    Task<T?> GetByIdAsync(int id);  // Verilen id değerine sahip tek bir kaydı veritabanından getirir.

    Task<List<T>> GetAllAsync();   // Veritabanındaki bütün aktif kayıtları liste olarak getirir.

    Task AddAsync(T entity);  // Bu metot çoğunlukla kaydı o anda SQL Server’a yazmaz. Sadece Entity Framework’e: Bu nesne eklenecek. bilgisini verir.

    void Update(T entity);   // Bir entity’nin değiştirildiğini Entity Framework’e bildirir.

    Task<bool> ExistsAsync(int id);  // Verilen Id’ye sahip bir kayıt olup olmadığını kontrol eder.

    Task<int> SaveChangesAsync();  // Entity Framework’te hazırlanan ekleme veya güncelleme işlemlerini gerçekten SQL Server’a uygular
}