using FCG.Users.Domain.SeedWork;
using FCG.Users.Domain.Usuarios;
using FCG.Users.Domain.Usuarios.Eventos;

namespace FCG.Users.Domain.Tests;

public class UsuarioTests
{
    private static Usuario NovoUsuario() =>
        Usuario.Cadastrar("Maria Silva", Email.Criar("Maria@Fiap.com.br"), "hash", PerfilUsuario.Usuario);

    [Fact]
    public void Cadastrar_deve_registrar_evento_UsuarioCadastrado()
    {
        var usuario = NovoUsuario();

        var evento = Assert.IsType<UsuarioCadastrado>(Assert.Single(usuario.EventosDeDominio));
        Assert.Equal(usuario.Id, evento.UsuarioId);
        Assert.Equal("maria@fiap.com.br", evento.Email);
        Assert.True(usuario.Ativo);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    public void Cadastrar_com_nome_invalido_deve_falhar(string nome) =>
        Assert.Throws<DomainException>(() =>
            Usuario.Cadastrar(nome, Email.Criar("a@b.com"), "hash", PerfilUsuario.Usuario));

    [Fact]
    public void Desativar_usuario_ja_inativo_deve_falhar()
    {
        var usuario = NovoUsuario();
        usuario.Desativar();

        Assert.Throws<DomainException>(usuario.Desativar);
    }

    [Theory]
    [InlineData("invalido")]
    [InlineData("sem@dominio")]
    [InlineData("")]
    public void Email_invalido_deve_falhar(string valor) =>
        Assert.Throws<DomainException>(() => Email.Criar(valor));

    [Fact]
    public void Emails_com_mesmo_valor_sao_iguais() =>
        Assert.Equal(Email.Criar(" Joao@FIAP.com "), Email.Criar("joao@fiap.com"));

    [Theory]
    [InlineData("curta1!")]
    [InlineData("semnumero!")]
    [InlineData("semespecial1")]
    public void Senha_fraca_deve_falhar(string senha) =>
        Assert.Throws<DomainException>(() => Senha.Criar(senha));

    [Fact]
    public void Senha_forte_deve_ser_aceita() =>
        Assert.Equal("Fiap@2026", Senha.Criar("Fiap@2026").Valor);
}
