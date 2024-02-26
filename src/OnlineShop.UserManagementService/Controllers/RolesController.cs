using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Library.Constants;

namespace OnlineShop.UserManagementService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpPost(RepoActions.Add)]
        public async Task<IdentityResult> Add(IdentityRole role)
        {
            var result = await _roleManager.CreateAsync(role);
            return result;
        }

        [HttpPost(RepoActions.Update)]
        public async Task<IdentityResult> Update(IdentityRole role)
        {
            var roleToBeUpdated = await _roleManager.FindByIdAsync(role.Id);
            if (roleToBeUpdated == null)
                return IdentityResult.Failed(new IdentityError() { Description = $"Role {role.Name} was not found." });

            roleToBeUpdated.Name = role.Name;

            var result = await _roleManager.UpdateAsync(roleToBeUpdated);
            return result;
        }

        [HttpPost(RepoActions.Remove)]
        public async Task<IdentityResult> Remove(IdentityRole role)
        {
            var result = await _roleManager.DeleteAsync(role);
            return result;
        }

        [HttpGet]
        public async Task<IdentityRole> Get(string name)
        {
            var result = await _roleManager.FindByNameAsync(name);
            return result;
        }

        [HttpGet(RepoActions.GetAll)]
        public IEnumerable<IdentityRole> Get()
        {
            var result = _roleManager.Roles.AsEnumerable();
            return result;
        }
    }
}
