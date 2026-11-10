using FCG.Users.Application.DTOs;
using FCG.Users.Application.UseCases;
using FCG.Users.Domain.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Users.Api.Controllers;

/// <summary>Cadastro público e autenticação (emissão do JWT).</summary>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController : ControllerBase
{
    /// <summary>Cadastra um usuário com perfil "Usuario" e dispara o UserCreatedEvent.</summary>
    [HttpPost("registrar")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Registrar(
        [FromBody] CadastrarUsuarioRequest request,
        [FromServices] CadastrarUsuarioUseCase useCase,
        CancellationToken ct)
    {
        var usuario = await useCase.ExecutarAsync(request, PerfilUsuario.Usuario, ct);
        return Created($"/api/usuarios/{usuario.Id}", usuario);
    }

    /// <summary>Autentica e devolve o token JWT de acesso.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        [FromServices] AutenticarUsuarioUseCase useCase,
        CancellationToken ct) =>
        Ok(await useCase.ExecutarAsync(request, ct));
}
