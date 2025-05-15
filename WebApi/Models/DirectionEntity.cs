using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    public class DirectionEntity
    {
        [Key, ForeignKey(nameof(Location))]
        public string LocationId { get; set; } = null!;
        public LocationEntity Location { get; set; } = null!;

        public string? Car { get; set; }
        public string? Metro { get; set; }
        public string? Bus { get; set; }
    }
}
