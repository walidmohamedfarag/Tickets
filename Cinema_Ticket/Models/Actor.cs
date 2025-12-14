namespace Cinema_Ticket.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public DateOnly BirthDate { get; set; }
        public string? Img { get; set; }
        public string? ImgPublicId { get; set; }
    }
}
