using AutoFixture;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using OnlineShop.Library.Clients.UserManagementService;
using OnlineShop.Library.Common.Models;
using OnlineShop.Library.Options;
using OnlineShop.Library.UserManagementService.Models;
using OnlineShop.Library.UserManagementService.Requests;
using OnlineShop.Library.UserManagementService.Responses;
using System.Net;

namespace OnlineShop.UserManagementService.ApiTests
{
    public class UserClientTests
    {
        private RolesClient _rolesClient;
        private ILoginClient _loginClient;
        private UsersClient _systemUnderTests;

        #region Test Data

        private static readonly Fixture _fixture = new();

        //password must contain at least one uppercase letter, one lowercase letter and one digit
        //this rule can be changed in the future
        private static readonly string _testUserPassword = string.Concat(_fixture.Create<string>(), "@", "P");
        private static readonly ApplicationUser _testUser = new()
        {
            UserName = _fixture.Create<string>(),
            Email = _fixture.Create<string>(),
            PhoneNumber = _fixture.Create<string>(),
            FirstName = _fixture.Create<string>(),
            LastName = _fixture.Create<string>(),
            DefaultAddress = _fixture.Build<Address>()
                .Without(a => a.Id)
                .With(a => a.PostalCode, _fixture.Create<string>()[..32])
                .Create(),
            
            DeliveryAddress = _fixture.Build<Address>()
                .Without(a => a.Id)
                .With(a => a.PostalCode, _fixture.Create<string>()[..32])
                .Create()
        };
        private static readonly List<IdentityRole> _testRoles = new()
        {
            _fixture.Build<IdentityRole>()
                .Without(r => r.Id)
                .Without(r => r.NormalizedName)
                .Without(r => r.ConcurrencyStamp)
                .Create(),

                _fixture.Build<IdentityRole>()
                .Without(r => r.Id)
                .Without(r => r.NormalizedName)
                .Without(r => r.ConcurrencyStamp)
                .Create(),
        };

        #endregion

        [SetUp]
        public async Task SetUp()
        {
            var serviceAddressOptionsMock = new Mock<IOptions<ServiceAdressOptions>>();

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            switch (env)
            {
                case "Docker":
                    serviceAddressOptionsMock.Setup(x => x.Value)
                        .Returns(new ServiceAdressOptions()
                        {
                            UserManagementService = "https://localhost:5003"
                        });
                    break;

                default:
                    serviceAddressOptionsMock.Setup(x => x.Value)
                        .Returns(new ServiceAdressOptions()
                        {
                            UserManagementService = "https://localhost:5003"
                        });
                    break;
            }

            _systemUnderTests = new UsersClient(new HttpClient(), serviceAddressOptionsMock.Object);
            _rolesClient = new RolesClient(new HttpClient(), serviceAddressOptionsMock.Object);
            _loginClient = new LoginClient(new HttpClient(), serviceAddressOptionsMock.Object);

            var identityServerApiOptions = new IdentityServerApiOptions()
            {
                ClientId = "test.client",
                ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",
                Scope = "OnlineShop.Api",
                GrantType = "client_credentials"
            };

            var token = await _loginClient.GetApiTokenByClientSeceret(identityServerApiOptions);
            _systemUnderTests.HttpClient.SetBearerToken(token.AccessToken);
            _rolesClient.HttpClient.SetBearerToken(token.AccessToken);
        }

        [Test]
        public async Task GIVEN_Users_Client_WHEN_I_add_user_THEN_user_is_being_added_to_database()
        {
            //Arrange
            var expected = _testUser;
            var password = _testUserPassword;

            //Act & Assert
            await AddUserAndAssert(expected, password);

            var getResponse = await _systemUnderTests.Get(expected.UserName);
            AssertUserManagementServiceResponseIsSuccess(getResponse);
            var actual = getResponse.Payload;

            AssertObjectsAreEqual(expected, actual);

            await RemoveUserAndAssert(actual);
        }

