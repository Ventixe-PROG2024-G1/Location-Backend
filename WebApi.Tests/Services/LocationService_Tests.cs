using Google.Protobuf.WellKnownTypes;
using Grpc.Core.Testing;
using Grpc.Core;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Grpc;
using WebApi.Services;
using WebApi.Models;
using FluentAssertions;

namespace WebApi.Tests.Services;

public class LocationService_Tests
{
    private readonly Mock<ILocationRepository> _repoMock;
    private readonly LocationService _service;
    private readonly ServerCallContext _context = TestServerCallContext.Create(
        method: "TestMethod",
        host: "localhost",
        deadline: DateTime.UtcNow.AddMinutes(1),
        requestHeaders: new Metadata(),
        cancellationToken: CancellationToken.None,
        peer: "127.0.0.1",
        authContext: null,
        contextPropagationToken: null,
        writeHeadersFunc: _ => Task.CompletedTask,
        writeOptionsGetter: () => null,
        writeOptionsSetter: _ => { });

    public LocationService_Tests()
    {
        _repoMock = new Mock<ILocationRepository>();
        _service = new LocationService(_repoMock.Object);
    }

    private LocationAddRequest CreateValidAddRequest() => new()
    {
        LocationName = "Central Park",
        StreetAddress = "123 Park Ave",
        PostalCode = "10001",
        CityName = "New York",
        MapId = "map123",
        CarDirection = "North",
        MetroDirection = "Line A",
        BusDirection = "Bus 5"
    };

    private AddLocationDto CreateValidAddDto() => new()
    {
        LocationName = "Central Park",
        StreetAddress = "123 Park Ave",
        PostalCode = "10001",
        CityName = "New York",
        MapId = "map123",
        CarDirection = "North",
        MetroDirection = "Line A",
        BusDirection = "Bus 5"
    };

    private LocationUpdateRequest CreateValidUpdateRequest() => new()
    {
        Id = "loc123",
        LocationName = "Central Park Updated",
        StreetAddress = "123 Park Ave",
        PostalCode = "10001",
        CityName = "New York",
        MapId = "map123",
        CarDirection = "East",
        MetroDirection = "Line B",
        BusDirection = "Bus 10"
    };

    private EditLocationDto CreateValidEditDto() => new()
    {
        Id = "loc123",
        LocationName = "Central Park Updated",
        StreetAddress = "123 Park Ave",
        PostalCode = "10001",
        CityName = "New York",
        MapId = "map123",
        CarDirection = "East",
        MetroDirection = "Line B",
        BusDirection = "Bus 10"
    };

    private LocationRequest CreateValidLocationRequest() => new() { LocationId = "loc123" };

    private LocationModel CreateValidLocationModel() => new()
    {
        Id = "loc123",
        LocationName = "Central Park",
        StreetAddress = "123 Park Ave",
        PostalCode = "10001",
        CityName = "New York",
        MapId = "map123",
        CarDirection = "North",
        MetroDirection = "Line A",
        BusDirection = "Bus 5"
    };

