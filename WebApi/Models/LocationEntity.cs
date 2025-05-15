using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class LocationEntity
{
    [Key]
    public string Id { get; set; } = null!;
    public string LocationName { get; set; } = null!;
    public string StreetAddress { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string CityName { get; set; } = null!;

    public virtual DirectionEntity? Direction { get; set; }

    public string MapId { get; set; } = null!;
}
