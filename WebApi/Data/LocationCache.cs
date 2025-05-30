using Microsoft.Extensions.Caching.Memory;
using WebApi.Models;

namespace WebApi.Data;

public interface ILocationCache
{
    IEnumerable<LocationModel>? Get();
    LocationModel? Get(string id);
    IEnumerable<LocationModel> Set(IEnumerable<LocationModel> data, int minutesCache = 30);
}

public class LocationCache(IMemoryCache cache) : ILocationCache
{
    private readonly IMemoryCache _cache = cache;
    private const string _key = "ALL_LOCATIONS";

    public IEnumerable<LocationModel>? Get()
    {
        return _cache.TryGetValue(_key, out IEnumerable<LocationModel>? data) ? data : default;
    }

    public LocationModel? Get(string id)
    {
        if (_cache.TryGetValue(_key, out IEnumerable<LocationModel>? data))
            return data?.FirstOrDefault(l => l.Id == id);
        return null;
    }

    public IEnumerable<LocationModel> Set(IEnumerable<LocationModel> data, int minutesCache = 30)
    {
        _cache.Remove(_key);
        return _cache.Set(_key, data, TimeSpan.FromMinutes(minutesCache));
    }
}