        [Test]
        public async Task GIVEN_Users_Client_WHEN_I_update_user_THEN_user_is_being_updated_in_database()
        {
            //Arrange
            var userToUpdate = _testUser;
            var userToUpdatePassword = _testUserPassword;
            var expected = new ApplicationUser()
            {
                UserName = _testUser.UserName,
                Email = _fixture.Create<string>(),
                PhoneNumber = _fixture.Create<string>(),
                FirstName = _fixture.Create<string>(),
                LastName = _fixture.Create<string>(),
                DefaultAddress = _fixture.Build<Address>()
                    .Without(a => a.Id)
                    .With(a => a.PostalCode, _fixture.Create<string>()[..32])
                    .Create(),

                DeliveryAddress = _fixture.Build<Address>()
                    .Without(a => a.Id)
                    .With(a => a.PostalCode, _fixture.Create<string>()[..32])
                    .Create(),
            };

            //Act & Assert
            await AddUserAndAssert(userToUpdate, userToUpdatePassword);

            var getResponse = await _systemUnderTests.Get(_testUser.UserName);
            AssertUserManagementServiceResponseIsSuccess(getResponse);
            var userBeforeUpdate = getResponse.Payload;

            //update user
            userBeforeUpdate.UserName = expected.UserName;
            userBeforeUpdate.Email = expected.Email;
            userBeforeUpdate.PhoneNumber = expected.PhoneNumber;
            userBeforeUpdate.FirstName = expected.FirstName;
            userBeforeUpdate.LastName = expected.LastName;
            userBeforeUpdate.DefaultAddress = expected.DefaultAddress;
            userBeforeUpdate.DeliveryAddress = expected.DeliveryAddress;

            var updateResponse = await _systemUnderTests.Update(userBeforeUpdate);
            Assert.That(updateResponse.Succeeded, Is.True);

            getResponse = await _systemUnderTests.Get(expected.UserName);
            AssertUserManagementServiceResponseIsSuccess(getResponse);
            var userAfterUpdate = getResponse.Payload;

            AssertObjectsAreEqual(expected, userAfterUpdate);

            await RemoveUserAndAssert(userAfterUpdate);
        }

        [Test]
        public async Task GIVEN_Users_Client_WHEN_I_remove_user_THEN_user_is_being_removed_in_database()
        {
            //Arrange
            var userToRemove = _testUser;
            var userToRemovePassword = _testUserPassword;

            //Act & Assert
            await AddUserAndAssert(userToRemove, userToRemovePassword);

            var getResponse = await _systemUnderTests.Get(userToRemove.UserName);
            AssertUserManagementServiceResponseIsSuccess(getResponse);
            var userBeforeRemove = getResponse.Payload;

            await RemoveUserAndAssert(userBeforeRemove);

            getResponse = await _systemUnderTests.Get(userBeforeRemove.UserName);
            Assert.That(getResponse.Code, Is.EqualTo(HttpStatusCode.NotFound.ToString()));

            var userAfterRemove = getResponse.Payload;
            Assert.That(userAfterRemove, Is.Null);
        }

        [Test]
        public async Task GIVEN_Users_Client_WHEN_I_change_user_password_THEN_user_password_is_being_changed_in_database()
        {
            //Arrange
            var userToChange = _testUser;
            var userToChangePassword = _testUserPassword;

            var changePasswordRequest = new UserPasswordChangeRequest()
            {
                UserName = userToChange.UserName,
                CurrentPassword = _testUserPassword,
                NewPassword = string.Concat(_fixture.Create<string>(), "@", "GGG")
            };

            //Act & Assert
            await AddUserAndAssert(userToChange, userToChangePassword);

            var getResponse = await _systemUnderTests.Get(_testUser.UserName);
            AssertUserManagementServiceResponseIsSuccess(getResponse);
            var userBeforePasswordChange = getResponse.Payload;
            var passwordHashBeforeChange = userBeforePasswordChange.PasswordHash;

            var changePasswordResponse = await _systemUnderTests.ChangePassword(changePasswordRequest);
            Assert.That(changePasswordResponse.Succeeded, Is.True);

            getResponse = await _systemUnderTests.Get(_testUser.UserName);
            AssertUserManagementServiceResponseIsSuccess(getResponse);
            var userAfterPasswordChange = getResponse.Payload;
            var passwordHashAfterChange = userAfterPasswordChange.PasswordHash;

            Assert.That(passwordHashBeforeChange, Is.Not.EqualTo(passwordHashAfterChange));

            await RemoveUserAndAssert(userAfterPasswordChange);
        }

