using Dominio.Academico.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Academico.Persistencia.Configuraciones;

/// <summary>
/// Mapeo de <see cref="Inscripcion"/> a la tabla <c>Academico.Inscripcion</c>.
/// </summary>
internal sealed class ConfiguracionInscripcion : IEntityTypeConfiguration<Inscripcion>
{
    /// <summary>
    /// Configura la tabla, la clave, las columnas y la relación con la materia.
    /// </summary>
    /// <param name="builder">Constructor de la entidad.</param>
    public void Configure(EntityTypeBuilder<Inscripcion> builder)
    {
        builder.ToTable("Inscripcion", "Academico");
        builder.HasKey(inscripcion => inscripcion.Id).HasName("PK_Inscripcion");
        builder.Property(inscripcion => inscripcion.Activo).IsRequired();
        builder.Property(inscripcion => inscripcion.FechaInscripcion).IsRequired();
        builder.Property(inscripcion => inscripcion.FechaModificacion);

        builder.HasOne(inscripcion => inscripcion.Materia)
            .WithMany()
            .HasForeignKey(inscripcion => inscripcion.MateriaId)
            .HasConstraintName("FK_Inscripcion_Materia");
    }
}
