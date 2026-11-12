using Dominio.Seguridad.Entidades;
using Dominio.Seguridad.ObjetosValor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Seguridad.Persistencia.Configuraciones;

/// <summary>
/// Mapeo de <see cref="Usuario"/> a la tabla <c>Seguridad.Usuario</c> y de sus relaciones con el rol y los perfiles.
/// </summary>
internal sealed class ConfiguracionUsuario : IEntityTypeConfiguration<Usuario>
{
    /// <summary>
    /// Configura tabla, columnas, índices únicos y relaciones de la cuenta.
    /// </summary>
    /// <param name="builder">Constructor de la entidad.</param>
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuario", "Seguridad");

        builder.HasKey(usuario => usuario.Id).HasName("PK_Usuario");
        builder.Property(usuario => usuario.Id).ValueGeneratedOnAdd();

        builder.Property(usuario => usuario.NombreCompleto)
            .HasConversion(nombre => nombre.Valor, valor => NombreCompleto.Crear(valor))
            .HasMaxLength(NombreCompleto.LongitudMaxima)
            .IsRequired();

        builder.Property(usuario => usuario.Identificacion)
            .HasConversion(identificacion => identificacion.Valor, valor => Identificacion.Crear(valor))
            .HasMaxLength(Identificacion.LongitudMaxima)
            .IsRequired();

        builder.Property(usuario => usuario.Email)
            .HasConversion(correo => correo.Valor, valor => CorreoElectronico.Crear(valor))
            .HasMaxLength(CorreoElectronico.LongitudMaxima)
            .IsRequired();

        builder.Property(usuario => usuario.Contrasena).IsRequired();

        builder.Property(usuario => usuario.RolId).HasColumnName("RoleId");

        builder.HasIndex(usuario => usuario.Email).IsUnique().HasDatabaseName("UQ_Usuario_Email");
        builder.HasIndex(usuario => usuario.Identificacion).IsUnique().HasDatabaseName("UQ_Usuario_Identificacion");
        builder.HasIndex(usuario => usuario.NombreCompleto).HasDatabaseName("IX_Usuario_NombreCompleto");
        builder.HasIndex(usuario => new { usuario.RolId, usuario.NombreCompleto }).HasDatabaseName("IX_Usuario_RoleId_NombreCompleto");

        builder.HasOne(usuario => usuario.Rol)
            .WithMany()
            .HasForeignKey(usuario => usuario.RolId)
            .HasConstraintName("FK_Usuario_Rol")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(usuario => usuario.PerfilEstudiante)
            .WithOne()
            .HasForeignKey<PerfilEstudiante>(perfil => perfil.UsuarioId)
            .HasConstraintName("FK_Estudiante_Usuario")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(usuario => usuario.PerfilProfesor)
            .WithOne()
            .HasForeignKey<PerfilProfesor>(perfil => perfil.UsuarioId)
            .HasConstraintName("FK_Profesor_Usuario")
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(usuario => usuario.Rol).AutoInclude();
        builder.Navigation(usuario => usuario.PerfilEstudiante).AutoInclude();
        builder.Navigation(usuario => usuario.PerfilProfesor).AutoInclude();
    }
}
