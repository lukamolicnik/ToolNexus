﻿using Microsoft.EntityFrameworkCore;
using ToolNexus.Domain.Users;

namespace ToolNexus.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public UserRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Users
                .Include(u => u.UserRole)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Users
                .Include(u => u.UserRole)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Users
                .Include(u => u.UserRole)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            
            // Poiščemo obstoječega uporabnika v trenutnem kontekstu
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            if (existingUser == null)
            {
                throw new InvalidOperationException("Uporabnik ne obstaja");
            }

            // Posodobimo samo potrebne lastnosti
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.UserRoleId = user.UserRoleId;
            existingUser.IsActive = user.IsActive;
            existingUser.PasswordHash = user.PasswordHash;
            existingUser.UpdatedBy = user.UpdatedBy;
            existingUser.UpdatedAt = user.UpdatedAt;

            await context.SaveChangesAsync();
            
            // Vrnemo posodobljeno entiteto z vključeno UserRole
            return await context.Users
                .Include(u => u.UserRole)
                .FirstAsync(u => u.Id == user.Id);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return false;

            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Users
                .AnyAsync(u => u.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Users
                .AnyAsync(u => u.Email == email);
        }

        public async Task<UserRole?> GetUserRoleByIdAsync(int roleId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.UserRoles
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }

        public async Task<List<UserRole>> GetAllUserRolesAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.UserRoles
                .OrderBy(r => r.Id)
                .ToListAsync();
        }
    }
}
