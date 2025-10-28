namespace Cinema_Ticket.Models
{
    public enum MovieType
    {
        Action,
        Comedy,
        Drama,
        Horror,
        SciFi,
        Romance
    }
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; } 
        public decimal Price { get; set; }
        public DateTime Date_Time { get; set; }
        public bool Status { get; set; }
        public string? MainImg { get; set; } 
        public string Type { get; set; } = string.Empty;
        public int CinemaId { get; set; }
        public Cinema? Cinema { get; set; }

    }
}
