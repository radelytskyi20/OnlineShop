using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Library.Clients.IdentityServer;
using OnlineShop.Library.Constants;
using OnlineShop.Library.Logging;
using OnlineShop.Library.Options;

namespace OnlineShop.UserManagementService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IIdentityServerClient _identityServerClient;

        public LoginController(ILogger<LoginController> logger, IIdentityServerClient identityServerClient)
        {
            _logger = logger;
            _identityServerClient = identityServerClient;
        }

        [HttpPost(LoginControllerRoutes.ByClientSecret)]
        public async Task<IActionResult> GetApiTokenByClientSecret([FromBody] IdentityServerApiOptions options)
        {
            try
            {
                var token = await _identityServerClient.GetApiToken(options);
                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(LoginController))
                    .WithMethod(nameof(GetApiTokenByClientSecret))
                    .WithComment(ex.ToString())
                    .WithOperation(LoginControllerRoutes.ByClientSecret)
                    .ToString()
                    );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpPost(LoginControllerRoutes.ByUsernameAndPassword)]
        public async Task<IActionResult> GetApiTokenByUsernameAndPassword([FromBody] IdentityServerUserNamePassword options)
        {
            try
            {
                var token = await _identityServerClient.GetApiToken(options);
                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(LoginController))
                    .WithMethod(nameof(GetApiTokenByUsernameAndPassword))
                    .WithComment(ex.ToString())
                    .WithOperation(LoginControllerRoutes.ByUsernameAndPassword)
                    .ToString()
                    );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }
    }
}
