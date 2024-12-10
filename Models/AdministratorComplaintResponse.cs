using System.ComponentModel.DataAnnotations;

namespace SimpleHelpDeskAPI.Models
{
    public class AdministratorComplaintResponse
    {
        [StringLength(150)]
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Content { get; set; }
    }
}