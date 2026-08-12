using TodoList.Application.Common.Responses;
using TodoList.Application.DTOs.Todos;
using TodoList.Application.Interfaces.Repositories;
using TodoList.Application.Interfaces.Services;
using TodoList.Domain.Entities;

namespace TodoList.Application.Services;

public class TodoService : ITodoService  // TodoService, ITodoService interface’inde belirlediğimiz kurallara uyacak.
{
    private readonly ITodoRepository _todoRepository;  // TodoService'in veritabanına doğrudan gitmesini istemiyoruz o yüzden _context demedik 
    private readonly ICategoryRepository _categoryRepository;
    public TodoService(
    ITodoRepository todoRepository,
    ICategoryRepository categoryRepository)
    {
        _todoRepository = todoRepository;
        _categoryRepository = categoryRepository;
    }
    public async Task<TodoResponseDto?> GetByIdAsync(int id)   // Bana bir görev Id'si ver, görevi bul ve API'ye uygun TodoResponseDto olarak döndür
    {
        var todo =
            await _todoRepository.GetByIdWithCategoryAsync(id);   // Bana bu görevi kategorisiyle getir. diyor repositoryde bunu yazmıştık daha önce burda direkt getirtiyoruz

        if (todo is null)
        {
            return null;
        }

        return new TodoResponseDto        // Repository bize: TodoItemverdi. Ama API tarafına doğrudan entity göndermek istemiyoruz  TodoItem  TodoResponseDto dönüşümü yapıyoruz buna mapping deniyo
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            Priority = todo.Priority,
            DueDate = todo.DueDate,
            IsCompleted = todo.IsCompleted,
            CompletedAt = todo.CompletedAt,
            CategoryId = todo.CategoryId,
            CategoryName = todo.Category?.Name,
            CategoryColor = todo.Category?.Color,     // Repository Category’yi beraber getirmeseydi burada kategori adı ve rengini rahatça alamazdık
            CreatedAt = todo.CreatedAt,
            UpdatedAt = todo.UpdatedAt
        };
    }

    public async Task<PagedResult<TodoResponseDto>> GetAllAsync(
    TodoFilterRequestDto filter)
    {
        var todos =
            await _todoRepository.GetFilteredAsync(filter);

        var totalCount =
            await _todoRepository.CountFilteredAsync(filter);

        var items = new List<TodoResponseDto>();

        foreach (var todo in todos)
        {
            items.Add(new TodoResponseDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                Priority = todo.Priority,
                DueDate = todo.DueDate,
                IsCompleted = todo.IsCompleted,
                CompletedAt = todo.CompletedAt,
                CategoryId = todo.CategoryId,
                CategoryName = todo.Category?.Name,
                CategoryColor = todo.Category?.Color,
                CreatedAt = todo.CreatedAt,
                UpdatedAt = todo.UpdatedAt
            });
        }

        int pageNumber =
            filter.PageNumber < 1 ? 1 : filter.PageNumber;

        int pageSize =
            filter.PageSize < 1 ? 10 : filter.PageSize;

        return new PagedResult<TodoResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<TodoResponseDto> CreateAsync(
    CreateTodoRequestDto request)
    {
        if (request.CategoryId.HasValue)
        {
            bool categoryExists =
                await _categoryRepository.ExistsAsync(
                    request.CategoryId.Value);

            if (!categoryExists)
            {
                throw new InvalidOperationException(
                    "Gönderilen kategori bulunamadı.");
            }
        }

        var todo = new TodoItem
        {
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            DueDate = request.DueDate,
            CategoryId = request.CategoryId,
            IsCompleted = false,
            CompletedAt = null,
            CreatedAt = DateTime.UtcNow
        };

        await _todoRepository.AddAsync(todo);

        await _todoRepository.SaveChangesAsync();

        var createdTodo =
    await _todoRepository.GetByIdWithCategoryAsync(todo.Id);

        if (createdTodo is null)
        {
            throw new InvalidOperationException(
                "Oluşturulan görev tekrar okunamadı.");
        }

        return new TodoResponseDto
        {
            Id = createdTodo.Id,
            Title = createdTodo.Title,
            Description = createdTodo.Description,
            Priority = createdTodo.Priority,
            DueDate = createdTodo.DueDate,
            IsCompleted = createdTodo.IsCompleted,
            CompletedAt = createdTodo.CompletedAt,
            CategoryId = createdTodo.CategoryId,
            CategoryName = createdTodo.Category?.Name,
            CategoryColor = createdTodo.Category?.Color,
            CreatedAt = createdTodo.CreatedAt,
            UpdatedAt = createdTodo.UpdatedAt
        };
    }
    public async Task<TodoResponseDto?> UpdateAsync(
    UpdateTodoRequestDto request)
    {
        var todo = await _todoRepository.GetByIdAsync(request.Id);  // Bu satır çalışınca repository veritabanına gider:Güncellenecek Todo'yu buluyoruz ama değişmiyoz id 5 olan veriyi getir demek

        if (todo is null)
        {
            return null;
        }

        if (request.CategoryId.HasValue)    // ıd var mı bakılır  Kategori gönderilmiş mi kullanıcı tarafından
        {
            bool categoryExists =
                await _categoryRepository.ExistsAsync(  // geçerli idyi kontrol eder Categories tablosunda Id'si 2 olan kategori var mı?
                    request.CategoryId.Value);

            if (!categoryExists)
            {
                throw new InvalidOperationException(
                    "Gönderilen kategori bulunamadı.");
            }
        }

        todo.Title = request.Title;
        todo.Description = request.Description;
        todo.Priority = request.Priority;
        todo.DueDate = request.DueDate;
        todo.CategoryId = request.CategoryId;
        todo.UpdatedAt = DateTime.UtcNow;

        _todoRepository.Update(todo); // Repository'ye: Bu Todo değişti.

      
        await _todoRepository.SaveChangesAsync(); // değişiklikler gerçekten SQL Server'a kaydediliyor.

        var updatedTodo =
            await _todoRepository.GetByIdWithCategoryAsync(todo.Id);  // Aynı Todo'yu güncellenmiş haliyle tekrar okuyoruz. CategoryName  CategoryColor da ekleniyo


        if (updatedTodo is null)
        {
            return null;
        }

        return new TodoResponseDto
        {
            Id = updatedTodo.Id,
            Title = updatedTodo.Title,
            Description = updatedTodo.Description,
            Priority = updatedTodo.Priority,
            DueDate = updatedTodo.DueDate,
            IsCompleted = updatedTodo.IsCompleted,
            CompletedAt = updatedTodo.CompletedAt,
            CategoryId = updatedTodo.CategoryId,
            CategoryName = updatedTodo.Category?.Name,
            CategoryColor = updatedTodo.Category?.Color,
            CreatedAt = updatedTodo.CreatedAt,
            UpdatedAt = updatedTodo.UpdatedAt
        };
    }
    public async Task<bool> ChangeStatusAsync(
    int id,                                             // id ve  IsCompleted verisini aldık bunu bool olarak döndürcez 
    ChangeTodoStatusRequestDto request)
    {
        var todo = await _todoRepository.GetByIdAsync(id);    // idyi veritabanından aldık todoya veridk

        if (todo is null)
        {
            return false;
        }

        todo.IsCompleted = request.IsCompleted;         //kullanıcının tamamlanma verisini veritabanındakiyle değiştirdik ama daha yazdırmadık

        todo.CompletedAt = request.IsCompleted    // if else bloğu eğer tamamlanmıssa güncel tarih girilcek tamamlanmadıysa null
            ? DateTime.UtcNow
            : null;

        todo.UpdatedAt = DateTime.UtcNow;         // Görevin durumu değiştiği için bu da bir güncelleme.

        _todoRepository.Update(todo);   // Artık todo değişti. repositorye bildiriyoruz

        await _todoRepository.SaveChangesAsync();  // Burada değişiklik SQL Server'a kaydediliyor.

        return true;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var todo = await _todoRepository.GetByIdAsync(id);

        if (todo is null)
        {
            return false;
        }

        todo.IsDeleted = true;  // Bu kayıt silinmiş kabul edilsin. zamanında  builder.HasQueryFilter(x => !x.IsDeleted);  yazdık sadece isdeleted false olanları getir demek kullanıcı görmez ama biz görebilirz
        todo.UpdatedAt = DateTime.UtcNow;  // güncellendi

        _todoRepository.Update(todo);  // Repository'ye bildiriyoruz

        await _todoRepository.SaveChangesAsync();    // kaydettik

        return true;
    }
}