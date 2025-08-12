using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using ToolNexus.Domain.Users;

namespace ToolNexus.Infrastructure
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            
            // Create context without audit interceptor for seeding
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            
            using var context = new ApplicationDbContext(optionsBuilder.Options);

            try
            {
                // Zagotovi, da baza obstaja
                //await context.Database.EnsureCreatedAsync();

                UserRole adminRole, supervisorRole, workerRole;

                // Preveri, če že obstajajo role
                if (!await context.UserRoles.AnyAsync())
                {
                    logger.LogInformation("Ustvarjam vloge...");
                    
                    adminRole = new UserRole
                    {
                        Name = "Administrator",
                        Code = "ADMIN",
                        Description = "Sistem administrator",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    
                    supervisorRole = new UserRole
                    {
                        Name = "Nadzornik Proizvodnje",
                        Code = "SUPERVISOR",
                        Description = "Nadzornik proizvodnje",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    workerRole = new UserRole
                    {
                        Name = "Delavec",
                        Code = "WORKER",
                        Description = "Osnovni delavec",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    context.UserRoles.Add(adminRole);
                    context.UserRoles.Add(supervisorRole);
                    context.UserRoles.Add(workerRole);

                    await context.SaveChangesAsync();
                }
                else
                {
                    logger.LogInformation("Vloge že obstajajo, pridobivam obstoječe...");
                    adminRole = await context.UserRoles.FirstAsync(r => r.Code == "ADMIN");
                    supervisorRole = await context.UserRoles.FirstAsync(r => r.Code == "SUPERVISOR");
                    workerRole = await context.UserRoles.FirstAsync(r => r.Code == "WORKER");
                }


                // Preveri, če že obstajajo uporabniki
                if (await context.Users.AnyAsync())
                {
                    logger.LogInformation("Uporabniki že obstajajo, preskačem seed podatke.");
                    return;
                }

                // Ustvari privzetega administratorja
                var adminUser = new User
                {
                    Username = "admin",
                    FirstName = "Admin",
                    LastName = "Administrator",
                    Email = "admin@toolnexus.si",
                    PasswordHash = HashPassword("admin123"), // Privzeto geslo
                    UserRoleId = adminRole.Id,
                    IsActive = true,
                    CreatedBy = null,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                context.Users.Add(adminUser);

                // Dodaj testnega nadzornika proizvodnje
                var supervisorUser = new User
                {
                    Username = "supervisor",
                    FirstName = "Janez",
                    LastName = "Nadzornik",
                    Email = "supervisor@toolnexus.si",
                    PasswordHash = HashPassword("supervisor123"),
                    UserRoleId = supervisorRole.Id,
                    IsActive = true,
                    CreatedBy = null,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                context.Users.Add(supervisorUser);

                // Dodaj testnega delavca
                var workerUser = new User
                {
                    Username = "worker",
                    FirstName = "Marko",
                    LastName = "Delavec",
                    Email = "worker@toolnexus.si",
                    PasswordHash = HashPassword("worker123"),
                    UserRoleId = workerRole.Id,
                    IsActive = true,
                    CreatedBy = null,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                context.Users.Add(workerUser);

                await context.SaveChangesAsync();

                logger.LogInformation("Uspešno ustvarjeni začetni uporabniki:");
                logger.LogInformation("- Administrator: admin / admin123");
                logger.LogInformation("- Nadzornik: supervisor / supervisor123");
                logger.LogInformation("- Delavec: worker / worker123");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Napaka pri ustvarjanju začetnih podatkov");
                throw;
            }
        }

        private static string HashPassword(string password)
        {
            // Uporabi BCrypt z work factor 12 za varno hashiranje (enako kot v UserService)
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }
    }
}
