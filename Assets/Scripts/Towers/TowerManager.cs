using UnityEngine;
using System.Linq;

/// <summary>
/// Gestiona la construcción de torres en la fase de preparación.
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
        public Sprite previewSprite;  // Sprite para la vista previa en el panel
    }
    public ConfigTorre[] torres;

    private TorreTipo seleccionActual;

    void Awake()
    {
        // Singleton (opcional)
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Devuelve la configuración completa de la torre solicitada.
    /// </summary>
    public ConfigTorre GetConfig(TorreTipo tipo)
        => torres.First(cfg => cfg.tipo == tipo);

    /// <summary>
    /// Costo en oro de la torre indicada.
    /// </summary>
    public int Costo(TorreTipo tipo)
        => GetConfig(tipo).costo;

    /// <summary>
    /// Prepara la construcción seleccionando el tipo de torre.
    /// </summary>
    public void PrepararConstruccion(TorreTipo tipo)
    {
        seleccionActual = tipo;
    }

    /// <summary>
    /// Instancia la torre escogida en la posición de construcción si hay oro suficiente.
    /// </summary>
    public bool ColocarTorreSeleccionada()
    {
        var cfg = GetConfig(seleccionActual);
        Vector3 pos = ZonasConstruccion.Instance.UltimaPosicionClick;
        if (GameManager.Instance.GastarOro(cfg.costo))
        {
            Instantiate(cfg.prefab, pos, Quaternion.identity);
            return true;
        }
        return false;
    }
}
