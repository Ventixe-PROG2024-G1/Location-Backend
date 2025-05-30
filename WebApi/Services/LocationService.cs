using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using WebApi.Data;
using WebApi.Extensions;
using WebApi.Grpc;
using WebApi.Models;

namespace WebApi.Services;

public class LocationService(ILocationRepository repo) : LocationGrpcService.LocationGrpcServiceBase
{
    private readonly ILocationRepository _repo = repo;

    public async override Task<ActionReply> AddLocation(LocationAddRequest request, ServerCallContext context)
    {
        try
        {
            if (request == null)
                return new ActionReply { Succeeded = false, StatusCode = 400 };

            var response = await _repo.AddAsync(request.MapTo<AddLocationDto>());
            return response.Succeded
                ? new ActionReply { Succeeded = true, StatusCode = 201 }
                : new ActionReply { Succeeded = false, StatusCode = (int)response.StatusCode! };
        }
        catch (RpcException ex)
        {
            return new ActionReply
            {
                Succeeded = false,
                StatusCode = ((int)ex.StatusCode),
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new ActionReply
            {
                Succeeded = false,
                StatusCode = 500,
                Message = ex.Message
            };
        }
    }

    public async override Task<ActionReply> DeleteLocation(LocationRequest request, ServerCallContext context)
    {
        try
        {
            if (request == null)
                return new ActionReply { Succeeded = false, StatusCode = 400 };

            var response = await _repo.DeleteAsync(request.LocationId);
            return response.Succeded
                ? new ActionReply { Succeeded = true, StatusCode = 200 }
                : new ActionReply { Succeeded = false, StatusCode = (int)response.StatusCode! };
        }
        catch (RpcException ex)
        {
            return new ActionReply
            {
                Succeeded = false,
                StatusCode = ((int)ex.StatusCode),
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new ActionReply
            {
                Succeeded = false,
                StatusCode = 500,
                Message = ex.Message
            };
        }
    }

    public async override Task<LocationReply> GetLocation(LocationRequest request, ServerCallContext context)
    {
        try
        {
            if (request == null)
                return new LocationReply { Succeeded = false, StatusCode = 400 };

            var response = await _repo.GetAsync(request.LocationId);
            return response.Succeded
                ? new LocationReply { Succeeded = true, StatusCode = 200, Location = response.Result!.MapTo<Location>() }
                : new LocationReply { Succeeded = false, StatusCode = (int)response.StatusCode! };
        }
        catch (RpcException ex)
        {
            return new LocationReply
            {
                Succeeded = false,
                StatusCode = ((int)ex.StatusCode),
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new LocationReply
            {
                Succeeded = false,
                StatusCode = 500,
                Message = ex.Message
            };
        }
    }

    public async override Task<LocationsReply> GetLocations(Empty request, ServerCallContext context)
    {
        try
        {
            if (request == null)
                return new LocationsReply { Succeeded = false, StatusCode = 400 };

            var response = await _repo.GetAllAsync();

            if (!response.Succeded)
                return new LocationsReply { Succeeded = false, StatusCode = (int)response.StatusCode! };

            var reply = new LocationsReply { Succeeded = true, StatusCode = 200 };

            foreach (var l in response.Result!)
                reply.Locations.Add(l.MapTo<Location>());

            return reply;
        }
        catch (RpcException ex)
        {
            return new LocationsReply
            {
                Succeeded = false,
                StatusCode = ((int)ex.StatusCode),
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new LocationsReply
            {
                Succeeded = false,
                StatusCode = 500,
                Message = ex.Message
            };
        }
    }

    public async override Task<ActionReply> UpdateLocation(LocationUpdateRequest request, ServerCallContext context)
    {
        try
        {
            if (request == null)
                return new ActionReply { Succeeded = false, StatusCode = 400 };

            var response = await _repo.UpdateAsync(request.MapTo<EditLocationDto>());
            return response.Succeded
                ? new ActionReply { Succeeded = true, StatusCode = 200 }
                : new ActionReply { Succeeded = false, StatusCode = (int)response.StatusCode! };
        }
        catch (RpcException ex)
        {
            return new ActionReply
            {
                Succeeded = false,
                StatusCode = ((int)ex.StatusCode),
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new ActionReply
            {
                Succeeded = false,
                StatusCode = 500,
                Message = ex.Message
            };
        }
    }
}
