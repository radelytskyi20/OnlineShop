﻿using AutoFixture;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using Moq;
using OnlineShop.Library.Clients;
using OnlineShop.Library.Clients.IdentityServer;
using OnlineShop.Library.Common.Interfaces;
using OnlineShop.Library.Options;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;

namespace OnlineShop.ArticlesService.ApiTests
{
    public abstract class ArticleServiceRepoBaseApiTest<TClient, TEntity>
        where TClient : IRepoClient<TEntity>, IHttpClientContainer
        where TEntity : IIdentifiable
    {
        protected readonly Fixture Fixture = new();
        protected IOptions<ServiceAdressOptions> ServiceAddressOptions;
        protected IdentityServerApiOptions IdentityServerApiOptions;
        protected IdentityServerClient IdentityServerClient;
        protected TClient SystemUnderTest = default!;

        public ArticleServiceRepoBaseApiTest()
        {
            ConfigureFixture();
        }

        protected virtual void ConfigureFixture()
        {
            Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => Fixture.Behaviors.Remove(b));

            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [SetUp]
        public async Task Setup()
        {
            SetServiceAddressOptions();
            SetIdentityServerApiOptions();

            IdentityServerClient = new IdentityServerClient(new HttpClient(), ServiceAddressOptions);

            CreateSystemUnderTest();
            await AuthorizeSystemUnderTests();
        }

        protected virtual void SetServiceAddressOptions()
        {
            var serviceAddressOptionsMock = new Mock<IOptions<ServiceAdressOptions>>();
            serviceAddressOptionsMock.Setup(x => x.Value)
                .Returns(new ServiceAdressOptions()
                {
                    IdentityServer = "https://localhost:5001",
                    ArticlesService = "https://localhost:5006"
                });
            ServiceAddressOptions = serviceAddressOptionsMock.Object;
        }

        protected virtual void SetIdentityServerApiOptions()
        {
            IdentityServerApiOptions = new IdentityServerApiOptions()
            {
                ClientId = "test.client",
                ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",
                Scope = "OnlineShop.Api",
                GrantType = "client_credentials"
            };
        }

        protected virtual async Task AuthorizeSystemUnderTests()
        {
            var token = await IdentityServerClient.GetApiToken(IdentityServerApiOptions);
            SystemUnderTest.HttpClient.SetBearerToken(token.AccessToken);
        }

        /// <summary>
        /// Creates an instance of type TClient
        /// </summary>
        protected abstract void CreateSystemUnderTest();

        [Test]
        public virtual async Task GIVEN_Repo_Client_WHEN_I_add_entity_THEN_it_is_being_added_to_database()
        {
            var expected = CreateExpectedEntity();

            var addResponse = await SystemUnderTest.Add(expected);
            Assert.That(addResponse.IsSuccessfull, Is.True);

            var getOneResponse = await SystemUnderTest.GetOne(addResponse.Payload);
            Assert.That(getOneResponse.IsSuccessfull, Is.True);
            var actual = getOneResponse.Payload;

            AssertObjectsAreEqual(expected, actual);

            var removeResponse = await SystemUnderTest.Remove(addResponse.Payload);
            Assert.That(removeResponse.IsSuccessfull, Is.True);
        }

        [Test]
        public virtual async Task GIVEN_Repo_Client_WHEN_I_add_several_entities_THEN_they_are_being_added_to_database()
        {
            var expected1 = CreateExpectedEntity();
            var expected2 = CreateExpectedEntity();

            var entitiesToAdd = new[] { expected1, expected2 };

            var addRangeResponse = await SystemUnderTest.AddRange(entitiesToAdd);
            Assert.That(addRangeResponse.IsSuccessfull, Is.True);

            var getAllResponse = await SystemUnderTest.GetAll();
            Assert.That(getAllResponse.IsSuccessfull, Is.True);
            var addedEntities = getAllResponse.Payload;

            foreach (var entityId in addRangeResponse.Payload)
            {
                var expectedEntity = entitiesToAdd.Single(e => e.Id == entityId);
                var actualEntity = addedEntities.Single(e => e.Id == entityId);
                AssertObjectsAreEqual(expectedEntity, actualEntity);
            }

            var removeRangeResponse = await SystemUnderTest.RemoveRange(addRangeResponse.Payload);
            Assert.That(removeRangeResponse.IsSuccessfull, Is.True);
        }

        [Test]
        public virtual async Task GIVEN_Repo_Client_WHEN_I_update_entity_THEN_it_is_being_updated_in_database()
        {
            var expected = CreateExpectedEntity();

            var addResponse = await SystemUnderTest.Add(expected);
            Assert.That(addResponse.IsSuccessfull, Is.True);

            AmendExpectedEntityForUpdating(expected);

            var updateResponse = await SystemUnderTest.Update(expected);
            Assert.That(updateResponse.IsSuccessfull, Is.True);
            var actual = updateResponse.Payload;

            AssertObjectsAreEqual(expected, actual);

            var removeResponse = await SystemUnderTest.Remove(addResponse.Payload);
            Assert.That(removeResponse.IsSuccessfull, Is.True);
        }

        protected abstract void AssertObjectsAreEqual(TEntity expected, TEntity actual);
        protected virtual TEntity CreateExpectedEntity() => Fixture.Build<TEntity>().Create();
        protected abstract void AmendExpectedEntityForUpdating(TEntity expected);
    }
}
