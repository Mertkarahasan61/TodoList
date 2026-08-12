using TodoList.Domain.Entities;

namespace TodoList.Application.Interfaces.Repositories;

public interface ICategoryRepository
    : IGenericRepository<Category>
{
}