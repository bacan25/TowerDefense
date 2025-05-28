using UnityEngine;
using System.Linq;

/// <summary>
/// Gestiona la selección e instanciación de torres en la fase de preparación.
/// </summary>
public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance { get; private set; }

    public enum TorreTipo { Ballesta, Canon, Magica }

    [System.Serializable]
    public struct ConfigTorre
    {
        public TorreTipo tipo;
        public GameObject prefab;
        public int costo;
        public Sprite previewSprite;
    }
    [Tooltip("Configuraciones de cada tipo de torre")]
    public ConfigTorre[] torres;

    private TorreTipo seleccionActual;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Obtiene la configuración de una torre por su tipo.
    /// </summary>
    public ConfigTorre GetConfig(TorreTipo tipo)
        => torres.First(cfg => cfg.tipo == tipo);

    /// <summary>
    /// Obtiene el coste de una torre sin tocar el oro.
    /// </summary>
    public int Costo(TorreTipo tipo)
        => GetConfig(tipo).costo;

    /// <summary>
    /// Prepara la construcción (se guarda el tipo seleccionado).
    /// </summary>
    public void PrepararConstruccion(TorreTipo tipo)
    {
        seleccionActual = tipo;
        ZonasConstruccion.Instance.IluminarTodas(true);
    }

    /// <summary>
    /// Instancia la torre seleccionada en el centro de la zona clicada,
    /// gasta el oro y limpia la selección.
    /// </summary>
    public bool ColocarTorreSeleccionada()
    {
        var zonas = ZonasConstruccion.Instance;
        var zone = zonas.SelectedZone;
        if (zone == null)
        {
            Debug.LogWarning("No hay zona seleccionada para construir.");
            return false;
        }

        var cfg = GetConfig(seleccionActual);

        // Aquí descontamos el oro y disparamos OnOroCambiado
        if (!GameManager.Instance.GastarOro(cfg.costo))
        {
            Debug.LogWarning("Oro insuficiente para construir.");
            return false;
        }

        // Instanciamos la torre en la posición central de la zona
        Vector3 center = zonas.SelectedZoneCenter;
        var towerObj = Instantiate(cfg.prefab, center, Quaternion.identity);

        // Asociamos la torre a la zona y apagamos el highlight
        zonas.PlaceTowerInZone(zone, towerObj);
        zonas.ClearSelection();
        zonas.IluminarTodas(false);

        return true;
    }
}
