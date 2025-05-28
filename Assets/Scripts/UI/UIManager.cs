using System;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Canvases")]
    [SerializeField] private GameObject canvasFPS;
    [SerializeField] private GameObject canvasIso;

    [Header("HUD FPS Elements")]
    [SerializeField] private TextMeshProUGUI fpsVidaText;
    [SerializeField] private TextMeshProUGUI fpsRondaText;
    [SerializeField] private TextMeshProUGUI fpsMinionsText;
    [SerializeField] private TextMeshProUGUI fpsOroText;
    [SerializeField] private TextMeshProUGUI fpsVivosText;

    [Header("HUD Isométrico Elements")]
    [SerializeField] private TextMeshProUGUI isoOroText;
    [SerializeField] private TextMeshProUGUI isoVivosText;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Inicializamos mostrando la fase de preparación
        ShowIsoHUD();
        // Disparamos los valores iniciales
        UpdateOro(GameManager.Instance.Oro);
        UpdateVida(CoreHealth.Instance.VidaActual);
        // Minions y ronda se actualizarán cuando comiencen oleadas
        UpdateVivos(0);
    }

    void OnEnable()
    {
        GameManager.OnOroCambiado += UpdateOro;
        CoreHealth.OnVidaNucleoCambiada += UpdateVida;
        WaveManager.OnMinionsRemainingChanged += UpdateMinions;
        WaveManager.OnRondaCambiada += UpdateRonda;
        WaveManager.OnEnemigosVivosChanged += UpdateVivos;
    }

    void OnDisable()
    {
        GameManager.OnOroCambiado -= UpdateOro;
        CoreHealth.OnVidaNucleoCambiada -= UpdateVida;
        WaveManager.OnMinionsRemainingChanged -= UpdateMinions;
        WaveManager.OnRondaCambiada -= UpdateRonda;
        WaveManager.OnEnemigosVivosChanged -= UpdateVivos;
    }

    // Actualizadores:
    private void UpdateOro(int oro)
    {
        fpsOroText.text = $"Oro: {oro}";
        isoOroText.text = $"Oro: {oro}";
    }

    private void UpdateVida(int vida)
    {
        fpsVidaText.text = $"Vida: {vida}%";
    }

    private void UpdateMinions(int quedan)
    {
        fpsMinionsText.text = $"Minions: {quedan}";
    }
    private void UpdateVivos(int vivos)
    {
        fpsVivosText.text = $"Vivos: {vivos}";
        isoVivosText.text = $"Vivos: {vivos}";
    }

    private void UpdateRonda(int ronda)
    {
        // Asumimos rondas 1-based en UI
        fpsRondaText.text = $"Ronda: {ronda + 1}";
    }

    // Métodos para cambiar de HUD:
    public void ShowFPSHUD()
    {
        canvasFPS.SetActive(true);
        canvasIso.SetActive(false);
    }

    public void ShowIsoHUD()
    {
        canvasIso.SetActive(true);
        canvasFPS.SetActive(false);
    }
}
