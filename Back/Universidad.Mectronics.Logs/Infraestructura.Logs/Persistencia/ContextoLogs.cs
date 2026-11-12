using Dominio.Logs.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Logs.Persistencia;

/// <summary>
/// Contexto de Entity Framework del microservicio de Logs. Mapea solo el esquema <c>Logs</c>. No usa migraciones: el esquema
/// lo crean los scripts de <c>Database/</c>.
/// </summary>
/// <param name="opciones">Opciones del contexto (proveedor SQL Server y cadena de conexión), registradas por inyección de dependencias.</param>
public sealed class ContextoLogs(DbContextOptions<ContextoLogs> opciones) : DbContext(opciones)
{
    /// <summary>Eventos registrados.</summary>
    public DbSet<RegistroEvento> Eventos => Set<RegistroEvento>();

    /// <summary>Catálogo de tipos de evento.</summary>
    public DbSet<TipoEvento> TiposEvento => Set<TipoEvento>();

    /// <summary>Catálogo de aplicaciones que registran eventos.</summary>
    public DbSet<AplicacionOrigen> Aplicaciones => Set<AplicacionOrigen>();

    /// <summary>
    /// Aplica todas las configuraciones de entidad de la carpeta <c>Persistencia/Configuraciones</c>.
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo de Entity Framework.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContextoLogs).Assembly);
    }
}
