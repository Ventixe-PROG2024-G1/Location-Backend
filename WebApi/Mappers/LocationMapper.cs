using WebApi.Models;

namespace WebApi.Mappers;

public static class LocationMapper
{
    public static LocationModel MapTo(LocationEntity entity)
    {
        var model = new LocationModel
        {
            Id = entity.Id,
            LocationName = entity.LocationName,
            StreetAddress = entity.StreetAddress,
            PostalCode = entity.PostalCode,
            CityName = entity.CityName,
            MapId = entity.MapId,

            CarDirection = entity.Direction?.Car,
            MetroDirection = entity.Direction?.Metro,
            BusDirection = entity.Direction?.Bus
        };
        return model;
    }

    public static LocationEntity MapTo(AddLocationDto dto)
    {
        var entity = new LocationEntity
        {
            Id = Guid.NewGuid().ToString(),
            LocationName = dto.LocationName,
            StreetAddress = dto.StreetAddress,
            PostalCode = dto.PostalCode,
            CityName = dto.CityName,
            MapId = dto.MapId,

            Direction = new DirectionEntity
            {
                Car = dto.CarDirection,
                Metro = dto.MetroDirection,
                Bus = dto.BusDirection
            }
        };
        return entity;
    }

    public static LocationEntity MapTo(EditLocationDto dto)
    {
        var entity = new LocationEntity
        {
            Id = dto.Id,
            LocationName = dto.LocationName,
            StreetAddress = dto.StreetAddress,
            PostalCode = dto.PostalCode,
            CityName = dto.CityName,
            MapId = dto.MapId,

            Direction = new DirectionEntity
            {
                LocationId = dto.Id,
                Car = dto.CarDirection,
                Metro = dto.MetroDirection,
                Bus = dto.BusDirection
            }
        };
        //entity.Direction.Location = entity;
        return entity;
    }
}
