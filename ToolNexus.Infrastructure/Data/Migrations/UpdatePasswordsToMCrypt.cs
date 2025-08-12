using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ToolNexus.Application.Users;
using ToolNexus.Infrastructure.Data;

namespace ToolNexus.Infrastructure.Data.Migrations
{
    public class UpdatePasswordsToBCrypt
    {
        public static async Task UpdateExistingPasswords(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            // Pridobi vse uporabnike
            var users = await dbContext.Users.ToListAsync();

            foreach (var user in users)
            {
                // Preveri če je geslo že v BCrypt formatu
                if (!user.PasswordHash.StartsWith("$2"))
                {
                    // Posodobi geslo na BCrypt format
                    // Opomba: To bo delovalo samo za testne uporabnike z znanimi gesli
                    string newPassword = user.Username switch
                    {
                        "admin" => "admin123",
                        "supervisor" => "supervisor123",
                        "worker" => "worker123",
                        _ => null
                    };

                    if (newPassword != null)
                    {
                        user.PasswordHash = userService.HashPassword(newPassword);
                        user.UpdatedAt = DateTime.Now;
                        user.UpdatedBy = null;
                    }
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}