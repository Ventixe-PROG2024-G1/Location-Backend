using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Tests.Data;

// AI Written
public class LocationRepository_Tests
{
    private readonly LocationContext _context;
    private readonly Mock<ILocationCache> _cacheMock;
    private readonly LocationRepository _repository;

    public LocationRepository_Tests()
    {
        var options = new DbContextOptionsBuilder<LocationContext>()
            .UseInMemoryDatabase(databaseName: "LocationDb_Test")
            .Options;
        _context = new LocationContext(options);
        _cacheMock = new Mock<ILocationCache>();
        _repository = new LocationRepository(_context, _cacheMock.Object);
    }

    private LocationContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<LocationContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new LocationContext(options);
    }

    [Fact]
    public async Task AddAsync_Should_Add_Location()
    {
        using var context = CreateContext();
        var cacheMock = new Mock<ILocationCache>();
        var repository = new LocationRepository(context, cacheMock.Object);

        var dto = new AddLocationDto
        {
            LocationName = "Test Location",
            StreetAddress = "123 Main St",
            PostalCode = "12345",
            CityName = "Test City",
            MapId = "map-1",
            CarDirection = "North",
            MetroDirection = "Green line",
            BusDirection = "Line 5"
        };

        var result = await repository.AddAsync(dto);

        result.Succeded.Should().BeTrue();
        result.StatusCode.Should().Be(201);
        context.Locations.Count().Should().Be(1);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Locations_From_Cache()
    {
        var models = new List<LocationModel>
        {
            new LocationModel
            {
                Id = "1",
                LocationName = "Cached Location",
                StreetAddress = "1 Street",
                PostalCode = "11111",
                CityName = "City1",
                MapId = "map-1"
            }
        };
        _cacheMock.Setup(c => c.Get()).Returns(models);

        var result = await _repository.GetAllAsync();

        result.Succeded.Should().BeTrue();
        result.Result.Should().BeEquivalentTo(models);
    }

    [Fact]
    public async Task GetAsync_Should_Return_Location_From_Cache()
    {
        var id = "loc-123";
        var model = new LocationModel
        {
            Id = id,
            LocationName = "Cached Location",
            StreetAddress = "Somewhere",
            PostalCode = "45678",
            CityName = "Testopolis",
            MapId = "map-99"
        };
        _cacheMock.SetupSequence(c => c.Get(id)).Returns((LocationModel?)null).Returns(model);
        _cacheMock.Setup(c => c.Set(It.IsAny<IEnumerable<LocationModel>>(), 30));

        var entity = new LocationEntity
        {
            Id = id,
            LocationName = model.LocationName,
            StreetAddress = model.StreetAddress,
            PostalCode = model.PostalCode,
            CityName = model.CityName,
            MapId = model.MapId,
            Direction = new DirectionEntity { LocationId = id }
        };
        _context.Locations.Add(entity);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAsync(id);

        result.Succeded.Should().BeTrue();
        result.Result!.Id.Should().Be(id);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Existing_Location()
    {
        var id = "edit-1";
        _context.Locations.Add(new LocationEntity
        {
            Id = id,
            LocationName = "Old Name",
            StreetAddress = "Old St",
            PostalCode = "00000",
            CityName = "Old City",
            MapId = "old-map",
            Direction = new DirectionEntity { LocationId = id }
        });
        await _context.SaveChangesAsync();

        var dto = new EditLocationDto
        {
            Id = id,
            LocationName = "New Name",
            StreetAddress = "New St",
            PostalCode = "99999",
            CityName = "New City",
            MapId = "new-map",
            CarDirection = "North",
            MetroDirection = "Red line",
            BusDirection = "Bus 42"
        };

        var result = await _repository.UpdateAsync(dto);

        result.Succeded.Should().BeTrue();
        result.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Existing_Location()
    {
        var id = "delete-1";
        _context.Locations.Add(new LocationEntity
        {
            Id = id,
            LocationName = "Delete Me",
            StreetAddress = "Del St",
            PostalCode = "32100",
            CityName = "Trashville",
            MapId = "del-map",
            Direction = new DirectionEntity { LocationId = id }
        });
        await _context.SaveChangesAsync();

        var result = await _repository.DeleteAsync(id);

        result.Succeded.Should().BeTrue();
        _context.Locations.FirstOrDefault(l => l.Id == id).Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_Should_Return_True_If_Location_Exists()
    {
        var id = "exists-1";
        _context.Locations.Add(new LocationEntity
        {
            Id = id,
            LocationName = "Exists",
            StreetAddress = "Exists St",
            PostalCode = "22222",
            CityName = "Exists City",
            MapId = "exists-map",
            Direction = new DirectionEntity { LocationId = id }
        });
        await _context.SaveChangesAsync();

        var result = await _repository.ExistsAsync(id);

        result.Succeded.Should().BeTrue();
        result.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ExistsAsync_Should_Return_False_If_Location_Does_Not_Exist()
    {
        var result = await _repository.ExistsAsync("nonexistent");

        result.Succeded.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }
}
