using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Abstractions.Persistence;

public interface IStockRepository
{
    Task<long> AddMovementAsync(StockMovement movement, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockMovement>> GetByProductAsync(long productId, CancellationToken cancellationToken = default);
}
