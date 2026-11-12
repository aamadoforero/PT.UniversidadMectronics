using Dominio.Seguridad.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Seguridad.Persistencia.Configuraciones;

/// <summary>
/// Mapeo de <see cref="CategoriaProfesor"/> a la tabla <c>Academico.CategoriaProfesor</c>.
/// </summary>
internal sealed class ConfiguracionCategoriaProfesor : IEntityTypeConfiguration<CategoriaProfesor>
{
    /// <summary>
    /// Configura la tabla, la clave y las columnas.
    /// </summary>
    /// <param name="builder">Constructor de la entidad.</param>
    public void Configure(EntityTypeBuilder<CategoriaProfesor> builder)
    {
        builder.ToTable("CategoriaProfesor", "Academico");
        builder.HasKey(categoria => categoria.Id).HasName("PK_CategoriaProfesor");
        builder.Property(categoria => categoria.Nombre).HasMaxLength(CategoriaProfesor.LongitudMaximaNombre).IsRequired();
    }
}
