using Blazored.LocalStorage;
using Front;
using Front.Auth;
using Front.Consumer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped<TokenStorage>();
builder.Services.AddTransient<AuthMessageHandler>();


builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7181/");
})
.AddHttpMessageHandler<AuthMessageHandler>();
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient"));

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthConsumer>();
builder.Services.AddScoped<ResponseConsumer>();
builder.Services.AddScoped<SurveyConsumer>();
builder.Services.AddScoped<UserConsumer>();


await builder.Build().RunAsync();
