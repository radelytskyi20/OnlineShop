using IdentityModel.Client;
using Microsoft.Extensions.Options;
using OnlineShop.Library.Clients.UserManagementService;
using OnlineShop.Library.Options;
using OnlineShop.Library.UserManagementService.Requests;

namespace OnlineShop.ConsoleAppTestApp
{
    public class AuthenticationServiceTest
    {
        private readonly ILoginClient _loginClient;
        private readonly UsersClient _usersClient;
        private readonly RolesClient _rolesClient;
        private readonly IdentityServerApiOptions _identityServerOptions;

        public AuthenticationServiceTest(
            ILoginClient loginClient,
            UsersClient usersClient,
            RolesClient rolesClient,
            IOptions<IdentityServerApiOptions> options
            )
        {
            _loginClient = loginClient;
            _usersClient = usersClient;
            _rolesClient = rolesClient;
            _identityServerOptions = options.Value;
        }

        private string testUserName = "rfihfh";
        private string[] testRoleNames = new[] { "ShopClient1", "ShopClient2", "ShopClient3" }; 

        public async Task<string> RunUsersClientTests(string[] args)
        {
            var token = await _loginClient.GetApiTokenByClientSeceret(_identityServerOptions);
            _usersClient.HttpClient.SetBearerToken(token.AccessToken);

            var addResult = await _usersClient.Add(new CreateUserRequest() 
            { 
                User = new Library.UserManagementService.Models.ApplicationUser() { UserName = testUserName }, 
                Password = "Password_1" 
            });
            Console.WriteLine($"ADD: {addResult.Succeeded}");

            Thread.Sleep(100);

            var changePasswordRequest = await _usersClient.ChangePassword(new UserPasswordChangeRequest() 
            { 
                UserName = testUserName, 
                CurrentPassword = "Password_1", 
                NewPassword = "Password_2" 
            });
            Console.WriteLine($"CHANGE PASSWORD: {changePasswordRequest.Succeeded}");

            Thread.Sleep(100);

            var getOneRequest = await _usersClient.Get(testUserName);
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

            var addToRoleRequest = await _usersClient.AddToRole(new AddRemoveRoleRequest()
            {
                UserName = testUserName,
                RoleName = testRoleNames[0]
            });
            Console.WriteLine($"ADD TO ROLE: {addToRoleRequest.Succeeded}");

            Thread.Sleep(100);

            var removeFromRoleRequest = await _usersClient.RemoveFromRole(new AddRemoveRoleRequest() 
            { 
                UserName = testUserName, 
                RoleName = testRoleNames[0]
            });
            Console.WriteLine($"REMOVE FROM ROLE: {removeFromRoleRequest.Succeeded}");

            Thread.Sleep(100);

            var addToRolesRequest = await _usersClient.AddToRoles(new AddRemoveRolesRequest()
            {
                UserName = testUserName,
                RoleNames = testRoleNames.Skip(1).ToArray()
            });
            Console.WriteLine($"ADD TO ROLES: {addToRolesRequest.Succeeded}");

            Thread.Sleep(100);

            var removeFromRolesRequest = await _usersClient.RemoveFromRoles(new AddRemoveRolesRequest() 
            { 
                UserName = testUserName, 
                RoleNames = testRoleNames.Skip(1).ToArray()
            });
            Console.WriteLine($"REMOVE FROM ROLES: {removeFromRolesRequest.Succeeded}");

            Thread.Sleep(100);

            return $"{nameof(RunUsersClientTests)}: OK";
        }

        public async Task<string> RunRolesClientTests(string[] args)
        {
            var token = await _loginClient.GetApiTokenByClientSeceret(_identityServerOptions);
            _rolesClient.HttpClient.SetBearerToken(token.AccessToken);

            foreach (var roleName in testRoleNames)
            {
                var addResult = await _rolesClient.Add(new Microsoft.AspNetCore.Identity.IdentityRole(roleName));
                Console.WriteLine($"ADD: {addResult.Succeeded}");

                Thread.Sleep(100);
            }

            var getOneRequest = await _rolesClient.Get(testRoleNames[0]);
            Console.WriteLine($"GET ONE: {getOneRequest.Code}");

            Thread.Sleep(100);

            var roleToUpdate = getOneRequest.Payload;
            testRoleNames[0] = "newTestRole";
            roleToUpdate.Name = testRoleNames[0];
            var updateResult = await _rolesClient.Update(roleToUpdate);
            Console.WriteLine($"UPDATE: {updateResult.Succeeded}");

            Thread.Sleep(100);

            var getAllRequest = await _rolesClient.GetAll();
            Console.WriteLine($"GET ALL: {getAllRequest.Code}");

            Thread.Sleep(100);

            return $"{nameof(RunRolesClientTests)}: OK";
        }

        public async Task<string> RunClearTmpDataTests(string[] args)
        {
            var token = await _loginClient.GetApiTokenByClientSeceret(_identityServerOptions);
            _rolesClient.HttpClient.SetBearerToken(token.AccessToken);
            _rolesClient.HttpClient.SetBearerToken(token.AccessToken);

            //remove test user
            var getTestUserRequest = await _usersClient.Get(testUserName);
            Console.WriteLine($"GET ONE: {getTestUserRequest.Code}");

            Thread.Sleep(100);

            var deleteUserResult = await _usersClient.Remove(getTestUserRequest.Payload);
            Console.WriteLine($"DELETE: {deleteUserResult.Succeeded}");

            Thread.Sleep(100);

            foreach (var roleName in testRoleNames)
            {
                var getOneRoleRequest = await _rolesClient.Get(roleName);
                Console.WriteLine($"GET ONE: {getOneRoleRequest.Code}");

                Thread.Sleep(100);

                var deleteRoleResult = await _rolesClient.Remove(getOneRoleRequest.Payload);
                Console.WriteLine($"DELETE: {deleteRoleResult.Succeeded}");
            }

            return $"{nameof(RunClearTmpDataTests)}: OK";
        }
    }
}
