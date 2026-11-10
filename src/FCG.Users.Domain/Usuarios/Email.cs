using System.Text.RegularExpressions;
using FCG.Users.Domain.SeedWork;

namespace FCG.Users.Domain.Usuarios;

/// <summary>
/// Value Object de e-mail: imutável, sem identidade, comparado pelo valor.
/// Normaliza (minúsculo/trim) e garante o formato na criação.
/// </summary>
public sealed record Email
{
    public const int TamanhoMaximo = 200;

    private static readonly Regex Formato = new(@"^[^@\s]+@[^@\s]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);

    public string Valor { get; }

    private Email(string valor) => Valor = valor;

    public static Email Criar(string? valor)
    {
        if (!EhValido(valor))
            throw new DomainException("E-mail em formato inválido.");

        return new Email(Normalizar(valor!));
    }

    public static bool EhValido(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return false;

        var normalizado = Normalizar(valor);
        return normalizado.Length <= TamanhoMaximo && Formato.IsMatch(normalizado);
    }

    private static string Normalizar(string valor) => valor.Trim().ToLowerInvariant();

    public override string ToString() => Valor;
}
