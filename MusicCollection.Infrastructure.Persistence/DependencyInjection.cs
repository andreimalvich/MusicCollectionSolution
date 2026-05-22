using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MusicCollection.Application.Common.Interfaces;
using MusicCollection.Infrastructure.Persistence.Contexts;

namespace MusicCollection.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, string connectionString)
    {
        // Регистрируем фабрику контекстов под MS SQL Server
        services.AddDbContextFactory<MusicDbContext>(options =>
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(MusicDbContext).Assembly.FullName)));

        // Связываем ИНТЕРФЕЙС с контекстом, создаваемым через фабрику
        services.AddTransient<IApplicationDbContext>(provider =>
            provider.GetRequiredService<IDbContextFactory<MusicDbContext>>().CreateDbContext());

        // Регистрируем сам MusicDbContext для системных нужд (например, миграций)
        services.AddTransient(provider =>
            provider.GetRequiredService<IDbContextFactory<MusicDbContext>>().CreateDbContext());

        return services;
    }
}
