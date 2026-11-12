using Dominio.Academico.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Academico.Persistencia.Configuraciones;

/// <summary>
/// Mapeo de <see cref="Estudiante"/> a la tabla <c>Academico.Estudiante</c>, con su cuenta, su programa y sus inscripciones.
/// </summary>
internal sealed class ConfiguracionEstudiante : IEntityTypeConfiguration<Estudiante>
{
    /// <summary>
    /// Configura la tabla, la clave y las relaciones.
    /// </summary>
    /// <param name="builder">Constructor de la entidad.</param>
    public void Configure(EntityTypeBuilder<Estudiante> builder)
    {
        builder.ToTable("Estudiante", "Academico");
        builder.HasKey(estudiante => estudiante.Id).HasName("PK_Estudiante");
        builder.Property(estudiante => estudiante.UbicacionSemestral).IsRequired();

        builder.HasOne(estudiante => estudiante.Usuario)
            .WithMany()
            .HasForeignKey(estudiante => estudiante.UsuarioId)
            .HasConstraintName("FK_Estudiante_Usuario");

        builder.HasOne(estudiante => estudiante.Programa)
            .WithMany()
            .HasForeignKey(estudiante => estudiante.ProgramaId)
            .HasConstraintName("FK_Estudiante_Programa");

        builder.HasMany(estudiante => estudiante.Inscripciones)
            .WithOne()
            .HasForeignKey(inscripcion => inscripcion.EstudianteId)
            .HasConstraintName("FK_Inscripcion_Estudiante");
        builder.Navigation(estudiante => estudiante.Inscripciones)
            .HasField("_inscripciones")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(estudiante => estudiante.InscripcionesActivas);
        builder.Ignore(estudiante => estudiante.CreditosInscritos);
    }
}
