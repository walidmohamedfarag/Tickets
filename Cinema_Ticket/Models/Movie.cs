namespace Cinema_Ticket.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public string Img { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime Date_Time { get; set; }
        public bool Status { get; set; }
        public string MainImg { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public int CinemaId { get; set; }
        public Cinema? Cinema { get; set; }

    }
}
