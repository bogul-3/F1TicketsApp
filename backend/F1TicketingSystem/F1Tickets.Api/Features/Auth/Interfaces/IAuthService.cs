using F1Tickets.Api.Features.Auth.DTOs;

namespace F1Tickets.Api.Features.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync (LoginRequestDto LoginRequestDto);
        Task<SignupResponseDto> SignUpAsync(SignupRequestDto SignupRequestDto);
    }
}
