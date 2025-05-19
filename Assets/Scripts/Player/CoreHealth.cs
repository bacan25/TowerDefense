using System;
using UnityEngine;

/// <summary>
/// Controla la salud del núcleo y notifica eventos de cambio.
/// </summary>
public class CoreHealth : MonoBehaviour 
{
    [Tooltip("Vida máxima del núcleo.")]
    public int vidaMaxima = 20;
    public int VidaActual { get; private set; }

    public static event Action<int> OnVidaNucleoCambiada;

    void Start()
    {
        VidaActual = vidaMaxima;
        OnVidaNucleoCambiada?.Invoke(VidaActual);
    }

    /// <summary>
    /// Aplica daño al núcleo y notifica al GameManager si llega a 0.
    /// </summary>
    public void AplicarDaño(int cantidad)
    {
        VidaActual -= cantidad;
        if (VidaActual < 0) VidaActual = 0;
        OnVidaNucleoCambiada?.Invoke(VidaActual);
        if (VidaActual == 0)
            GameManager.Instance.GameOver(false);
    }
}
