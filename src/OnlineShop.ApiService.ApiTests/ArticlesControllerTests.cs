using AutoFixture;
using Newtonsoft.Json;
using OnlineShop.Library.ArticlesService.Models;
using OnlineShop.Library.Constants;
using System.Text;

namespace OnlineShop.ApiService.ApiTests
{
    public class ArticlesControllerTests : BaseRepoControllerTests<Article>
    {
        public ArticlesControllerTests() : base() { ControllerName = "articles"; }

        [Test]
        public virtual async Task GIVEN_ArticlesController_WHEN_I_add_entity_THEN_it_is_being_added_to_database()
        {
            var expected = Fixture.Build<Article>().Create();

            var addJsonContent = JsonConvert.SerializeObject(expected);
            var addHttpContent = new StringContent(addJsonContent, Encoding.UTF8, MediaType);
            var addResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Add}", addHttpContent);
            Assert.That(addResponse.IsSuccessStatusCode, Is.True);

            var getOneResponse = await SystemUnderTests.GetAsync($"{ControllerName}?id={expected.Id}");
            Assert.That(getOneResponse.IsSuccessStatusCode, Is.True);
            var getOneContent = await getOneResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<Article>(getOneContent);
            Assert.That(actual, Is.Not.Null);

            AssertObjectsAreEqual(expected, actual);

            var removeJsonContent = JsonConvert.SerializeObject(actual.Id);
            var removeHttpContent = new StringContent(removeJsonContent, Encoding.UTF8, MediaType);
            var removeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Remove}", removeHttpContent);
            Assert.That(removeResponse.IsSuccessStatusCode, Is.True);
        }

        [Test]
        public virtual async Task GIVEN_ArticlesController_WHEN_I_add_several_entities_THEN_they_are_being_added_to_database()
        {
            var expected1 = Fixture.Build<Article>().Create();
            var expected2 = Fixture.Build<Article>().Create();

            var articlesToAdd = new[] { expected1, expected2 };
            var addRangeJsonContent = JsonConvert.SerializeObject(articlesToAdd);
            var addRangeHttpContent = new StringContent(addRangeJsonContent, Encoding.UTF8, MediaType);
            var addRangeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.AddRange}", addRangeHttpContent);
            Assert.That(addRangeResponse.IsSuccessStatusCode, Is.True);
            var addRangeResponseContent = await addRangeResponse.Content.ReadAsStringAsync();
            var addedArticlesIds = JsonConvert.DeserializeObject<IEnumerable<Guid>>(addRangeResponseContent);
            Assert.That(addedArticlesIds, Is.Not.Null);

            var getAllResponse = await SystemUnderTests.GetAsync($"{ControllerName}/{RepoActions.GetAll}");
            Assert.That(getAllResponse.IsSuccessStatusCode, Is.True);
            var getAllContent = await getAllResponse.Content.ReadAsStringAsync();
            var addedArticles = JsonConvert.DeserializeObject<IEnumerable<Article>>(getAllContent);
            Assert.That(addedArticles, Is.Not.Null);

            foreach (var articleId in addedArticlesIds)
            {
                var expected = articlesToAdd.Single(a => a.Id == articleId);
                var actual = addedArticles.Single(a => a.Id == articleId);
                AssertObjectsAreEqual(expected, actual);
            }

            var removeRangeJsonContent = JsonConvert.SerializeObject(addedArticlesIds);
            var removeRangeHttpContent = new StringContent(removeRangeJsonContent, Encoding.UTF8, MediaType);
            var removeRangeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.RemoveRange}", removeRangeHttpContent);
            Assert.That(removeRangeResponse.IsSuccessStatusCode, Is.True);
        }

        [Test]
        public virtual async Task GIVEN_ArticlesController_WHEN_I_update_entity_THEN_it_is_being_updated_in_database()
        {
            var expected = Fixture.Build<Article>().Create();

            var addJsonContent = JsonConvert.SerializeObject(expected);
            var addHttpContent = new StringContent(addJsonContent, Encoding.UTF8, MediaType);
            var addResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Add}", addHttpContent);
            Assert.That(addResponse.IsSuccessStatusCode, Is.True);

            expected.Name = Fixture.Create<string>();
            expected.Description = Fixture.Create<string>();

            var updateJsonContent = JsonConvert.SerializeObject(expected);
            var updateHttpContent = new StringContent(updateJsonContent, Encoding.UTF8, MediaType);
            var updateResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Update}", updateHttpContent);
            Assert.That(updateResponse.IsSuccessStatusCode, Is.True);

            var getOneResponse = await SystemUnderTests.GetAsync($"{ControllerName}?id={expected.Id}");
            Assert.That(getOneResponse.IsSuccessStatusCode, Is.True);
            var getOneContent = await getOneResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<Article>(getOneContent);
            Assert.That(actual, Is.Not.Null);

            AssertObjectsAreEqual(expected, actual);

            var removeJsonContent = JsonConvert.SerializeObject(actual.Id);
            var removeHttpContent = new StringContent(removeJsonContent, Encoding.UTF8, MediaType);
            var removeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Remove}", removeHttpContent);
            Assert.That(removeResponse.IsSuccessStatusCode, Is.True);
        }

        protected override void AssertObjectsAreEqual(Article expected, Article actual)
        {
            Assert.Multiple(() =>
            {
                Assert.That(expected.Id, Is.EqualTo(actual.Id));
                Assert.That(expected.Name, Is.EqualTo(actual.Name));
                Assert.That(expected.Description, Is.EqualTo(actual.Description));
            });
        }
    }
}
