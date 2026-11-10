using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FCG.Users.Application.Abstractions;
using FCG.Users.Domain.Usuarios;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FCG.Users.Infrastructure.Security;

/// <summary>
/// Emite o JWT validado pelos demais microsserviços (mesmo Secret, Issuer e Audience).
/// O perfil vai como role, habilitando [Authorize(Roles = "Administrador")].
/// </summary>
public sealed class GeradorDeTokenJwt : IGeradorDeToken
{
    private readonly JwtOptions _options;

    public GeradorDeTokenJwt(IOptions<JwtOptions> options) => _options = options.Value;

    public TokenDeAcesso Gerar(Usuario usuario)
    {
        var expiraEm = DateTime.UtcNow.AddMinutes(_options.MinutosExpiracao);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email.Valor),
            new("name", usuario.Nome),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, usuario.Perfil.ToString())
        };

        var credenciais = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiraEm,
            signingCredentials: credenciais);

        return new TokenDeAcesso(new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
    }
}
