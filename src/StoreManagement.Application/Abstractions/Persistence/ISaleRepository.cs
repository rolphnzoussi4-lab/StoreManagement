using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Abstractions.Persistence;

public interface ISaleRepository
{
    Task<Sale?> GetByIdAsync(long saleId, CancellationToken cancellationToken = default);
    Task<long> AddAsync(Sale sale, CancellationToken cancellationToken = default);
    Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default);
}
