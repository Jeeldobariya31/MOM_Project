using MOM.Services;

namespace MOM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add session support for authentication
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Initialize DataService with connection string
            var connectionString = app.Configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(connectionString))
            {
                try
                {
                    DataService.Initialize(connectionString);
                    Console.WriteLine("DataService initialized successfully with database connection.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: DataService initialization failed: {ex.Message}");
                    Console.WriteLine("Falling back to static data initialization.");
                    DataService.InitializeWithFallback();
                }
            }
            else
            {
                Console.WriteLine("Warning: No connection string found. Initializing DataService with static data only.");
                DataService.InitializeWithFallback();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
