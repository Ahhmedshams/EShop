var builder = WebApplication.CreateBuilder(args);


builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));


var app = builder.Build();

app.MapGet(pattern: "/", () => "EShop Gateway");
app.MapReverseProxy();
app.Run();