    [Fact]
    public async Task AddLocation_Returns201_WhenSuccess()
    {
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<AddLocationDto>()))
            .ReturnsAsync(new DataResponse { Succeded = true });

        var request = CreateValidAddRequest();

        var result = await _service.AddLocation(request, _context);

        result.Succeeded.Should().BeTrue();
        result.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task AddLocation_Returns400_WhenRequestNull()
    {
        var result = await _service.AddLocation(null!, _context);

        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task AddLocation_ReturnsStatusCodeFromRepo_WhenFailed()
    {
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<AddLocationDto>()))
            .ReturnsAsync(new DataResponse { Succeded = false, StatusCode = 409 });

        var result = await _service.AddLocation(CreateValidAddRequest(), _context);

        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task DeleteLocation_Returns200_WhenSuccess()
    {
        _repoMock
            .Setup(r => r.DeleteAsync(It.IsAny<string>()))
            .ReturnsAsync(new DataResponse { Succeded = true });

        var request = CreateValidLocationRequest();

        var result = await _service.DeleteLocation(request, _context);

        result.Succeeded.Should().BeTrue();
        result.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task DeleteLocation_Returns400_WhenRequestNull()
    {
        var result = await _service.DeleteLocation(null!, _context);

        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task DeleteLocation_ReturnsStatusCodeFromRepo_WhenFailed()
    {
        _repoMock
            .Setup(r => r.DeleteAsync(It.IsAny<string>()))
            .ReturnsAsync(new DataResponse { Succeded = false, StatusCode = 404 });

        var request = CreateValidLocationRequest();

        var result = await _service.DeleteLocation(request, _context);

        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetLocation_ReturnsLocation_WhenSuccess()
    {
        var locationModel = CreateValidLocationModel();

        _repoMock
            .Setup(r => r.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(new DataResponse<LocationModel> { Succeded = true, Result = locationModel });

        var request = CreateValidLocationRequest();

        var result = await _service.GetLocation(request, _context);

        result.Succeeded.Should().BeTrue();
        result.StatusCode.Should().Be(200);
        result.Location.Should().NotBeNull();
        result.Location.Id.Should().Be(locationModel.Id);
        result.Location.LocationName.Should().Be(locationModel.LocationName);
    }

    [Fact]
    public async Task GetLocation_Returns400_WhenRequestNull()
    {
        var result = await _service.GetLocation(null!, _context);

        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetLocation_ReturnsStatusCodeFromRepo_WhenFailed()
    {
        _repoMock
            .Setup(r => r.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(new DataResponse<LocationModel> { Succeded = false, StatusCode = 404 });

        var result = await _service.GetLocation(CreateValidLocationRequest(), _context);

        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetLocations_ReturnsList_WhenSuccess()
    {
        var locations = new List<LocationModel>
            {
                CreateValidLocationModel(),
                new LocationModel
                {
                    Id = "loc456",
                    LocationName = "Times Square",
                    StreetAddress = "456 Broadway",
                    PostalCode = "10002",
                    CityName = "New York",
                    MapId = "map456",
                    CarDirection = "South",
                    MetroDirection = "Line C",
                    BusDirection = "Bus 15"
                }
            };

        _repoMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new DataResponse<IEnumerable<LocationModel>> { Succeded = true, Result = locations });

        var result = await _service.GetLocations(new Empty(), _context);

        result.Succeeded.Should().BeTrue();
        result.StatusCode.Should().Be(200);
        result.Locations.Should().HaveCount(2);
        result.Locations[0].Id.Should().Be(locations[0].Id);
        result.Locations[1].LocationName.Should().Be("Times Square");
    }

    [Fact]
    public async Task GetLocations_Returns400_WhenRequestNull()
    {
        var result = await _service.GetLocations(null!, _context);

        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetLocations_ReturnsStatusCodeFromRepo_WhenFailed()
    {
        _repoMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new DataResponse<IEnumerable<LocationModel>> { Succeded = false, StatusCode = 500 });

        var result = await _service.GetLocations(new Empty(), _context);

        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task UpdateLocation_Returns200_WhenSuccess()
    {
        _repoMock
            .Setup(r => r.UpdateAsync(It.IsAny<EditLocationDto>()))
            .ReturnsAsync(new DataResponse { Succeded = true });

        var request = CreateValidUpdateRequest();

        var result = await _service.UpdateLocation(request, _context);

        result.Succeeded.Should().BeTrue();
        result.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task UpdateLocation_Returns400_WhenRequestNull()
    {
        var result = await _service.UpdateLocation(null!, _context);

        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task UpdateLocation_ReturnsStatusCodeFromRepo_WhenFailed()
    {
        _repoMock
            .Setup(r => r.UpdateAsync(It.IsAny<EditLocationDto>()))
            .ReturnsAsync(new DataResponse { Succeded = false, StatusCode = 409 });

        var result = await _service.UpdateLocation(CreateValidUpdateRequest(), _context);

        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task UpdateLocation_Returns500_OnException()
    {
        _repoMock
            .Setup(r => r.UpdateAsync(It.IsAny<EditLocationDto>()))
            .ThrowsAsync(new Exception("fail"));

        var result = await _service.UpdateLocation(CreateValidUpdateRequest(), _context);

        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(500);
        result.Message.Should().Be("fail");
    }
}
