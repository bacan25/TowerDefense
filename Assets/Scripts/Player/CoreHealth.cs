using System;
using UnityEngine;

/// <summary>
/// Controla la salud del núcleo y notifica eventos de cambio.
/// </summary>
public class CoreHealth : MonoBehaviour 
{
    // 1) Singleton
    public static CoreHealth Instance { get; private set; }

    [Tooltip("Vida máxima del núcleo.")]
    public int vidaMaxima = 20;

    // Vida actual expuesta para lecturas
    public int VidaActual { get; private set; }

    // Evento para notificar cambios de vida
    public static event Action<int> OnVidaNucleoCambiada;

    void Awake()
    {
        // Inicializa singleton
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Inicializa vida y dispara el evento inmediatamente
        VidaActual = vidaMaxima;
        OnVidaNucleoCambiada?.Invoke(VidaActual);
    }

    /// <summary>
    /// Aplica daño al núcleo y notifica al GameManager si llega a 0.
    /// </summary>
    public void AplicarDaño(int cantidad)
    {
        VidaActual = Mathf.Max(VidaActual - cantidad, 0);
        OnVidaNucleoCambiada?.Invoke(VidaActual);

        if (VidaActual == 0)
            GameManager.Instance.GameOver(false);
    }
}
