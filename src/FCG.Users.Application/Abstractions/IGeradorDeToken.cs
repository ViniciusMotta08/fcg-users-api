using FCG.Users.Domain.Usuarios;

namespace FCG.Users.Application.Abstractions;

/// <summary>Porta de saída: emissão do token de acesso (JWT na Infraestrutura).</summary>
public interface IGeradorDeToken
{
    TokenDeAcesso Gerar(Usuario usuario);
}

public sealed record TokenDeAcesso(string Token, DateTime ExpiraEm);
