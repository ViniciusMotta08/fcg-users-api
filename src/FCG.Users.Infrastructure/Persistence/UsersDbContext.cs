using FCG.Users.Application.Abstractions;
using FCG.Users.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace FCG.Users.Infrastructure.Persistence;

/// <summary>Banco exclusivo do contexto de Usuários (database-per-service).</summary>
public sealed class UsersDbContext : ContextoComOutbox
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options, IIntegrationEventMapper mapeador)
        : base(options, mapeador) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
}
