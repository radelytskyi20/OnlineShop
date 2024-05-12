using OnlineShop.Library.Options;
using OnlineShop.Library.UserManagementService.Models;

namespace OnlineShop.Library.Clients.UserManagementService
{
    public interface ILoginClient
    {
        Task<UserManagementServiceToken> GetApiTokenByClientSeceret(IdentityServerApiOptions options);
        Task<UserManagementServiceToken> GetApiTokenByUsernameAndPassword(IdentityServerUserNamePassword options);
    }
}
