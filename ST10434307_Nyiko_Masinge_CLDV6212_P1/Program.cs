using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ST10434307_Nyiko_Masinge_CLDV6212_P1.Data;
using ST10434307_Nyiko_Masinge_CLDV6212_P1.Models;
using ST10434307_Nyiko_Masinge_CLDV6212_P1.Services;
using ST10434307_Nyiko_Masinge_CLDV6212_P1.Services.Storage;

namespace ST10434307_Nyiko_Masinge_CLDV6212_P1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure logging first to capture startup errors
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Logging.AddEventSourceLogger();

            // Add services to the container
            builder.Services.AddControllersWithViews();

            // Configure Entity Framework with SQL Server for Identity
            var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(defaultConnection))
            {
                throw new InvalidOperationException("DefaultConnection string is missing from configuration. Please ensure it's set in appsettings.json or Azure App Service configuration.");
            }

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(defaultConnection, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                    sqlOptions.CommandTimeout(60);
                });
            });

            // Configure Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Configure cookie authentication
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
            });

            // Retrieve the connection string from appsettings.json
            var storageConnectionString = builder.Configuration.GetConnectionString("storageConnectionString");
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                throw new InvalidOperationException("Storage connection string is missing from configuration. Please ensure 'storageConnectionString' is set in appsettings.json or Azure App Service configuration.");
            }

            // Register storage services as singletons
            // Using factory pattern to delay creation until first use
            // This helps with startup resilience
            builder.Services.AddSingleton(provider =>
            {
                try
                {
                    return new TableStorageServices(storageConnectionString, "customer");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to create TableStorageServices. Please verify the storage connection string is valid. Error: {ex.Message}", ex);
                }
            });
            
            builder.Services.AddSingleton(provider =>
            {
                try
                {
                    return new BlobStorageServices(storageConnectionString, "customer-profile-pictures");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to create BlobStorageServices. Please verify the storage connection string is valid. Error: {ex.Message}", ex);
                }
            });
            
            builder.Services.AddSingleton(provider =>
            {
                try
                {
                    return new QueueStorageServices(storageConnectionString, "customer-management-logs-messages");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to create QueueStorageServices. Please verify the storage connection string is valid. Error: {ex.Message}", ex);
                }
            });
            
            builder.Services.AddSingleton(provider =>
            {
                try
                {
                    return new FileShareStorageServices(storageConnectionString, "customer-logs-file");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to create FileShareStorageServices. Please verify the storage connection string is valid. Error: {ex.Message}", ex);
                }
            });

            // Register product and order storage services
            builder.Services.AddSingleton(provider =>
            {
                try
                {
                    return new ProductStorageService(storageConnectionString, "products");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to create ProductStorageService. Please verify the storage connection string is valid. Error: {ex.Message}", ex);
                }
            });
            
            builder.Services.AddSingleton(provider =>
            {
                try
                {
                    return new OrderStorageService(storageConnectionString, "orders");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to create OrderStorageService. Please verify the storage connection string is valid. Error: {ex.Message}", ex);
                }
            });

            // Register FunctionService with HttpClient
            builder.Services.AddHttpClient<FunctionService>();

            // Add session support for shopping cart
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure logging for the application
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Application starting up...");

            // Ensures database is created and seed admin user with error handling
            try
            {
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var scopeLogger = services.GetRequiredService<ILogger<Program>>();

                    scopeLogger.LogInformation("Checking database connection...");

                    // Test database connection and create if needed
                    try
                    {
                        if (await context.Database.CanConnectAsync())
                        {
                            scopeLogger.LogInformation("Database connection successful. Ensuring database is created...");
                            context.Database.EnsureCreated();
                            scopeLogger.LogInformation("Database ensured created successfully.");
                        }
                        else
                        {
                            scopeLogger.LogWarning("Cannot connect to database. Attempting to create database...");
                            context.Database.EnsureCreated();
                            scopeLogger.LogInformation("Database created successfully.");
                        }
                    }
                    catch (Exception dbEx)
                    {
                        scopeLogger.LogError(dbEx, "Database connection or creation failed. This may be due to network issues, incorrect connection string, or SQL Server firewall settings. Please verify your Azure SQL Database connection string and firewall rules.");
                        throw; // Re-throw to be caught by outer try-catch
                    }

                    // Create roles
                    scopeLogger.LogInformation("Checking roles...");
                    if (!await roleManager.RoleExistsAsync("Admin"))
                    {
                        await roleManager.CreateAsync(new IdentityRole("Admin"));
                        scopeLogger.LogInformation("Admin role created.");
                    }
                    if (!await roleManager.RoleExistsAsync("Customer"))
                    {
                        await roleManager.CreateAsync(new IdentityRole("Customer"));
                        scopeLogger.LogInformation("Customer role created.");
                    }

                    // Create default admin user
                    scopeLogger.LogInformation("Checking admin user...");
                    if (await userManager.FindByEmailAsync("admin@abcretail.com") == null)
                    {
                        var adminUser = new ApplicationUser
                        {
                            UserName = "admin@abcretail.com",
                            Email = "admin@abcretail.com",
                            EmailConfirmed = true,
                            FirstName = "Admin",
                            LastName = "User"
                        };
                        var result = await userManager.CreateAsync(adminUser, "Admin@123");
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(adminUser, "Admin");
                            scopeLogger.LogInformation("Admin user created successfully.");
                        }
                        else
                        {
                            scopeLogger.LogWarning("Failed to create admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                        }
                    }
                    else
                    {
                        scopeLogger.LogInformation("Admin user already exists.");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database. The application will continue, but some features may not work correctly.");
                // Don't throw - allow the app to start even if database initialization fails
                // This helps with troubleshooting as the app can still show error pages
            }

            // Configured the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            // Map routes
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Store}/{action=Index}/{id?}");

            logger.LogInformation("Application configured successfully. Starting web server...");

            app.Run();
        }
    }
}
