using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Library.Clients.UserManagementService;
using OnlineShop.Library.Options;
using OnlineShop.Library.UserManagementService.Requests;

namespace OnlineShop.ApiService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly ILoginClient _loginClient;

        public LoginController(ILoginClient loginClient) { _loginClient = loginClient; }

        [AllowAnonymous]
        [HttpPost("")]
        public async Task<IActionResult> LoginByUserNameAndPassword([FromForm] LoginRequest request)
        {
            var options = new IdentityServerUserNamePassword()
            {
                UserName = request.UserName,
                Password = request.Password,
            };

            var token = await _loginClient.GetApiTokenByUsernameAndPassword(options);
            return Ok(token);
        }
    }
}
