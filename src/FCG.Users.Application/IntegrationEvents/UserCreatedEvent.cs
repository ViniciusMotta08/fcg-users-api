using FCG.Users.Application.Abstractions;

namespace FCG.Users.Application.IntegrationEvents;

/// <summary>
/// Evento publicado quando um usuário é cadastrado. Consumido pelo NotificationsAPI
/// para enviar o e-mail de boas-vindas.
/// </summary>
public sealed record UserCreatedEvent : IntegrationEvent
{
    public const string Rota = "user.created";

    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;

    public override string RoutingKey => Rota;
}
