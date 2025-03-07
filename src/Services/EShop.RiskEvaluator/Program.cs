using EShop.RiskEvaluator.Extentions;
using EShop.RiskEvaluator.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();
builder.Services.AddHealthChecks();
builder.AddServices();
builder.Services.AddGrpc();

var app = builder.Build();
app.MapGrpcService<EvaluatorService>();
app.Run();