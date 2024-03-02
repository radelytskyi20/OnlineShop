using AutoFixture;
using Newtonsoft.Json;
using OnlineShop.Library.ArticlesService.Models;
using OnlineShop.Library.Constants;
using System.Text;

namespace OnlineShop.ApiService.ApiTests
{
    public class PriceListsControllerTests : BaseRepoControllerTests<PriceList>
    {
        public PriceListsControllerTests() : base() { ControllerName = "pricelists"; }

        [Test]
        public virtual async Task GIVEN_PriceListsController_WHEN_I_add_entity_THEN_it_is_being_added_to_database()
        {
            var article = Fixture.Build<Article>()
                .Without(a => a.PriceLists)
                .Create();

            var addArticleJsonContent = JsonConvert.SerializeObject(article);
            var addArticleHttpContent = new StringContent(addArticleJsonContent, Encoding.UTF8, MediaType);
            var addArticleResponse = await SystemUnderTests.PostAsync($"articles/{RepoActions.Add}", addArticleHttpContent);
            Assert.That(addArticleResponse.IsSuccessStatusCode, Is.True);

            var expected = Fixture.Build<PriceList>()
                .With(pl => pl.ArticleId, article.Id)
                .Create();

            var addJsonContent = JsonConvert.SerializeObject(expected);
            var addHttpContent = new StringContent(addJsonContent, Encoding.UTF8, MediaType);
            var addResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Add}", addHttpContent);
            Assert.That(addResponse.IsSuccessStatusCode, Is.True);

            var getOneResponse = await SystemUnderTests.GetAsync($"{ControllerName}?id={expected.Id}");
            Assert.That(getOneResponse.IsSuccessStatusCode, Is.True);
            var getOneContent = await getOneResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<PriceList>(getOneContent);
            Assert.That(actual, Is.Not.Null);

            AssertObjectsAreEqual(expected, actual);

            var removeJsonContent = JsonConvert.SerializeObject(actual.Id);
            var removeHttpContent = new StringContent(removeJsonContent, Encoding.UTF8, MediaType);
            var removeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Remove}", removeHttpContent);
            Assert.That(removeResponse.IsSuccessStatusCode, Is.True);

