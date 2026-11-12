using System.ComponentModel.DataAnnotations;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Abstractions.Services;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}

public interface ISessionManager
{
    UserSession? CurrentSession { get; }
    UserSession StartSession(long userId, string username, string displayName, bool isAdministrator);
    bool EndSession();
}

public sealed record UserSession(long UserId, string Username, string DisplayName, bool IsAdministrator, DateTimeOffset StartedAtUtc);
