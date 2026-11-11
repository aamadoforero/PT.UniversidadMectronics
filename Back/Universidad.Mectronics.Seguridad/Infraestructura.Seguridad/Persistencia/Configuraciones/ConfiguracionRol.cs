using Dominio.Seguridad.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Seguridad.Persistencia.Configuraciones;

/// <summary>
/// Mapeo de <see cref="Rol"/> a la tabla <c>Seguridad.Rol</c> y de su relación muchos a muchos con
/// <see cref="Permiso"/> a través de la tabla <c>Seguridad.RolPermiso</c>, que no tiene clase en el dominio.
/// </summary>
internal sealed class ConfiguracionRol : IEntityTypeConfiguration<Rol>
{
    /// <summary>
    /// Configura tabla, columnas, índice único y la relación con los permisos.
    /// </summary>
    /// <param name="builder">Constructor de la entidad.</param>
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("Rol", "Seguridad");

        builder.HasKey(rol => rol.Id).HasName("PK_Rol");
        builder.Property(rol => rol.Id).ValueGeneratedOnAdd();

        builder.Property(rol => rol.Nombre)
            .HasMaxLength(Rol.LongitudMaximaNombre)
            .IsRequired();

        builder.HasIndex(rol => rol.Nombre).IsUnique().HasDatabaseName("UQ_Rol_Nombre");

        builder.Property(rol => rol.TipoPerfil)
            .HasConversion<byte>()
            .IsRequired();

        builder.HasMany(rol => rol.Permisos)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "RolPermiso",
                permiso => permiso.HasOne<Permiso>()
                    .WithMany()
                    .HasForeignKey("PermisoId")
                    .HasConstraintName("FK_RolPermiso_Permiso")
                    .OnDelete(DeleteBehavior.Restrict),
                rol => rol.HasOne<Rol>()
                    .WithMany()
                    .HasForeignKey("RolId")
                    .HasConstraintName("FK_RolPermiso_Rol")
                    .OnDelete(DeleteBehavior.Restrict),
                rolPermiso =>
                {
                    rolPermiso.ToTable("RolPermiso", "Seguridad");
                    rolPermiso.HasKey("RolId", "PermisoId").HasName("PK_RolPermiso");
                    rolPermiso.HasIndex("PermisoId").HasDatabaseName("IX_RolPermiso_PermisoId");
                });

        builder.Navigation(rol => rol.Permisos)
            .HasField("_permisos")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .AutoInclude();
    }
}