        [Test]
        public async Task GIVEN_Users_Client_WHEN_I_get_user_by_username_THEN_user_is_being_returned_as_a_result_from_database()
        {
            //Arrange
            var expected = _testUser;
            var password = _testUserPassword;

            //Act & Assert
            await AddUserAndAssert(expected, password);

            var getResponse = await _systemUnderTests.Get(expected.UserName);
            AssertUserManagementServiceResponseIsSuccess(getResponse);
            var actual = getResponse.Payload;

            AssertObjectsAreEqual(expected, actual);

            await RemoveUserAndAssert(actual);
        }

        [Test]
        public async Task GIVEN_Users_Client_WHEN_I_get_users_from_database_THEN_users_are_being_returned_as_a_result_from_database()
        {
            //Arrange
            var user1 = _testUser;
            var user2 = new ApplicationUser()
            {
                UserName = _fixture.Create<string>(),
                Email = _fixture.Create<string>(),
                PhoneNumber = _fixture.Create<string>(),
                FirstName = _fixture.Create<string>(),
                LastName = _fixture.Create<string>(),
                DefaultAddress = _fixture.Build<Address>()
                        .Without(a => a.Id)
                        .With(a => a.PostalCode, _fixture.Create<string>()[..32])
                        .Create(),

                DeliveryAddress = _fixture.Build<Address>()
                        .Without(a => a.Id)
                        .With(a => a.PostalCode, _fixture.Create<string>()[..32])
                        .Create()
            };
            var password = _testUserPassword;
            var expectedUsers = new List<ApplicationUser>() { user1, user2 };

            //Act & Assert
            foreach (var testUserToAdd in expectedUsers)
                await AddUserAndAssert(testUserToAdd, password);

            var getAllResponse = await _systemUnderTests.GetAll();
            AssertUserManagementServiceResponseIsSuccess(getAllResponse);
            var actualUsers = getAllResponse.Payload;

            foreach (var expectedUser in expectedUsers)
            {
                var actualUser = actualUsers.FirstOrDefault(u => u.UserName.Equals(expectedUser.UserName));
                Assert.That(actualUser, Is.Not.Null);
                AssertObjectsAreEqual(expectedUser, actualUser);
            }

            var usersToRemove = actualUsers.Where(actual =>
            {
                return expectedUsers.Any(expected => expected.UserName.Equals(actual.UserName));
            });
            foreach (var userToRemove in usersToRemove)
                await RemoveUserAndAssert(userToRemove);
        }

        [Test]
        public async Task GIVEN_Users_Client_WHEN_I_add_role_to_user_THEN_role_is_being_added_to_user_in_database()
        {
            //Arrange
            var roleToAdd = _testRoles[0];
            var testUser = _testUser;
            var testUserPassword = _testUserPassword;

            //Act & Assert
            await AddRoleAndAssert(roleToAdd);
            await AddUserAndAssert(testUser, testUserPassword);

            var getUserResponse = await _systemUnderTests.Get(_testUser.UserName);
            AssertUserManagementServiceResponseIsSuccess(getUserResponse);
            var userBeforeAddRole = getUserResponse.Payload;

            await AddRoleToUserAndAssert(userBeforeAddRole, roleToAdd);

            getUserResponse = await _systemUnderTests.Get(userBeforeAddRole.UserName);
            AssertUserManagementServiceResponseIsSuccess(getUserResponse);
            var userAfterAddRole = getUserResponse.Payload;

            await RemoveUserAndAssert(userAfterAddRole);
            await RemoveRoleAndAssert(roleToAdd);
        }

