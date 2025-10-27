
namespace Cinema_Ticket
{
    public static class AppConfiguration
    {
        public static void RegisterConfig(this IServiceCollection services , string connection)
        {
            services.AddScoped<IRepositroy<Cinema>, Repositroy<Cinema>>();
            services.AddScoped<IRepositroy<Movie>, Repositroy<Movie>>();
            services.AddScoped<IRepositroy<MovieActor>, Repositroy<MovieActor>>();
            services.AddScoped<IRepositroy<Actor>, Repositroy<Actor>>();
            services.AddScoped<IRepositroy<MovieSubImg>, Repositroy<MovieSubImg>>();
            services.AddDbContext<ApplicationDB>(option =>
            {
                option.UseSqlServer(connection);
            });

        }
    }
}
