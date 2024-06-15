using OnlineShop.Ui;
using OnlineShop.Ui.Abstractions;
using OnlineShop.Ui.Abstractions.Interfaces;
using OnlineShop.Ui.States;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new("https://localhost:5009") }); //all requests to backend will send to api service
builder.Services.AddSingleton<ILoginStatusManager, LoginStatusManager>();

builder.Services.AddTransient<IArticlesProvider, ArticlesProvider>();

builder.Services.AddScoped<CartState>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
