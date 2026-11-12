namespace Dominio.Academico.Politicas;

/// <summary>
/// Reglas de las materias y de su asignación a profesores.
/// </summary>
public static class PoliticaMateria
{
    /// <summary>Créditos de toda materia (R4). La base de datos exige el mismo valor con <c>CK_Materia_Creditos</c>.</summary>
    public const int CreditosPorMateria = 3;

    /// <summary>Máximo de materias activas que dicta un profesor (R6).</summary>
    public const int MaximoMateriasPorProfesor = 2;
}
