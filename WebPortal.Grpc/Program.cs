using WebPortal.Application.Common;
using WebPortal.Application.Abstractions.Security;
using WebPortal.Infrastructure.Extensions;
using WebPortal.Infrastructure.Monitoring;
using WebPortal.Grpc.Security;
using WebPortal.Grpc.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddGrpc();


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, GrpcUserContext>();
builder.Services.AddWebPortalInfrastructure(builder.Configuration);
builder.Services.AddWebPortalApplication();

builder.Services.AddHealthChecks()
    .AddCheck<SqlConnectionHealthCheck>("sql")
    .AddCheck<RedisConnectionHealthCheck>("redis");

var app = builder.Build();

app.MapGrpcService<PortalGrpcService>();
app.MapGrpcService<AdminGrpcService>();

app.MapHealthChecks("/healthz");

app.MapGet("/", () => "WebPortal gRPC Service đang chạy.");

app.Run();
