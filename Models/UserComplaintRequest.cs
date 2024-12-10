using System.ComponentModel.DataAnnotations;

namespace SimpleHelpDeskAPI.Models
{
    public class UserComplaintRequest
    {
        [StringLength(150)]
        [Required]
        public string? Title { get; set; }
        public IFormFile? ImageOrVideo { get; set; }
        [Required]
        public string? Content { get; set; }
    }
}