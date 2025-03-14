using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace EShop.Catalog.Infrastructure;

public static class InfrastructureExtensions
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<CatalogDbContext>(configure =>
        {
            configure.UseNpgsql(builder.Configuration.GetConnectionString("CatalogDb"));
        });

       
     
    }

}