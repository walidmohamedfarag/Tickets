namespace Cinema_Ticket.Models.ViewModel
{
    public class MovieDetailVM
    {
        public Movie? Movie { get; set; }
        public IEnumerable<MovieActor>? Actors { get; set; }
        public IEnumerable<Movie>? MoviesWithSameType { get; set; }
    }
}
