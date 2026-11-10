using FCG.Users.Domain.Usuarios;

namespace FCG.Users.Application.DTOs;

/// <summary>Cadastro público de usuário.</summary>
public sealed record CadastrarUsuarioRequest(string Nome, string Email, string Senha);

public sealed record LoginRequest(string Email, string Senha);

public sealed record AtualizarUsuarioRequest(string Nome, string Email);

public sealed record AlterarPerfilRequest(PerfilUsuario Perfil);

public sealed record AlterarSenhaRequest(string SenhaAtual, string NovaSenha);

public sealed record UsuarioResponse(Guid Id, string Nome, string Email, string Perfil, bool Ativo, DateTime CadastradoEm);

public sealed record LoginResponse(string Token, DateTime ExpiraEm, UsuarioResponse Usuario);
