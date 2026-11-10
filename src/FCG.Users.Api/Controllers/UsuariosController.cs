using FCG.Users.Api.Extensions;
using FCG.Users.Application.Abstractions;
using FCG.Users.Application.DTOs;
using FCG.Users.Application.UseCases;
using FCG.Users.Domain.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Users.Api.Controllers;

/// <summary>Perfil do usuário autenticado e administração de usuários.</summary>
[ApiController]
[Route("api/usuarios")]
[Authorize]
public sealed class UsuariosController : ControllerBase
{
    private readonly ConsultarUsuariosUseCase _consultas;
    private readonly GerenciarUsuarioUseCase _gerenciar;

    public UsuariosController(ConsultarUsuariosUseCase consultas, GerenciarUsuarioUseCase gerenciar)
    {
        _consultas = consultas;
        _gerenciar = gerenciar;
    }

    /// <summary>Dados do próprio usuário autenticado.</summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterMeusDados(CancellationToken ct) =>
        Ok(await _consultas.ObterPorIdAsync(User.ObterUsuarioId(), ct));

    /// <summary>Troca de senha do próprio usuário.</summary>
    [HttpPut("me/senha")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AlterarMinhaSenha([FromBody] AlterarSenhaRequest request, CancellationToken ct)
    {
        await _gerenciar.AlterarSenhaAsync(User.ObterUsuarioId(), request, ct);
        return NoContent();
    }

    [HttpGet]
    [Authorize(Roles = ClaimsPrincipalExtensions.PerfilAdministrador)]
    [ProducesResponseType(typeof(ResultadoPaginado<UsuarioResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 20, CancellationToken ct = default) =>
        Ok(await _consultas.ListarAsync(pagina, tamanhoPagina, ct));

    [HttpGet("{id:guid}")]
    [Authorize(Roles = ClaimsPrincipalExtensions.PerfilAdministrador)]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct) =>
        Ok(await _consultas.ObterPorIdAsync(id, ct));

    /// <summary>Cadastro feito por administrador (permite definir o perfil).</summary>
    [HttpPost]
    [Authorize(Roles = ClaimsPrincipalExtensions.PerfilAdministrador)]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cadastrar(
        [FromBody] CadastrarUsuarioRequest request,
        [FromServices] CadastrarUsuarioUseCase useCase,
        [FromQuery] PerfilUsuario perfil = PerfilUsuario.Usuario,
        CancellationToken ct = default)
    {
        var usuario = await useCase.ExecutarAsync(request, perfil, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = ClaimsPrincipalExtensions.PerfilAdministrador)]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarUsuarioRequest request, CancellationToken ct) =>
        Ok(await _gerenciar.AtualizarDadosAsync(id, request, ct));

    [HttpPatch("{id:guid}/perfil")]
    [Authorize(Roles = ClaimsPrincipalExtensions.PerfilAdministrador)]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AlterarPerfil(Guid id, [FromBody] AlterarPerfilRequest request, CancellationToken ct) =>
        Ok(await _gerenciar.AlterarPerfilAsync(id, request, ct));

    [HttpPatch("{id:guid}/desativar")]
    [Authorize(Roles = ClaimsPrincipalExtensions.PerfilAdministrador)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Desativar(Guid id, CancellationToken ct)
    {
        await _gerenciar.DesativarAsync(id, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/ativar")]
    [Authorize(Roles = ClaimsPrincipalExtensions.PerfilAdministrador)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Ativar(Guid id, CancellationToken ct)
    {
        await _gerenciar.AtivarAsync(id, ct);
        return NoContent();
    }
}
