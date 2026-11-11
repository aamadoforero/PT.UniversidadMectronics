using Dominio.Seguridad.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Seguridad.Persistencia.Configuraciones;

/// <summary>
/// Mapeo de <see cref="Permiso"/> a la tabla <c>Seguridad.Permiso</c>.
/// </summary>
internal sealed class ConfiguracionPermiso : IEntityTypeConfiguration<Permiso>
{
    /// <summary>
    /// Configura tabla, columnas e índice único por código.
    /// </summary>
    /// <param name="builder">Constructor de la entidad.</param>
    public void Configure(EntityTypeBuilder<Permiso> builder)
    {
        builder.ToTable("Permiso", "Seguridad");

        builder.HasKey(permiso => permiso.Id).HasName("PK_Permiso");
        builder.Property(permiso => permiso.Id).ValueGeneratedOnAdd();

        builder.Property(permiso => permiso.Codigo)
            .HasMaxLength(Permiso.LongitudMaximaCodigo)
            .IsRequired();

        builder.Property(permiso => permiso.Descripcion)
            .HasMaxLength(Permiso.LongitudMaximaDescripcion)
            .IsRequired();

        builder.HasIndex(permiso => permiso.Codigo).IsUnique().HasDatabaseName("UQ_Permiso_Codigo");
    }
}
