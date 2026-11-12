using Dominio.Logs.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Logs.Persistencia.Configuraciones;

/// <summary>
/// Mapeo de <see cref="RegistroEvento"/> a la tabla <c>Logs.RegistroEvento</c>.
/// </summary>
internal sealed class ConfiguracionRegistroEvento : IEntityTypeConfiguration<RegistroEvento>
{
    /// <summary>
    /// Configura la tabla, la clave y las columnas.
    /// </summary>
    /// <param name="builder">Constructor de la entidad.</param>
    public void Configure(EntityTypeBuilder<RegistroEvento> builder)
    {
        builder.ToTable("RegistroEvento", "Logs");
        builder.HasKey(evento => evento.Id).HasName("PK_RegistroEvento");
        builder.Property(evento => evento.Id).ValueGeneratedOnAdd();
        builder.Property(evento => evento.FechaEvento).IsRequired();
        builder.Property(evento => evento.TipoEventoId).IsRequired();
        builder.Property(evento => evento.AplicacionId).IsRequired();
        builder.Property(evento => evento.Mensaje).IsRequired();
        builder.Property(evento => evento.Excepcion);
    }
}
