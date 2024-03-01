using AutoFixture;
using Newtonsoft.Json;
using OnlineShop.Library.ArticlesService.Models;
using OnlineShop.Library.Constants;
using OnlineShop.Library.OrdersService.Models;
using System.Text;

namespace OnlineShop.ApiService.ApiTests
{
    public class OrderStatusTracksControllerTests : BaseRepoControllerTests<OrderStatusTrack>
    {
        public OrderStatusTracksControllerTests() : base() { ControllerName = "orderstatustracks"; }

        private const int _maxOrderStatusId = 4;

        [Test]
        public virtual async Task GIVEN_OrderStatusTracksController_WHEN_I_add_entity_THEN_it_is_being_added_to_database()
        {
            var order = Fixture.Build<Order>()
                .With(o => o.Articles, Fixture.CreateMany<OrderedArticle>().ToList())
                .Without(o => o.OrderStatusTracks)
                .Create();

            await AddOrderAndAssert(order);

            var expected = Fixture.Build<OrderStatusTrack>()
                .With(ost => ost.OrderId, order.Id)
                .With(ost => ost.OrderStatusId, Fixture.Create<int>() % _maxOrderStatusId + 1) //random value from 1 to 4
                .Without(ost => ost.OrderStatus)
                .Create();

            var addJsonContent = JsonConvert.SerializeObject(expected);
            var addHttpContent = new StringContent(addJsonContent, Encoding.UTF8, MediaType);
            var addResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Add}", addHttpContent);
            Assert.That(addResponse.IsSuccessStatusCode, Is.True);

            var getOneResponse = await SystemUnderTests.GetAsync($"{ControllerName}?id={expected.Id}");
            Assert.That(getOneResponse.IsSuccessStatusCode, Is.True);
            var getOneHttpContent = await getOneResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<OrderStatusTrack>(getOneHttpContent);
            Assert.That(actual, Is.Not.Null);

            AssertObjectsAreEqual(expected, actual);

            var removeJsonContent = JsonConvert.SerializeObject(actual.Id);
            var removeHttpContent = new StringContent(removeJsonContent, Encoding.UTF8, MediaType);
            var removeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Remove}", removeHttpContent);
            Assert.That(removeResponse.IsSuccessStatusCode, Is.True);

