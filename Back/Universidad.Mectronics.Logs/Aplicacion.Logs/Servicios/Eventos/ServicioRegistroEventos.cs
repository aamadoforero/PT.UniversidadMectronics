using Aplicacion.Logs.Errores;
using Aplicacion.Logs.Interfaces;
using Aplicacion.Logs.Validadores;
using Dominio.Logs.Entidades;
using FluentValidation;
using FluentValidation.Results;
using Universidad.Mectronics.Transversal.Auditoria;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Logs.Servicios.Eventos;

/// <summary>
/// Implementación del registro de eventos: valida el evento y lo encola para guardarlo en segundo plano. La aplicación
/// de origen se toma de la clave autenticada.
/// </summary>
/// <param name="aplicacionActual">Aplicación autenticada que envía el evento.</param>
/// <param name="consultaCatalogos">Traducción de nombres de tipo y aplicación a identificadores.</param>
/// <param name="cola">Cola de eventos pendientes de guardar.</param>
/// <param name="validador">Reglas de formato del evento.</param>
public sealed class ServicioRegistroEventos(
    IAplicacionActual aplicacionActual,
    IConsultaCatalogos consultaCatalogos,
    IColaEventos cola,
    IValidator<RegistrarEventoDto> validador) : IServicioRegistroEventos
{
    /// <summary>
    /// Valida el evento, resuelve sus identificadores en los catálogos y lo deja en la cola.
    /// </summary>
    /// <param name="datos">Evento recibido.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Éxito si el evento quedó en la cola, o el error que impidió aceptarlo.</returns>
    public async Task<Resultado> RegistrarAsync(RegistrarEventoDto datos, CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(datos);

        string? nombreAplicacion = aplicacionActual.Nombre;
        if (nombreAplicacion is null)
        {
            return ErroresEvento.CrearAplicacionNoIdentificada();
        }

        ValidationResult validacion = await validador.ValidateAsync(datos, cancelacion);
        if (!validacion.IsValid)
        {
            return ErroresEvento.CrearDatosInvalidos(validacion.ObtenerDetalleError());
        }

        byte? aplicacionId = await consultaCatalogos.ObtenerIdAplicacionAsync(nombreAplicacion, cancelacion);
        if (aplicacionId is null)
        {
            return ErroresEvento.CrearAplicacionNoRegistrada(nombreAplicacion);
        }

        byte? tipoEventoId = await consultaCatalogos.ObtenerIdTipoEventoAsync(datos.Tipo!, cancelacion);
        if (tipoEventoId is null)
        {
            return ErroresEvento.CrearTipoNoRegistrado(datos.Tipo!);
        }

        RegistroEvento evento = RegistroEvento.Crear(tipoEventoId.Value, aplicacionId.Value, datos.Mensaje!, datos.Excepcion, datos.Fecha!.Value);
        return cola.Encolar(evento) ? Resultado.CrearExito() : ErroresEvento.CrearServicioDeteniendose();
    }
}
