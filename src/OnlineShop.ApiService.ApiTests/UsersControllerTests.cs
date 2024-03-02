using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using OnlineShop.Library.Common.Models;
using OnlineShop.Library.Constants;
using OnlineShop.Library.UserManagementService.Models;
using OnlineShop.Library.UserManagementService.Requests;
using System.Text;

namespace OnlineShop.ApiService.ApiTests
{
    public class UsersControllerTests : BaseRepoControllerTests<ApplicationUser>
    {
        public UsersControllerTests() : base() { ControllerName = "users"; }

        [Test]
        public virtual async Task GIVEN_UsersController_WHEN_I_add_entity_THEN_it_is_being_added_to_database()
        {
            var expected = CreateApplicationUser();
            var createUserRequest = new CreateUserRequest()
            {
                User = expected,
                Password = CreatePassword()
            };

            var addJsonContent = JsonConvert.SerializeObject(createUserRequest);
            var addHttpContent = new StringContent(addJsonContent, Encoding.UTF8, MediaType);
            var addResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Add}", addHttpContent);
            Assert.That(addResponse.IsSuccessStatusCode, Is.True);

            var getOneResponse = await SystemUnderTests.GetAsync($"{ControllerName}?userName={expected.UserName}");
            Assert.That(getOneResponse.IsSuccessStatusCode, Is.True);
            var getOneContent = await getOneResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<ApplicationUser>(getOneContent);
            Assert.That(actual, Is.Not.Null);

            AssertObjectsAreEqual(expected, actual);

