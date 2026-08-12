using TodoList.Application.Interfaces.Repositories;
using TodoList.Domain.Entities;
using TodoList.Infrastructure.Persistence;

namespace TodoList.Infrastructure.Repositories;

public class CategoryRepository
    : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(TodoDbContext context)
        : base(context)
    {
    }
}