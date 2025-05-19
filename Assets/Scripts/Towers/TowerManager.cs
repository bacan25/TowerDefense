using UnityEngine;

/// <summary>
/// Gestiona la construcción de torres en la fase de preparación.
/// </summary>
public class TowerManager : MonoBehaviour
{
    [Tooltip("Prefab de la torre a construir.")]
    public GameObject prefabTorre;
    [Tooltip("Costo en oro para construir esta torre.")]
    public int costoTorreBase = 50;

    /// <summary>
    /// Intenta colocar una torre en la posición especificada.
    /// </summary>
    /// <param name="posicion">Posición en el mundo donde construir.</param>
    /// <returns>True si se construyó, false si no había suficiente oro.</returns>
    public bool ColocarTorre(Vector3 posicion)
    {
        // Usa el método GastarOro que ya descuenta el costo y dispara OnOroCambiado
        if (GameManager.Instance.GastarOro(costoTorreBase))
        {
            Instantiate(prefabTorre, posicion, Quaternion.identity);
            return true;
        }
        else
        {
            Debug.LogWarning("No hay suficiente oro para construir la torre.");
            return false;
        }
    }
}
