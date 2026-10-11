using F1Tickets.Api.Domain.Enums;

namespace F1Tickets.Api.Features.Auth.DTOs
{
    public class LoginRequestDto
    {
        public string Email { get; set; } 
        public string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } 
        public string Name { get; set; } 
        public string Surname { get; set; } 
        public string Email { get; set; } 
        public string Role { get; set; } 
    }
}
