using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interfaz Strategy para selección de objetivos de la torre.
/// </summary>
public interface ITargetSelectionStrategy
{
    /// <summary>
    /// Selecciona un objetivo entre los enemigos en rango.
    /// </summary>
    /// <param name="torre">La torre que selecciona el objetivo.</param>
    /// <param name="enemigosEnRango">Lista de enemigos detectados en rango.</param>
    /// <returns>El enemigo elegido o null si ninguno.</returns>
    Enemy SeleccionarObjetivo(Tower torre, List<Enemy> enemigosEnRango);
}

/// <summary>
/// Estrategia concreta: selecciona el enemigo más cercano a la torre.
/// </summary>
public class SeleccionarMasCercano : ITargetSelectionStrategy 
{
    public Enemy SeleccionarObjetivo(Tower torre, List<Enemy> enemigosEnRango)
    {
        Enemy elegido = null;
        float minDistancia = float.MaxValue;
        foreach (Enemy enemigo in enemigosEnRango)
        {
            float dist = Vector3.Distance(enemigo.transform.position, torre.transform.position);
            if (dist < minDistancia)
            {
                minDistancia = dist;
                elegido = enemigo;
            }
        }
        return elegido;
    }
}

/// <summary>
/// Estrategia concreta: selecciona el enemigo con menor salud.
/// </summary>
public class SeleccionarMasDebil : ITargetSelectionStrategy 
{
    public Enemy SeleccionarObjetivo(Tower torre, List<Enemy> enemigosEnRango)
    {
        Enemy elegido = null;
        float minSalud = float.MaxValue;
        foreach (Enemy enemigo in enemigosEnRango)
        {
            if (enemigo.Salud < minSalud)
            {
                minSalud = enemigo.Salud;
                elegido = enemigo;
            }
        }
        return elegido;
    }
}
