namespace FCG.Users.Domain.Usuarios;

/// <summary>Repositório do agregado Usuario (contrato no Domínio, implementação na Infraestrutura).</summary>
public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<Usuario?> ObterPorEmailAsync(Email email, CancellationToken ct = default);
    Task<bool> ExisteComEmailAsync(Email email, Guid? exceto = null, CancellationToken ct = default);
    Task<bool> ExisteAdministradorAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Usuario>> ListarAsync(int pagina, int tamanhoPagina, CancellationToken ct = default);
    Task<int> ContarAsync(CancellationToken ct = default);
    Task AdicionarAsync(Usuario usuario, CancellationToken ct = default);
}
