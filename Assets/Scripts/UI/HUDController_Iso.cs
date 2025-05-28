using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HUDController_Iso : MonoBehaviour
{
    [Header("Referencias UI")]

    [SerializeField] private TextMeshProUGUI dineroText;
    [SerializeField] private Button btnTerminarPrep;
    [SerializeField] private Button btnBallesta;
    [SerializeField] private Button btnCanon;
    [SerializeField] private Button btnTorreMagica;

    [Header("Panel Confirmación")]
    [SerializeField] private GameObject panelConfirmar;
    [SerializeField] private TextMeshProUGUI txtNombreTorre;
    [SerializeField] private TextMeshProUGUI txtCostoTorre;
    [SerializeField] private Image imgPreviewTorre;
    [SerializeField] private Button btnConfirmar;
    [SerializeField] private Button btnCancelar;


    private TowerManager towerManager;
    private TowerManager.TorreTipo tipoSeleccionado;

    void Awake()
    {
        // Comprueba asignaciones mínimas
        if (dineroText == null || btnTerminarPrep == null ||
            btnBallesta == null || btnCanon == null || btnTorreMagica == null ||
            btnConfirmar == null || btnCancelar == null || panelConfirmar == null)
        {
            Debug.LogError("HUDController_Iso: faltan referencias en el Inspector.");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        towerManager = FindObjectOfType<TowerManager>();
        if (towerManager == null)
        {
            Debug.LogError("HUDController_Iso: no se encontró TowerManager en la escena.");
            enabled = false;
            return;
        }

        // Botones de selección
        btnBallesta.onClick.AddListener(() => Seleccionar(TowerManager.TorreTipo.Ballesta));
        btnCanon.onClick.AddListener(() => Seleccionar(TowerManager.TorreTipo.Canon));
        btnTorreMagica.onClick.AddListener(() => Seleccionar(TowerManager.TorreTipo.Magica));

        // Botones de confirmación
        btnConfirmar.onClick.AddListener(ConfirmarConstruccion);
        btnCancelar.onClick.AddListener(CancelarConstruccion);

        // Botón terminar preparación
        btnTerminarPrep.onClick.AddListener(() =>
        {
            panelConfirmar.SetActive(false);
            ZonasConstruccion.Instance?.Iluminar(false);
            GameManager.Instance.IniciarOleada();
        });

        // Init de HUD
        SetupHUD();
    }

    void OnEnable()
    {
        GameManager.OnOroCambiado += ActualizarDinero;
        GameManager.OnFaseConstruccionChanged += OnFaseConstruccionChanged;
    }

    void OnDisable()
    {
        GameManager.OnOroCambiado -= ActualizarDinero;
        GameManager.OnFaseConstruccionChanged -= OnFaseConstruccionChanged;
    }

    /// <summary>
    /// Inicializa los valores del HUD al arrancar la escena.
    /// </summary>
    private void SetupHUD()
    {
        // Actualiza oro inmediatamente
        ActualizarDinero(GameManager.Instance.Oro);

        // Deshabilita panel de confirmación al inicio
        panelConfirmar.SetActive(false);

        // Asegura que los botones de torre empiecen deshabilitados si no hay oro
        ActualizarBotonesConstruccion(GameManager.Instance.Oro);
    }

    private void ActualizarDinero(int oro)
    {
        dineroText.text = $"Oro: {oro}";
        ActualizarBotonesConstruccion(oro);
    }

    /// <summary>
    /// Habilita o deshabilita los botones de torre según el coste.
    /// </summary>
    private void ActualizarBotonesConstruccion(int oro)
    {
        btnBallesta.interactable = oro >= towerManager.Costo(TowerManager.TorreTipo.Ballesta);
        btnCanon.interactable = oro >= towerManager.Costo(TowerManager.TorreTipo.Canon);
        btnTorreMagica.interactable = oro >= towerManager.Costo(TowerManager.TorreTipo.Magica);
    }

    private void Seleccionar(TowerManager.TorreTipo tipo)
    {
        tipoSeleccionado = tipo;
        var cfg = towerManager.GetConfig(tipo);  // Método que devuelve ConfigTorre

        // Actualiza panel de confirmación
        txtNombreTorre.text = cfg.tipo.ToString();
        txtCostoTorre.text = $"Costo: {cfg.costo}";
        imgPreviewTorre.sprite = cfg.previewSprite;  // Añade previewSprite en ConfigTorre

        // Bloquea todos los botones de selección para evitar doble click
        btnBallesta.interactable = false;
        btnCanon.interactable = false;
        btnTorreMagica.interactable = false;

        towerManager.PrepararConstruccion(tipo);
        ZonasConstruccion.Instance.Iluminar(true);
        panelConfirmar.SetActive(true);
    }


    public void ConfirmarConstruccion()
    {
        var cfg = towerManager.GetConfig(tipoSeleccionado);
        // Antes de llamar a ColocarTorreSeleccionada, bloquea el botón si no hay oro:
        btnConfirmar.interactable = GameManager.Instance.Oro >= cfg.costo;

        if (towerManager.ColocarTorreSeleccionada())
            CerrarPanel();
        else
        {
            Debug.LogWarning("Oro insuficiente para construir.");
            // Aquí podrías mostrar un popup o un sonido de error
            CerrarPanel();
        }
    }
    public void CancelarConstruccion()
    {
        CerrarPanel();
    }

    private void CerrarPanel()
    {
        panelConfirmar.SetActive(false);
        ZonasConstruccion.Instance.Iluminar(false);

        // Reactiva botones de selección y actualiza su estado según oro
        int oro = GameManager.Instance.Oro;
        ActualizarBotonesConstruccion(oro);

        // Limpia preview (opcional)
        imgPreviewTorre.sprite = null;
    }

    /// <summary>
    /// Si salimos de la fase de construcción, ocultamos paneles y desactivamos highlights.
    /// </summary>
    private void OnFaseConstruccionChanged(bool enConstruccion)
    {
        if (!enConstruccion)
        {
            panelConfirmar.SetActive(false);
            ZonasConstruccion.Instance.Iluminar(false);
        }
    }
}
