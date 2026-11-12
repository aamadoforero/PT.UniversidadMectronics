using Dominio.Logs.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Logs.Persistencia.Configuraciones;

/// <summary>
/// Mapeo de <see cref="AplicacionOrigen"/> a la tabla <c>Logs.Aplicacion</c>.
/// </summary>
internal sealed class ConfiguracionAplicacionOrigen : IEntityTypeConfiguration<AplicacionOrigen>
{
    /// <summary>
    /// Configura la tabla, la clave y las columnas.
    /// </summary>
    /// <param name="builder">Constructor de la entidad.</param>
    public void Configure(EntityTypeBuilder<AplicacionOrigen> builder)
    {
        builder.ToTable("Aplicacion", "Logs");
        builder.HasKey(aplicacion => aplicacion.Id).HasName("PK_Aplicacion");
        builder.Property(aplicacion => aplicacion.Nombre).HasMaxLength(AplicacionOrigen.LongitudMaximaNombre).IsRequired();
    }
}
