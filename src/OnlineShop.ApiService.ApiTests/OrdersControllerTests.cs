using AutoFixture;
using Newtonsoft.Json;
using OnlineShop.Library.ArticlesService.Models;
using OnlineShop.Library.Constants;
using OnlineShop.Library.OrdersService.Models;
using System.Text;

namespace OnlineShop.ApiService.ApiTests
{
    public class OrdersControllerTests : BaseRepoControllerTests<Order>
    {
        public OrdersControllerTests() : base() { ControllerName = "orders"; }

        private const int _maxOrderStatusId = 4;

        [Test]
        public virtual async Task GIVEN_OrdersController_WHEN_I_add_entity_THEN_it_is_being_added_to_database()
        {
            var expected = Fixture.Build<Order>()
                .With(o => o.Articles, Fixture.CreateMany<OrderedArticle>().ToList())
                .Without(o => o.OrderStatusTracks)
                .Create();

            var orderStatusTracks = Fixture.Build<OrderStatusTrack>()
                .With(ost => ost.OrderId, expected.Id)
                .With(ost => ost.OrderStatusId, Fixture.Create<int>() % _maxOrderStatusId + 1)
                .Without(ost => ost.OrderStatus)
                .CreateMany();

            expected.OrderStatusTracks = orderStatusTracks.ToList();

            var addJsonContent = JsonConvert.SerializeObject(expected);
            var addHttpContent = new StringContent(addJsonContent, Encoding.UTF8, MediaType);
            var addResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Add}", addHttpContent);
            Assert.That(addResponse.IsSuccessStatusCode, Is.True);

            var getOneResponse = await SystemUnderTests.GetAsync($"{ControllerName}?id={expected.Id}");
            Assert.That(getOneResponse.IsSuccessStatusCode, Is.True);
            var getOneContent = await getOneResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<Order>(getOneContent);
            Assert.That(actual, Is.Not.Null);

            AssertObjectsAreEqual(expected, actual);

