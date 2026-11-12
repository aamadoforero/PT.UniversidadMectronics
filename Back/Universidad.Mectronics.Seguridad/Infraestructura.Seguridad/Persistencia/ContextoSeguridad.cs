using Dominio.Seguridad.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Seguridad.Persistencia;

/// <summary>Contexto de Entity Framework del microservicio de Seguridad.</summary>
/// <param name="opciones">Opciones del contexto (proveedor SQL Server y cadena de conexión), registradas por inyección
/// de dependencias.</param>
public sealed class ContextoSeguridad(DbContextOptions<ContextoSeguridad> opciones) : DbContext(opciones)
{
    /// <summary>Cuentas de usuario. Cada consulta incluye siempre su rol, sus permisos y su perfil académico.</summary>
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    /// <summary>Roles de autorización. Cada consulta incluye siempre sus permisos.</summary>
    public DbSet<Rol> Roles => Set<Rol>();

    /// <summary>Catálogo de permisos de todos los microservicios.</summary>
    public DbSet<Permiso> Permisos => Set<Permiso>();

    /// <summary>Programas académicos (solo lectura).</summary>
    public DbSet<Programa> Programas => Set<Programa>();

    /// <summary>Categorías de profesor (solo lectura).</summary>
    public DbSet<CategoriaProfesor> CategoriasProfesor => Set<CategoriaProfesor>();

    /// <summary>Inscripciones de los estudiantes; Seguridad solo las cancela al desactivar una cuenta.</summary>
    public DbSet<Inscripcion> Inscripciones => Set<Inscripcion>();

    /// <summary>
    /// Aplica todas las configuraciones de entidad (<c>IEntityTypeConfiguration</c>) de la carpeta
    /// <c>Persistencia/Configuraciones</c>.
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo de Entity Framework.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContextoSeguridad).Assembly);
    }
}
