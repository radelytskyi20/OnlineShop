using AutoFixture;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using Moq;
using OnlineShop.Library.Clients.UserManagementService;
using OnlineShop.Library.Options;

namespace OnlineShop.ApiService.ApiTests
{
    public abstract class BaseRepoControllerTests<TEntity>
    {
        protected readonly Fixture Fixture = new Fixture();
        protected ILoginClient LoginClient;
        protected HttpClient SystemUnderTests;
        
        protected string ControllerName { get; set; } = string.Empty;
        protected string MediaType { get; set; } = "application/json";

        public BaseRepoControllerTests()
        {
            Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => Fixture.Behaviors.Remove(b));
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [SetUp]
        public async Task Setup()
        {
            var serviceAddressOptionsMock = new Mock<IOptions<ServiceAdressOptions>>();

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            switch (env)
            {
                case "Docker":
                    serviceAddressOptionsMock.Setup(x => x.Value)
                        .Returns(new ServiceAdressOptions()
                        {
                            OrdersService = "https://localhost:5005",
                            ArticlesService = "https://localhost:5006",
                            UserManagementService = "https://localhost:5003",
                            ApiService = "https://localhost:5009"
                        });
                    break;

                default:
                    serviceAddressOptionsMock.Setup(x => x.Value)
                        .Returns(new ServiceAdressOptions()
                        {
                            OrdersService = "https://localhost:5005",
                            ArticlesService = "https://localhost:5006",
                            UserManagementService = "https://localhost:5003",
                            ApiService = "https://localhost:5009"
                        });
                    break;
            }

            SystemUnderTests = new HttpClient() { BaseAddress = new Uri(serviceAddressOptionsMock.Object.Value.ApiService) };
            LoginClient = new LoginClient(new HttpClient(), serviceAddressOptionsMock.Object);

            var token = await LoginClient.GetApiTokenByUsernameAndPassword(new IdentityServerUserNamePassword()
            {
                UserName = "yaroslav",
                Password = "Pass_123"
            });

            SystemUnderTests.SetBearerToken(token.AccessToken); //authorize api service
        }

        protected abstract void AssertObjectsAreEqual(TEntity expected, TEntity actual);
    }
}