            var removeJsonContent = JsonConvert.SerializeObject(actual.Id);
            var removeHttpContent = new StringContent(removeJsonContent, Encoding.UTF8, MediaType);
            var removeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Remove}", removeHttpContent);
            Assert.That(removeResponse.IsSuccessStatusCode, Is.True);
        }

        [Test]
        public virtual async Task GIVEN_OrdersController_WHEN_I_add_several_entities_THEN_they_are_being_added_to_database()
        {
            var expected1 = Fixture.Build<Order>()
                .With(o => o.Articles, Fixture.CreateMany<OrderedArticle>().ToList())
                .Without(o => o.OrderStatusTracks)
                .Create();

            var orderStatusTracks1 = Fixture.Build<OrderStatusTrack>()
                .With(ost => ost.OrderId, expected1.Id)
                .With(ost => ost.OrderStatusId, Fixture.Create<int>() % _maxOrderStatusId + 1)
                .Without(ost => ost.OrderStatus)
                .CreateMany();

            expected1.OrderStatusTracks = orderStatusTracks1.ToList();

            var expected2 = Fixture.Build<Order>()
                .With(o => o.Articles, Fixture.CreateMany<OrderedArticle>().ToList())
                .Without(o => o.OrderStatusTracks)
                .Create();

            var ordersStatusTracks2 = Fixture.Build<OrderStatusTrack>()
                .With(ost => ost.OrderId, expected1.Id)
                .With(ost => ost.OrderStatusId, Fixture.Create<int>() % _maxOrderStatusId + 1)
                .Without(ost => ost.OrderStatus)
                .CreateMany();

            expected2.OrderStatusTracks = ordersStatusTracks2.ToList();

            var ordersToAdd = new[] { expected1, expected2 };

            var addRangeJsonContent = JsonConvert.SerializeObject(ordersToAdd);
            var addRangeHttpContent = new StringContent(addRangeJsonContent, Encoding.UTF8, MediaType);
            var addRangeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.AddRange}", addRangeHttpContent);
            Assert.That(addRangeResponse.IsSuccessStatusCode, Is.True);
            
            var addRangeResponseContent = await addRangeResponse.Content.ReadAsStringAsync();
            var addedOrdersIds = JsonConvert.DeserializeObject<IEnumerable<Guid>>(addRangeResponseContent);
            Assert.That(addedOrdersIds, Is.Not.Null);

            var getAllResponse = await SystemUnderTests.GetAsync($"{ControllerName}/{RepoActions.GetAll}");
            Assert.That(getAllResponse.IsSuccessStatusCode, Is.True);
            var getAllContent = await getAllResponse.Content.ReadAsStringAsync();
            var addedOrders = JsonConvert.DeserializeObject<IEnumerable<Order>>(getAllContent);
            Assert.That(addedOrders, Is.Not.Null);

            foreach (var orderId in addedOrdersIds)
            {
                var expected = ordersToAdd.Single(o => o.Id == orderId);
                var actual = addedOrders.Single(o => o.Id == orderId);
                AssertObjectsAreEqual(expected, actual);
            }

            var removeRangeJsonContent = JsonConvert.SerializeObject(addedOrdersIds);
            var removeRangeHttpContent = new StringContent(removeRangeJsonContent, Encoding.UTF8, MediaType);
            var removeRangeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.RemoveRange}", removeRangeHttpContent);
            Assert.That(removeRangeResponse.IsSuccessStatusCode, Is.True);
        }

        [Test]
        public virtual async Task GIVEN_OrdersController_WHEN_I_update_entity_THEN_it_is_being_updated_in_database()
        {
            var orderedArticles = Fixture.CreateMany<OrderedArticle>().ToList();
            var expected = Fixture.Build<Order>()
                .With(o => o.Articles, orderedArticles)
                .Without(o => o.OrderStatusTracks)
                .Create();

            var orderStatusTracks = Fixture.Build<OrderStatusTrack>()
                .With(ost => ost.OrderId, expected.Id)
                .With(ost => ost.OrderStatusId, Fixture.Create<int>() % _maxOrderStatusId + 1)
                .Without(ost => ost.OrderStatus)
                .CreateMany()
                .ToList();

            expected.OrderStatusTracks = orderStatusTracks;

            var addJsonContent = JsonConvert.SerializeObject(expected);
            var addHttpContent = new StringContent(addJsonContent, Encoding.UTF8, MediaType);
            var addResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Add}", addHttpContent);
            Assert.That(addResponse.IsSuccessStatusCode, Is.True);

            orderedArticles.ForEach(oa => oa.Name = Fixture.Create<string>());
            orderStatusTracks.ForEach(ost => ost.Assigned = Fixture.Create<DateTime>());
            expected.AddressId = Fixture.Create<Guid>();
            expected.UserId = Fixture.Create<Guid>();
            expected.Articles = orderedArticles;
            expected.OrderStatusTracks = orderStatusTracks;

            var updateJsonContent = JsonConvert.SerializeObject(expected);
            var updateHttpContent = new StringContent(updateJsonContent, Encoding.UTF8, MediaType);
            var updateResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Update}", updateHttpContent);
            Assert.That(updateResponse.IsSuccessStatusCode, Is.True);

            var getOneResponse = await SystemUnderTests.GetAsync($"{ControllerName}?id={expected.Id}");
            Assert.That(getOneResponse.IsSuccessStatusCode, Is.True);
            var getOneContent = await getOneResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<Order>(getOneContent);
            Assert.That(actual, Is.Not.Null);

            AssertObjectsAreEqual(expected, actual);

            var removeJsonContent = JsonConvert.SerializeObject(actual.Id);
            var removeHttpContent = new StringContent(removeJsonContent, Encoding.UTF8, MediaType);
            var removeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Remove}", removeHttpContent);
            Assert.That(removeResponse.IsSuccessStatusCode, Is.True);
        }

        protected override void AssertObjectsAreEqual(Order expected, Order actual)
        {
            Assert.Multiple(() =>
            {
                Assert.That(expected.Id, Is.EqualTo(actual.Id));
                Assert.That(expected.AddressId, Is.EqualTo(actual.AddressId));
                Assert.That(expected.UserId, Is.EqualTo(actual.UserId));
                Assert.That(expected.Created, Is.EqualTo(actual.Created));
                Assert.That(expected.Articles, Has.Count.EqualTo(actual.Articles.Count));
                Assert.That(expected.OrderStatusTracks, Has.Count.EqualTo(actual.OrderStatusTracks.Count));
            });
        }
    }
}
