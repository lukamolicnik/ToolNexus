using ToolNexus.Application.Users.DTOs;
using ToolNexus.Domain.Users;

namespace ToolNexus.Application.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticationService _authenticationService;

        public UserService(IUserRepository userRepository, IAuthenticationService authenticationService)
        {
            _userRepository = userRepository;
            _authenticationService = authenticationService;
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
                PasswordHash = _authenticationService.HashPassword(createUserDto.Password),
                Role = createUserDto.Role,
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
            existingUser.Role = updateUserDto.Role;
            existingUser.IsActive = updateUserDto.IsActive;
            existingUser.UpdatedBy = updatedBy;
            existingUser.UpdatedAt = DateTime.UtcNow;

            // Posodobi geslo, če je podano
            if (!string.IsNullOrWhiteSpace(updateUserDto.NewPassword))
            {
                existingUser.PasswordHash = _authenticationService.HashPassword(updateUserDto.NewPassword);
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
    }
}
