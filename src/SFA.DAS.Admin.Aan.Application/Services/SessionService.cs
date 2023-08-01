﻿using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.Admin.Aan.Application.Services;

public interface ISessionService
{
    void Set(string value, string key);
    void Set<T>(T model);
    string Get(string key);
    T Get<T>();
    void Delete(string key);
    void Delete<T>(T model);
    void Clear();
    bool Contains<T>();
}

public class SessionService : ISessionService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public void Set(string value, string key) => _httpContextAccessor.HttpContext?.Session.SetString(key, value);
    public void Set<T>(T model) => Set(JsonSerializer.Serialize(model), typeof(T).Name);

    public string Get(string key) => _httpContextAccessor.HttpContext?.Session.GetString(key)!;

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
}
