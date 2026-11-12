using Dominio.Logs.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Logs.Persistencia.Configuraciones;

/// <summary>
/// Mapeo de <see cref="TipoEvento"/> a la tabla <c>Logs.TipoEvento</c>.
/// </summary>
internal sealed class ConfiguracionTipoEvento : IEntityTypeConfiguration<TipoEvento>
{
    /// <summary>
    /// Configura la tabla, la clave y las columnas.
    /// </summary>
    /// <param name="builder">Constructor de la entidad.</param>
    public void Configure(EntityTypeBuilder<TipoEvento> builder)
    {
        builder.ToTable("TipoEvento", "Logs");
        builder.HasKey(tipo => tipo.Id).HasName("PK_TipoEvento");
        builder.Property(tipo => tipo.Nombre).HasMaxLength(TipoEvento.LongitudMaximaNombre).IsRequired();
    }
}