            var removeJsonContent = JsonConvert.SerializeObject(actual);
            var removeHttpContent = new StringContent(removeJsonContent, Encoding.UTF8, MediaType);
            var removeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Remove}", removeHttpContent);
            Assert.That(removeResponse.IsSuccessStatusCode, Is.True);
        }

        [Test]
        public virtual async Task GIVEN_UsersController_WHEN_I_get_all_entities_THEN_they_are_returning_from_database()
        {
            var expected1 = CreateApplicationUser();
            var expected2 = CreateApplicationUser();

            var usersToAdd = new[] { expected1, expected2 };

            foreach (var user in usersToAdd)
            {
                var createUserRequest = new CreateUserRequest() { User = user, Password = CreatePassword() };
                var addJsonContent = JsonConvert.SerializeObject(createUserRequest);
                var addHttpContent = new StringContent(addJsonContent, Encoding.UTF8, MediaType);
                var addResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Add}", addHttpContent);
                Assert.That(addResponse.IsSuccessStatusCode, Is.True);
            }

            var getAllResponse = await SystemUnderTests.GetAsync($"{ControllerName}/{RepoActions.GetAll}");
            Assert.That(getAllResponse.IsSuccessStatusCode, Is.True);
            var getAllContent = await getAllResponse.Content.ReadAsStringAsync();
            var addedUsers = JsonConvert.DeserializeObject<IEnumerable<ApplicationUser>>(getAllContent);
            Assert.That(addedUsers, Is.Not.Null);

            foreach (var expected in usersToAdd)
            {
                var actual = addedUsers.Single(u => u.UserName == expected.UserName);
                AssertObjectsAreEqual(expected, actual);
            }

            foreach (var user in usersToAdd)
            {
                var removeJsonContent = JsonConvert.SerializeObject(user);
                var removeHttpContent = new StringContent(removeJsonContent, Encoding.UTF8, MediaType);
                var removeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Remove}", removeHttpContent);
                Assert.That(removeResponse.IsSuccessStatusCode, Is.True);
            }
        }

        [Test]
        public virtual async Task GIVEN_UsersController_WHEN_I_update_entity_THEN_it_is_being_updated_in_database()
        {
            var defaultAddress = Fixture.Build<Address>()
                .Without(a => a.Id)
                .With(a => a.PostalCode, Fixture.Create<string>()[..32])
                .Create();

            var deliveryAddress = Fixture.Build<Address>()
                .Without(a => a.Id)
                .With(a => a.PostalCode, Fixture.Create<string>()[..32])
                .Create();

            var expected = CreateApplicationUser();

            expected.DefaultAddress = defaultAddress;
            expected.DeliveryAddress = deliveryAddress;

            var createUserRequest = new CreateUserRequest()
            {
                User = expected,
                Password = CreatePassword()
            };

            var addJsonContent = JsonConvert.SerializeObject(createUserRequest);
            var addHttpContent = new StringContent(addJsonContent, Encoding.UTF8, MediaType);
            var addResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Add}", addHttpContent);
            Assert.That(addResponse.IsSuccessStatusCode, Is.True);

            defaultAddress.Country = Fixture.Create<string>();
            defaultAddress.City = Fixture.Create<string>();
            deliveryAddress.Country = Fixture.Create<string>();
            deliveryAddress.City = Fixture.Create<string>();

            expected.FirstName = Fixture.Create<string>();
            expected.LastName = Fixture.Create<string>();
            expected.PhoneNumber = Fixture.Create<string>();
            expected.Email = Fixture.Create<string>();
            expected.DefaultAddress = defaultAddress;
            expected.DeliveryAddress = deliveryAddress;

            var updateJsonContent = JsonConvert.SerializeObject(expected);
            var updateHttpContent = new StringContent(updateJsonContent, Encoding.UTF8, MediaType);
            var updateResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Update}", updateHttpContent);
            Assert.That(updateResponse.IsSuccessStatusCode, Is.True);

            var getOneResponse = await SystemUnderTests.GetAsync($"{ControllerName}?userName={expected.UserName}");
            Assert.That(getOneResponse.IsSuccessStatusCode, Is.True);
            var getOneContent = await getOneResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<ApplicationUser>(getOneContent);
            Assert.That(actual, Is.Not.Null);

            AssertObjectsAreEqual(expected, actual);

            var removeJsonContent = JsonConvert.SerializeObject(actual);
            var removeHttpContent = new StringContent(removeJsonContent, Encoding.UTF8, MediaType);
            var removeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Remove}", removeHttpContent);
            Assert.That(removeResponse.IsSuccessStatusCode, Is.True);
        }

        [Test]
        public virtual async Task GIVEN_UsersController_WHEN_I_change_user_password_THEN_it_is_being_changed_in_database()
        {
            var user = CreateApplicationUser();
            var password = CreatePassword();
            var createUserRequest = new CreateUserRequest()
            {
                User = user,
                Password = password
            };

            var addJsonContent = JsonConvert.SerializeObject(createUserRequest);
            var addHttpContent = new StringContent(addJsonContent, Encoding.UTF8, MediaType);
            var addResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Add}", addHttpContent);
            Assert.That(addResponse.IsSuccessStatusCode, Is.True);

            var getOneResponse = await SystemUnderTests.GetAsync($"{ControllerName}?userName={user.UserName}");
            Assert.That(getOneResponse.IsSuccessStatusCode, Is.True);
            var getOneContent = await getOneResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<ApplicationUser>(getOneContent);
            Assert.That(actual, Is.Not.Null);
            var oldPasswordHash = actual.PasswordHash;

            var userPasswordChangeRequest = new UserPasswordChangeRequest()
            {
                UserName = user.UserName,
                CurrentPassword = password,
                NewPassword = CreatePassword()
            };

            var changePasswordJsonContent = JsonConvert.SerializeObject(userPasswordChangeRequest);
            var changePasswordHttpContent = new StringContent(changePasswordJsonContent, Encoding.UTF8, MediaType);
            var changePasswordResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{UsersControllerRoutes.ChangePassword}", changePasswordHttpContent);
            Assert.That(changePasswordResponse.IsSuccessStatusCode, Is.True);

            getOneResponse = await SystemUnderTests.GetAsync($"{ControllerName}?userName={user.UserName}");
            Assert.That(getOneResponse.IsSuccessStatusCode, Is.True);
            getOneContent = await getOneResponse.Content.ReadAsStringAsync();
            actual = JsonConvert.DeserializeObject<ApplicationUser>(getOneContent);
            Assert.That(actual, Is.Not.Null);
            var newPasswordHash = actual.PasswordHash;

            Assert.That(oldPasswordHash, Is.Not.EqualTo(newPasswordHash));

            var removeJsonContent = JsonConvert.SerializeObject(actual);
            var removeHttpContent = new StringContent(removeJsonContent, Encoding.UTF8, MediaType);
            var removeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Remove}", removeHttpContent);
            Assert.That(removeResponse.IsSuccessStatusCode, Is.True);
        }

        [Test]
        public virtual async Task GIVEN_UsersController_WHEN_I_add_role_to_user_THEN_it_is_being_added_to_user_in_database()
        {
            var role = Fixture.Build<IdentityRole>()
                .Without(r => r.ConcurrencyStamp)
                .Without(r => r.NormalizedName)
                .Create();

            await AddRoleAndAssert(role);

            var user = CreateApplicationUser();
            var createUserRequest = new CreateUserRequest()
            {
                User = user,
                Password = CreatePassword()
            };

            var addJsonContent = JsonConvert.SerializeObject(createUserRequest);
            var addHttpContent = new StringContent(addJsonContent, Encoding.UTF8, MediaType);
            var addResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Add}", addHttpContent);
            Assert.That(addResponse.IsSuccessStatusCode, Is.True);

            var addRemoveRoleRequest = new AddRemoveRoleRequest()
            {
                UserName = user.UserName,
                RoleName = role.Name
            };

            var addRemoveJsonContent = JsonConvert.SerializeObject(addRemoveRoleRequest);
            var addRemoveHttpContent = new StringContent(addRemoveJsonContent, Encoding.UTF8, MediaType);
            
            var addToRoleResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{UsersControllerRoutes.AddToRole}", addRemoveHttpContent);
            Assert.That(addToRoleResponse.IsSuccessStatusCode, Is.True);

            var removeFromRoleResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{UsersControllerRoutes.RemoveFromRole}", addRemoveHttpContent);
            Assert.That(removeFromRoleResponse.IsSuccessStatusCode, Is.True);

            var getOneResponse = await SystemUnderTests.GetAsync($"{ControllerName}?userName={user.UserName}");
            Assert.That(getOneResponse.IsSuccessStatusCode, Is.True);
            var getOneContent = await getOneResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<ApplicationUser>(getOneContent);
            Assert.That(actual, Is.Not.Null);

            var removeJsonContent = JsonConvert.SerializeObject(actual);
            var removeHttpContent = new StringContent(removeJsonContent, Encoding.UTF8, MediaType);
            var removeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Remove}", removeHttpContent);
            Assert.That(removeResponse.IsSuccessStatusCode, Is.True);

            await RemoveRoleAndAssert(role);
        }

        [Test]
        public virtual async Task GIVEN_UsersController_WHEN_I_add_roles_to_user_THEN_they_are_being_added_to_user_in_database()
        {
            var role1 = Fixture.Build<IdentityRole>()
                .Without(r => r.ConcurrencyStamp)
                .Without(r => r.NormalizedName)
                .Create();

            var role2 = Fixture.Build<IdentityRole>()
                .Without(r => r.ConcurrencyStamp)
                .Without(r => r.NormalizedName)
                .Create();

            var rolesToAdd = new[] { role1, role2 };

            foreach (var role in rolesToAdd)
                await AddRoleAndAssert(role);

            var user = CreateApplicationUser();
            var createUserRequest = new CreateUserRequest()
            {
                User = user,
                Password = CreatePassword()
            };

            var addJsonContent = JsonConvert.SerializeObject(createUserRequest);
            var addHttpContent = new StringContent(addJsonContent, Encoding.UTF8, MediaType);
            var addResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Add}", addHttpContent);
            Assert.That(addResponse.IsSuccessStatusCode, Is.True);

            var addRemoveRolesRequest = new AddRemoveRolesRequest()
            {
                UserName = user.UserName,
                RoleNames = new[] { role1.Name, role2.Name }
            };

            var addRemoveJsonContent = JsonConvert.SerializeObject(addRemoveRolesRequest);
            var addRemoveHttpContent = new StringContent(addRemoveJsonContent, Encoding.UTF8, MediaType);

            var addToRolesResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{UsersControllerRoutes.AddToRoles}", addRemoveHttpContent);
            Assert.That(addToRolesResponse.IsSuccessStatusCode, Is.True);

            var removeFromRolesResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{UsersControllerRoutes.RemoveFromRoles}", addRemoveHttpContent);
            Assert.That(removeFromRolesResponse.IsSuccessStatusCode, Is.True);

            var getOneResponse = await SystemUnderTests.GetAsync($"{ControllerName}?userName={user.UserName}");
            Assert.That(getOneResponse.IsSuccessStatusCode, Is.True);
            var getOneContent = await getOneResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<ApplicationUser>(getOneContent);
            Assert.That(actual, Is.Not.Null);

            var removeJsonContent = JsonConvert.SerializeObject(actual);
            var removeHttpContent = new StringContent(removeJsonContent, Encoding.UTF8, MediaType);
            var removeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Remove}", removeHttpContent);
            Assert.That(removeResponse.IsSuccessStatusCode, Is.True);

            foreach (var role in rolesToAdd)
                await RemoveRoleAndAssert(role);
        }

        protected virtual async Task AddRoleAndAssert(IdentityRole role)
        {
            var addJsonContent = JsonConvert.SerializeObject(role);
            var addHttpContent = new StringContent(addJsonContent, Encoding.UTF8, MediaType);
            var addResponse = await SystemUnderTests.PostAsync($"roles/{RepoActions.Add}", addHttpContent);
            Assert.That(addResponse.IsSuccessStatusCode, Is.True);
        }
        protected virtual async Task RemoveRoleAndAssert(IdentityRole role)
        {
            var removeJsonContent = JsonConvert.SerializeObject(role);
            var removeHttpContent = new StringContent(removeJsonContent, Encoding.UTF8, MediaType);
            var removeResponse = await SystemUnderTests.PostAsync($"roles/{RepoActions.Remove}", removeHttpContent);
            Assert.That(removeResponse.IsSuccessStatusCode, Is.True);
        }

        protected virtual string CreatePassword() => string.Concat(Fixture.Create<string>(), "@", "P");
        protected virtual ApplicationUser CreateApplicationUser()
        {
            var applicationUser = Fixture.Build<ApplicationUser>()
                .Without(u => u.Id)
                .Without(u => u.ConcurrencyStamp)
                .Without(u => u.NormalizedEmail)
                .Without(u => u.NormalizedUserName)
                .Without(u => u.PasswordHash)
                .Without(u => u.SecurityStamp)
                .Without(u => u.DeliveryAddress)
                .Without(u => u.DefaultAddress)
                .Create();

            var defaultAddress = Fixture.Build<Address>()
                .Without(a => a.Id)
                .With(a => a.PostalCode, Fixture.Create<string>()[..32])
                .Create();

            var deliveryAddress = Fixture.Build<Address>()
                .Without(a => a.Id)
                .With(a => a.PostalCode, Fixture.Create<string>()[..32])
                .Create();

            applicationUser.DefaultAddress = defaultAddress;
            applicationUser.DeliveryAddress = deliveryAddress;
            
            return applicationUser;
        }

        protected override void AssertObjectsAreEqual(ApplicationUser expected, ApplicationUser actual)
        {
            Assert.Multiple(() =>
            {
                Assert.That(expected.UserName, Is.EqualTo(actual.UserName));
                Assert.That(expected.Email, Is.EqualTo(actual.Email));
                Assert.That(expected.PhoneNumber, Is.EqualTo(actual.PhoneNumber));
                AssertAddressesAreEqual(expected.DefaultAddress, actual.DefaultAddress);
                AssertAddressesAreEqual(expected.DeliveryAddress, actual.DeliveryAddress);
                Assert.That(expected.FirstName, Is.EqualTo(actual.FirstName));
                Assert.That(expected.LastName, Is.EqualTo(actual.LastName));
            });
        }

        protected virtual void AssertAddressesAreEqual(Address expected, Address actual)
        {
            Assert.Multiple(() =>
            {
                Assert.That(expected.Country, Is.EqualTo(actual.Country));
                Assert.That(expected.City, Is.EqualTo(actual.City));
                Assert.That(expected.PostalCode, Is.EqualTo(actual.PostalCode));
                Assert.That(expected.AddressLine1, Is.EqualTo(actual.AddressLine1));
                Assert.That(expected.AddressLine2, Is.EqualTo(actual.AddressLine2));
            });
        }
    }
}
