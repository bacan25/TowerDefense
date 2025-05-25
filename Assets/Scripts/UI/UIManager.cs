using UnityEngine;
using UnityEngine.UI;
using TMPro;


/// <summary>
/// Actualiza la interfaz de usuario en base a eventos del juego.
/// </summary>
public class UIManager : MonoBehaviour 
{
    [SerializeField] TextMeshProUGUI oroTexto;
    [SerializeField] TextMeshProUGUI vidaTexto;

    void OnEnable()
    {
        GameManager.OnOroCambiado += ActualizarOroUI;
        CoreHealth.OnVidaNucleoCambiada += ActualizarVidaUI;
    }

    void OnDisable()
    {
        GameManager.OnOroCambiado -= ActualizarOroUI;
        CoreHealth.OnVidaNucleoCambiada -= ActualizarVidaUI;
    }

    void ActualizarOroUI(int nuevoOro)
    {
        oroTexto.text = $"Oro: {nuevoOro}";
    }

    void ActualizarVidaUI(int vida)
    {
        vidaTexto.text = $"Núcleo: {vida}%";
    }
}
