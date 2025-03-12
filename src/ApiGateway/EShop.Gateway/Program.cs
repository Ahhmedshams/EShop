using EShop.Shared.Observability;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transforms =>
    {
        transforms.AddRequestTransform(async transforms =>
        {
            var correlationId = Guid.NewGuid().ToString("N");
            transforms.ProxyRequest.Headers.SetCorrelationId(correlationId);
        });
    });


var app = builder.Build();
app.MapGet(pattern: "/", () => "EShop Gateway");
app.MapReverseProxy();
app.Run();

