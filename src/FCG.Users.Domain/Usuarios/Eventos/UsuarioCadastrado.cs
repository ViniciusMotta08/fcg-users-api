using FCG.Users.Domain.SeedWork;

namespace FCG.Users.Domain.Usuarios.Eventos;

/// <summary>Evento de domínio: um novo usuário foi cadastrado na plataforma.</summary>
public sealed record UsuarioCadastrado(Guid UsuarioId, string Nome, string Email, PerfilUsuario Perfil) : DomainEvent;
