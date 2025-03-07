using System.Text.Json.Serialization;
using EShop.Shared.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCorrelationId();
app.UseHttpsRedirection();
app.MapGet("/", (HttpContext ctx) => $"Hello From User Service! /n CorrelationId: {ctx.GetCorrelationId()}");
app.Run();

