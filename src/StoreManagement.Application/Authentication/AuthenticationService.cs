using StoreManagement.Application.Abstractions.Persistence;
using StoreManagement.Application.Abstractions.Services;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Authentication;

public sealed class AuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISessionManager _sessionManager;

    public AuthenticationService(IUserRepository userRepository, IPasswordHasher passwordHasher, ISessionManager sessionManager)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _sessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
    }

    public async Task<UserSession?> SignInAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) return null;

        var user = await _userRepository.GetByUsernameAsync(username.Trim(), cancellationToken);
        if (user is null || user.IsActive != 1 || !_passwordHasher.Verify(password, user.PasswordHash)) return null;

        return _sessionManager.StartSession(user.UserId, user.Username, user.DisplayName, user.IsAdministrator);
    }

    public bool SignOut() => _sessionManager.EndSession();
}
