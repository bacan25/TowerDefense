using UnityEngine;

/// <summary>
/// Interfaz Strategy para el movimiento de los enemigos.
/// </summary>
public interface IMovementStrategy
{
    /// <summary>
    /// Aplica el movimiento al enemigo según la estrategia.
/// </summary>
/// <param name="enemigo">El enemigo a mover.</param>
    void Mover(Enemy enemigo);
}
