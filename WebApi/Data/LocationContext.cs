using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApi.Mappers;
using WebApi.Models;

namespace WebApi.Data;

public class LocationContext(DbContextOptions<LocationContext> options) : DbContext(options)
{
    public DbSet<LocationEntity> Locations { get; set; }
    public DbSet<DirectionEntity> Directions { get; set; }
}
