namespace WebApi.Models;

public class EditLocationDto
{
    public string Id { get; set; } = null!;
    public string LocationName { get; set; } = null!;
    public string StreetAddress { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string CityName { get; set; } = null!;
    public string MapId { get; set; } = null!;
    public string? CarDirection { get; set; }
    public string? MetroDirection { get; set; }
    public string? BusDirection { get; set; }
}
