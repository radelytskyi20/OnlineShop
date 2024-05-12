using AutoFixture;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using OnlineShop.Library.Clients.UserManagementService;
using OnlineShop.Library.Options;
using OnlineShop.Library.UserManagementService.Responses;
using System.Net;

namespace OnlineShop.UserManagementService.ApiTests
{
    public class RolesClientTests
    {
        private RolesClient _systemUnderTests;
        private ILoginClient _loginClient;

        #region Test Data

        private static readonly Fixture _fixture = new();
        private static readonly IdentityRole _testRole = new()
        {
            Name = _fixture.Create<string>(),
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

            _systemUnderTests = new RolesClient(new HttpClient(), serviceAddressOptionsMock.Object);
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
        }

        [Test]
        public async Task GIVEN_Roles_Client_WHEN_I_add_role_THEN_role_is_being_added_to_database()
        {
            //Arrange
            var expected = _testRole;

            //Act & Assert
            await AddRoleAndAssert(expected);

            var getRoleResponse = await _systemUnderTests.Get(expected.Name);
            AssertUserManagementServiceResponseIsSuccess(getRoleResponse);
            var actual = getRoleResponse.Payload;

            AssertObjectsAreEqual(expected, actual);
            await RemoveRoleAndAssert(actual);
        }

        [Test]
        public async Task GIVEN_Roles_Client_WHEN_I_get_role_by_name_THEN_role_is_being_returned_as_a_result_from_database()
        {
            //Arrange
            var expected = _testRole;

            //Act & Assert
            await AddRoleAndAssert(expected);

            var getRoleResponse = await _systemUnderTests.Get(expected.Name);
            AssertUserManagementServiceResponseIsSuccess(getRoleResponse);
            var actual = getRoleResponse.Payload;

            AssertObjectsAreEqual(expected, actual);
            await RemoveRoleAndAssert(actual);
        }

        [Test]
        public async Task GIVEN_Roles_Client_WHEN_I_get_roles_from_database_THEN_roles_are_being_returned_as_a_result_from_database()
        {
            //Arrange
            var role1 = _testRole;
            var role2 = new IdentityRole() { Name = _fixture.Create<string>() };
            var role3 = new IdentityRole() { Name = _fixture.Create<string>() };
            var expectedRoles = new List<IdentityRole>() { role1, role2, role3 };

            //Act & Assert
            foreach (var role in expectedRoles)
                await AddRoleAndAssert(role);

            var getRolesResponse = await _systemUnderTests.GetAll();
            AssertUserManagementServiceResponseIsSuccess(getRolesResponse);
            var actualRoles = getRolesResponse.Payload;

            foreach (var expectedRole in expectedRoles)
            {
                var actualRole = actualRoles.FirstOrDefault(aRole => aRole.Name.Equals(expectedRole.Name));
                Assert.That(actualRole, Is.Not.Null);
                AssertObjectsAreEqual(expectedRole, actualRole);
            }

            var rolesToRemove = actualRoles.Where(aRole => expectedRoles.Any(eRole => aRole.Name.Equals(eRole.Name)));
            foreach (var roleToRemove in rolesToRemove)
                await RemoveRoleAndAssert(roleToRemove);
        }

        [Test]
        public async Task GIVEN_Roles_Client_WHEN_I_remove_role_THEN_role_is_being_removed_from_database()
        {
            //Arrange
            var expected = _testRole;

            //Act & Assert
            await AddRoleAndAssert(expected);

            var getRoleResponse = await _systemUnderTests.Get(expected.Name);
            AssertUserManagementServiceResponseIsSuccess(getRoleResponse);
            var roleBeforeRemove = getRoleResponse.Payload;

            await RemoveRoleAndAssert(roleBeforeRemove);

            getRoleResponse = await _systemUnderTests.Get(roleBeforeRemove.Name);
            Assert.That(getRoleResponse.Code, Is.EqualTo(HttpStatusCode.NoContent.ToString()));

            var roleAfterRemove = getRoleResponse.Payload;
            Assert.That(roleAfterRemove, Is.Null);
        }

        [Test]
        public async Task GIVEN_Roles_Client_WHEN_I_update_role_THEN_role_is_being_updated_in_database()
        {
            //Arrange
            var roleToUpdate = _testRole;
            var expected = new IdentityRole() { Name = _fixture.Create<string>() };

            //Act & Assert
            await AddRoleAndAssert(roleToUpdate);

            var getRoleResponse = await _systemUnderTests.Get(roleToUpdate.Name);
            AssertUserManagementServiceResponseIsSuccess(getRoleResponse);
            var roleBeforeUpdate = getRoleResponse.Payload;

            roleBeforeUpdate.Name = expected.Name;

            var updateRoleResponse = await _systemUnderTests.Update(roleBeforeUpdate);
            Assert.That(updateRoleResponse.Succeeded, Is.True);

            getRoleResponse = await _systemUnderTests.Get(expected.Name);
            AssertUserManagementServiceResponseIsSuccess(getRoleResponse);
            var roleAfterUpdate = getRoleResponse.Payload;

            AssertObjectsAreEqual(expected, roleAfterUpdate);
            await RemoveRoleAndAssert(roleAfterUpdate);
        }

        private void AssertObjectsAreEqual(IdentityRole expected, IdentityRole actual) =>
            Assert.That(actual.Name, Is.EqualTo(expected.Name));

        private async Task AddRoleAndAssert(IdentityRole role)
        {
            var addRoleRequest = await _systemUnderTests.Add(role);
            Assert.That(addRoleRequest.Succeeded, Is.True);
        }
        private async Task RemoveRoleAndAssert(IdentityRole role)
        {
            var removeRoleRequest = await _systemUnderTests.Remove(role);
            Assert.That(removeRoleRequest.Succeeded, Is.True);
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
    }
}
