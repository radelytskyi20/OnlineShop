using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OnlineShop.Library.ArticlesService.Models;
using OnlineShop.Library.ArticlesService.Repo;
using OnlineShop.Library.Common.Interfaces;
using OnlineShop.Library.Constants;
using OnlineShop.Library.Data;
using OnlineShop.Library.UserManagementService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ArticlesService",
        Version = "v1"
    });
});

builder.Services.AddTransient<IRepo<Article>, ArticlesRepo>();
builder.Services.AddTransient<IRepo<PriceList>, PriceListsRepo>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<UsersDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
    .AddJwtBearer(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Authority = "https://localhost:5001";
        options.RequireHttpsMetadata = false;
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
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ArticlesService v1");
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
