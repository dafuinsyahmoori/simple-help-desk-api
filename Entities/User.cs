namespace SimpleHelpDeskAPI.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateOnly BirthDate { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

        public ICollection<ComplaintRequest> ComplaintRequests { get; set; } = new List<ComplaintRequest>();
    }
}