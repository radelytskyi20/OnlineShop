using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Library.Constants;
using OnlineShop.Library.Logging;
using System.Data;

namespace OnlineShop.UserManagementService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RolesController> _logger;
        public RolesController(RoleManager<IdentityRole> roleManager, ILogger<RolesController> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpPost(RepoActions.Add)]
        public async Task<IActionResult> Add(IdentityRole role)
        {
            try
            {
                _logger.LogInformation(new LogEntry()
                    .WithClass(nameof(RolesController))
                    .WithMethod(nameof(Add))
                    .WithComment("Adding role")
                    .WithOperation(RepoActions.Add)
                    .WithParametres($"{nameof(role.Name)}: {role.Name}")
                    .ToString()
                    );

                var result = await _roleManager.CreateAsync(role);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(RolesController))
                    .WithMethod(nameof(Add))
                    .WithComment(ex.ToString())
                    .WithOperation(RepoActions.Add)
                    .WithParametres($"{nameof(role.Name)}: {role.Name}")
                    .ToString()
                    );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpPost(RepoActions.Update)]
        public async Task<IActionResult> Update(IdentityRole role)
        {
            try
            {
                _logger.LogInformation(new LogEntry()
                    .WithClass(nameof(RolesController))
                    .WithMethod(nameof(Update))
                    .WithComment("Updating role")
                    .WithOperation(RepoActions.Update)
                    .WithParametres($"{nameof(role.Name)}: {role.Name}")
                    .ToString()
                    );

                var roleToBeUpdated = await _roleManager.FindByIdAsync(role.Id);
                if (roleToBeUpdated == null)
                {
                    var description = $"Role {role.Name} was not found.";

                    _logger.LogWarning(new LogEntry()
                        .WithClass(nameof(RolesController))
                        .WithMethod(nameof(Update))
                        .WithComment(description)
                        .WithOperation(RepoActions.Update)
                        .WithParametres($"{nameof(role.Name)}: {role.Name}")
                        .ToString()
                        );

                    return BadRequest(IdentityResult.Failed(new IdentityError() { Description = description }));
                }

                roleToBeUpdated.Name = role.Name;

                var result = await _roleManager.UpdateAsync(roleToBeUpdated);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(RolesController))
                    .WithMethod(nameof(Update))
                    .WithComment(ex.ToString())
                    .WithOperation(RepoActions.Update)
                    .WithParametres($"{nameof(role.Name)}: {role.Name}")
                    .ToString()
                    );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpPost(RepoActions.Remove)]
        public async Task<IActionResult> Remove(IdentityRole role)
        {
            try
            {
                _logger.LogInformation(new LogEntry()
                    .WithClass(nameof(RolesController))
                    .WithMethod(nameof(Remove))
                    .WithComment("Removing role")
                    .WithOperation(RepoActions.Remove)
                    .WithParametres($"{nameof(role.Name)}: {role.Name}")
                    .ToString()
                    );

                var roleToBeRemoved = await _roleManager.FindByNameAsync(role.Name);
                if (roleToBeRemoved == null)
                {
                    var description = $"Role {role.Name} was not found.";

                    _logger.LogWarning(new LogEntry()
                        .WithClass(nameof(RolesController))
                        .WithMethod(nameof(Remove))
                        .WithComment(description)
                        .WithOperation(RepoActions.Remove)
                        .WithParametres($"{nameof(role.Name)}: {role.Name}")
                        .ToString()
                        );

                    return BadRequest(IdentityResult.Failed(new IdentityError() { Description = description }));
                }

                var result = await _roleManager.DeleteAsync(roleToBeRemoved);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(RolesController))
                    .WithMethod(nameof(Remove))
                    .WithComment(ex.ToString())
                    .WithOperation(RepoActions.Remove)
                    .WithParametres($"{nameof(role.Name)}: {role.Name}")
                    .ToString()
                    );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }  
        }

        [HttpGet]
        public async Task<IActionResult> Get(string name)
        {
            try
            {
                _logger.LogInformation(new LogEntry()
                    .WithClass(nameof(RolesController))
                    .WithMethod(nameof(Get))
                    .WithComment("Getting role")
                    .WithParametres($"{nameof(name)}: {name}")
                    .ToString()
                    );

                var result = await _roleManager.FindByNameAsync(name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(RolesController))
                    .WithMethod(nameof(Get))
                    .WithComment(ex.ToString())
                    .WithParametres($"{nameof(name)}: {name}")
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
                    .WithClass(nameof(RolesController))
                    .WithMethod(nameof(Get))
                    .WithComment("Getting roles")
                    .WithOperation(RepoActions.GetAll)
                    .WithParametres(LoggingConstants.NoParameters)
                    .ToString()
                    );

                var result = _roleManager.Roles.AsEnumerable();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(RolesController))
                    .WithMethod(nameof(Get))
                    .WithComment(ex.ToString())
                    .WithOperation(RepoActions.GetAll)
                    .WithParametres(LoggingConstants.NoParameters)
                    .ToString()
                    );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }
    }
}
