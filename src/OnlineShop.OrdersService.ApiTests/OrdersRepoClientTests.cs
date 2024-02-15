﻿using AutoFixture;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using Moq;
using OnlineShop.Library.ArticlesService.Models;
using OnlineShop.Library.Clients.IdentityServer;
using OnlineShop.Library.Clients.OrdersService;
using OnlineShop.Library.Options;
using OnlineShop.Library.OrdersService.Models;

namespace OnlineShop.OrdersService.ApiTests
{
    public class OrdersRepoClientTests
    {
        private readonly Fixture _fixture = new();
        private IdentityServerClient _identityServerClient;
        private OrdersClient _systemUnderTest;

        public OrdersRepoClientTests()
        {
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));

            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [SetUp]
        public async Task Setup()
        {
            var serviceAddressOptionsMock = new Mock<IOptions<ServiceAdressOptions>>();
            serviceAddressOptionsMock.Setup(x => x.Value)
                .Returns(new ServiceAdressOptions()
                {
                    IdentityServer = "https://localhost:5001",
                    OrdersService = "https://localhost:5005"
                });

            _identityServerClient = new IdentityServerClient(new HttpClient(), serviceAddressOptionsMock.Object);
            _systemUnderTest = new OrdersClient(new HttpClient(), serviceAddressOptionsMock.Object);

            var identityServerApiOptions = new IdentityServerApiOptions()
            {
                ClientId = "test.client",
                ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",
                Scope = "OnlineShop.Api",
                GrantType = "client_credentials"
            };

            var token = await _identityServerClient.GetApiToken(identityServerApiOptions);
            _systemUnderTest.HttpClient.SetBearerToken(token.AccessToken);
        }

        [Test]
        public async Task GIVEN_Orders_Repo_Clinet_WHEN_I_add_order_THEN_it_is_being_added_to_database()
        {
            var expected = _fixture.Build<Order>()
                .With(o => o.Articles, _fixture.CreateMany<OrderedArticle>().ToList())
                .Create();

            var addResponse = await _systemUnderTest.Add(expected);
            Assert.That(addResponse.IsSuccessfull, Is.True);

            var getOneResponse = await _systemUnderTest.GetOne(addResponse.Payload);
            Assert.That(getOneResponse.IsSuccessfull, Is.True);
            var actual = getOneResponse.Payload;

            AssertObjectsAreEqual(expected, actual);

            var removeResponse = await _systemUnderTest.Remove(addResponse.Payload);
            Assert.That(removeResponse.IsSuccessfull, Is.True);
        }

        [Test]
        public async Task GIVEN_Orders_Repo_Client_WHEN_I_add_several_orders_THEN_they_are_being_added_to_database()
        {
            var expected1 = _fixture.Build<Order>()
                .With(o => o.Articles, _fixture.CreateMany<OrderedArticle>().ToList())
                .Create();

            var expected2 = _fixture.Build<Order>()
                .With(o => o.Articles, _fixture.CreateMany<OrderedArticle>().ToList())
                .Create();

            var ordersToAdd = new[] { expected1, expected2 };

            var addRangeResponse = await _systemUnderTest.AddRange(ordersToAdd);
            Assert.That(addRangeResponse.IsSuccessfull, Is.True);

            var getAllResponse = await _systemUnderTest.GetAll();
            var addedOrders = getAllResponse.Payload;

            foreach (var orderId in addRangeResponse.Payload)
            {
                var expectedOrder = ordersToAdd.Single(o => o.Id == orderId);
                var actualOrder = addedOrders.Single(o => o.Id == orderId);
                AssertObjectsAreEqual(expectedOrder, actualOrder);
            }

            var removeRangeResponse = await _systemUnderTest.RemoveRange(addRangeResponse.Payload);
            Assert.That(removeRangeResponse.IsSuccessfull, Is.True);
        }

        [Test]
        public async Task GIVEN_Orders_Repo_Client_WHEN_I_update_order_THEN_it_is_being_update_in_database()
        {
            var orderedArticles = _fixture.CreateMany<OrderedArticle>().ToList();
            var expected = _fixture.Build<Order>()
                .With(o => o.Articles, orderedArticles)
                .Create();

            var addResponse = await _systemUnderTest.Add(expected);
            Assert.That(addResponse.IsSuccessfull, Is.True);

            orderedArticles.ForEach(oa => oa.Name = _fixture.Create<string>());
            expected.UserId = _fixture.Create<Guid>();
            expected.AddressId = _fixture.Create<Guid>();
            expected.Articles = orderedArticles;

            var updateResponse = await _systemUnderTest.Update(expected);
            Assert.That(updateResponse.IsSuccessfull, Is.True);
            var actual = updateResponse.Payload;

            AssertObjectsAreEqual(expected, actual);

            var removeResponse = await _systemUnderTest.Remove(addResponse.Payload);
            Assert.That(removeResponse.IsSuccessfull, Is.True);
        }

        private void AssertObjectsAreEqual(Order expected, Order actual)
        {
            Assert.Multiple(() =>
            {
                Assert.That(expected.Id, Is.EqualTo(actual.Id));
                Assert.That(expected.AddressId, Is.EqualTo(actual.AddressId));
                Assert.That(expected.UserId, Is.EqualTo(actual.UserId));
                Assert.That(expected.Created, Is.EqualTo(actual.Created));
                Assert.That(expected.Articles, Has.Count.EqualTo(actual.Articles.Count));
            });
        }
    }
}
