using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestiona el estado global del juego, fases, economía y eventos principales.
/// Singleton para acceso global.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Economía")]
    [Tooltip("Oro inicial con el que comienza el jugador.")]
    public int oroInicial = 100;
    public int Oro { get; private set; }

    [Header("Cámaras")]
    [Tooltip("Cámara para la fase de construcción (vista isométrica).")]
    public GameObject CameraIso;
    [Tooltip("Cámara para la fase de combate (vista FPS).")]
    public GameObject CameraFPS;
    public GameObject fullBodyPrefab;
    public PlayerShooting shootingScript;

    [Header("Fases")]
    public bool FaseConstruccion { get; private set; } = true;

    public static event Action<bool> OnFaseConstruccionChanged;

    /// <summary>
    /// Evento Observer para cambios en el oro.
    /// </summary>
    public static event Action<int> OnOroCambiado;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        Oro = oroInicial;
        OnOroCambiado?.Invoke(Oro);
        IniciarPreparacion();
    }

    /// <summary>
    /// Entra en fase de construcción (isométrica).
    /// </summary>
    public void IniciarPreparacion()
    {
        FaseConstruccion = true;
        CameraIso.SetActive(true);
        fullBodyPrefab.SetActive(true);
        shootingScript.enabled = false;
        CameraFPS.SetActive(false);
        UIManager.Instance.ShowIsoHUD();
        OnFaseConstruccionChanged?.Invoke(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Entra en fase de combate/oleada (FPS).
    /// </summary>
    public void IniciarOleada()
    {
        FaseConstruccion = false;
        CameraIso.SetActive(false);
        fullBodyPrefab.SetActive(false);
        shootingScript.enabled = true;
        CameraFPS.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UIManager.Instance.ShowFPSHUD();
        OnFaseConstruccionChanged?.Invoke(false);
        WaveManager.Instance.ComenzarOleada();
    }

    /// <summary>
    /// Suma oro al jugador (usado al destruir enemigos).
    /// </summary>
    public void RecompensaPorEnemigo(int cantidadOro)
    {
        Oro += cantidadOro;
        OnOroCambiado?.Invoke(Oro);
    }

    /// <summary>
    /// Intenta gastar oro. Devuelve true si hay suficiente y descuenta la cantidad.
    /// </summary>
    public bool GastarOro(int cantidad)
    {
        if (Oro >= cantidad)
        {
            Oro -= cantidad;
            OnOroCambiado?.Invoke(Oro);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Maneja el fin del juego (victoria o derrota).
    /// </summary>
    /// <param name="victoria">True si ganó, false si perdió.</param>
    public void GameOver(bool victoria)
    {
        if(!victoria)
        {
            SceneManager.LoadScene(2);
        }
        //Time.timeScale = 0f;
    }
}
