namespace Cinema_Ticket.Models
{
    public class Cart
    {
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
