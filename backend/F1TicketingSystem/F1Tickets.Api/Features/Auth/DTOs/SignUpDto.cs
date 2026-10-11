using F1Tickets.Api.Domain.Enums;

namespace F1Tickets.Api.Features.Auth.DTOs
{
    public class SignupRequestDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } 
        public string Surname { get; set; } 
        public string Email { get; set; } 
        public string Password { get; set; } 
        public string Address { get; set; } 
        public string PostalCode { get; set; } 
        public Guid CityId { get; set; }
    }

    public class SignupResponseDto
    {
        public string Token { get; set; } 
        public string Name { get; set; } 
        public string Surname { get; set; } 
        public string Email { get; set; } 
        public string Role { get; set; } 
    }
}
