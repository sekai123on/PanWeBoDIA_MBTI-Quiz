namespace MbtiApi.Application.DTOs.Request
{
    public record RegisterRequestDto(string Username, string Email, string Password);
    public record LoginRequestDto(string Email, string Password);
}
