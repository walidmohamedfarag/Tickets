namespace Cinema_Ticket
{
    public class ModelVM
    {
        public Movie? Movie { get; set; }
        public IEnumerable<MovieSubImg>? subImgs { get; set; }
        public IEnumerable<Cinema>? Cinemas { get; set; }
    }
}
