using System.ComponentModel.DataAnnotations;

namespace SimpleHelpDeskAPI.Models
{
    public class UserForm
    {
        [StringLength(15)]
        [Required]
        public string? FirstName { get; set; }
        [StringLength(15)]
        public string? MiddleName { get; set; }
        [StringLength(15)]
        public string? LastName { get; set; }
        [Required]
        public DateOnly BirthDate { get; set; }
        [RegularExpression("^\\d+$")]
        [StringLength(15)]
        public string? PhoneNumber { get; set; }
        [EmailAddress]
        [StringLength(60)]
        [Required]
        public string? Email { get; set; }
        [StringLength(20, MinimumLength = 8)]
        [Required]
        public string? Password { get; set; }
    }
}