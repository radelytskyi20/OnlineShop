using OnlineShop.ApiService.Authorization;
using OnlineShop.Library.Clients.IdentityServer;
using OnlineShop.Library.Clients.UserManagementService;
using OnlineShop.Library.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpClient<UsersClient>();
builder.Services.AddHttpClient<IdentityServerClient>();
builder.Services.AddHttpClient<RolesClient>();

builder.Services.AddTransient<IClientAuthorization, HttpClientAuthorization>();
builder.Services.AddTransient<IUsersClient, UsersClient>();
builder.Services.AddTransient<IRolesClient, RolesClient>();
builder.Services.AddTransient<IIdentityServerClient, IdentityServerClient>();

builder.Services.Configure<IdentityServerApiOptions>(builder.Configuration.GetSection(IdentityServerApiOptions.SectionName));
builder.Services.Configure<ServiceAdressOptions>(builder.Configuration.GetSection(ServiceAdressOptions.SectionName));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
