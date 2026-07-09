namespace MbtiApi.Application.DTOs.Response
{
    //Traditional ViewModel approach
    public record RegisterRequestDto(string Username, string Email, string Password);
    public record LoginRequestDto(string Email, string Password);
    public record AuthResponseDto(string Username, string Email, string Token);
}
