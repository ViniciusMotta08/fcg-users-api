using FCG.Users.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace FCG.Users.Infrastructure.Persistence.Repositories;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly UsersDbContext _context;

    public UsuarioRepository(UsersDbContext context) => _context = context;

    public Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id, ct);

    // A comparação usa o próprio Value Object: o EF aplica o conversor e gera "WHERE Email = @p".
    public Task<Usuario?> ObterPorEmailAsync(Email email, CancellationToken ct = default) =>
        _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<bool> ExisteComEmailAsync(Email email, Guid? exceto = null, CancellationToken ct = default) =>
        _context.Usuarios.AnyAsync(u => u.Email == email && (exceto == null || u.Id != exceto), ct);

    public Task<bool> ExisteAdministradorAsync(CancellationToken ct = default) =>
        _context.Usuarios.AnyAsync(u => u.Perfil == PerfilUsuario.Administrador, ct);

    public async Task<IReadOnlyList<Usuario>> ListarAsync(int pagina, int tamanhoPagina, CancellationToken ct = default) =>
        await _context.Usuarios
            .AsNoTracking()
            .OrderBy(u => u.Nome)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(ct);

    public Task<int> ContarAsync(CancellationToken ct = default) => _context.Usuarios.CountAsync(ct);

    public async Task AdicionarAsync(Usuario usuario, CancellationToken ct = default) =>
        await _context.Usuarios.AddAsync(usuario, ct);
}
