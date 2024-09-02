using System.ComponentModel.DataAnnotations;

namespace NZWalks.API.Models.DTO
{
    public class WalkDto
    {
        public Guid Id { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Name has to be a minimum of 3 characters")]
        [MaxLength(50, ErrorMessage = "Name has to be a maximum of 50 characters")]
        public string Name { get; set; }
        public string Description { get; set; }

        public double LengthInKm { get; set; }
        public string? WalkImageUrl { get; set; }
        [Required]
        public Guid DifficultyId { get; set; }
        [Required]
        public Guid RegionId { get; set; }

        public RegionDto Region { get; set; }
        public DifficultyDto Difficulty { get; set; }
    }
}
