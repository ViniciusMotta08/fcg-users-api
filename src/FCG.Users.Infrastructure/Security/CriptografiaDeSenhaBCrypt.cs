using FCG.Users.Domain.Usuarios;

namespace FCG.Users.Infrastructure.Security;

/// <summary>Hash de senha com BCrypt (o salt vai embutido no hash).</summary>
public sealed class CriptografiaDeSenhaBCrypt : ICriptografiaDeSenha
{
    private const int FatorDeTrabalho = 12;

    public string GerarHash(Senha senha) => BCrypt.Net.BCrypt.HashPassword(senha.Valor, FatorDeTrabalho);

    public bool Conferir(string senhaInformada, string hash)
    {
        if (string.IsNullOrWhiteSpace(senhaInformada) || string.IsNullOrWhiteSpace(hash))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(senhaInformada, hash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return false;
        }
    }
}
