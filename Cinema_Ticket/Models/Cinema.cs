namespace Cinema_Ticket.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Img { get; set; }
        public string? PublicId { get; set; }
    }
}
