using Infraestructura.Seguridad.Persistencia.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Seguridad.Persistencia.Configuraciones;

/// <summary>
/// Mapeo de solo lectura de <see cref="CategoriaProfesorCatalogo"/> a la tabla <c>Academico.CategoriaProfesor</c>.
/// </summary>
internal sealed class ConfiguracionCategoriaProfesorCatalogo : IEntityTypeConfiguration<CategoriaProfesorCatalogo>
{
    /// <summary>
    /// Configura la tabla, la clave y las columnas.
    /// </summary>
    /// <param name="builder">Constructor de la entidad.</param>
    public void Configure(EntityTypeBuilder<CategoriaProfesorCatalogo> builder)
    {
        builder.ToTable("CategoriaProfesor", "Academico");
        builder.HasKey(categoria => categoria.Id).HasName("PK_CategoriaProfesor");
        builder.Property(categoria => categoria.Nombre).HasMaxLength(150).IsRequired();
    }
}
