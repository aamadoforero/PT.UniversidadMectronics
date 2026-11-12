using Aplicacion.Logs.Interfaces;
using Dominio.Logs.Entidades;

namespace Infraestructura.Logs.Persistencia.Repositorios;

/// <summary>
/// Implementación de <see cref="IRepositorioEventos"/> con Entity Framework.
/// </summary>
/// <param name="contexto">Contexto de la petición.</param>
internal sealed class RepositorioEventos(ContextoLogs contexto) : IRepositorioEventos
{
    /// <summary>
    /// Agrega un evento nuevo al contexto.
    /// </summary>
    /// <param name="evento">Evento a registrar.</param>
    public void Agregar(RegistroEvento evento) => contexto.Eventos.Add(evento);
}
