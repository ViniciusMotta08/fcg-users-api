using FCG.Users.Application.DTOs;
using FCG.Users.Domain.Usuarios;

namespace FCG.Users.Application.Mappers;

public static class UsuarioMapper
{
    public static UsuarioResponse ParaResponse(Usuario usuario) => new(
        usuario.Id,
        usuario.Nome,
        usuario.Email.Valor,
        usuario.Perfil.ToString(),
        usuario.Ativo,
        usuario.CadastradoEm);
}
