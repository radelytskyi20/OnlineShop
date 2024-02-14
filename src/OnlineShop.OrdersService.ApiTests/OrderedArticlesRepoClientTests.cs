using AutoFixture;
using Microsoft.Extensions.Options;
using Moq;
using OnlineShop.Library.ArticlesService.Models;
using OnlineShop.Library.Clients.OrdersService;
using OnlineShop.Library.Options;
using OnlineShop.Library.OrdersService.Models;

namespace OnlineShop.OrdersService.ApiTests
{
    public class OrderedArticlesRepoClientTests
    {
        private readonly Fixture _fixture = new();
        //private IdentityServerClient _identityServerClient;
        private OrdersClient _ordersClient;
        private OrderedArticlesClient _systemUnderTest;

        public OrderedArticlesRepoClientTests()
        {
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));

            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [SetUp]
        public void Setup()
        {
            var serviceAddressOptionsMock = new Mock<IOptions<ServiceAdressOptions>>();
            serviceAddressOptionsMock.Setup(x => x.Value)
                .Returns(new ServiceAdressOptions()
                {
                    IdentityServer = "https://localhost:5001",
                    OrdersService = "https://localhost:5005"
                });

            //_identityServerClient = new IdentityServerClient(new HttpClient(), serviceAddressOptionsMock.Object);
            _ordersClient = new OrdersClient(new HttpClient(), serviceAddressOptionsMock.Object);
            _systemUnderTest = new OrderedArticlesClient(new HttpClient(), serviceAddressOptionsMock.Object);

            //var identityServerApiOptions = new IdentityServerApiOptions()
            //{
            //    ClientId = "test.client",
            //    ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",
            //    Scope = "OnlineShop.Api",
            //    GrantType = "client_credentials"
            //};

            //var token = await _identityServerClient.GetApiToken(identityServerApiOptions);
            //_systemUnderTest.HttpClient.SetBearerToken(token.AccessToken);
        }

        [Test]
        public async Task GIVEN_Ordered_Articles_Repo_Client_WHEN_I_add_ordered_article_THEN_it_is_being_added_to_database()
        {
            var order = _fixture.Build<Order>()
                .With(o => o.Articles, Enumerable.Empty<OrderedArticle>().ToList())
                .Create();

            var addOrderResponse = await _ordersClient.Add(order);
            Assert.That(addOrderResponse.IsSuccessfull, Is.True);

            var expected = _fixture.Build<OrderedArticle>()
                .With(o => o.OrderId, order.Id)
                .Create();

            var addOrderedArticleResponse = await _systemUnderTest.Add(expected);
            Assert.That(addOrderedArticleResponse.IsSuccessfull, Is.True);

            var getOneResponse = await _systemUnderTest.GetOne(addOrderedArticleResponse.Payload);
            Assert.That(getOneResponse.IsSuccessfull, Is.True);
            var actual = getOneResponse.Payload;

            AssertObjectsAreEqual(expected, actual);
            
            var removeOrederResponse = await _ordersClient.Remove(addOrderResponse.Payload);
            Assert.That(removeOrederResponse.IsSuccessfull, Is.True);
        }

        [Test]
        public async Task GIVEN_Ordered_Articles_Repo_Client_WHEN_I_add_several_ordered_articles_THEN_they_are_being_added_to_database()
        {
            var order = _fixture.Build<Order>()
                .With(o => o.Articles, Enumerable.Empty<OrderedArticle>().ToList())
                .Create();

            var addOrderResponse = await _ordersClient.Add(order);
            Assert.That(addOrderResponse.IsSuccessfull, Is.True);

            var expected1 = _fixture.Build<OrderedArticle>()
                .With(o => o.OrderId, order.Id)
                .Create();

            var expected2 = _fixture.Build<OrderedArticle>()
                .With(o => o.OrderId, order.Id)
                .Create();

            var orderedArticlesToAdd = new[] { expected1, expected2 };

            var addOrderedArticleResponse = await _systemUnderTest.AddRange(orderedArticlesToAdd);
            Assert.That(addOrderedArticleResponse.IsSuccessfull, Is.True);

            var getAllResponse = await _systemUnderTest.GetAll();
            Assert.That(getAllResponse.IsSuccessfull, Is.True);
            var addedOrderedArticles = getAllResponse.Payload;

            foreach (var orderedArticleId in addOrderedArticleResponse.Payload)
            {
                var expectedOrder = orderedArticlesToAdd.Single(o => o.Id == orderedArticleId);
                var actualOrder = addedOrderedArticles.Single(o => o.Id == orderedArticleId);
                AssertObjectsAreEqual(expectedOrder, actualOrder);
            }

            var removeOrederResponse = await _ordersClient.Remove(order.Id);
            Assert.That(removeOrederResponse.IsSuccessfull, Is.True);
        }

        [Test]
        public async Task GIVEN_Ordered_Articles_Repo_Client_WHEN_I_update_ordered_article_THEN_it_is_being_updated_in_database()
        {
            var order = _fixture.Build<Order>()
                .With(o => o.Articles, Enumerable.Empty<OrderedArticle>().ToList())
                .Create();

            var addOrderResponse = await _ordersClient.Add(order);
            Assert.That(addOrderResponse.IsSuccessfull, Is.True);

            var expected = _fixture.Build<OrderedArticle>()
                .With(o => o.OrderId, order.Id)
                .Create();

            var addOrderedArticleResponse = await _systemUnderTest.Add(expected);
            Assert.That(addOrderedArticleResponse.IsSuccessfull, Is.True);

            expected.Name = _fixture.Create<string>();
            expected.Description = _fixture.Create<string>();
            expected.Price = _fixture.Create<decimal>();
            expected.Quantity = _fixture.Create<int>();

            var updateResponse = await _systemUnderTest.Update(expected);
            Assert.That(updateResponse.IsSuccessfull, Is.True);

            var getOneResponse = await _systemUnderTest.GetOne(addOrderedArticleResponse.Payload);
            Assert.That(getOneResponse.IsSuccessfull, Is.True);
            var actual = getOneResponse.Payload;

            AssertObjectsAreEqual(expected, actual);

            var removeOrederResponse = await _ordersClient.Remove(order.Id);
            Assert.That(removeOrederResponse.IsSuccessfull, Is.True);
        }

        private void AssertObjectsAreEqual(OrderedArticle expected, OrderedArticle actual)
        {
            Assert.Multiple(() =>
            {
                Assert.That(actual.Id, Is.EqualTo(expected.Id));
                Assert.That(actual.Name, Is.EqualTo(expected.Name));
                Assert.That(actual.Description, Is.EqualTo(expected.Description));
                Assert.That(actual.Price, Is.EqualTo(expected.Price));
                Assert.That(actual.Quantity, Is.EqualTo(expected.Quantity));

                if (expected.Price != actual.Price)
                {
                    Assert.That(actual.PriceListName, Is.EqualTo("Manualy assigned"));
                }
            });
        }
    }
}
