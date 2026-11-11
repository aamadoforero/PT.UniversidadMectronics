using Infraestructura.Seguridad.Persistencia.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Seguridad.Persistencia.Configuraciones;

/// <summary>
/// Mapeo de solo lectura de <see cref="ProgramaCatalogo"/> a la tabla <c>Academico.Programa</c>. Solo se mapean las columnas
/// que Seguridad necesita.
/// </summary>
internal sealed class ConfiguracionProgramaCatalogo : IEntityTypeConfiguration<ProgramaCatalogo>
{
    /// <summary>
    /// Configura la tabla, la clave y las columnas.
    /// </summary>
    /// <param name="builder">Constructor de la entidad.</param>
    public void Configure(EntityTypeBuilder<ProgramaCatalogo> builder)
    {
        builder.ToTable("Programa", "Academico");
        builder.HasKey(programa => programa.Id).HasName("PK_Programa");
        builder.Property(programa => programa.Nombre).HasMaxLength(150).IsRequired();
        builder.Property(programa => programa.Activo).IsRequired();
    }
}
