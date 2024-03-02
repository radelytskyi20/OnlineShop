using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using OnlineShop.Library.Constants;
using System.Text;

namespace OnlineShop.ApiService.ApiTests
{
    public class RolesControllerTests : BaseRepoControllerTests<IdentityRole>
    {
        public RolesControllerTests() : base() { ControllerName = "roles"; }

        [Test]
        public virtual async Task GIVEN_RolesController_WHEN_I_add_entity_THEN_it_is_being_added_to_database()
        {
            var expected = Fixture.Build<IdentityRole>()
                .Without(r => r.ConcurrencyStamp)
                .Without(r => r.NormalizedName)
                .Create();

            var addJsonContent = JsonConvert.SerializeObject(expected);
            var addHttpContent = new StringContent(addJsonContent, Encoding.UTF8, MediaType);
            var addResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Add}", addHttpContent);
            Assert.That(addResponse.IsSuccessStatusCode, Is.True);

            var getOneResponse = await SystemUnderTests.GetAsync($"{ControllerName}?name={expected.Name}");
            Assert.That(getOneResponse.IsSuccessStatusCode, Is.True);
            var getOneContent = await getOneResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<IdentityRole>(getOneContent);
            Assert.That(actual, Is.Not.Null);

            AssertObjectsAreEqual(expected, actual);

            var removeJsonContent = JsonConvert.SerializeObject(actual);
            var removeHttpContent = new StringContent(removeJsonContent, Encoding.UTF8, MediaType);
            var removeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Remove}", removeHttpContent);
            Assert.That(removeResponse.IsSuccessStatusCode, Is.True);
        }

        [Test]
        public virtual async Task GIVEN_RolesController_WHEN_I_get_all_entities_THEN_they_are_returning_from_database()
        {
            var expected1 = Fixture.Build<IdentityRole>()
                .Without(r => r.ConcurrencyStamp)
                .Without(r => r.NormalizedName)
                .Create();

            var expected2 = Fixture.Build<IdentityRole>()
                .Without(r => r.ConcurrencyStamp)
                .Without(r => r.NormalizedName)
                .Create();

            var rolesToAdd = new[] { expected1, expected2 };

            foreach (var role in rolesToAdd)
            {
                var addJsonContent = JsonConvert.SerializeObject(role);
                var addHttpContent = new StringContent(addJsonContent, Encoding.UTF8, MediaType);
                var addResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Add}", addHttpContent);
                Assert.That(addResponse.IsSuccessStatusCode, Is.True);
            }

            var getAllResponse = await SystemUnderTests.GetAsync($"{ControllerName}/{RepoActions.GetAll}");
            Assert.That(getAllResponse.IsSuccessStatusCode, Is.True);
            var getAllContent = await getAllResponse.Content.ReadAsStringAsync();
            var addedRoles = JsonConvert.DeserializeObject<IEnumerable<IdentityRole>>(getAllContent);
            Assert.That(addedRoles, Is.Not.Null);

            foreach (var expectedRole in rolesToAdd)
            {
                var actual = addedRoles.Single(r => r.Id == expectedRole.Id);
                AssertObjectsAreEqual(expectedRole, actual);
            }

            foreach (var role in rolesToAdd)
            {
                var removeJsonContent = JsonConvert.SerializeObject(role);
                var removeHttpContent = new StringContent(removeJsonContent, Encoding.UTF8, MediaType);
                var removeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Remove}", removeHttpContent);
                Assert.That(removeResponse.IsSuccessStatusCode, Is.True);
            }
        }

        [Test]
        public virtual async Task GIVEN_RolesController_WHEN_I_update_entity_THEN_it_is_being_updated_in_database()
        {
            var expected = Fixture.Build<IdentityRole>()
                .Without(r => r.ConcurrencyStamp)
                .Without(r => r.NormalizedName)
                .Create();

            var addJsonContent = JsonConvert.SerializeObject(expected);
            var addHttpContent = new StringContent(addJsonContent, Encoding.UTF8, MediaType);
            var addResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Add}", addHttpContent);
            Assert.That(addResponse.IsSuccessStatusCode, Is.True);

            expected.Name = Fixture.Create<string>();

            var updateJsonContent = JsonConvert.SerializeObject(expected);
            var updateHttpContent = new StringContent(updateJsonContent, Encoding.UTF8, MediaType);
            var updateResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Update}", updateHttpContent);
            Assert.That(updateResponse.IsSuccessStatusCode, Is.True);

            var getOneResponse = await SystemUnderTests.GetAsync($"{ControllerName}?name={expected.Name}");
            Assert.That(getOneResponse.IsSuccessStatusCode, Is.True);
            var getOneContent = await getOneResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<IdentityRole>(getOneContent);
            Assert.That(actual, Is.Not.Null);

            AssertObjectsAreEqual(expected, actual);

            var removeJsonContent = JsonConvert.SerializeObject(actual);
            var removeHttpContent = new StringContent(removeJsonContent, Encoding.UTF8, MediaType);
            var removeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Remove}", removeHttpContent);
            Assert.That(removeResponse.IsSuccessStatusCode, Is.True);
        }

        protected override void AssertObjectsAreEqual(IdentityRole expected, IdentityRole actual)
        {
            Assert.Multiple(() =>
            {
                Assert.That(expected.Id, Is.EqualTo(actual.Id));
                Assert.That(expected.Name, Is.EqualTo(actual.Name));
            });
        }
    }
}
