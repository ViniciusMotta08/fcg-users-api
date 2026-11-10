using FCG.Users.Application.Abstractions;
using FCG.Users.Application.DTOs;
using FCG.Users.Application.Exceptions;
using FCG.Users.Application.Mappers;
using FCG.Users.Domain.Usuarios;

namespace FCG.Users.Application.UseCases;

/// <summary>Caso de uso: autenticar por e-mail e senha e emitir o token de acesso.</summary>
public sealed class AutenticarUsuarioUseCase
{
    private const string CredenciaisInvalidas = "E-mail ou senha inválidos.";

    private readonly IUsuarioRepository _usuarios;
    private readonly ICriptografiaDeSenha _criptografia;
    private readonly IGeradorDeToken _geradorDeToken;

    public AutenticarUsuarioUseCase(IUsuarioRepository usuarios, ICriptografiaDeSenha criptografia, IGeradorDeToken geradorDeToken)
    {
        _usuarios = usuarios;
        _criptografia = criptografia;
        _geradorDeToken = geradorDeToken;
    }

    public async Task<LoginResponse> ExecutarAsync(LoginRequest request, CancellationToken ct = default)
    {
        if (!Email.EhValido(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
            throw new NaoAutorizadoException(CredenciaisInvalidas);

        var usuario = await _usuarios.ObterPorEmailAsync(Email.Criar(request.Email), ct);

        // Mesma mensagem para e-mail inexistente e senha errada: não revela quais e-mails existem.
        if (usuario is null || !_criptografia.Conferir(request.Senha, usuario.SenhaHash))
            throw new NaoAutorizadoException(CredenciaisInvalidas);

        if (!usuario.Ativo)
            throw new NaoAutorizadoException("Usuário inativo. Procure um administrador.");

        var token = _geradorDeToken.Gerar(usuario);
        return new LoginResponse(token.Token, token.ExpiraEm, UsuarioMapper.ParaResponse(usuario));
    }
}
