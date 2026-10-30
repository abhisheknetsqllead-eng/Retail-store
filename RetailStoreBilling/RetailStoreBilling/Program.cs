using Microsoft.EntityFrameworkCore;
using RetailStoreBilling.Data;
using RetailStoreBilling.Services;
using RetailStoreBilling.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<RetailDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RetailDb")));

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<BillingService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
