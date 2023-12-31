using IdentityModel.Client;
using Microsoft.Extensions.Options;
using OnlineShop.Library.Clients.IdentityServer;
using OnlineShop.Library.Clients.UserManagementService;
using OnlineShop.Library.Options;
using OnlineShop.Library.UserManagementService.Requests;

namespace OnlineShop.ConsoleAppTestApp
{
    public class AuthenticationServiceTest
    {
        private readonly IdentityServerClient _identityServerClient;
        private readonly UsersClient _usersClient;
        private readonly RolesClient _rolesClient;
        private readonly IdentityServerApiOptions _identityServerOptions;

        public AuthenticationServiceTest(
            IdentityServerClient identityServerClient,
            UsersClient usersClient,
            RolesClient rolesClient,
            IOptions<IdentityServerApiOptions> options
            )
        {
            _identityServerClient = identityServerClient;
            _usersClient = usersClient;
            _rolesClient = rolesClient;
            _identityServerOptions = options.Value;
        }

        public async Task<string> RunUsersClientTests(string[] args)
        {
            var token = await _identityServerClient.GetApiToken(_identityServerOptions);
            _usersClient.HttpClient.SetBearerToken(token.AccessToken);

            var userName = "rfihfh";
            var roleName = "ShopClient3";
            var roleNames = new[] { "ShopClient3", "ShopClient4" };

            var addResult = await _usersClient.Add(new CreateUserRequest() { User = new Library.UserManagementService.Models.ApplicationUser() { UserName = userName }, Password = "Password_1" });
            Console.WriteLine($"ADD: {addResult.Succeeded}");

            Thread.Sleep(100);

            var changePasswordRequest = await _usersClient.ChangePassword(new UserPasswordChangeRequest() { UserName = userName, CurrentPassword = "Password_1", NewPassword = "Password_2" });
            Console.WriteLine($"CHANGE PASSWORD: {changePasswordRequest.Succeeded}");

            Thread.Sleep(100);

            var getOneRequest = await _usersClient.Get(userName);
            Console.WriteLine($"GET ONE: {getOneRequest.Code}");

            Thread.Sleep(100);

            var getAllRequest = await _usersClient.GetAll();
            Console.WriteLine($"GET ALL {getAllRequest.Code}");

            Thread.Sleep(100);

            var userToUpdate = getOneRequest.Payload;
            userToUpdate.DefaultAddress = new Library.Common.Models.Address()
            {
                City = "Kharkiv",
                Country = "Ukraine",
                PostalCode = "1234",
                AddressLine1 = "Heroiv Pratsi",
                AddressLine2 = "100"
            };

            var updateResult = await _usersClient.Update(userToUpdate);
            Console.WriteLine($"UPDATE: {updateResult.Succeeded}");

            Thread.Sleep(100);

            var addToRoleRequest = await _usersClient.AddToRole(new AddRemoveRoleRequest() { UserName = userName, RoleName = roleName });
            Console.WriteLine($"ADD TO ROLE: {addToRoleRequest.Succeeded}");

            Thread.Sleep(100);

            var removeFromRoleRequest = await _usersClient.RemoveFromRole(new AddRemoveRoleRequest() { UserName = userName, RoleName = roleName });
            Console.WriteLine($"REMOVE FROM ROLE: {removeFromRoleRequest.Succeeded}");

            Thread.Sleep(100);

            var addToRolesRequest = await _usersClient.AddToRoles(new AddRemoveRolesRequest() { UserName = userName, RoleNames = roleNames });
            Console.WriteLine($"ADD TO ROLES: {addToRolesRequest.Succeeded}");

            Thread.Sleep(100);

            var removeFromRolesRequest = await _usersClient.RemoveFromRoles(new AddRemoveRolesRequest() { UserName = userName, RoleNames = roleNames });
            Console.WriteLine($"REMOVE FROM ROLES: {removeFromRolesRequest.Succeeded}");

            Thread.Sleep(100);

            getOneRequest = await _usersClient.Get(userName); //проверить!!! возвращает пользователя будто мы не обновляли его!
            Console.WriteLine($"GET ONE: {getOneRequest.Code}");

            Thread.Sleep(100);

            var deleteResult = await _usersClient.Remove(getOneRequest.Payload); //некорректно удаляем пользователя!
            Console.WriteLine($"DELETE: {deleteResult.Succeeded}");

            Thread.Sleep(100);

            return "OK";
        }

        public async Task<string> RunRolesClientTests(string[] args)
        {
            var token = await _identityServerClient.GetApiToken(_identityServerOptions);
            _rolesClient.HttpClient.SetBearerToken(token.AccessToken);

            var roleName = "testRole";

            var addResult = await _rolesClient.Add(new Microsoft.AspNetCore.Identity.IdentityRole(roleName));
            Console.WriteLine($"ADD: {addResult.Succeeded}");

            Thread.Sleep(100);

            var getOneRequest = await _rolesClient.Get(roleName);
            Console.WriteLine($"GET ONE: {getOneRequest.Code}");

            Thread.Sleep(100);

            var roleToUpdate = getOneRequest.Payload;
            roleName = "newTestRole";
            roleToUpdate.Name = roleName;
            var updateResult = await _rolesClient.Update(roleToUpdate);
            Console.WriteLine($"UPDATE: {updateResult.Succeeded}");

            Thread.Sleep(100);

            getOneRequest = await _rolesClient.Get(roleName);
            Console.WriteLine($"GET ONE: {getOneRequest.Code}");

            Thread.Sleep(100);

            var deleteResult = await _rolesClient.Remove(getOneRequest.Payload);
            Console.WriteLine($"DELETE: {deleteResult.Succeeded}");

            Thread.Sleep(100);

            var getAllRequest = await _rolesClient.GetAll();
            Console.WriteLine($"GET ALL: {getAllRequest.Code}");

            return "OK";
        }
    }
}
