using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.ApiService.Authorization;
using OnlineShop.Library.Clients.UserManagementService;
using OnlineShop.Library.Constants;

namespace OnlineShop.ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class RolesController : ControllerWithClientAuthorization<IRolesClient>
    {
        public RolesController(IRolesClient client, IClientAuthorization clientAuthorization) : base(client, clientAuthorization)
        { }

        [HttpPost(RepoActions.Add)]
        public async Task<IdentityResult> Add(IdentityRole role)
        {
            var result = await Client.Add(role);
            return result;
        }

        [HttpPost(RepoActions.Update)]
        public async Task<IdentityResult> Update(IdentityRole role)
        {
            var result = await Client.Update(role);
            return result;
        }

        [HttpPost(RepoActions.Remove)]
        public async Task<IdentityResult> Remove(IdentityRole role)
        {
            var result = await Client.Remove(role);
            return result;
        }

        [HttpGet]
        public async Task<IdentityRole> Get(string name)
        {
            var result = await Client.Get(name);
            return result.Payload;
        }

        [HttpGet(RepoActions.GetAll)]
        public async Task<IEnumerable<IdentityRole>> GetAll()
        {
            var result = await Client.GetAll();
            return result.Payload;
        }
    }
}
