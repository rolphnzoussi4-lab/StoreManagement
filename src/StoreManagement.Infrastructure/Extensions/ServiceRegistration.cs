using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using StoreManagement.Application.Abstractions.Persistence;
using StoreManagement.Infrastructure.Persistence.Sqlite;
using StoreManagement.Infrastructure.Persistence.Sqlite.Repositories;

namespace StoreManagement.Infrastructure.Extensions;

public static class ServiceRegistration
{
    public static IServiceCollection AddStoreInfrastructure(this IServiceCollection services, string databasePath)
    {
        services.AddSingleton(new SQLiteConnectionFactory(databasePath));
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<DatabaseInitializer>();
        return services;
    }
}
