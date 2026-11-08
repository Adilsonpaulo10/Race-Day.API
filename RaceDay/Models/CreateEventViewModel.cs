using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models
{
    public class CreateEventViewModel
    {
        [Required]
        public string EventName { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Upcoming";
    }
}