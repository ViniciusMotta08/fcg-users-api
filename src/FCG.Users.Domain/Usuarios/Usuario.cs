using FCG.Users.Domain.SeedWork;
using FCG.Users.Domain.Usuarios.Eventos;

namespace FCG.Users.Domain.Usuarios;

/// <summary>
/// Raiz de Agregação do contexto de Identidade. Concentra dados cadastrais,
/// credencial (hash da senha), perfil de acesso e situação (ativo/inativo).
/// </summary>
public sealed class Usuario : AggregateRoot<Guid>
{
    public const int TamanhoMinimoNome = 3;
    public const int TamanhoMaximoNome = 150;

    // Exigido pelo EF Core.
    private Usuario() { }

    public string Nome { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string SenhaHash { get; private set; } = string.Empty;
    public PerfilUsuario Perfil { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime CadastradoEm { get; private set; }
    public DateTime? AtualizadoEm { get; private set; }

    public bool EhAdministrador => Perfil == PerfilUsuario.Administrador;

    /// <summary>Cadastra um usuário e registra o evento <see cref="UsuarioCadastrado"/>.</summary>
    public static Usuario Cadastrar(string nome, Email email, string senhaHash, PerfilUsuario perfil)
    {
        ValidarNome(nome);
        ValidarPerfil(perfil);

        if (email is null)
            throw new DomainException("E-mail do usuário é obrigatório.");

        if (string.IsNullOrWhiteSpace(senhaHash))
            throw new DomainException("Senha do usuário é obrigatória.");

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = nome.Trim(),
            Email = email,
            SenhaHash = senhaHash,
            Perfil = perfil,
            Ativo = true,
            CadastradoEm = DateTime.UtcNow
        };

        usuario.RegistrarEvento(new UsuarioCadastrado(usuario.Id, usuario.Nome, usuario.Email.Valor, usuario.Perfil));
        return usuario;
    }

    public void AtualizarDados(string nome, Email email)
    {
        ValidarNome(nome);
        Nome = nome.Trim();
        Email = email ?? throw new DomainException("E-mail do usuário é obrigatório.");
        MarcarAtualizacao();
    }

    public void AlterarSenha(string novoSenhaHash)
    {
        if (string.IsNullOrWhiteSpace(novoSenhaHash))
            throw new DomainException("Senha do usuário é obrigatória.");

        SenhaHash = novoSenhaHash;
        MarcarAtualizacao();
    }

    public void AlterarPerfil(PerfilUsuario perfil)
    {
        ValidarPerfil(perfil);
        Perfil = perfil;
        MarcarAtualizacao();
    }

    public void Desativar()
    {
        if (!Ativo)
            throw new DomainException("Usuário já está inativo.");

        Ativo = false;
        MarcarAtualizacao();
    }

    public void Ativar()
    {
        if (Ativo)
            throw new DomainException("Usuário já está ativo.");

        Ativo = true;
        MarcarAtualizacao();
    }

    private void MarcarAtualizacao() => AtualizadoEm = DateTime.UtcNow;

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Trim().Length < TamanhoMinimoNome)
            throw new DomainException($"Nome é obrigatório e deve ter ao menos {TamanhoMinimoNome} caracteres.");

        if (nome.Trim().Length > TamanhoMaximoNome)
            throw new DomainException($"Nome deve ter no máximo {TamanhoMaximoNome} caracteres.");
    }

    private static void ValidarPerfil(PerfilUsuario perfil)
    {
        if (!Enum.IsDefined(perfil))
            throw new DomainException("Perfil de usuário inválido.");
    }
}
