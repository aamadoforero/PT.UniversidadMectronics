using Dominio.Seguridad.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Seguridad.Persistencia.Configuraciones;

/// <summary>
/// Mapeo de <see cref="Inscripcion"/> a la tabla <c>Academico.Inscripcion</c>. Solo incluye las columnas que Seguridad lee
/// o modifica al cancelar; el resto las administra Académico.
/// </summary>
internal sealed class ConfiguracionInscripcion : IEntityTypeConfiguration<Inscripcion>
{
    /// <summary>
    /// Configura tabla, clave y columnas de la inscripción.
    /// </summary>
    /// <param name="builder">Constructor de la entidad.</param>
    public void Configure(EntityTypeBuilder<Inscripcion> builder)
    {
        builder.ToTable("Inscripcion", "Academico");
        builder.HasKey(inscripcion => inscripcion.Id).HasName("PK_Inscripcion");
        builder.Property(inscripcion => inscripcion.EstudianteId).IsRequired();
        builder.Property(inscripcion => inscripcion.Activo).IsRequired();
        builder.Property(inscripcion => inscripcion.FechaModificacion);
    }
}
