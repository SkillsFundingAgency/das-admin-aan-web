using Microsoft.AspNetCore.Http;
using SFA.DAS.Admin.Aan.Application.Constants;
using System.Text.Json;

namespace SFA.DAS.Admin.Aan.Application.Services;

public class SessionService : ISessionService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public void Set(string key, string value) => _httpContextAccessor.HttpContext?.Session.SetString(
        key, value);

    public void Set<T>(T model) => Set(typeof(T).Name, JsonSerializer.Serialize(model));

    public string? Get(string key) => _httpContextAccessor.HttpContext?.Session.GetString(key);

    public T Get<T>()
    {
        var json = Get(typeof(T).Name);
        return (string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json))!;
    }

    public void Delete(string key)
    {
        if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Session.Keys.Any(k => k == key))
            _httpContextAccessor.HttpContext.Session.Remove(key);
    }

    public void Delete<T>(T model) => Delete(typeof(T).Name);

    public void Clear() => _httpContextAccessor.HttpContext?.Session.Clear();

    public bool Contains<T>()
    {
        var result = _httpContextAccessor.HttpContext?.Session.Keys.Any(k => k == typeof(T).Name);
        return result.GetValueOrDefault();
    }

    public Guid GetMemberId()
    {
        var id = Guid.Empty;

        var memberId = Get(SessionKeys.MemberId);

        if (Guid.TryParse(memberId, out var newGuid))
        {
            id = newGuid;
        }

        return id;
    }

}
