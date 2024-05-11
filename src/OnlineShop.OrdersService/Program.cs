using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using OnlineShop.Library.ArticlesService.Models;
using OnlineShop.Library.Common.Interfaces;
using OnlineShop.Library.Constants;
using OnlineShop.Library.Data;
using OnlineShop.Library.Options;
using OnlineShop.Library.OrdersService.Models;
using OnlineShop.Library.OrdersService.Repo;
using OnlineShop.Library.UserManagementService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OrdersService",
        Version = "v1"
    });
});

builder.Services.AddTransient<IRepo<Order>, OrdersRepo>();
builder.Services.AddTransient<IRepo<OrderedArticle>, OrderedArticlesRepo>();
builder.Services.AddTransient<IRepo<OrderStatusTrack>, OrderStatusTracksRepo>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<UsersDbContext>()
    .AddDefaultTokenProviders();

var serviceAddressOptions = new ServiceAdressOptions();
builder.Configuration.GetSection(ServiceAdressOptions.SectionName).Bind(serviceAddressOptions);

builder.Services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
    .AddJwtBearer(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Authority = serviceAddressOptions.IdentityServer;
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters() { ValidateAudience = false };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", IdConstants.ApiScope);
    });
});

builder.Services.AddDbContext<OrdersDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString(ConnectionNames.OrdersConnection));
});

builder.Services.AddDbContext<UsersDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString(ConnectionNames.UserConnection));
});

// NLog: Setup NLog for Dependency injection
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var isDocker = environment == EnvironmentNames.Docker;
LogManager.Setup().LoadConfigurationFromFile(isDocker ? "nlog.Docker.config" : "nlog.config");

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Info($"Is docker environment: {isDocker}; Environment name: {environment}.");

builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "OrdersService v1");
});
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers().RequireAuthorization("ApiScope");
});

app.Run();
