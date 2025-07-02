using System.Data;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using ToolNexus.Application.Users.DTOs;
using ToolNexus.Domain.Users;

namespace ToolNexus.Application.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            return user != null ? UserDto.FromUser(user) : null;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return users.Select(UserDto.FromUser).ToList();
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto, string createdBy)
        {
            // Preveri, če uporabniško ime že obstaja
            if (await _userRepository.UsernameExistsAsync(createUserDto.Username))
            {
                throw new InvalidOperationException("Uporabniško ime že obstaja");
            }

            // Preveri, če email že obstaja
            if (await _userRepository.EmailExistsAsync(createUserDto.Email))
            {
                throw new InvalidOperationException("Email že obstaja");
            }

            // Ustvari novega uporabnika
            var user = new User
            {
                Username = createUserDto.Username,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email,
                PasswordHash = this.HashPassword(createUserDto.Password),
                UserRoleId = createUserDto.UserRoleId,
                IsActive = createUserDto.IsActive,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateUserAsync(user);
            return UserDto.FromUser(createdUser);
        }

        public async Task<UserDto> UpdateUserAsync(UpdateUserDto updateUserDto, string updatedBy)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(updateUserDto.Id);
            if (existingUser == null)
            {
                throw new InvalidOperationException("Uporabnik ne obstaja");
            }

            // Preveri, če se email spreminja in če novi email že obstaja
            if (existingUser.Email != updateUserDto.Email)
            {
                if (await _userRepository.EmailExistsAsync(updateUserDto.Email))
                {
                    throw new InvalidOperationException("Email že obstaja");
                }
            }

            // Posodobi podatke
            existingUser.FirstName = updateUserDto.FirstName;
            existingUser.LastName = updateUserDto.LastName;
            existingUser.Email = updateUserDto.Email;
            existingUser.UserRoleId = updateUserDto.UserRoleId;
            existingUser.IsActive = updateUserDto.IsActive;
            existingUser.UpdatedBy = updatedBy;
            existingUser.UpdatedAt = DateTime.UtcNow;

            // Posodobi geslo, če je podano
            if (!string.IsNullOrWhiteSpace(updateUserDto.NewPassword))
            {
                existingUser.PasswordHash = this.HashPassword(updateUserDto.NewPassword);
            }

            var updatedUser = await _userRepository.UpdateUserAsync(existingUser);
            return UserDto.FromUser(updatedUser);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _userRepository.UsernameExistsAsync(username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userRepository.EmailExistsAsync(email);
        }

        public async Task<AuthenticationResult> AuthenticateAsync(LoginDto loginDto)
        {
            // Pridobi uporabnika po uporabniškem imenu
            var user = await _userRepository.GetUserByUsernameAsync(loginDto.Username);

            if (user == null)
            {
                return AuthenticationResult.FailureResult("Nepravilno uporabniško ime ali geslo");
            }

            // Preveri, če je uporabnik aktiven
            if (!user.IsActive)
            {
                return AuthenticationResult.FailureResult("Uporabniški račun je onemogočen");
            }

            // Preveri geslo
            if (!VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return AuthenticationResult.FailureResult("Nepravilno uporabniško ime ali geslo");
            }

            // Uspešna prijava
            var userDto = UserDto.FromUser(user);
            return AuthenticationResult.SuccessResult(userDto);
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        public bool VerifyPassword(string password, string hash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == hash;
        }

        public async Task<UserRoleDto> GetUserRoleByIdAsync(int roleId)
        {
            var userRole = await _userRepository.GetUserRoleByIdAsync(roleId);

            return userRole != null ? UserRoleDto.FromUserRole(userRole) : null;
        }

        public async Task<List<UserRoleDto>> GetAllUserRolesAsync()
        {
            var userRoles = await _userRepository.GetAllUserRolesAsync();
            return userRoles.Select(UserRoleDto.FromUserRole).ToList();
        }
    }
}