        [Test]
        public async Task GIVEN_Users_Client_WHEN_I_add_roles_to_user_THEN_roles_are_being_added_to_user_in_database()
        {
            //Arrange
            var rolesToAdd = _testRoles;
            var testUser = _testUser;
            var testUserPassword = _testUserPassword;

            //Act & Assert
            foreach (var roleToAdd in rolesToAdd)
                await AddRoleAndAssert(roleToAdd);

            await AddUserAndAssert(testUser, testUserPassword);

            var getUserResponse = await _systemUnderTests.Get(_testUser.UserName);
            AssertUserManagementServiceResponseIsSuccess(getUserResponse);
            var userBeforeAddRoles = getUserResponse.Payload;

            await AddRolesToUserAndAssert(userBeforeAddRoles, rolesToAdd);

            getUserResponse = await _systemUnderTests.Get(userBeforeAddRoles.UserName);
            AssertUserManagementServiceResponseIsSuccess(getUserResponse);
            var userAfterAddRoles = getUserResponse.Payload;

            await RemoveUserAndAssert(userAfterAddRoles);

            foreach (var roleToRemove in rolesToAdd)
                await RemoveRoleAndAssert(roleToRemove);
        }

        [Test]
        public async Task GIVEN_Users_Client_WHEN_I_remove_role_from_user_THEN_role_is_being_removed_from_user_in_database()
        {
            //Arrange
            var roleToRemove = _testRoles[0];
            var testUser = _testUser;
            var testUserPassword = _testUserPassword;

            var removeRoleFromUserRequest = new AddRemoveRoleRequest()
            {
                UserName = testUser.UserName,
                RoleName = roleToRemove.Name
            };

            //Act & Assert
            await AddRoleAndAssert(roleToRemove);
            await AddUserAndAssert(testUser, testUserPassword);

            var getUserResponse = await _systemUnderTests.Get(_testUser.UserName);
            AssertUserManagementServiceResponseIsSuccess(getUserResponse);
            var userBeforeAddRole = getUserResponse.Payload;

            await AddRoleToUserAndAssert(userBeforeAddRole, roleToRemove);

            getUserResponse = await _systemUnderTests.Get(userBeforeAddRole.UserName);
            AssertUserManagementServiceResponseIsSuccess(getUserResponse);
            var userAfterAddRole = getUserResponse.Payload;

            var removeRoleFromUserResponse = await _systemUnderTests.RemoveFromRole(removeRoleFromUserRequest);
            Assert.That(removeRoleFromUserResponse.Succeeded, Is.True);

            getUserResponse = await _systemUnderTests.Get(userAfterAddRole.UserName);
            AssertUserManagementServiceResponseIsSuccess(getUserResponse);
            var userAfterRemoveRole = getUserResponse.Payload;

            await RemoveUserAndAssert(userAfterRemoveRole);
            await RemoveRoleAndAssert(roleToRemove);
        }

        [Test]
        public async Task GIVEN_Users_Client_WHEN_I_remove_roles_from_user_THEN_roles_are_being_removed_from_user_in_database()
        {
            //Arrange
            var rolesToRemove = _testRoles;
            var testUser = _testUser;
            var testUserPassword = _testUserPassword;

            var removeRolesFromUserRequest = new AddRemoveRolesRequest()
            {
                UserName = testUser.UserName,
                RoleNames = rolesToRemove.Select(r => r.Name).ToArray()
            };

            //Act & Assert
            foreach (var roleToRemove in rolesToRemove)
                await AddRoleAndAssert(roleToRemove);

            await AddUserAndAssert(testUser, testUserPassword);

            var getUserResponse = await _systemUnderTests.Get(_testUser.UserName);
            AssertUserManagementServiceResponseIsSuccess(getUserResponse);
            var userBeforeAddRoles = getUserResponse.Payload;

            await AddRolesToUserAndAssert(userBeforeAddRoles, rolesToRemove);

            getUserResponse = await _systemUnderTests.Get(userBeforeAddRoles.UserName);
            AssertUserManagementServiceResponseIsSuccess(getUserResponse);
            var userAfterAddRoles = getUserResponse.Payload;

            var removeRolesFromUserResponse = await _systemUnderTests.RemoveFromRoles(removeRolesFromUserRequest);
            Assert.That(removeRolesFromUserResponse.Succeeded, Is.True);

            getUserResponse = await _systemUnderTests.Get(userAfterAddRoles.UserName);
            AssertUserManagementServiceResponseIsSuccess(getUserResponse);
            var userAfterRemoveRoles = getUserResponse.Payload;

            await RemoveUserAndAssert(userAfterRemoveRoles);

            foreach (var roleToRemove in rolesToRemove)
                await RemoveRoleAndAssert(roleToRemove);
        }

