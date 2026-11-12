using Aplicacion.Seguridad.Dtos.Usuarios;
using Aplicacion.Seguridad.Errores;
using Aplicacion.Seguridad.Interfaces;
using Aplicacion.Seguridad.Mapeos;
using Aplicacion.Seguridad.Validadores;
using Dominio.Seguridad.Autorizacion;
using Dominio.Seguridad.Entidades;
using FluentValidation;
using FluentValidation.Results;
using Universidad.Mectronics.Transversal.Paginacion;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Seguridad.Servicios.Usuarios;

/// <summary>
/// Implementación de los casos de uso de lectura de la administración de cuentas.
/// </summary>
/// <param name="autorizador">Reglas de alcance sobre las cuentas.</param>
/// <param name="repositorioUsuarios">Acceso a las cuentas.</param>
/// <param name="reloj">Hora actual en UTC, para indicar si una cuenta está bloqueada.</param>
/// <param name="validadorBusqueda">Reglas de los criterios de búsqueda.</param>
public sealed class ServicioConsultaUsuarios(
    AutorizadorUsuarios autorizador,
    IRepositorioUsuarios repositorioUsuarios,
    IReloj reloj,
    IValidator<BuscarUsuariosDto> validadorBusqueda) : IServicioConsultaUsuarios
{
    /// <summary>
    /// Busca cuentas con filtros opcionales y devuelve una página de resultados.
    /// </summary>
    /// <param name="criterios">Filtros y paginación.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La página de cuentas, o un error de validación o de falta de permiso.</returns>
    public async Task<Resultado<Pagina<UsuarioResumenDto>>> BuscarAsync(
        BuscarUsuariosDto criterios,
        CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(criterios);
        if (!autorizador.TienePermiso(CodigosPermisoUsuarios.Consultar))
        {
            return ErroresUsuario.CrearSinPermiso();
        }

        ValidationResult validacion = await validadorBusqueda.ValidateAsync(criterios, cancelacion);
        if (!validacion.IsValid)
        {
            return ErroresUsuario.CrearDatosInvalidos(validacion.ObtenerDetalleError());
        }

        FiltroUsuariosDto filtro = new(
            string.IsNullOrWhiteSpace(criterios.Texto) ? null : criterios.Texto.Trim(),
            criterios.RolId,
            criterios.Activo,
            criterios.NumeroPagina,
            criterios.TamanoPagina);

        Pagina<Usuario> pagina = await repositorioUsuarios.BuscarAsync(filtro, cancelacion);
        DateTimeOffset ahora = reloj.AhoraUtc;
        return pagina.Convertir(usuario => usuario.ConvertirAUsuarioResumenDto(ahora));
    }

    /// <summary>
    /// Obtiene el detalle de una cuenta.
    /// </summary>
    /// <param name="id">Identificador de la cuenta.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>El detalle, o un error de cuenta inexistente o de falta de permiso.</returns>
    public async Task<Resultado<UsuarioDetalleDto>> ObtenerAsync(int id, CancellationToken cancelacion)
    {
        if (!autorizador.TienePermiso(CodigosPermisoUsuarios.Consultar))
        {
            return ErroresUsuario.CrearSinPermiso();
        }

        Usuario? usuario = await repositorioUsuarios.ObtenerPorIdAsync(id, cancelacion);
        if (usuario is null)
        {
            return ErroresUsuario.CrearNoEncontrado(id);
        }

        return usuario.ConvertirAUsuarioDetalleDto(reloj.AhoraUtc);
    }
}
