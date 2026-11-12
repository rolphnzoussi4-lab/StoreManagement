using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Abstractions.Persistence;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(long categoryId, CancellationToken cancellationToken = default);
}
