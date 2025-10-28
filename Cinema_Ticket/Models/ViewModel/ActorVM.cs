namespace Cinema_Ticket.Models.ViewModel
{
    public class ActorVM
    {
        public Actor Actor { get; set; }
        public IEnumerable<Movie> Movies { get; set; }

    }
}