        private void AssertObjectsAreEqual(ApplicationUser expected, ApplicationUser actual)
        {
            Assert.Multiple(() =>
            {
                Assert.That(actual.UserName, Is.EqualTo(expected.UserName));
                Assert.That(actual.Email, Is.EqualTo(expected.Email));
                Assert.That(actual.PhoneNumber, Is.EqualTo(expected.PhoneNumber));
                AssertObjectsAreEqual(expected.DefaultAddress, actual.DefaultAddress);
                AssertObjectsAreEqual(expected.DeliveryAddress, actual.DeliveryAddress);
                Assert.That(actual.FirstName, Is.EqualTo(expected.FirstName));
                Assert.That(actual.LastName, Is.EqualTo(expected.LastName));
            });
        }
        private void AssertObjectsAreEqual(Address expected, Address actual)
        {
            Assert.Multiple(() =>
            {
                Assert.That(actual.Country, Is.EqualTo(expected.Country));
                Assert.That(actual.City, Is.EqualTo(expected.City));
                Assert.That(actual.PostalCode, Is.EqualTo(expected.PostalCode));
                Assert.That(actual.AddressLine1, Is.EqualTo(expected.AddressLine1));
                Assert.That(actual.AddressLine2, Is.EqualTo(expected.AddressLine2));
            });
        }
        private void AssertUserManagementServiceResponseIsSuccess<T>(UserManagementServiceResponse<T> response)
        {
            Assert.Multiple(() =>
            {
                Assert.That(response.Code, Is.EqualTo(HttpStatusCode.OK.ToString()));
                Assert.That(response.Description, Is.EqualTo(HttpStatusCode.OK.ToString()));
                Assert.That(response.Payload, Is.Not.Null);
            });
        }
        private async Task AddUserAndAssert(ApplicationUser user, string password)
        {
            var createUserRequest = new CreateUserRequest()
            {
                User = user,
                Password = password
            };

            var addResponse = await _systemUnderTests.Add(createUserRequest);
            Assert.That(addResponse.Succeeded, Is.True);
        }
        private async Task RemoveUserAndAssert(ApplicationUser user)
        {
            var removeUserResponse = await _systemUnderTests.Remove(user);
            Assert.That(removeUserResponse.Succeeded, Is.True);
        }
        private async Task AddRoleAndAssert(IdentityRole role)
        {
            var addRoleResponse = await _rolesClient.Add(role);
            Assert.That(addRoleResponse.Succeeded, Is.True);
        }
        private async Task RemoveRoleAndAssert(IdentityRole role)
        {
            var removeRoleResponse = await _rolesClient.Remove(role);
            Assert.That(removeRoleResponse.Succeeded, Is.True);
        }
        private async Task AddRolesToUserAndAssert(ApplicationUser user, IEnumerable<IdentityRole> roles)
        {
            var addRolesToUserRequest = new AddRemoveRolesRequest()
            {
                UserName = user.UserName,
                RoleNames = roles.Select(r => r.Name).ToArray()
            };

            var addRolesToUserResponse = await _systemUnderTests.AddToRoles(addRolesToUserRequest);
            Assert.That(addRolesToUserResponse.Succeeded, Is.True);
        }
        private async Task AddRoleToUserAndAssert(ApplicationUser user, IdentityRole role)
        {
            var addRoleToUserRequest = new AddRemoveRoleRequest()
            {
                UserName = user.UserName,
                RoleName = role.Name
            };

            var addRoleToUserResponse = await _systemUnderTests.AddToRole(addRoleToUserRequest);
            Assert.That(addRoleToUserResponse.Succeeded, Is.True);
        }
    }
}
