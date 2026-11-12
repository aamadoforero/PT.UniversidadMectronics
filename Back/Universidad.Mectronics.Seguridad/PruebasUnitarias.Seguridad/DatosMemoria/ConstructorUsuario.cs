using Dominio.Seguridad.Entidades;
using Dominio.Seguridad.ObjetosValor;

namespace PruebasUnitarias.Seguridad.DatosMemoria;

/// <summary>
/// Builder de <see cref="Usuario"/> para pruebas, con valores por defecto válidos.
/// </summary>
public sealed class ConstructorUsuario
{
    /// <summary>Identificador que se asigna a la cuenta, como si la hubiera guardado la base de datos.</summary>
    private int _id = 100;

    /// <summary>Correo de la cuenta.</summary>
    private string _email = "ana.perez@universidad.edu";

    /// <summary>Identificación de la cuenta.</summary>
    private string _identificacion = "1000";

    /// <summary>Contraseña en texto plano; la cuenta guarda <c>hash:{contraseña}</c>, el formato del hash falso de las pruebas.</summary>
    private string _contrasena = "Clave#2026";

    /// <summary>Rol de la cuenta; define qué fábrica se usa.</summary>
    private Rol _rol = FabricaRoles.CrearEstudiante();

    /// <summary>Indica si la contraseña ya fue definida por el usuario (no temporal).</summary>
    private bool _contrasenaDefinitiva;

    /// <summary>Semestre del perfil de estudiante.</summary>
    private const int UbicacionSemestral = 3;

    /// <summary>
    /// Asigna el identificador de la cuenta.
    /// </summary>
    /// <param name="id">Identificador.</param>
    /// <returns>El mismo constructor.</returns>
    public ConstructorUsuario AsignarId(int id)
    {
        _id = id;
        return this;
    }

    /// <summary>
    /// Asigna el correo de la cuenta.
    /// </summary>
    /// <param name="email">Correo.</param>
    /// <returns>El mismo constructor.</returns>
    public ConstructorUsuario AsignarEmail(string email)
    {
        _email = email;
        return this;
    }

    /// <summary>
    /// Asigna la identificación de la cuenta.
    /// </summary>
    /// <param name="identificacion">Identificación.</param>
    /// <returns>El mismo constructor.</returns>
    public ConstructorUsuario AsignarIdentificacion(string identificacion)
    {
        _identificacion = identificacion;
        return this;
    }

    /// <summary>
    /// Asigna la contraseña de la cuenta.
    /// </summary>
    /// <param name="contrasena">Contraseña en texto plano.</param>
    /// <returns>El mismo constructor.</returns>
    public ConstructorUsuario AsignarContrasena(string contrasena)
    {
        _contrasena = contrasena;
        return this;
    }

    /// <summary>
    /// Asigna el rol de la cuenta.
    /// </summary>
    /// <param name="rol">Rol.</param>
    /// <returns>El mismo constructor.</returns>
    public ConstructorUsuario AsignarRol(Rol rol)
    {
        _rol = rol;
        return this;
    }

    /// <summary>
    /// Marca la contraseña como definida por el usuario, es decir, ya no temporal.
    /// </summary>
    /// <returns>El mismo constructor.</returns>
    public ConstructorUsuario MarcarContrasenaDefinitiva()
    {
        _contrasenaDefinitiva = true;
        return this;
    }

    /// <summary>
    /// Crea la cuenta con la fábrica que corresponde al tipo de perfil del rol.
    /// </summary>
    /// <returns>La cuenta.</returns>
    public Usuario Construir()
    {
        NombreCompleto nombre = NombreCompleto.Crear("Ana Pérez");
        Identificacion identificacion = Identificacion.Crear(_identificacion);
        CorreoElectronico email = CorreoElectronico.Crear(_email);
        string hash = "hash:" + _contrasena;
        DateTimeOffset ahora = FechasPrueba.Inicio;

        Usuario usuario = _rol.TipoPerfil switch
        {
            TipoPerfil.Estudiante => Usuario.CrearEstudiante(nombre, identificacion, email, hash, _rol, 1, UbicacionSemestral, ahora),
            TipoPerfil.Profesor => Usuario.CrearProfesor(nombre, identificacion, email, hash, _rol, 1, ahora),
            _ => Usuario.CrearSinPerfilAcademico(nombre, identificacion, email, hash, _rol, ahora)
        };

        if (_contrasenaDefinitiva)
        {
            usuario.CambiarContrasena(hash, ahora);
        }

        return AsignadorIdentificador.AsignarId(usuario, _id);
    }
}
