using StoreManagement.Application.Abstractions.Services;

namespace StoreManagement.Infrastructure.Authentication;

public sealed class SessionManager : ISessionManager
{
    private readonly object _sync = new();
    private UserSession? _currentSession;

    public UserSession? CurrentSession
    {
        get
        {
            lock (_sync)
            {
                return _currentSession;
            }
        }
    }

    public UserSession StartSession(long userId, string username, string displayName, bool isAdministrator)
    {
        var session = new UserSession(userId, username, displayName, isAdministrator, DateTimeOffset.UtcNow);
        lock (_sync)
        {
            _currentSession = session;
        }
        return session;
    }

    public bool EndSession()
    {
        lock (_sync)
        {
            if (_currentSession is null) return false;
            _currentSession = null;
            return true;
        }
    }
}
