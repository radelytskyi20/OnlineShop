using AutoFixture;
using Newtonsoft.Json;
using OnlineShop.Library.ArticlesService.Models;
using OnlineShop.Library.Constants;
using OnlineShop.Library.OrdersService.Models;
using System.Text;

namespace OnlineShop.ApiService.ApiTests
{
    public class OrderedArticlesControllerTests : BaseRepoControllerTests<OrderedArticle>
    {
        public OrderedArticlesControllerTests() : base() { ControllerName = "orderedarticles"; }

        [Test]
        public virtual async Task GIVEN_OrderedArticlesController_WHEN_I_add_entity_THEN_it_is_being_added_to_database()
        {
            var order = Fixture.Build<Order>()
                .With(o => o.Articles, Enumerable.Empty<OrderedArticle>().ToList())
                .Without(o => o.OrderStatusTracks)
                .Create();

            await AddOrderAndAssert(order);

            var expected = Fixture.Build<OrderedArticle>()
                .With(oa => oa.Order, order)
                .With(oa => oa.OrderId, order.Id)
                .Create();

            var addJsonContent = JsonConvert.SerializeObject(expected);
            var addHttpContent = new StringContent(addJsonContent, Encoding.UTF8, MediaType);
            var addResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Add}", addHttpContent);
            Assert.That(addResponse.IsSuccessStatusCode, Is.True);

            var getOneResponse = await SystemUnderTests.GetAsync($"{ControllerName}?id={expected.Id}");
            Assert.That(getOneResponse.IsSuccessStatusCode, Is.True);
            var getOneContent = await getOneResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<OrderedArticle>(getOneContent);
            Assert.That(actual, Is.Not.Null);

            AssertObjectsAreEqual(expected, actual);

            var removeJsonContent = JsonConvert.SerializeObject(actual.Id);
            var removeHttpContent = new StringContent(removeJsonContent, Encoding.UTF8, MediaType);
            var removeResposne = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Remove}", removeHttpContent);
            Assert.That(removeResposne.IsSuccessStatusCode, Is.True);

            await RemoveOrderAndAssert(order.Id);
        }

        [Test]
        public virtual async Task GIVEN_OrderedArticlesController_WHEN_I_add_several_entities_THEN_they_are_being_added_to_database()
        {
            var order = Fixture.Build<Order>()
                .With(o => o.Articles, Enumerable.Empty<OrderedArticle>().ToList())
                .Without(o => o.OrderStatusTracks)
                .Create();

            await AddOrderAndAssert(order);

            var expected1 = Fixture.Build<OrderedArticle>()
                .With(oa => oa.Order, order)
                .With(oa => oa.OrderId, order.Id)
                .Create();

            var expected2 = Fixture.Build<OrderedArticle>()
                .With(oa => oa.Order, order)
                .With(oa => oa.OrderId, order.Id)
                .Create();

            var orderedArticlesToAdd = new[] { expected1, expected2 };

            var addRangeJsonContent = JsonConvert.SerializeObject(orderedArticlesToAdd);
            var addRangeHttpContent = new StringContent(addRangeJsonContent, Encoding.UTF8, MediaType);
            var addRangeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.AddRange}", addRangeHttpContent);
            Assert.That(addRangeResponse.IsSuccessStatusCode, Is.True);
            
            var addRangeContent = await addRangeResponse.Content.ReadAsStringAsync();
            var addedOrderedArticlesIds = JsonConvert.DeserializeObject<IEnumerable<Guid>>(addRangeContent);
            Assert.That(addedOrderedArticlesIds, Is.Not.Null);

            var getAllResponse = await SystemUnderTests.GetAsync($"{ControllerName}/{RepoActions.GetAll}");
            Assert.That(getAllResponse.IsSuccessStatusCode, Is.True);
            var getAllResponseContent = await getAllResponse.Content.ReadAsStringAsync();
            var addedOrderedArticles = JsonConvert.DeserializeObject<IEnumerable<OrderedArticle>>(getAllResponseContent);
            Assert.That(addedOrderedArticles, Is.Not.Null);

            foreach (var orderedArticleId in addedOrderedArticlesIds)
            {
                var expected = orderedArticlesToAdd.Single(oa => oa.Id == orderedArticleId);
                var actual = addedOrderedArticles.Single(oa => oa.Id == orderedArticleId);
                AssertObjectsAreEqual(expected, actual);
            }

            var removeRangeJsonContent = JsonConvert.SerializeObject(addedOrderedArticlesIds);
            var removeRangeHttpContent = new StringContent(removeRangeJsonContent, Encoding.UTF8, MediaType);
            var removeRangeResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.RemoveRange}", removeRangeHttpContent);
            Assert.That(removeRangeResponse.IsSuccessStatusCode, Is.True);

            await RemoveOrderAndAssert(order.Id);
        }

        [Test]
        public virtual async Task GIVEN_OrderedArticlesController_WHEN_I_update_entity_THEN_it_is_being_updated_in_database()
        {
            var order = Fixture.Build<Order>()
                .With(o => o.Articles, Enumerable.Empty<OrderedArticle>().ToList())
                .Without(o => o.OrderStatusTracks)
                .Create();

            await AddOrderAndAssert(order);

            var expected = Fixture.Build<OrderedArticle>()
                .With(oa => oa.Order, order)
                .With(oa => oa.OrderId, order.Id)
                .Create();

            var addJsonContent = JsonConvert.SerializeObject(expected);
            var addHttpContent = new StringContent(addJsonContent, Encoding.UTF8, MediaType);
            var addResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Add}", addHttpContent);
            Assert.That(addResponse.IsSuccessStatusCode, Is.True);

            expected.Name = Fixture.Create<string>();
            expected.Description = Fixture.Create<string>();
            expected.Price = Fixture.Create<decimal>();
            expected.Quantity = Fixture.Create<int>();

            var updateJsonContent = JsonConvert.SerializeObject(expected);
            var updateHttpContent = new StringContent(updateJsonContent, Encoding.UTF8, MediaType);
            var updateResponse = await SystemUnderTests.PostAsync($"{ControllerName}/{RepoActions.Update}", updateHttpContent);
            Assert.That(updateResponse.IsSuccessStatusCode, Is.True);

            var getOneResponse = await SystemUnderTests.GetAsync($"{ControllerName}?id={expected.Id}");
            Assert.That(getOneResponse.IsSuccessStatusCode, Is.True);
            var getOneHttpContent = await getOneResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<OrderedArticle>(getOneHttpContent);
            Assert.That(actual, Is.Not.Null);

            AssertObjectsAreEqual(expected, actual);

            var removeJsonContent = JsonConvert.SerializeObject(actual.Id);
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

        protected override void AssertObjectsAreEqual(OrderedArticle expected, OrderedArticle actual)
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
