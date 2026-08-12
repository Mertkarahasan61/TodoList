using Microsoft.EntityFrameworkCore;
using TodoList.Application.Interfaces.Repositories;
using TodoList.Domain.Common;
using TodoList.Infrastructure.Persistence;
// Nasıl yapılacağını gösteriyor.
namespace TodoList.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T>
    where T : BaseEntity
{
    private readonly TodoDbContext _context;   // TodoDbContext, uygulamamız ile SQL Server arasındaki bağlantıyı yönetiyor. veri okuma ekleme vb
    private readonly DbSet<T> _dbSet;        // DbSet<T>, hangi tabloyla çalıştığımızı temsil eder.

    public GenericRepository(TodoDbContext context)  
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id)   // Verilen id numarasına sahip ilk kaydı getirir.
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();   // Tablodaki kayıtları SQL Server’dan getirir ve C# tarafında bir listeye dönüştürür
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);   // Bu kategori veritabanına eklenecek. diye kayıt tutar 
    }
                                   
    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public async Task<bool> ExistsAsync(int id)
    {                                                           // Verilen Id’ye sahip en az bir kayıt olup olmadığını kontrol eder.
        return await _dbSet.AnyAsync(x => x.Id == id);   // Bu şartı sağlayan herhangi bir kayıt var mı?
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}// Task’ın tamamlanmasını bekleyip içindeki gerçek sonucu çıkarır. Beklerken uygulamanın tamamını gereksiz yere kilitlemez.