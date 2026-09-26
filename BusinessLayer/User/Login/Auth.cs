using BusinessLayer.User.Login.LoginDto;
using DataLayer.Data;
using DataLayer.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BusinessLayer.User.Login
{
    public class Auth
    {
        private readonly SiSDBDbContext _context;

        public Auth(SiSDBDbContext context)
        {
            _context = context;
        }

        private static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public async Task<TokenResponseDto> LoginAsync(LoginRequestDto request)
        {

            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null||!user.IsActive)
            {
                return new TokenResponseDto { IsAuthenticated = false };
            }

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isValidPassword)
            {
                return new TokenResponseDto { IsAuthenticated = false };
            }

            var claims = new[]
            {
                new Claim("id", user.UserId.ToString()),
                new Claim("role", user.Role.RoleName),
                new Claim("email",user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SIS_SYSTEM_SECRET_KEY_FOR_JWT_AUTHENTICATION_2026"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "UserApi",
                audience: "ApiUsers",
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: creds
            );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = GenerateRefreshToken();
            user.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            user.RefreshTokenRevokedAt = null;

            await _context.SaveChangesAsync();


            return new TokenResponseDto
            {
                IsAuthenticated = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(RefreshRequestDto request)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null||!user.IsActive) {
                return new TokenResponseDto { IsAuthenticated = false };
            }

            if (user.RefreshTokenRevokedAt != null)
            {
                return new TokenResponseDto { IsAuthenticated = false };
            }

            if(user.RefreshTokenExpiresAt==null||user.RefreshTokenExpiresAt <= DateTime.UtcNow)
            {
                return new TokenResponseDto { IsAuthenticated = false };
            }

            bool refreshValid = !string.IsNullOrEmpty(user.RefreshTokenHash) && BCrypt.Net.BCrypt.Verify(request.RefreshToken, user.RefreshTokenHash);
            if (!refreshValid)
            {
                return new TokenResponseDto { IsAuthenticated = false };
            }


            var claims = new[]
            {
                new Claim("id", user.UserId.ToString()),
                new Claim("role", user.Role.RoleName),
                new Claim("email",user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SIS_SYSTEM_SECRET_KEY_FOR_JWT_AUTHENTICATION_2026"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "UserApi",
                audience: "ApiUsers",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = GenerateRefreshToken();
            user.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            user.RefreshTokenRevokedAt = null;

            await _context.SaveChangesAsync();


            return new TokenResponseDto
            {
                IsAuthenticated = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

        }

        public bool Logout(LogoutRequestDto request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
            {
                return false;
            }

            bool refreshValid = !string.IsNullOrEmpty(user.RefreshTokenHash) && BCrypt.Net.BCrypt.Verify(request.RefreshToken, user.RefreshTokenHash);

            if (!refreshValid)
            {
                return false; 
            }


            user.RefreshTokenRevokedAt = DateTime.UtcNow;
            user.RefreshTokenHash = null;
            _context.SaveChanges();
            return true;

        }
    }
}