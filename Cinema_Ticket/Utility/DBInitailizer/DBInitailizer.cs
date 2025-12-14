
using Microsoft.IdentityModel.Tokens;

namespace Cinema_Ticket.Utility.DBInitailizer
{
    public class DBInitailizer : IDBInitailizer
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDB contextDB;
        private readonly ILogger<ApplicationDB> loger;

        public DBInitailizer(UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager, ApplicationDB _contextDB, ILogger<ApplicationDB> _loger)
        {
            userManager = _userManager;
            roleManager = _roleManager;
            contextDB = _contextDB;
            loger = _loger;
        }

        public void Initialize()
        {
            try
            {

                if (contextDB.Database.GetPendingMigrations().Any())
                    contextDB.Database.Migrate();
                if (roleManager.Roles is null)
                {
                    roleManager.CreateAsync(new(StaticRole.SUPER_ADMIN)).GetAwaiter().GetResult();
                    roleManager.CreateAsync(new(StaticRole.ADMIN)).GetAwaiter().GetResult();
                    roleManager.CreateAsync(new(StaticRole.EMPLOYEE)).GetAwaiter().GetResult();
                    roleManager.CreateAsync(new(StaticRole.CUSTOMER)).GetAwaiter().GetResult();
                }
                var user = new ApplicationUser()
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    UserName = "superadmin",
                    Email = "superadmin@gmail.com",
                    EmailConfirmed = true
                };
                userManager.CreateAsync(user, "SuperAdmin123").GetAwaiter().GetResult();
                userManager.AddToRoleAsync(user, StaticRole.SUPER_ADMIN).GetAwaiter().GetResult();
            }
            catch(Exception ex)
            {
                loger.LogError($"Error: {ex.Message}");
            }
        }
    }
}
