using Launcher.Models;
using Launcher.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Launcher.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ResponseModel<string>> Login([FromBody]LoginModel data)
        {
            string userIp = HttpContext.Connection.RemoteIpAddress.ToString();

            return await _authService.Login(data, userIp);
        }

        [HttpPost("verify")]
        public async Task<ResponseModel<bool>> Verify([FromBody] string Email)
        {
            return await _authService.Verify(Email);
        }

        [HttpPost("register")]
        public async Task<ResponseModel<bool>> Register([FromBody] RegisterModel data)
        {
            return await _authService.Register(data);
        }

        [HttpPost("start")]
        public async Task<ResponseModel<LoginParamsResponse>> StartGame([FromBody] string token)
        {
            string userIp = HttpContext.Connection.RemoteIpAddress.ToString();

            return await _authService.StartGame(token, userIp);
        }
    }
}
