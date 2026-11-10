using FCG.Users.Application.DTOs;
using FCG.Users.Application.Exceptions;
using FCG.Users.Application.Mappers;
using FCG.Users.Domain.SeedWork;
using FCG.Users.Domain.Usuarios;

namespace FCG.Users.Application.UseCases;

/// <summary>
/// Caso de uso: cadastrar usuário. A confirmação grava o usuário e o UserCreatedEvent
/// (via Outbox) na mesma transação.
/// </summary>
public sealed class CadastrarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarios;
    private readonly ICriptografiaDeSenha _criptografia;
    private readonly IUnitOfWork _unitOfWork;

    public CadastrarUsuarioUseCase(IUsuarioRepository usuarios, ICriptografiaDeSenha criptografia, IUnitOfWork unitOfWork)
    {
        _usuarios = usuarios;
        _criptografia = criptografia;
        _unitOfWork = unitOfWork;
    }

    public async Task<UsuarioResponse> ExecutarAsync(
        CadastrarUsuarioRequest request, PerfilUsuario perfil, CancellationToken ct = default)
    {
        var email = Email.Criar(request.Email);
        var senha = Senha.Criar(request.Senha);

        if (await _usuarios.ExisteComEmailAsync(email, null, ct))
            throw new ConflitoException("Já existe um usuário cadastrado com este e-mail.");

        var usuario = Usuario.Cadastrar(request.Nome, email, _criptografia.GerarHash(senha), perfil);

        await _usuarios.AdicionarAsync(usuario, ct);
        await _unitOfWork.ConfirmarAsync(ct);

        return UsuarioMapper.ParaResponse(usuario);
    }
}
