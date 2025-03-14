using System.Reflection;
using EShop.Catalog.Endpoints;
using EShop.Catalog.Infrastructure;
using EShop.Shared.Observability;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.AddInfrastructureServices();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseCorrelationId();
app.UseHttpsRedirection();
app.MapGet("/", (HttpContext ctx) => $"Hello From Catalog Service! /n CorrelationId: {ctx.GetCorrelationId()}");
app.MapCatalogBrandEndpoint();
app.MapProductEndpoints();
app.MapCatalogCategoryEndpoints();
app.Run();

