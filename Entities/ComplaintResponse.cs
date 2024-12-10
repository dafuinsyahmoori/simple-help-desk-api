namespace SimpleHelpDeskAPI.Entities
{
    public class ComplaintResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public Administrator Administrator { get; set; } = null!;

        public ComplaintRequest ComplaintRequest { get; set; } = null!;
    }
}