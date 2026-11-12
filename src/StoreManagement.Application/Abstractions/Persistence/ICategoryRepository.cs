using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Abstractions.Persistence;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(long categoryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<long> AddAsync(Category category, CancellationToken cancellationToken = default);
}
