using FCG.Users.Application.DTOs;
using FCG.Users.Application.Exceptions;
using FCG.Users.Application.Mappers;
using FCG.Users.Domain.SeedWork;
using FCG.Users.Domain.Usuarios;

namespace FCG.Users.Application.UseCases;

/// <summary>Casos de uso de manutenção do usuário: dados, senha, perfil e situação.</summary>
public sealed class GerenciarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarios;
    private readonly ICriptografiaDeSenha _criptografia;
    private readonly IUnitOfWork _unitOfWork;

    public GerenciarUsuarioUseCase(IUsuarioRepository usuarios, ICriptografiaDeSenha criptografia, IUnitOfWork unitOfWork)
    {
        _usuarios = usuarios;
        _criptografia = criptografia;
        _unitOfWork = unitOfWork;
    }

    public async Task<UsuarioResponse> AtualizarDadosAsync(Guid id, AtualizarUsuarioRequest request, CancellationToken ct = default)
    {
        var usuario = await ObterAsync(id, ct);
        var email = Email.Criar(request.Email);

        if (await _usuarios.ExisteComEmailAsync(email, id, ct))
            throw new ConflitoException("Já existe outro usuário cadastrado com este e-mail.");

        usuario.AtualizarDados(request.Nome, email);
        await _unitOfWork.ConfirmarAsync(ct);

        return UsuarioMapper.ParaResponse(usuario);
    }

    public async Task AlterarSenhaAsync(Guid id, AlterarSenhaRequest request, CancellationToken ct = default)
    {
        var usuario = await ObterAsync(id, ct);

        if (!_criptografia.Conferir(request.SenhaAtual, usuario.SenhaHash))
            throw new DomainException("Senha atual incorreta.");

        usuario.AlterarSenha(_criptografia.GerarHash(Senha.Criar(request.NovaSenha)));
        await _unitOfWork.ConfirmarAsync(ct);
    }

    public async Task<UsuarioResponse> AlterarPerfilAsync(Guid id, AlterarPerfilRequest request, CancellationToken ct = default)
    {
        var usuario = await ObterAsync(id, ct);

        usuario.AlterarPerfil(request.Perfil);
        await _unitOfWork.ConfirmarAsync(ct);

        return UsuarioMapper.ParaResponse(usuario);
    }

    public async Task DesativarAsync(Guid id, CancellationToken ct = default)
    {
        var usuario = await ObterAsync(id, ct);
        usuario.Desativar();
        await _unitOfWork.ConfirmarAsync(ct);
    }

    public async Task AtivarAsync(Guid id, CancellationToken ct = default)
    {
        var usuario = await ObterAsync(id, ct);
        usuario.Ativar();
        await _unitOfWork.ConfirmarAsync(ct);
    }

    private async Task<Usuario> ObterAsync(Guid id, CancellationToken ct) =>
        await _usuarios.ObterPorIdAsync(id, ct)
        ?? throw new RecursoNaoEncontradoException($"Usuário {id} não encontrado.");
}
