using EventHub.Web.Components;
using EventHub.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

//httpClinet gọi auth service
builder.Services.AddHttpClient("AuthService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5001");
}
);

// dang ky authApi
builder.Services.AddScoped<AuthApiService>();

// dang ky authService
builder.Services.AddScoped<AuthSessionService>();

builder.Services.AddScoped<RefreshTokenService>();

builder.Services.AddScoped<AuthTokenRefreshService>();

//dang ky authState
builder.Services.AddScoped<CustomAuthenticationStateProvider>();

builder.Services.AddScoped<
    Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider>(
    provider => provider.GetRequiredService<CustomAuthenticationStateProvider>());

var app = builder.Build();

// Configure the HTTP request pipeline. 
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
