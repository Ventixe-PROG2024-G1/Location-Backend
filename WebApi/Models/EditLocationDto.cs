using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class EditLocationDto
{
    [Required]
    [DataType(DataType.Text)]
    public string Id { get; set; } = null!;

    [Required]
    [DataType(DataType.Text)]
    public string LocationName { get; set; } = null!;

    [Required]
    [DataType(DataType.Text)]
    public string StreetAddress { get; set; } = null!;

    [Required]
    [DataType(DataType.PostalCode)]
    public string PostalCode { get; set; } = null!;

    [Required]
    [DataType(DataType.Text)]
    public string CityName { get; set; } = null!;

    [Required]
    [DataType(DataType.Text)]
    public string MapId { get; set; } = null!;

    [DataType(DataType.Text)]
    public string? CarDirection { get; set; }

    [DataType(DataType.Text)]
    public string? MetroDirection { get; set; }

    [DataType(DataType.Text)]
    public string? BusDirection { get; set; }
}
