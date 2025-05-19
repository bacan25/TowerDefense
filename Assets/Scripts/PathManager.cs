using UnityEngine;

/// <summary>
/// Gestiona la ruta de waypoints para los enemigos.
/// </summary>
public class PathManager : MonoBehaviour
{
    public static PathManager Instance { get; private set; }

    [Tooltip("Waypoints que los enemigos seguirán en orden.")]
    public Transform[] waypoints;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// Devuelve el waypoint en el índice especificado o null si no existe.
    /// </summary>
    public Transform GetWaypoint(int index)
    {
        if (index >= 0 && index < waypoints.Length)
            return waypoints[index];
        return null;
    }
}
