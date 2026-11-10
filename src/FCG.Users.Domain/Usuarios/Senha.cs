using FCG.Users.Domain.SeedWork;

namespace FCG.Users.Domain.Usuarios;

/// <summary>
/// Value Object da senha em texto puro. Existe apenas para aplicar a política de senha segura
/// antes do hash — nunca é persistido (o que vai para o banco é o hash).
/// </summary>
public sealed class Senha
{
    public const int TamanhoMinimo = 8;

    public string Valor { get; }

    private Senha(string valor) => Valor = valor;

    /// <summary>Política: mínimo de 8 caracteres, com letra, número e caractere especial.</summary>
    public static Senha Criar(string? valor)
    {
        if (!EhForte(valor))
            throw new DomainException(
                $"Senha inválida. Use no mínimo {TamanhoMinimo} caracteres, com letras, números e caracteres especiais.");

        return new Senha(valor!);
    }

    public static bool EhForte(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor) || valor.Length < TamanhoMinimo)
            return false;

        return valor.Any(char.IsLetter)
               && valor.Any(char.IsDigit)
               && valor.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));
    }

    public override string ToString() => new('*', Valor.Length);
}
