using Dominio.Academico.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Academico.Persistencia.Configuraciones;

/// <summary>
/// Mapeo de <see cref="Programa"/> a la tabla <c>Academico.Programa</c>.
/// </summary>
internal sealed class ConfiguracionPrograma : IEntityTypeConfiguration<Programa>
{
    /// <summary>
    /// Configura la tabla, la clave y las columnas.
    /// </summary>
    /// <param name="builder">Constructor de la entidad.</param>
    public void Configure(EntityTypeBuilder<Programa> builder)
    {
        builder.ToTable("Programa", "Academico");
        builder.HasKey(programa => programa.Id).HasName("PK_Programa");
        builder.Property(programa => programa.Nombre).HasMaxLength(150).IsRequired();
        builder.Property(programa => programa.Activo).IsRequired();
    }
}
