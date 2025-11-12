

namespace Cinema_Ticket.MapestrConfigration
{
    public static class MapConfig
    {
        public static void RegisterMaps(this IServiceCollection services)
        {
            TypeAdapterConfig<ApplicationUser, RegisterVM>.NewConfig()
                .Map(d => d.FullName, s => $"{s.FirstName} {s.LastName}").TwoWays();
        }
    }
}
