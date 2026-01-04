using MudBlazor.Services;
using WebPortal.Application.Abstractions.Security;
using WebPortal.Application.Common;
using WebPortal.Infrastructure.Extensions;
using WebPortal.Infrastructure.Monitoring;
using WebPortal.Web.Security;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();


builder.Services.AddMudServices();


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, WebUserContext>();

builder.Services.AddWebPortalInfrastructure(builder.Configuration);
builder.Services.AddWebPortalApplication();


builder.Services.AddHealthChecks()
    .AddCheck<SqlConnectionHealthCheck>("sql")
    .AddCheck<RedisConnectionHealthCheck>("redis");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapHealthChecks("/healthz");

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
