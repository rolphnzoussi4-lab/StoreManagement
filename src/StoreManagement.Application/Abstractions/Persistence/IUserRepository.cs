using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(long userId, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<long> AddAsync(User user, CancellationToken cancellationToken = default);
}
