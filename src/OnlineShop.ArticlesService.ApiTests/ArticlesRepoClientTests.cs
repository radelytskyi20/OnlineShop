using AutoFixture;
using OnlineShop.Library.ArticlesService.Models;
using OnlineShop.Library.Clients.ArticlesService;

namespace OnlineShop.ArticlesService.ApiTests
{
    public class ArticlesRepoClientTests : ArticleServiceRepoBaseApiTest<ArticlesClient, Article>
    {
        protected override void AmendExpectedEntityForUpdating(Article expected)
        {
            expected.Name = Fixture.Create<string>();
            expected.Description = Fixture.Create<string>();
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

        protected override void CreateSystemUnderTest()
        {
            SystemUnderTest = new ArticlesClient(new HttpClient(), ServiceAddressOptions);
        }
    }
}
