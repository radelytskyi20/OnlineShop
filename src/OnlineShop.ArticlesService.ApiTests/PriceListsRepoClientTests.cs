using AutoFixture;
using IdentityModel.Client;
using OnlineShop.Library.ArticlesService.Models;
using OnlineShop.Library.Clients.ArticlesService;

namespace OnlineShop.ArticlesService.ApiTests
{
    public class PriceListsRepoClientTests : ArticleServiceRepoBaseApiTest<PriceListsClient, PriceList>
    {
        private ArticlesClient ArticlesClient = null!;
        private Guid _articleId;

        [Test]
        public override async Task GIVEN_Repo_Client_WHEN_I_add_entity_THEN_it_is_being_added_to_database()
        {
            var articleId = await CreateArticleAndAssert();
            await base.GIVEN_Repo_Client_WHEN_I_add_entity_THEN_it_is_being_added_to_database();
            await RemoveArticleAndAssert(articleId);
        }

        [Test]
        public override async Task GIVEN_Repo_Client_WHEN_I_add_several_entities_THEN_they_are_being_added_to_database()
        {
            var articleId = await CreateArticleAndAssert();
            await base.GIVEN_Repo_Client_WHEN_I_add_several_entities_THEN_they_are_being_added_to_database();
            await RemoveArticleAndAssert(articleId);
        }

        [Test]
        public override async Task GIVEN_Repo_Client_WHEN_I_update_entity_THEN_it_is_being_updated_in_database()
        {
            var articleId = await CreateArticleAndAssert();
            await base.GIVEN_Repo_Client_WHEN_I_update_entity_THEN_it_is_being_updated_in_database();
            await RemoveArticleAndAssert(articleId);
        }

        protected override void AmendExpectedEntityForUpdating(PriceList expected)
        {
            expected.Name = Fixture.Create<string>();
            expected.Price = Fixture.Create<decimal>();
            expected.ValidFrom = Fixture.Create<DateTime>();
            expected.ValidTo = Fixture.Create<DateTime>();
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

        protected override void CreateSystemUnderTest()
        {
            SystemUnderTest = new PriceListsClient(new HttpClient(), ServiceAddressOptions);
            ArticlesClient = new ArticlesClient(new HttpClient(), ServiceAddressOptions);
        }

        protected override async Task AuthorizeSystemUnderTests()
        {
            var token = await IdentityServerClient.GetApiToken(IdentityServerApiOptions);
            SystemUnderTest.HttpClient.SetBearerToken(token.AccessToken);
            ArticlesClient.HttpClient.SetBearerToken(token.AccessToken);
        }

        protected override PriceList CreateExpectedEntity() => 
            Fixture.Build<PriceList>()
            .With(priceList => priceList.ArticleId, _articleId)
            .Create();

        private async Task<Guid> CreateArticleAndAssert()
        {
            var article = Fixture.Build<Article>().Create();

            var addArticleResponse = await ArticlesClient.Add(article);
            Assert.That(addArticleResponse.IsSuccessfull, Is.True);
            _articleId = article.Id;

            return article.Id;
        }
        private async Task RemoveArticleAndAssert(Guid articleId) 
        {
            var removeArticleResponse = await ArticlesClient.Remove(articleId);
            Assert.That(removeArticleResponse.IsSuccessfull, Is.True);
        }
    }
}
