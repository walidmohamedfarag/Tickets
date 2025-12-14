namespace Cinema_Ticket.Models
{
    public class MovieSubImg
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
        public string? Img { get; set; } 
        public string? PublicId { get; set; } 
    }
}
