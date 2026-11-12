using Dominio.Academico.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Academico.Persistencia.Configuraciones;

/// <summary>
/// Mapeo de <see cref="Usuario"/> a la tabla <c>Seguridad.Usuario</c>. Solo se mapean las columnas que Académico necesita.
/// </summary>
internal sealed class ConfiguracionUsuario : IEntityTypeConfiguration<Usuario>
{
    /// <summary>
    /// Configura la tabla, la clave y las columnas.
    /// </summary>
    /// <param name="builder">Constructor de la entidad.</param>
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuario", "Seguridad");
        builder.HasKey(usuario => usuario.Id).HasName("PK_Usuario");
        builder.Property(usuario => usuario.NombreCompleto).HasMaxLength(150).IsRequired();
        builder.Property(usuario => usuario.Activo).IsRequired();
    }
}
