using FCG.Users.Application.Abstractions;
using FCG.Users.Application.IntegrationEvents;
using FCG.Users.Domain.SeedWork;
using FCG.Users.Domain.Usuarios;
using FCG.Users.Infrastructure.Messaging;
using FCG.Users.Infrastructure.Messaging.Outbox;
using FCG.Users.Infrastructure.Persistence;
using FCG.Users.Infrastructure.Persistence.Repositories;
using FCG.Users.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Users.Infrastructure;

/// <summary>Registro dos adaptadores de infraestrutura (banco, mensageria e segurança).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AdicionarInfraestrutura(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("UsersDb");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("ConnectionStrings:UsersDb não configurada.");

        services.AddDbContext<UsersDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure(5)));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UsersDbContext>());
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddSingleton<IIntegrationEventMapper, MapeadorDeEventosDeIntegracao>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.Secao));
        services.AddSingleton<IGeradorDeToken, GeradorDeTokenJwt>();
        services.AddSingleton<ICriptografiaDeSenha, CriptografiaDeSenhaBCrypt>();

        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.Secao));
        services.AddSingleton<RabbitMqConexao>();
        services.AddSingleton<PublicadorRabbitMq>();
        services.AddHostedService<ProcessadorDeOutbox<UsersDbContext>>();

        return services;
    }
}
