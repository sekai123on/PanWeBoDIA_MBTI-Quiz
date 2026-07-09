using MbtiApi.Application.DTOs.Request;
using MbtiApi.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MbtiApi.API.Controllers
{
    [Route("api/[controller]")] //URLကနေ api/accountဖြစ်မှာ
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")] //URLကနေ api/account/registerဖြစ်မှာ
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterAsync(request);
            if (result == null) 
            {
                return BadRequest(new { message = "User already exists." });
            }
            return Ok(result);
        }
        [HttpPost("login")] //URLကနေ api/account/loginဖြစ်မှာ
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);
            if (result == null)
            {
                return BadRequest(new { message = "Invalid Email or Password." });
            }
            return Ok(result);
        }
    }
}
