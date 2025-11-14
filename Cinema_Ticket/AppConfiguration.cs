
using Cinema_Ticket.Utility;
using Cinema_Ticket.Utility.DBInitailizer;
using System.Text.RegularExpressions;

namespace Cinema_Ticket
{
    public static class AppConfiguration
    {
        public static void RegisterConfig(this IServiceCollection services , string connection)
        {
            services.AddIdentity<ApplicationUser,IdentityRole>(option=>
            {
                option.User.RequireUniqueEmail = true;
                option.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<ApplicationDB>()
            .AddDefaultTokenProviders();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IRepositroy<Cinema>, Repositroy<Cinema>>();
            services.AddScoped<IRepositroy<Movie>, Repositroy<Movie>>();
            services.AddScoped<IRepositroy<MovieActor>, Repositroy<MovieActor>>();
            services.AddScoped<IRepositroy<Actor>, Repositroy<Actor>>();
            services.AddScoped<IRepositroy<MovieSubImg>, Repositroy<MovieSubImg>>();
            services.AddScoped<IRepositroy<ApplicationUserOTP>, Repositroy<ApplicationUserOTP>>();
            services.AddScoped<IRepositroy<Cart>, Repositroy<Cart>>();
            services.AddScoped<IDBInitailizer, DBInitailizer>();
            services.AddDbContext<ApplicationDB>(option =>
            {
                option.UseSqlServer(connection);
            });

        }
        public static string TrimExtraSpace(this string word)
        {
            word = Regex.Replace(word, @"\s+", " ");
            return word.Trim();
        }
    }

}
