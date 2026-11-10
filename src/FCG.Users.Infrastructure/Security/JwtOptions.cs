namespace FCG.Users.Infrastructure.Security;

/// <summary>Configuração do JWT. Secret vem do Secret do Kubernetes; demais valores do ConfigMap.</summary>
public sealed class JwtOptions
{
    public const string Secao = "Jwt";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "FCG.UsersApi";
    public string Audience { get; set; } = "FCG.Clientes";
    public int MinutosExpiracao { get; set; } = 120;
}
