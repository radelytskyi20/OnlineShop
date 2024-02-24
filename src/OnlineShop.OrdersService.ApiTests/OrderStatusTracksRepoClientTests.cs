using AutoFixture;
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
    public class OrderStatusTracksRepoClientTests
    {
        private readonly Fixture _fixture = new();
        private IdentityServerClient _identityServerClient;
        private OrdersClient _ordersClient;
        private OrderStatusTracksClient _systemUnderTest;

        //max id value from OrderStatuses table; see OrdersDbContext.cs, OnModelCreating method
        private const int _maxOrderStatusId = 4;

        public OrderStatusTracksRepoClientTests()
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
            _ordersClient = new OrdersClient(new HttpClient(), serviceAddressOptionsMock.Object);
            _systemUnderTest = new OrderStatusTracksClient(new HttpClient(), serviceAddressOptionsMock.Object);

            var identityServerApiOptions = new IdentityServerApiOptions()
            {
                ClientId = "test.client",
                ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",
                Scope = "OnlineShop.Api",
                GrantType = "client_credentials"
            };

            var token = await _identityServerClient.GetApiToken(identityServerApiOptions);
            _ordersClient.HttpClient.SetBearerToken(token.AccessToken);
            _systemUnderTest.HttpClient.SetBearerToken(token.AccessToken);
        }

        [Test]
        public async Task GIVEN_OrderStatusTracks_Repo_Clinet_WHEN_I_add_order_status_track_THEN_it_is_being_added_to_database()
        {
            var order = _fixture.Build<Order>()
                .With(o => o.Articles, _fixture.CreateMany<OrderedArticle>().ToList())
                .Without(o => o.OrderStatusTracks)
                .Create();

            var addOrderResponse = await _ordersClient.Add(order);
            Assert.That(addOrderResponse.IsSuccessfull, Is.True);

            var expected = _fixture.Build<OrderStatusTrack>()
                .With(ost => ost.OrderId, order.Id)
                .With(ost => ost.OrderStatusId, _fixture.Create<int>() % _maxOrderStatusId + 1) //random value from 1 to 4
                .Without(ost => ost.OrderStatus)
                .Create();

            var addOrderStatusTrackResponse = await _systemUnderTest.Add(expected);
            Assert.That(addOrderStatusTrackResponse.IsSuccessfull, Is.True);
            
            var getOneResponse = await _systemUnderTest.GetOne(addOrderStatusTrackResponse.Payload);
            Assert.That(getOneResponse.IsSuccessfull, Is.True);
            var actual = getOneResponse.Payload;

            AssertObjectsAreEqual(expected, actual);

            var removeOrderStatusTrackResponse = await _systemUnderTest.Remove(addOrderStatusTrackResponse.Payload);
            Assert.That(removeOrderStatusTrackResponse.IsSuccessfull, Is.True);

            var removeOrderResponse = await _ordersClient.Remove(addOrderResponse.Payload);
            Assert.That(removeOrderResponse.IsSuccessfull, Is.True);
        }

        [Test]
        public async Task GIVEN_OrderStatusTracks_Repo_Clinet_WHEN_I_add_several_order_status_tracks_THEN_they_are_being_added_to_database()
        {
            var order = _fixture.Build<Order>()
                .With(o => o.Articles, _fixture.CreateMany<OrderedArticle>().ToList())
                .Without(o => o.OrderStatusTracks)
                .Create();

            var addOrderResponse = await _ordersClient.Add(order);
            Assert.That(addOrderResponse.IsSuccessfull, Is.True);

            var expected1 = _fixture.Build<OrderStatusTrack>()
                .With(ost => ost.OrderId, order.Id)
                .With(ost => ost.OrderStatusId, _fixture.Create<int>() % _maxOrderStatusId + 1)
                .Without(ost => ost.OrderStatus)
                .Create();

            var expected2 = _fixture.Build<OrderStatusTrack>()
                .With(ost => ost.OrderId, order.Id)
                .With(ost => ost.OrderStatusId, _fixture.Create<int>() % _maxOrderStatusId + 1)
                .Without(ost => ost.OrderStatus)
                .Create();

            var orderStatusTracksToAdd = new[] { expected1, expected2 };

            var addRangeOrderStatusTracksResponse = await _systemUnderTest.AddRange(orderStatusTracksToAdd);
            Assert.That(addRangeOrderStatusTracksResponse.IsSuccessfull, Is.True);

            var getAllResponse = await _systemUnderTest.GetAll();
            Assert.That(getAllResponse.IsSuccessfull, Is.True);
            var addedOrderStatusTracks = getAllResponse.Payload;

            foreach (var orderStatusTrackId in addRangeOrderStatusTracksResponse.Payload)
            {
                var expected = orderStatusTracksToAdd.Single(ost => ost.Id == orderStatusTrackId);
                var actual = addedOrderStatusTracks.Single(ost => ost.Id == orderStatusTrackId);
                AssertObjectsAreEqual(expected, actual);
            }

            var removeOrderStatusTracksResponse = await _systemUnderTest.RemoveRange(addRangeOrderStatusTracksResponse.Payload);
            Assert.That(removeOrderStatusTracksResponse.IsSuccessfull, Is.True);

            var removeOrderResponse = await _ordersClient.Remove(addOrderResponse.Payload);
            Assert.That(removeOrderResponse.IsSuccessfull, Is.True);
        }

        [Test]
        public async Task GIVEN_OrderStatusTracks_Repo_Clinet_WHEN_I_update_order_status_track_THEN_it_is_being_updated_in_database()
        {
            var order = _fixture.Build<Order>()
                .With(o => o.Articles, _fixture.CreateMany<OrderedArticle>().ToList())
                .Without(o => o.OrderStatusTracks)
                .Create();

            var expectedOrderStatusTrack = _fixture.Build<OrderStatusTrack>()
                .With(ost => ost.OrderId, order.Id)
                .With(ost => ost.OrderStatusId, _fixture.Create<int>() % _maxOrderStatusId + 1)
                .Without(ost => ost.OrderStatus)
                .Create();

            order.OrderStatusTracks = new List<OrderStatusTrack> { expectedOrderStatusTrack };

            var addOrderResponse = await _ordersClient.Add(order);
            Assert.That(addOrderResponse.IsSuccessfull, Is.True);

            expectedOrderStatusTrack.Assigned = _fixture.Create<DateTime>();

            var updateOrderStatusTrackResponse = await _systemUnderTest.Update(expectedOrderStatusTrack);
            Assert.That(updateOrderStatusTrackResponse.IsSuccessfull, Is.True);
            var actual = updateOrderStatusTrackResponse.Payload;

            AssertObjectsAreEqual(expectedOrderStatusTrack, actual);

            var removeOrderResponse = await _ordersClient.Remove(addOrderResponse.Payload);
            Assert.That(removeOrderResponse.IsSuccessfull, Is.True);
        }

        private void AssertObjectsAreEqual(OrderStatusTrack expected, OrderStatusTrack actual)
        {
            Assert.Multiple(() =>
            {
                Assert.That(actual.Id, Is.EqualTo(expected.Id));
                Assert.That(actual.Assigned, Is.EqualTo(expected.Assigned));
                Assert.That(actual.OrderId, Is.EqualTo(expected.OrderId));
                Assert.That(actual.OrderStatusId, Is.EqualTo(expected.OrderStatusId));
            });
        }
    }
}