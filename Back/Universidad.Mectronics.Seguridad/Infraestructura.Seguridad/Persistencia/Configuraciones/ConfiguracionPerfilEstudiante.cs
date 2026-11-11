using Dominio.Seguridad.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Seguridad.Persistencia.Configuraciones;

/// <summary>
/// Mapeo de <see cref="PerfilEstudiante"/> a la tabla <c>Academico.Estudiante</c>. La relación con el usuario
/// se configura en <see cref="ConfiguracionUsuario"/>.
/// </summary>
internal sealed class ConfiguracionPerfilEstudiante : IEntityTypeConfiguration<PerfilEstudiante>
{
    /// <summary>
    /// Configura tabla, clave e índices del perfil de estudiante.
    /// </summary>
    /// <param name="builder">Constructor de la entidad.</param>
    public void Configure(EntityTypeBuilder<PerfilEstudiante> builder)
    {
        builder.ToTable("Estudiante", "Academico");

        builder.HasKey(perfil => perfil.Id).HasName("PK_Estudiante");
        builder.Property(perfil => perfil.Id).ValueGeneratedOnAdd();

        builder.Property(perfil => perfil.ProgramaId).IsRequired();
        builder.Property(perfil => perfil.UbicacionSemestral).IsRequired();

        builder.HasIndex(perfil => perfil.UsuarioId).IsUnique().HasDatabaseName("UQ_Estudiante_UsuarioId");
        builder.HasIndex(perfil => perfil.ProgramaId).HasDatabaseName("IX_Estudiante_ProgramaId");
    }
}