            await RemoveOrderAndAssert(order.Id);
        }

        [Test]
        public async Task GIVEN_OrderStatusTracksController_WHEN_I_add_several_entities_THEN_they_are_being_added_to_database()
        {
            var order = Fixture.Build<Order>()
                .With(o => o.Articles, Fixture.CreateMany<OrderedArticle>().ToList())
                .Without(o => o.OrderStatusTracks)
                .Create();

            await AddOrderAndAssert(order);

            var expected1 = Fixture.Build<OrderStatusTrack>()
                .With(ost => ost.OrderId, order.Id)
                .With(ost => ost.OrderStatusId, Fixture.Create<int>() % _maxOrderStatusId + 1)
                .Without(ost => ost.OrderStatus)
                .Create();

            var expected2 = Fixture.Build<OrderStatusTrack>()
                .With(ost => ost.OrderId, order.Id)
                .With(ost => ost.OrderStatusId, Fixture.Create<int>() % _maxOrderStatusId + 1)
                .Without(ost => ost.OrderStatus)
                .Create();

            var orderStatusTracksToAdd = new[] { expected1, expected2 };

            var addRangeOrderStatusTracksJsonContent = JsonConvert.SerializeObject(orderStatusTracksToAdd);
            var addRangeOrderStatusTracksHttpContent = new StringContent(addRangeOrderStatusTracksJsonContent, Encoding.UTF8, MediaType);
            var addRangeOrderStatusTracksResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.AddRange}", addRangeOrderStatusTracksHttpContent);
            Assert.That(addRangeOrderStatusTracksResponse.IsSuccessStatusCode, Is.True);
            
            var addRangeResponseContent = await addRangeOrderStatusTracksResponse.Content.ReadAsStringAsync();
            var addedOrderStatusTracksIds = JsonConvert.DeserializeObject<IEnumerable<Guid>>(addRangeResponseContent);
            Assert.That(addedOrderStatusTracksIds, Is.Not.Null);

            var getAllResponse = await SystemUnderTests.GetAsync($"{ControllerName}/{RepoActions.GetAll}");
            Assert.That(getAllResponse.IsSuccessStatusCode, Is.True);
            var getAllContent = await getAllResponse.Content.ReadAsStringAsync();
            var addedOrderStatusTracks = JsonConvert.DeserializeObject<IEnumerable<OrderStatusTrack>>(getAllContent);
            Assert.That(addedOrderStatusTracks, Is.Not.Null);

            foreach (var orderStatusTrackId in addedOrderStatusTracksIds)
            {
                var expected = orderStatusTracksToAdd.Single(ost => ost.Id == orderStatusTrackId);
                var actual = addedOrderStatusTracks.Single(ost => ost.Id == orderStatusTrackId);
                AssertObjectsAreEqual(expected, actual);
            }

            var removeRangeJsonContent = JsonConvert.SerializeObject(addedOrderStatusTracksIds);
            var removeRangeHttpContent = new StringContent(removeRangeJsonContent, Encoding.UTF8, MediaType);
            var removeRangeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.RemoveRange}", removeRangeHttpContent);
            Assert.That(removeRangeResponse.IsSuccessStatusCode, Is.True);

            await RemoveOrderAndAssert(order.Id);
        }

        [Test]
        public virtual async Task GIVEN_OrderStatusTracksController_WHEN_I_update_entity_THEN_it_is_being_updated_in_database()
        {
            var order = Fixture.Build<Order>()
                .With(o => o.Articles, Fixture.CreateMany<OrderedArticle>().ToList())
                .Without(o => o.OrderStatusTracks)
                .Create();

            await AddOrderAndAssert(order);

            var expected = Fixture.Build<OrderStatusTrack>()
                .With(ost => ost.OrderId, order.Id)
                .With(ost => ost.OrderStatusId, Fixture.Create<int>() % _maxOrderStatusId + 1)
                .Without(ost => ost.OrderStatus)
                .Create();

            var addJsonContent = JsonConvert.SerializeObject(expected);
            var addHttpContent = new StringContent(addJsonContent, Encoding.UTF8, MediaType);
            var addResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Add}", addHttpContent);
            Assert.That(addResponse.IsSuccessStatusCode, Is.True);

            expected.Assigned = Fixture.Create<DateTime>();

            var updateJsonContent = JsonConvert.SerializeObject(expected);
            var updateHttpContent = new StringContent(updateJsonContent, Encoding.UTF8, MediaType);
            var updateResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Update}", updateHttpContent);
            Assert.That(updateResponse.IsSuccessStatusCode, Is.True);

            var getOneResponse = await SystemUnderTests.GetAsync($"{ControllerName}?id={expected.Id}");
            Assert.That(getOneResponse.IsSuccessStatusCode, Is.True);
            var getOneContent = await getOneResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<OrderStatusTrack>(getOneContent);
            Assert.That(actual, Is.Not.Null);

            AssertObjectsAreEqual(expected, actual);

            var removeJsonContent = JsonConvert.SerializeObject(expected.Id);
            var removeHttpContent = new StringContent(removeJsonContent, Encoding.UTF8, MediaType);
            var removeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Remove}", removeHttpContent);
            Assert.That(removeResponse.IsSuccessStatusCode, Is.True);

            await RemoveOrderAndAssert(order.Id);
        }

        protected virtual async Task AddOrderAndAssert(Order order)
        {
            var addOrderJsonContent = JsonConvert.SerializeObject(order);
            var addOrderHttpContent = new StringContent(addOrderJsonContent, Encoding.UTF8, MediaType);
            var addOrderResponse = await SystemUnderTests.PostAsync($"orders/{RepoActions.Add}", addOrderHttpContent);
            Assert.That(addOrderResponse.IsSuccessStatusCode, Is.True);
        }
        protected virtual async Task RemoveOrderAndAssert(Guid id)
        {
            var removeOrderJsonContent = JsonConvert.SerializeObject(id);
            var removeOrderHttpContent = new StringContent(removeOrderJsonContent, Encoding.UTF8, MediaType);
            var removeOrderResponse = await SystemUnderTests.PostAsync($"orders/{RepoActions.Remove}", removeOrderHttpContent);
            Assert.That(removeOrderResponse.IsSuccessStatusCode, Is.True);
        }

        protected override void AssertObjectsAreEqual(OrderStatusTrack expected, OrderStatusTrack actual)
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
