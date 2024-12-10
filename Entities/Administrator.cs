namespace SimpleHelpDeskAPI.Entities
{
    public class Administrator
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

        public ICollection<ComplaintResponse> ComplaintResponses { get; set; } = new List<ComplaintResponse>();
    }
}