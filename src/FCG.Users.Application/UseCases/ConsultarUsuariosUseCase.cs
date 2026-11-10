using FCG.Users.Application.Abstractions;
using FCG.Users.Application.DTOs;
using FCG.Users.Application.Exceptions;
using FCG.Users.Application.Mappers;
using FCG.Users.Domain.Usuarios;

namespace FCG.Users.Application.UseCases;

/// <summary>Consultas de usuários (somente leitura).</summary>
public sealed class ConsultarUsuariosUseCase
{
    private readonly IUsuarioRepository _usuarios;

    public ConsultarUsuariosUseCase(IUsuarioRepository usuarios) => _usuarios = usuarios;

    public async Task<UsuarioResponse> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var usuario = await _usuarios.ObterPorIdAsync(id, ct)
                      ?? throw new RecursoNaoEncontradoException($"Usuário {id} não encontrado.");

        return UsuarioMapper.ParaResponse(usuario);
    }

    public async Task<ResultadoPaginado<UsuarioResponse>> ListarAsync(int pagina, int tamanhoPagina, CancellationToken ct = default)
    {
        (pagina, tamanhoPagina) = Paginacao.Normalizar(pagina, tamanhoPagina);

        var usuarios = await _usuarios.ListarAsync(pagina, tamanhoPagina, ct);
        var total = await _usuarios.ContarAsync(ct);

        return new ResultadoPaginado<UsuarioResponse>(
            usuarios.Select(UsuarioMapper.ParaResponse).ToList(), pagina, tamanhoPagina, total);
    }
}
