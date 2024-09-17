using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.Data.EF.Configuration
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .UseMySqlIdentityColumn()
                .ValueGeneratedOnAdd();
            builder.Property(e => e.Nome)
                   .HasMaxLength(100);
            builder.Property(e => e.Email)
                   .HasMaxLength(100);
            builder.Property(e => e.Senha)
                   .HasMaxLength(50);
            builder.HasIndex(e => e.Email).IsUnique(true);
        }
    }
}