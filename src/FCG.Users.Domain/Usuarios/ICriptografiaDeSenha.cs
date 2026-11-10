namespace FCG.Users.Domain.Usuarios;

/// <summary>
/// Gera e confere o hash da senha. O domínio define o contrato;
/// a implementação (BCrypt) fica na Infraestrutura.
/// </summary>
public interface ICriptografiaDeSenha
{
    string GerarHash(Senha senha);
    bool Conferir(string senhaInformada, string hash);
}
