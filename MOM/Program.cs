using MOM.Services;
using MOM.Filters;

namespace MOM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews(options =>
            {
                // Add global authentication filter
                options.Filters.Add<AuthenticationFilter>();
            });

            // Add session support for authentication
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "MOM.Session";
            });

            // Add memory cache for session storage
            builder.Services.AddMemoryCache();

            var app = builder.Build();

            // Initialize DataService
            try
            {
                var connectionString = app.Configuration.GetConnectionString("DefaultConnection");
                if (!string.IsNullOrEmpty(connectionString))
                {
                    DataService.Initialize(connectionString);
                    Console.WriteLine("DataService initialized successfully with database connection.");
                }
                else
                {
                    Console.WriteLine("No connection string found. Initializing DataService with static data.");
                    DataService.InitializeWithFallback();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DataService initialization failed: {ex.Message}");
                Console.WriteLine("Falling back to static data initialization.");
                DataService.InitializeWithFallback();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            
            // Enable session middleware
            app.UseSession();
            
            app.UseAuthorization();

            // Set default route to Auth/Login
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Login}/{id?}");

            Console.WriteLine("MOM Application starting...");
            Console.WriteLine("Navigate to: https://localhost:5001 or http://localhost:5000");
            Console.WriteLine("Demo Credentials:");
            Console.WriteLine("  Admin: admin / admin123");
            Console.WriteLine("  Manager: manager / manager123");
            Console.WriteLine("  User: user / user123");

            app.Run();
        }
    }
}
