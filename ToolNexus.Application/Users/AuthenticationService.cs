using System.Security.Cryptography;
using System.Text;
using ToolNexus.Application.Users.DTOs;
using ToolNexus.Domain.Users;

namespace ToolNexus.Application.Users
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
    }
}
