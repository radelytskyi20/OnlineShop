using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Library.Constants;
using OnlineShop.Library.Logging;
using OnlineShop.Library.UserManagementService.Models;
using OnlineShop.Library.UserManagementService.Requests;

namespace OnlineShop.UserManagementService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserManager<ApplicationUser> userManager, ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost(RepoActions.Add)]
        public async Task<IActionResult> Add(CreateUserRequest request)
        {
            try
            {
                _logger.LogInformation(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(Add))
                    .WithComment("Adding new user")
                    .WithOperation(RepoActions.Add)
                    .WithParametres($"{nameof(request.User.Id)}: {request.User.Id}")
                    .ToString()
                    );

                var result = await _userManager.CreateAsync(request.User, request.Password);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(Add))
                    .WithComment(ex.ToString())
                    .WithOperation(RepoActions.Add)
                    .WithParametres($"{nameof(request.User.Id)}: {request.User.Id}")
                    .ToString()
                    );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpPost(RepoActions.Update)]
        public async Task<IActionResult> Update(ApplicationUser user)
        {
            try
            {
                _logger.LogInformation(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(Update))
                    .WithComment("Updating user")
                    .WithOperation(RepoActions.Update)
                    .WithParametres($"{nameof(user.Id)}: {user.Id}")
                    .ToString()
                    );

                var userToBeUpdated = await _userManager.FindByNameAsync(user.UserName);
                if (userToBeUpdated == null)
                {
                    var description = $"User {user.UserName} was not found.";

                    _logger.LogWarning(new LogEntry()
                        .WithClass(nameof(UsersController))
                        .WithMethod(nameof(Update))
                        .WithComment(description)
                        .WithOperation(RepoActions.Update)
                        .WithParametres($"{nameof(user.Id)}: {user.Id}")
                        .ToString()
                        );

                    return BadRequest(IdentityResult.Failed(new IdentityError() { Description = description }));
                }

                userToBeUpdated.DefaultAddress = user.DefaultAddress;
                userToBeUpdated.DeliveryAddress = user.DeliveryAddress;
                userToBeUpdated.FirstName = user.FirstName;
                userToBeUpdated.LastName = user.LastName;
                userToBeUpdated.PhoneNumber = user.PhoneNumber;
                userToBeUpdated.Email = user.Email;

                var result = await _userManager.UpdateAsync(userToBeUpdated);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(Update))
                    .WithComment(ex.ToString())
                    .WithOperation(RepoActions.Update)
                    .WithParametres($"{nameof(user.Id)}: {user.Id}")
                    .ToString()
                    );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpPost(RepoActions.Remove)]
        public async Task<IActionResult> Remove(ApplicationUser user)
        {
            try
            {
                _logger.LogInformation(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(Remove))
                    .WithComment("Removing user")
                    .WithOperation(RepoActions.Remove)
                    .WithParametres($"{nameof(user.Id)}: {user.Id}")
                    .ToString()
                    );

                var userToBeRemoved = await _userManager.FindByNameAsync(user.UserName);
                if (userToBeRemoved == null)
                {
                    var description = $"User {user.UserName} was not found.";

                    _logger.LogWarning(new LogEntry()
                        .WithClass(nameof(UsersController))
                        .WithMethod(nameof(Remove))
                        .WithComment(description)
                        .WithOperation(RepoActions.Remove)
                        .WithParametres($"{nameof(user.Id)}: {user.Id}")
                        .ToString()
                        );

                    return BadRequest(IdentityResult.Failed(new IdentityError() { Description = description }));
                }

                var result = await _userManager.DeleteAsync(userToBeRemoved);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(Remove))
                    .WithComment(ex.ToString())
                    .WithOperation(RepoActions.Remove)
                    .WithParametres($"User id: {user.Id}")
                    .ToString()
                    );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(string userName)
        {
            try
            {
                _logger.LogInformation(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(Get))
                    .WithComment("Getting user")
                    .WithParametres($"{nameof(userName)}: {userName}")
                    .ToString()
                    );

                var result = await _userManager.Users
                    .Include(u => u.DefaultAddress)
                    .Include(u => u.DeliveryAddress)
                    .FirstOrDefaultAsync(u => u.UserName == userName);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(Get))
                    .WithComment(ex.ToString())
                    .WithParametres($"{nameof(userName)}: {userName}")
                    .ToString()
                    );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpGet(RepoActions.GetAll)]
        public IActionResult Get()
        {
            try
            {
                _logger.LogInformation(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(Get))
                    .WithComment("Getting all users")
                    .WithOperation(RepoActions.GetAll)
                    .WithParametres(LoggingConstants.NoParameters)
                    .ToString()
                    );

                var result = _userManager.Users
                    .Include(u => u.DefaultAddress)
                    .Include(u => u.DeliveryAddress)
                    .AsEnumerable();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(Get))
                    .WithComment(ex.ToString())
                    .WithOperation(RepoActions.GetAll)
                    .WithParametres(LoggingConstants.NoParameters)
                    .ToString()
                    );
                
                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpPost(UsersControllerRoutes.ChangePassword)]
        public async Task<IActionResult> ChangePassword(UserPasswordChangeRequest request)
        {
            try
            {
                _logger.LogInformation(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(ChangePassword))
                    .WithComment("Changing password")
                    .WithOperation(UsersControllerRoutes.ChangePassword)
                    .WithParametres($"{nameof(request.UserName)}: {request.UserName}")
                    .ToString()
                    );

                var user = await _userManager.FindByNameAsync(request.UserName);
                if (user == null)
                {
                    var description = $"User {request.UserName} was not found.";

                    _logger.LogWarning(new LogEntry()
                        .WithClass(nameof(UsersController))
                        .WithMethod(nameof(ChangePassword))
                        .WithComment(description)
                        .WithOperation(UsersControllerRoutes.ChangePassword)
                        .WithParametres($"{nameof(request.UserName)}: {request.UserName}")
                        .ToString()
                        );

                    return BadRequest(IdentityResult.Failed(new IdentityError() { Description = description }));
                }

                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(ChangePassword))
                    .WithComment(ex.ToString())
                    .WithOperation(UsersControllerRoutes.ChangePassword)
                    .WithParametres($"{nameof(request.UserName)}: {request.UserName}")
                    .ToString()
                    );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpPost(UsersControllerRoutes.AddToRole)]
        public async Task<IActionResult> AddToRole(AddRemoveRoleRequest request)
        {
            try
            {
                _logger.LogInformation(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(AddToRole))
                    .WithComment("Adding to role")
                    .WithOperation(UsersControllerRoutes.AddToRole)
                    .WithParametres($"{nameof(request.UserName)}: {request.UserName}")
                    .ToString()
                    );

                var user = await _userManager.FindByNameAsync(request.UserName);
                if (user == null)
                {
                    var description = $"User {user?.UserName} was not found.";

                    _logger.LogWarning(new LogEntry()
                        .WithClass(nameof(UsersController))
                        .WithMethod(nameof(AddToRole))
                        .WithComment(description)
                        .WithOperation(UsersControllerRoutes.AddToRole)
                        .WithParametres($"{nameof(request.UserName)}: {request.UserName}")
                        .ToString()
                        );

                    return BadRequest(IdentityResult.Failed(new IdentityError() { Description = description }));
                }

                var result = await _userManager.AddToRoleAsync(user, request.RoleName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(AddToRole))
                    .WithComment(ex.ToString())
                    .WithOperation(UsersControllerRoutes.AddToRole)
                    .WithParametres($"{nameof(request.UserName)}: {request.UserName}")
                    .ToString()
                    );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpPost(UsersControllerRoutes.AddToRoles)]
        public async Task<IActionResult> AddToRoles(AddRemoveRolesRequest request)
        {
            try
            {
                _logger.LogInformation(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(AddToRoles))
                    .WithComment("Adding to roles")
                    .WithOperation(UsersControllerRoutes.AddToRoles)
                    .WithParametres($"{nameof(request.UserName)}: {request.UserName}")
                    .ToString()
                    );

                var user = await _userManager.FindByNameAsync(request.UserName);
                if (user == null)
                {
                    var description = $"User {user?.UserName} was not found.";

                    _logger.LogWarning(new LogEntry()
                        .WithClass(nameof(UsersController))
                        .WithMethod(nameof(AddToRoles))
                        .WithComment(description)
                        .WithOperation(UsersControllerRoutes.AddToRoles)
                        .WithParametres($"{nameof(request.UserName)}: {request.UserName}")
                        .ToString()
                        );

                    return BadRequest(IdentityResult.Failed(new IdentityError() { Description = description }));
                }

                var result = await _userManager.AddToRolesAsync(user, request.RoleNames);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(AddToRoles))
                    .WithComment(ex.ToString())
                    .WithOperation(UsersControllerRoutes.AddToRoles)
                    .WithParametres($"{nameof(request.UserName)}: {request.UserName}")
                    .ToString()
                    );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpPost(UsersControllerRoutes.RemoveFromRole)]
        public async Task<IActionResult> RemoveFromRole(AddRemoveRoleRequest request)
        {
            try
            {
                _logger.LogInformation(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(RemoveFromRole))
                    .WithComment("Removing from role")
                    .WithOperation(UsersControllerRoutes.RemoveFromRole)
                    .WithParametres($"{nameof(request.UserName)}: {request.UserName}")
                    .ToString()
                    );

                var user = await _userManager.FindByNameAsync(request.UserName);
                if (user == null)
                {
                    var description = $"User {user?.UserName} was not found.";

                    _logger.LogWarning(new LogEntry()
                        .WithClass(nameof(UsersController))
                        .WithMethod(nameof(RemoveFromRole))
                        .WithComment(description)
                        .WithOperation(UsersControllerRoutes.RemoveFromRole)
                        .WithParametres($"{nameof(request.UserName)}: {request.UserName}")
                        .ToString()
                        );

                    return BadRequest(IdentityResult.Failed(new IdentityError() { Description = description }));
                }

                var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                        .WithClass(nameof(UsersController))
                        .WithMethod(nameof(RemoveFromRole))
                        .WithComment(ex.ToString())
                        .WithOperation(UsersControllerRoutes.RemoveFromRole)
                        .WithParametres($"{nameof(request.UserName)}: {request.UserName}")
                        .ToString()
                        );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpPost(UsersControllerRoutes.RemoveFromRoles)]
        public async Task<IActionResult> RemoveFromRoles(AddRemoveRolesRequest request)
        {
            try
            {
                _logger.LogInformation(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(RemoveFromRoles))
                    .WithComment("Removing from roles")
                    .WithOperation(UsersControllerRoutes.RemoveFromRoles)
                    .WithParametres($"{nameof(request.UserName)}: {request.UserName}")
                    .ToString()
                    );

                var user = await _userManager.FindByNameAsync(request.UserName);
                if (user == null)
                {
                    var description = $"User {user?.UserName} was not found.";

                    _logger.LogWarning(new LogEntry()
                        .WithClass(nameof(UsersController))
                        .WithMethod(nameof(RemoveFromRoles))
                        .WithComment(description)
                        .WithOperation(UsersControllerRoutes.RemoveFromRoles)
                        .WithParametres($"{nameof(request.UserName)}: {request.UserName}")
                        .ToString()
                        );

                    return BadRequest(IdentityResult.Failed(new IdentityError() { Description = description }));
                }

                var result = await _userManager.RemoveFromRolesAsync(user, request.RoleNames);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                        .WithClass(nameof(UsersController))
                        .WithMethod(nameof(RemoveFromRoles))
                        .WithComment(ex.ToString())
                        .WithOperation(UsersControllerRoutes.RemoveFromRoles)
                        .WithParametres($"{nameof(request.UserName)}: {request.UserName}")
                        .ToString()
                        );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }
    }
}
