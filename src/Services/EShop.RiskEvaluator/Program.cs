using EShop.RiskEvaluator.Extentions;
using EShop.RiskEvaluator.Services;
using EShop.Shared.Observability;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();
builder.Services.AddHealthChecks();
builder.AddServices();
builder.Services.AddGrpc();

var app = builder.Build();
app.UseCorrelationId();
app.MapGrpcService<EvaluatorService>();
app.Run();