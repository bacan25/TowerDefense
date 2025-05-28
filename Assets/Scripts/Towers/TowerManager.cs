// TowerManager.cs
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

    public ConfigTorre GetConfig(TorreTipo tipo)
        => torres.First(cfg => cfg.tipo == tipo);

    public int Costo(TorreTipo tipo)
        => GetConfig(tipo).costo;

    /// <summary>
    /// Prepara la construcción (se guarda el tipo seleccionado).
    /// </summary>
    public void PrepararConstruccion(TorreTipo tipo)
    {
        seleccionActual = tipo;
        // Opcional: iluminar zonas
        ZonasConstruccion.Instance.IluminarTodas(true);
    }

    /// <summary>
    /// Instancia la torre seleccionada en el centro de la zona clicada,
    /// gasta oro y reemplaza la torre previa en esa zona si existía.
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

        if (!GameManager.Instance.GastarOro(cfg.costo))
        {
            Debug.LogWarning("Oro insuficiente para construir.");
            return false;
        }

        Vector3 center = zonas.SelectedZoneCenter;
        var towerObj = Instantiate(cfg.prefab, center, Quaternion.identity);

        zonas.PlaceTowerInZone(zone, towerObj);

        // Opcional: una vez colocada, dejamos de iluminar o deseleccionamos
        zonas.ClearSelection();
        zonas.IluminarTodas(false);

        return true;
    }
}
