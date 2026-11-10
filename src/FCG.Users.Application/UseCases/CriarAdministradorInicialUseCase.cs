using FCG.Users.Application.DTOs;
using FCG.Users.Domain.Usuarios;

namespace FCG.Users.Application.UseCases;

/// <summary>
/// Cria o primeiro administrador, caso ainda não exista nenhum.
/// Só é executado quando habilitado explicitamente na configuração (AdministradorInicial:Habilitado).
/// </summary>
public sealed class CriarAdministradorInicialUseCase
{
    private readonly IUsuarioRepository _usuarios;
    private readonly CadastrarUsuarioUseCase _cadastrarUsuario;

    public CriarAdministradorInicialUseCase(IUsuarioRepository usuarios, CadastrarUsuarioUseCase cadastrarUsuario)
    {
        _usuarios = usuarios;
        _cadastrarUsuario = cadastrarUsuario;
    }

    /// <returns>true se o administrador foi criado; false se já existia um.</returns>
    public async Task<bool> ExecutarAsync(string nome, string email, string senha, CancellationToken ct = default)
    {
        if (await _usuarios.ExisteAdministradorAsync(ct))
            return false;

        await _cadastrarUsuario.ExecutarAsync(new CadastrarUsuarioRequest(nome, email, senha), PerfilUsuario.Administrador, ct);
        return true;
    }
}
