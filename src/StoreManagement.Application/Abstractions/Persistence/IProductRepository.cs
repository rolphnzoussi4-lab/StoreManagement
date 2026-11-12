using StoreManagement.Application.Features.Products;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Abstractions.Persistence;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(long productId, CancellationToken cancellationToken = default);
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> SearchAsync(ProductFilter filter, CancellationToken cancellationToken = default);
    Task<long> AddAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
}
