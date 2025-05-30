using FluentAssertions;
using WebApi.Mappers;
using WebApi.Models;

namespace WebApi.Tests.Mappers;

public class LocationMapper_Tests
{
    [Fact]
    public void MapTo_LocationModel_From_LocationEntity_Should_Map_All_Properties()
    {
        var entity = new LocationEntity
        {
            Id = "loc1",
            LocationName = "My Place",
            StreetAddress = "123 Main St",
            PostalCode = "12345",
            CityName = "Townsville",
            MapId = "map123",
            Direction = new DirectionEntity
            {
                Car = "CarDir",
                Metro = "MetroDir",
                Bus = "BusDir"
            }
        };

        var model = LocationMapper.MapTo(entity);

        model.Should().NotBeNull();
        model.Id.Should().Be(entity.Id);
        model.LocationName.Should().Be(entity.LocationName);
        model.StreetAddress.Should().Be(entity.StreetAddress);
        model.PostalCode.Should().Be(entity.PostalCode);
        model.CityName.Should().Be(entity.CityName);
        model.MapId.Should().Be(entity.MapId);
        model.CarDirection.Should().Be(entity.Direction!.Car);
        model.MetroDirection.Should().Be(entity.Direction.Metro);
        model.BusDirection.Should().Be(entity.Direction.Bus);
    }

    [Fact]
    public void MapTo_LocationEntity_From_AddLocationDto_Should_Map_All_Properties_And_Generate_New_Id()
    {
        var dto = new AddLocationDto
        {
            LocationName = "New Location",
            StreetAddress = "456 Elm St",
            PostalCode = "54321",
            CityName = "Cityville",
            MapId = "map456",
            CarDirection = "CarD",
            MetroDirection = "MetroD",
            BusDirection = "BusD"
        };

        var entity = LocationMapper.MapTo(dto);

        entity.Should().NotBeNull();
        entity.Id.Should().NotBeNullOrEmpty();
        entity.LocationName.Should().Be(dto.LocationName);
        entity.StreetAddress.Should().Be(dto.StreetAddress);
        entity.PostalCode.Should().Be(dto.PostalCode);
        entity.CityName.Should().Be(dto.CityName);
        entity.MapId.Should().Be(dto.MapId);

        entity.Direction.Should().NotBeNull();
        entity.Direction!.Car.Should().Be(dto.CarDirection);
        entity.Direction.Metro.Should().Be(dto.MetroDirection);
        entity.Direction.Bus.Should().Be(dto.BusDirection);
    }

    [Fact]
    public void MapTo_LocationEntity_From_EditLocationDto_Should_Map_All_Properties()
    {
        var dto = new EditLocationDto
        {
            Id = "edit123",
            LocationName = "Edited Location",
            StreetAddress = "789 Oak St",
            PostalCode = "98765",
            CityName = "Village",
            MapId = "map789",
            CarDirection = "CarEdit",
            MetroDirection = "MetroEdit",
            BusDirection = "BusEdit"
        };

        var entity = new LocationEntity
        {
            Id = "edit123",
            LocationName = "Location",
            StreetAddress = "789 Oak St",
            PostalCode = "98765",
            CityName = "Village",
            MapId = "map789",
            Direction = new DirectionEntity
            {
                LocationId = "edit123",
                Car = "Car",
                Metro = "Metro",
                Bus = "Bus"
            }
        };

        entity = LocationMapper.MapTo(dto, entity);

        entity.Should().NotBeNull();
        entity.Id.Should().Be(dto.Id);
        entity.LocationName.Should().Be(dto.LocationName);
        entity.StreetAddress.Should().Be(dto.StreetAddress);
        entity.PostalCode.Should().Be(dto.PostalCode);
        entity.CityName.Should().Be(dto.CityName);
        entity.MapId.Should().Be(dto.MapId);

        entity.Direction.Should().NotBeNull();
        entity.Direction!.LocationId.Should().Be(dto.Id);
        entity.Direction.Car.Should().Be(dto.CarDirection);
        entity.Direction.Metro.Should().Be(dto.MetroDirection);
        entity.Direction.Bus.Should().Be(dto.BusDirection);
    }
}
