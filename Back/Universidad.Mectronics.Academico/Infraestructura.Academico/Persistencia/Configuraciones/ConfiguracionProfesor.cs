using Dominio.Academico.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Academico.Persistencia.Configuraciones;

/// <summary>
/// Mapeo de <see cref="Profesor"/> a la tabla <c>Academico.Profesor</c>, con su cuenta.
/// </summary>
internal sealed class ConfiguracionProfesor : IEntityTypeConfiguration<Profesor>
{
    /// <summary>
    /// Configura la tabla, la clave y la relación con la cuenta.
    /// </summary>
    /// <param name="builder">Constructor de la entidad.</param>
    public void Configure(EntityTypeBuilder<Profesor> builder)
    {
        builder.ToTable("Profesor", "Academico");
        builder.HasKey(profesor => profesor.Id).HasName("PK_Profesor");
        builder.HasOne(profesor => profesor.Usuario)
            .WithMany()
            .HasForeignKey(profesor => profesor.UsuarioId)
            .HasConstraintName("FK_Profesor_Usuario");
    }
}
