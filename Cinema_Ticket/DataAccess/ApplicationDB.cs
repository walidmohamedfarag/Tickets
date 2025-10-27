

namespace Cinema_Ticket.DataAccess
{
    public class ApplicationDB : DbContext
    {
        public ApplicationDB(DbContextOptions<ApplicationDB> options) : base(options)
        {
            
        }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieActor> MovieActors { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<MovieSubImg> MovieSubImgs { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    =>optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=CinemaTicket;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MovieActorConfigration).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
