using F1Tickets.Api.Domain.Entities;
using F1Tickets.Api.Domain.Enums;
using F1Tickets.Api.Features.Auth.DTOs;
using F1Tickets.Api.Features.Auth.Interfaces;
using F1Tickets.Api.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace F1Tickets.Api.Features.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly F1TicketsDbContext _db;
        private readonly JwtService _jwt;

        public AuthService(F1TicketsDbContext db, JwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == loginRequestDto.Email);

            if (user == null || !PasswordHelper.VerifyPassword(loginRequestDto.Password, user.Password))
                throw new Exception("Invalid email or password"); 

            var token = _jwt.GenerateToken(user);

            return new LoginResponseDto
            {
                UserId = user.Id,
                Token = token,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public async Task<SignupResponseDto> SignUpAsync(SignupRequestDto SignupRequestDto)
        {
            // 1️⃣ Provjeri da li email već postoji
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == SignupRequestDto.Email);
            if (existingUser != null)
                throw new Exception("Email already registered");

            // 2️⃣ Hashiraj lozinku
            var hashedPassword = PasswordHelper.HashPassword(SignupRequestDto.Password);

            // 3️⃣ Kreiraj user entitet
            var user = new User
            {
                Name = SignupRequestDto.Name,
                Surname = SignupRequestDto.Surname,
                Email = SignupRequestDto.Email,
                Password = hashedPassword,
                Address = SignupRequestDto.Address,
                PostalCode = SignupRequestDto.PostalCode,
                CityId = SignupRequestDto.CityId,
                Role = UserRole.USER
            };

            // 4️⃣ Spremi u bazu
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // 5️⃣ Generiši JWT token
            var token = _jwt.GenerateToken(user);

            return new SignupResponseDto
            {
                Token = token,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Role = user.Role.ToString(),
            };
        }
    }
}
