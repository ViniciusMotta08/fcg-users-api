using FCG.Users.Application.Abstractions;
using FCG.Users.Domain.SeedWork;
using FCG.Users.Domain.Usuarios.Eventos;

namespace FCG.Users.Application.IntegrationEvents;

/// <summary>Tradução dos eventos de domínio do contexto de Usuários para a linguagem publicada.</summary>
public sealed class MapeadorDeEventosDeIntegracao : IIntegrationEventMapper
{
    public IEnumerable<IntegrationEvent> Mapear(IDomainEvent eventoDeDominio)
    {
        if (eventoDeDominio is UsuarioCadastrado cadastrado)
        {
            yield return new UserCreatedEvent
            {
                OccurredAt = cadastrado.OcorreuEm,
                UserId = cadastrado.UsuarioId,
                Name = cadastrado.Nome,
                Email = cadastrado.Email,
                Role = cadastrado.Perfil.ToString()
            };
        }
    }
}
