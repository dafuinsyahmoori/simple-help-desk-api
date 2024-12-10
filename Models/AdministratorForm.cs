using System.ComponentModel.DataAnnotations;

namespace SimpleHelpDeskAPI.Models
{
    public class AdministratorForm
    {
        [StringLength(15)]
        [Required]
        public string? FirstName { get; set; }
        [StringLength(15)]
        public string? LastName { get; set; }
        [EmailAddress]
        [StringLength(60)]
        [Required]
        public string? Email { get; set; }
        [StringLength(20, MinimumLength = 8)]
        [Required]
        public string? Password { get; set; }
    }
}