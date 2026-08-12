using Microsoft.EntityFrameworkCore;
using TodoList.Application.DTOs.Todos;
using TodoList.Application.Interfaces.Repositories;
using TodoList.Domain.Entities;
using TodoList.Infrastructure.Persistence;

namespace TodoList.Infrastructure.Repositories;

public class TodoRepository            // Id ile getir Hepsini getir Ekle  Güncelle  Kayıt var mı kontrol et  Kaydet  Görevi kategorisiyle getir Filtrelenmiş görevleri getir  Filtreye uyan toplam görev sayısını bul
    : GenericRepository<TodoItem>, ITodoRepository
{
    private readonly TodoDbContext _context;  // private oldugu için bunu sadece todorepository kullanabilir  // _context, veritabanına ulaşmamızı sağlayan değişkendir.

    public TodoRepository(TodoDbContext context)   // base(context): aynı context’i miras aldığımız GenericRepository sınıfının constructor’ına gönderir. bu metotları miras alıyor ama üst sınıfın constructor’ı yine kendi alanlarını hazırlamak zorunda.
        : base(context)
    {                                                                                        // TodoRepository, GenericRepository<TodoItem> sınıfından miras alıyordu. GenericRepository’nin constructor’ı da context istiyor:
        _context = context;   // Dışarıdan gelen context’i sınıfın kendi değişkenine kaydediyoruz.
    }

    public async Task<TodoItem?> GetByIdWithCategoryAsync(int id)
    {
        return await _context.TodoItems  // TodoItems tablosunda sorgu başlatır.
            .AsNoTracking()  // Bu kayıt üzerinde şu anda değişiklik yapmayacağımızı Entity Framework’e bildirir.
            .Include(x => x.Category)   // Görevin bağlı olduğu kategori bilgisini de sorguya ekler
            .FirstOrDefaultAsync(x => x.Id == id);  // Gönderilen id değerine sahip görevi getirir. Bulamazsa null döndürür.
    }

    public async Task<List<TodoItem>> GetFilteredAsync(  // Bu metot bir TodoFilterRequestDto alır ve filtrelere uyan görevleri liste olarak döndürür filterde Arama = "staj"  Durum = active   Kategori = 2  seyler bulunur todofilterrequestdtodan gelir 
        TodoFilterRequestDto filter)
    {
        IQueryable<TodoItem> query = _context.TodoItems  // queryable sırayla sorgular sonra sql serverden getirtir  örnek sadece aktif görevler
            .AsNoTracking()
            .Include(x => x.Category);

        query = ApplyFilters(query, filter);  // Hazırladığım sorguyu al, kullanıcının filtrelerini buna ekle ve bana geri ver.

        bool isAscending = string.Equals(
            filter.SortDirection,              // artanmı azalan mı sıralancak karar veriyo 
            "asc",
            StringComparison.OrdinalIgnoreCase);

        query = filter.SortBy?.Trim().ToLowerInvariant() switch
        {
            "duedate" => isAscending  // son teslim tarihi
                ? query.OrderBy(x => x.DueDate) // artan isterse
                : query.OrderByDescending(x => x.DueDate), // azalan istere

            _ => isAscending  // geriye kalanlar
                ? query.OrderBy(x => x.CreatedAt)     // oluşturulma tarihine göre sıralıyoruz. artan
                : query.OrderByDescending(x => x.CreatedAt)    // azalan
        };

        int pageNumber =
            filter.PageNumber < 1 ? 1 : filter.PageNumber; // kullanıcıdan alınan sayfa numarası 1 den küçük olamaz olursa otomatik 1 olur sayfa 20 ise gene 20

        int pageSize =
            filter.PageSize < 1 ? 10 : filter.PageSize;  // 1 den küçükse 10 olur 15 se 15 dir

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountFilteredAsync(
        TodoFilterRequestDto filter)
    {
        IQueryable<TodoItem> query = _context.TodoItems
            .AsNoTracking();

        query = ApplyFilters(query, filter);

        return await query.CountAsync();
    }

    private static IQueryable<TodoItem> ApplyFilters(
        IQueryable<TodoItem> query,
        TodoFilterRequestDto filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            string search = filter.Search.Trim();

            string searchField = filter.SearchField?
                .Trim()
                .ToLowerInvariant() ?? "title";

            query = searchField switch
            {
                // Kullanıcı Açıklama seçtiyse sadece Description alanında arar.
                "description" => query.Where(x =>
                    x.Description != null &&
                    x.Description.Contains(search)),

                // Varsayılan olarak sadece Title alanında arar.
                _ => query.Where(x =>
                    x.Title.Contains(search))
            };
        }
        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            string status = filter.Status
                .Trim()
                .ToLowerInvariant();

            DateTime today = DateTime.Today;

            query = status switch
            {
                "active" => query.Where(x =>
                    !x.IsCompleted),

                "completed" => query.Where(x =>
                    x.IsCompleted),

                "overdue" => query.Where(x =>
                    !x.IsCompleted &&
                    x.DueDate.HasValue &&
                    x.DueDate.Value < today),

                _ => query
            };
        }

        if (filter.Priority.HasValue)
        {
            query = query.Where(x =>
                x.Priority == filter.Priority.Value);
        }

        if (filter.CategoryId.HasValue)
        {
            if (filter.CategoryId.Value == 0)
            {
                query = query.Where(x => x.CategoryId == null);
            }
            else
            {
                query = query.Where(
                    x => x.CategoryId == filter.CategoryId.Value
                );
            }
        }
        return query;
    }
}