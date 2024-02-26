using OnlineShop.ApiService.Authorization;
using OnlineShop.Library.ArticlesService.Models;
using OnlineShop.Library.Clients;
using OnlineShop.Library.Clients.ArticlesService;
using OnlineShop.Library.Clients.IdentityServer;
using OnlineShop.Library.Clients.OrdersService;
using OnlineShop.Library.Clients.UserManagementService;
using OnlineShop.Library.Options;
using OnlineShop.Library.OrdersService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpClient<UsersClient>();
builder.Services.AddHttpClient<IdentityServerClient>();
builder.Services.AddHttpClient<RolesClient>();
builder.Services.AddHttpClient<ArticlesClient>();
builder.Services.AddHttpClient<PriceListsClient>();
builder.Services.AddHttpClient<OrderedArticlesClient>();
builder.Services.AddHttpClient<OrdersClient>();
builder.Services.AddHttpClient<OrderStatusTracksClient>();

builder.Services.AddTransient<IClientAuthorization, HttpClientAuthorization>();
builder.Services.AddTransient<IUsersClient, UsersClient>();
builder.Services.AddTransient<IRolesClient, RolesClient>();
builder.Services.AddTransient<IRepoClient<OrderedArticle>, OrderedArticlesClient>();
builder.Services.AddTransient<IRepoClient<Article>, ArticlesClient>();
builder.Services.AddTransient<IRepoClient<PriceList>, PriceListsClient>();
builder.Services.AddTransient<IRepoClient<Order>, OrdersClient>();
builder.Services.AddTransient<IRepoClient<OrderStatusTrack>, OrderStatusTracksClient>();
builder.Services.AddTransient<IIdentityServerClient, IdentityServerClient>();

builder.Services.Configure<IdentityServerApiOptions>(builder.Configuration.GetSection(IdentityServerApiOptions.SectionName));
builder.Services.Configure<ServiceAdressOptions>(builder.Configuration.GetSection(ServiceAdressOptions.SectionName));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