            var removeArticleJsonContent = JsonConvert.SerializeObject(article.Id);
            var removeArticleHttpContent = new StringContent(removeArticleJsonContent, Encoding.UTF8, MediaType);
            var removeArticleResponse = await SystemUnderTests.PostAsync($"articles/{RepoActions.Remove}", removeArticleHttpContent);
            Assert.That(removeArticleResponse.IsSuccessStatusCode, Is.True);
        }

        [Test]
        public virtual async Task GIVEN_PriceListsController_WHEN_I_add_several_entities_THEN_they_are_being_added_to_database()
        {
            var article = Fixture.Build<Article>()
                .Without(a => a.PriceLists)
                .Create();

            var addArticleJsonContent = JsonConvert.SerializeObject(article);
            var addArticleHttpContent = new StringContent(addArticleJsonContent, Encoding.UTF8, MediaType);
            var addArticleResponse = await SystemUnderTests.PostAsync($"articles/{RepoActions.Add}", addArticleHttpContent);
            Assert.That(addArticleResponse.IsSuccessStatusCode, Is.True);

            var expected1 = Fixture.Build<PriceList>()
                .With(pl => pl.ArticleId, article.Id)
                .Create();

            var expected2 = Fixture.Build<PriceList>()
                .With(pl => pl.ArticleId, article.Id)
                .Create();

            var priceListsToAdd = new[] { expected1, expected2 };

            var addRangeJsonContent = JsonConvert.SerializeObject(priceListsToAdd);
            var addRangeHttpContent = new StringContent(addRangeJsonContent, Encoding.UTF8, MediaType);
            var addRangeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.AddRange}", addRangeHttpContent);
            Assert.That(addRangeResponse.IsSuccessStatusCode, Is.True);
            var addRangeResponseContent = await addRangeResponse.Content.ReadAsStringAsync();
            var addedPriceListsIds = JsonConvert.DeserializeObject<IEnumerable<Guid>>(addRangeResponseContent);
            Assert.That(addedPriceListsIds, Is.Not.Null);

            var getAllResponse = await SystemUnderTests.GetAsync($"{ControllerName}/{RepoActions.GetAll}");
            Assert.That(getAllResponse.IsSuccessStatusCode, Is.True);
            var getAllContent = await getAllResponse.Content.ReadAsStringAsync();
            var addedPriceLists = JsonConvert.DeserializeObject<IEnumerable<PriceList>>(getAllContent);
            Assert.That(addedPriceLists, Is.Not.Null);

            foreach (var priceListId in addedPriceListsIds)
            {
                var expected = priceListsToAdd.Single(pl => pl.Id == priceListId);
                var actual = addedPriceLists.Single(pl => pl.Id == priceListId);
                AssertObjectsAreEqual(expected, actual);
            }

            var removeRangeJsonContent = JsonConvert.SerializeObject(addedPriceListsIds);
            var removeRangeHttpContent = new StringContent(removeRangeJsonContent, Encoding.UTF8, MediaType);
            var removeRangeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.RemoveRange}", removeRangeHttpContent);
            Assert.That(removeRangeResponse.IsSuccessStatusCode, Is.True);

            var removeArticleJsonContent = JsonConvert.SerializeObject(article.Id);
            var removeArticleHttpContent = new StringContent(removeArticleJsonContent, Encoding.UTF8, MediaType);
            var removeArticleResponse = await SystemUnderTests.PostAsync($"articles/{RepoActions.Remove}", removeArticleHttpContent);
            Assert.That(removeArticleResponse.IsSuccessStatusCode, Is.True);
        }

        [Test]
        public virtual async Task GIVEN_PriceListsController_WHEN_I_update_entity_THEN_it_is_being_updated_in_database()
        {
            var article = Fixture.Build<Article>()
                .Without(a => a.PriceLists)
                .Create();

            var addArticleJsonContent = JsonConvert.SerializeObject(article);
            var addArticleHttpContent = new StringContent(addArticleJsonContent, Encoding.UTF8, MediaType);
            var addArticleResponse = await SystemUnderTests.PostAsync($"articles/{RepoActions.Add}", addArticleHttpContent);
            Assert.That(addArticleResponse.IsSuccessStatusCode, Is.True);

            var expected = Fixture.Build<PriceList>()
                .With(pl => pl.ArticleId, article.Id)
                .Create();

            var addJsonContent = JsonConvert.SerializeObject(expected);
            var addHttpContent = new StringContent(addJsonContent, Encoding.UTF8, MediaType); 
            var addResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Add}", addHttpContent);
            Assert.That(addResponse.IsSuccessStatusCode, Is.True);

            expected.Name = Fixture.Create<string>();
            expected.Price = Fixture.Create<decimal>();
            expected.ValidFrom = Fixture.Create<DateTime>();
            expected.ValidTo = Fixture.Create<DateTime>();

            var updateJsonContent = JsonConvert.SerializeObject(expected);
            var updateHttpContent = new StringContent(updateJsonContent, Encoding.UTF8, MediaType);
            var updateResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Update}", updateHttpContent);
            Assert.That(updateResponse.IsSuccessStatusCode, Is.True);

            var getOneResponse = await SystemUnderTests.GetAsync($"{ControllerName}?id={expected.Id}");
            Assert.That(getOneResponse.IsSuccessStatusCode, Is.True);
            var getOneContent = await getOneResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<PriceList>(getOneContent);
            Assert.That(actual, Is.Not.Null);

            AssertObjectsAreEqual(expected, actual);

            var removeJsonContent = JsonConvert.SerializeObject(actual.Id);
            var removeHttpContent = new StringContent(removeJsonContent, Encoding.UTF8, MediaType);
            var removeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Remove}", removeHttpContent);
            Assert.That(removeResponse.IsSuccessStatusCode, Is.True);

            var removeArticleJsonContent = JsonConvert.SerializeObject(article.Id);
            var removeArticleHttpContent = new StringContent(removeArticleJsonContent, Encoding.UTF8, MediaType);
            var removeArticleResponse = await SystemUnderTests.PostAsync($"articles/{RepoActions.Remove}", removeArticleHttpContent);
            Assert.That(removeArticleResponse.IsSuccessStatusCode, Is.True);
        }

        protected override void AssertObjectsAreEqual(PriceList expected, PriceList actual)
        {
            Assert.Multiple(() =>
            {
                Assert.That(expected.Name, Is.EqualTo(actual.Name));
                Assert.That(expected.Price, Is.EqualTo(actual.Price));
                Assert.That(expected.ValidFrom, Is.EqualTo(actual.ValidFrom));
                Assert.That(expected.ValidTo, Is.EqualTo(actual.ValidTo));
            });
        }
    }
}
