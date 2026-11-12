using Dominio.Academico.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Academico.Persistencia.Configuraciones;

/// <summary>
/// Mapeo de <see cref="Materia"/> a la tabla <c>Academico.Materia</c>, con su profesor.
/// </summary>
internal sealed class ConfiguracionMateria : IEntityTypeConfiguration<Materia>
{
    /// <summary>
    /// Configura la tabla, la clave, las columnas y la relación con el profesor.
    /// </summary>
    /// <param name="builder">Constructor de la entidad.</param>
    public void Configure(EntityTypeBuilder<Materia> builder)
    {
        builder.ToTable("Materia", "Academico");
        builder.HasKey(materia => materia.Id).HasName("PK_Materia");
        builder.Property(materia => materia.Nombre).HasMaxLength(Materia.LongitudMaximaNombre).IsRequired();
        builder.Property(materia => materia.Creditos).IsRequired();
        builder.Property(materia => materia.ProgramaId).IsRequired();
        builder.Property(materia => materia.Activo).IsRequired();
        builder.Property(materia => materia.FechaCreacion).IsRequired();
        builder.Property(materia => materia.FechaModificacion);

        builder.HasOne(materia => materia.Profesor)
            .WithMany()
            .HasForeignKey(materia => materia.ProfesorId)
            .HasConstraintName("FK_Materia_Profesor");
    }
}
