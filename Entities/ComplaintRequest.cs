namespace SimpleHelpDeskAPI.Entities
{
    public class ComplaintRequest
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ImageOrVideoUrl { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = null!;

        public ComplaintResponse? ComplaintResponse { get; set; }
    }
}