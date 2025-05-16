using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApi.Mappers;
using WebApi.Models;

namespace WebApi.Data;

public class LocationRepository(LocationContext context, LocationCache cache)
{
    private readonly LocationContext _context = context;
    private readonly LocationCache _cache = cache;

    public async Task<DataResponse> ExistsAsync(string id)
    {
        var any = await _context.Locations.AnyAsync(l => l.Id == id);
        return any
            ? new DataResponse { Succeded = true, StatusCode = 200 }
            : new DataResponse { Succeded = false, StatusCode = 404 };
    }

    public async Task<DataResponse<LocationModel>> GetAsync(string id)
    {
        var model = _cache.Get(id);
        if (model == null)
        {
            await RefreshCache();
            model = _cache.Get(id);
        }
        if (model != null)
            return new DataResponse<LocationModel>
            {
                Succeded = true,
                StatusCode = 200,
                Result = model
            };
        return new DataResponse<LocationModel>
        {
            Succeded = false,
            StatusCode = 404,
        };
    }

    public async Task<DataResponse<IEnumerable<LocationModel>>> GetAllAsync()
    {
        var models = _cache.Get();
        if (models == null)
            models = await RefreshCache();
        return new DataResponse<IEnumerable<LocationModel>>
        {
            Succeded = true,
            StatusCode = 200,
            Result = models
        };
    }

    public async Task<DataResponse> AddAsync(AddLocationDto dto)
    {
        if (dto == null)
            return new DataResponse { Succeded = false, StatusCode = 400 };

        try
        {
            _context.Locations.Add(LocationMapper.MapTo(dto));
            await _context.SaveChangesAsync();
            await RefreshCache();
            return new DataResponse { Succeded = true, StatusCode = 201 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception: " + ex.Message);
            if (ex.InnerException != null)
                Debug.WriteLine("InnerException: " + ex.InnerException);
            return new DataResponse { Succeded = false, StatusCode = 500, Message = ex.Message };
        }
    }

    public async Task<DataResponse> UpdateAsync(EditLocationDto dto)
    {
        if (dto == null)
            return new DataResponse { Succeded = false, StatusCode = 400 };

        try
        {
            _context.Locations.Update(LocationMapper.MapTo(dto));
            await _context.SaveChangesAsync();
            await RefreshCache();
            return new DataResponse { Succeded = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception: " + ex.Message);
            if (ex.InnerException != null)
                Debug.WriteLine("InnerException: " + ex.InnerException);
            return new DataResponse { Succeded = false, StatusCode = 500, Message = ex.Message };
        }
    }

    public async Task<DataResponse> DeleteAsync(string id)
    {
        var entity = await _context.Locations.FirstOrDefaultAsync(l => l.Id == id);
        if (entity == null)
            return new DataResponse { Succeded = false, StatusCode = 400 };

        try
        {
            _context.Locations.Remove(entity);
            await _context.SaveChangesAsync();
            await RefreshCache();
            return new DataResponse { Succeded = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception: " + ex.Message);
            if (ex.InnerException != null)
                Debug.WriteLine("InnerException: " + ex.InnerException);
            return new DataResponse { Succeded = false, StatusCode = 500, Message = ex.Message };
        }
    }

    private async Task<IEnumerable<LocationModel>> RefreshCache()
    {
        IQueryable<LocationEntity> query = _context.Locations;
        query = query.Include(i => i.Direction);
        query = query.OrderBy(s => s.LocationName);

        var entities = await query.ToListAsync();
        var models = entities.Select(e => LocationMapper.MapTo(e));
        _cache.Set(models);

        return models;
    }
}
