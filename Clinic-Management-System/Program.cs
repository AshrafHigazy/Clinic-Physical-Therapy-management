using Clinic_Management_System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Management_System
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ================================
            // 1. Configure Database
            // ================================
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // ================================
            // 2. Identity + Roles
            // ================================
            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // ================================
            // 3. SEED ROLES
            // ================================
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roles = { "AdminDoctor", "Secretary" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }

            // ================================
            // 4. SEED USERS (Doctor + Secretary)
            // ================================
            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var users = new List<(string Email, string Password, string Role)>
                {
                    // Doctor
                    ("Ageza@d.com", "Doctor500*", "AdminDoctor"),

                    // Secretary
                    ("Asmaa@r.com", "Asmaa2000*", "Secretary"),
                };

                foreach (var u in users)
                {
                    var exist = await userManager.FindByEmailAsync(u.Email);

                    if (exist == null)
                    {
                        var user = new IdentityUser
                        {
                            UserName = u.Email,
                            Email = u.Email,
                            EmailConfirmed = true
                        };

                        var result = await userManager.CreateAsync(user, u.Password);

                        if (result.Succeeded)
                            await userManager.AddToRoleAsync(user, u.Role);
                    }
                }
            }

            // ================================
            // 5. HTTP Pipeline Config
            // ================================
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // ================================
            // 6. DEFAULT ROUTE
            // ================================
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.MapRazorPages();

            await app.RunAsync();
        }
    }
}
