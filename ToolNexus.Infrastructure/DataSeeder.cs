using Microsoft.EntityFrameworkCore;
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
            var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            
            using var context = await contextFactory.CreateDbContextAsync();

            try
            {
                // Zagotovi, da baza obstaja
                //await context.Database.EnsureCreatedAsync();

                // Preveri, če že obstajajo role
                if (await context.UserRoles.AnyAsync())
                {
                    logger.LogInformation("Vloge že obstajajo, preskačem seed podatke.");
                    return;
                }

                var adminRole = new UserRole
                {
                    Name = "Administrator",
                    Code = "ADMIN",
                    Description = "Sistem administrator",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                
                var supervisorRole = new UserRole
                {
                    Name = "Nadzornik Proizvodnje",
                    Code = "SUPERVISOR",
                    Description = "Nadzornik proizvodnje",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var workerRole = new UserRole
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
                    CreatedBy = "System",
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
                    CreatedBy = "System",
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
                    CreatedBy = "System",
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
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
