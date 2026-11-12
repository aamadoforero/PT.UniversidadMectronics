using Dominio.Academico.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Academico.Persistencia;

/// <summary>
/// Contexto de Entity Framework del microservicio Académico. Escribe materias e inscripciones y lee estudiantes,
/// profesores, programas y cuentas.
/// </summary>
/// <param name="opciones">Opciones del contexto, registradas por inyección de dependencias.</param>
public sealed class ContextoAcademico(DbContextOptions<ContextoAcademico> opciones) : DbContext(opciones)
{
    /// <summary>Estudiantes con sus inscripciones.</summary>
    public DbSet<Estudiante> Estudiantes => Set<Estudiante>();

    /// <summary>Inscripciones de los estudiantes.</summary>
    public DbSet<Inscripcion> Inscripciones => Set<Inscripcion>();

    /// <summary>Materias de los programas.</summary>
    public DbSet<Materia> Materias => Set<Materia>();

    /// <summary>Profesores (solo lectura).</summary>
    public DbSet<Profesor> Profesores => Set<Profesor>();

    /// <summary>Programas académicos (solo lectura).</summary>
    public DbSet<Programa> Programas => Set<Programa>();

    /// <summary>Cuentas de usuario (solo lectura).</summary>
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    /// <summary>
    /// Aplica todas las configuraciones de entidad de la carpeta <c>Persistencia/Configuraciones</c>.
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo de Entity Framework.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContextoAcademico).Assembly);
    }
}
