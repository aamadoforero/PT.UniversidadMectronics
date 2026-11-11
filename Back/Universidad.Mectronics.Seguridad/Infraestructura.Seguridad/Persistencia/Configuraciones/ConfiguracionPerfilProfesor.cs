using Dominio.Seguridad.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Seguridad.Persistencia.Configuraciones;

/// <summary>
/// Mapeo de <see cref="PerfilProfesor"/> a la tabla <c>Academico.Profesor</c>. La relación con el usuario
/// se configura en <see cref="ConfiguracionUsuario"/>.
/// </summary>
internal sealed class ConfiguracionPerfilProfesor : IEntityTypeConfiguration<PerfilProfesor>
{
    /// <summary>
    /// Configura tabla, clave e índices del perfil de profesor.
    /// </summary>
    /// <param name="builder">Constructor de la entidad.</param>
    public void Configure(EntityTypeBuilder<PerfilProfesor> builder)
    {
        builder.ToTable("Profesor", "Academico");

        builder.HasKey(perfil => perfil.Id).HasName("PK_Profesor");
        builder.Property(perfil => perfil.Id).ValueGeneratedOnAdd();

        builder.Property(perfil => perfil.CategoriaId).IsRequired();

        builder.HasIndex(perfil => perfil.UsuarioId).IsUnique().HasDatabaseName("UQ_Profesor_UsuarioId");
        builder.HasIndex(perfil => perfil.CategoriaId).HasDatabaseName("IX_Profesor_CategoriaId");
    }
}
