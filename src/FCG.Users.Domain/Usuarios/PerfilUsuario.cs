namespace FCG.Users.Domain.Usuarios;

/// <summary>Nível de acesso do usuário na plataforma.</summary>
public enum PerfilUsuario
{
    /// <summary>Acessa a plataforma e compra jogos.</summary>
    Usuario = 1,

    /// <summary>Administra usuários e o catálogo de jogos.</summary>
    Administrador = 2
}
