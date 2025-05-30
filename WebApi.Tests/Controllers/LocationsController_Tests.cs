using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Tests.Controllers;

// AI Written
public class LocationsController_Tests
{
    private readonly Mock<ILocationRepository> _repoMock;
    private readonly LocationsController _controller;

    public LocationsController_Tests()
    {
        _repoMock = new Mock<ILocationRepository>();
        _controller = new LocationsController(_repoMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithLocations_WhenSuccess()
    {
        // Arrange
        var locations = new List<LocationModel>
    {
        new LocationModel { Id = "1", LocationName = "Loc1", CityName = "City1", MapId = "map1", PostalCode = "123", StreetAddress = "Addr1" }
    };
        _repoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new DataResponse<IEnumerable<LocationModel>>
            {
                Succeded = true,
                StatusCode = 200,
                Result = locations
            });

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(locations);
    }

    [Fact]
    public async Task GetAll_ReturnsStatusCode_WhenFails()
    {
        // Arrange
        _repoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new DataResponse<IEnumerable<LocationModel>>
            {
                Succeded = false,
                StatusCode = 500
            });

        // Act
        var result = await _controller.GetAll();

        // Assert
        var statusResult = result.Should().BeOfType<StatusCodeResult>().Subject;
        statusResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WithLocation_WhenFound()
    {
        // Arrange
        var location = new LocationModel { Id = "id1", LocationName = "Loc1" };
        _repoMock.Setup(r => r.GetAsync("id1"))
            .ReturnsAsync(new DataResponse<LocationModel>
            {
                Succeded = true,
                StatusCode = 200,
                Result = location
            });

        // Act
        var result = await _controller.GetById("id1");

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(location);
    }

    [Fact]
    public async Task GetById_ReturnsStatusCode_WhenNotFound()
    {
        // Arrange
        _repoMock.Setup(r => r.GetAsync("id1"))
            .ReturnsAsync(new DataResponse<LocationModel>
            {
                Succeded = false,
                StatusCode = 404
            });

        // Act
        var result = await _controller.GetById("id1");

        // Assert
        var statusResult = result.Should().BeOfType<StatusCodeResult>().Subject;
        statusResult.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Add_ReturnsCreated_WhenSuccess()
    {
        // Arrange
        var dto = new AddLocationDto { LocationName = "Loc1", StreetAddress = "Addr", PostalCode = "12345", CityName = "City", MapId = "map" };
        _repoMock.Setup(r => r.AddAsync(dto))
            .ReturnsAsync(new DataResponse { Succeded = true, StatusCode = 201 });

        // Act
        var result = await _controller.Add(dto);

        // Assert
        result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task Add_ReturnsBadRequest_WhenModelInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("LocationName", "Required");

        // Act
        var result = await _controller.Add(new AddLocationDto());

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Add_ReturnsStatusCode_WhenFails()
    {
        // Arrange
        var dto = new AddLocationDto { LocationName = "Loc1", StreetAddress = "Addr", PostalCode = "12345", CityName = "City", MapId = "map" };
        _repoMock.Setup(r => r.AddAsync(dto))
            .ReturnsAsync(new DataResponse { Succeded = false, StatusCode = 500 });

        // Act
        var result = await _controller.Add(dto);

        // Assert
        var statusResult = result.Should().BeOfType<StatusCodeResult>().Subject;
        statusResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenSuccess()
    {
        // Arrange
        var dto = new EditLocationDto { Id = "id", LocationName = "Loc1", StreetAddress = "Addr", PostalCode = "12345", CityName = "City", MapId = "map" };
        _repoMock.Setup(r => r.UpdateAsync(dto))
            .ReturnsAsync(new DataResponse { Succeded = true, StatusCode = 200 });

        // Act
        var result = await _controller.Update(dto);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenModelInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("LocationName", "Required");

        // Act
        var result = await _controller.Update(new EditLocationDto());

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_ReturnsStatusCode_WhenFails()
    {
        // Arrange
        var dto = new EditLocationDto { Id = "id", LocationName = "Loc1", StreetAddress = "Addr", PostalCode = "12345", CityName = "City", MapId = "map" };
        _repoMock.Setup(r => r.UpdateAsync(dto))
            .ReturnsAsync(new DataResponse { Succeded = false, StatusCode = 404 });

        // Act
        var result = await _controller.Update(dto);

        // Assert
        var statusResult = result.Should().BeOfType<StatusCodeResult>().Subject;
        statusResult.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Delete_ReturnsOk_WhenSuccess()
    {
        // Arrange
        _repoMock.Setup(r => r.DeleteAsync("id"))
            .ReturnsAsync(new DataResponse { Succeded = true, StatusCode = 200 });

        // Act
        var result = await _controller.Delete("id");

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Delete_ReturnsStatusCode_WhenFails()
    {
        // Arrange
        _repoMock.Setup(r => r.DeleteAsync("id"))
            .ReturnsAsync(new DataResponse { Succeded = false, StatusCode = 400 });

        // Act
        var result = await _controller.Delete("id");

        // Assert
        var statusResult = result.Should().BeOfType<StatusCodeResult>().Subject;
        statusResult.StatusCode.Should().Be(400);
    }
}
