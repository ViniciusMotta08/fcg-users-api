using FCG.Users.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Users.Infrastructure.Persistence.Configurations;

public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever();

        builder.Property(u => u.Nome).HasMaxLength(Usuario.TamanhoMaximoNome).IsRequired();

        // Value Object Email persistido como uma única coluna.
        builder.Property(u => u.Email)
            .HasConversion(email => email.Valor, valor => Email.Criar(valor))
            .HasColumnName("Email")
            .HasMaxLength(Email.TamanhoMaximo)
            .IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.SenhaHash).HasMaxLength(255).IsRequired();
        builder.Property(u => u.Perfil).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(u => u.Ativo).IsRequired();
        builder.Property(u => u.CadastradoEm).IsRequired();

        builder.Ignore(u => u.EhAdministrador);
    }
}
