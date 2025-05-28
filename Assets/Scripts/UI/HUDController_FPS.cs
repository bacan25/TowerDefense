using TMPro;
using UnityEngine;

public class HUDController_FPS : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI vidaText, rondaText, minionsText, dineroText;

    void OnEnable()
    {
        CoreHealth.OnVidaNucleoCambiada += ActualizarVida;
        WaveManager.OnMinionsRemainingChanged += ActualizarMinions;
        WaveManager.OnRondaCambiada += ActualizarRonda;
        GameManager.OnOroCambiado += ActualizarDinero;
    }
    void OnDisable()
    {
        CoreHealth.OnVidaNucleoCambiada -= ActualizarVida;
        WaveManager.OnMinionsRemainingChanged -= ActualizarMinions;
        WaveManager.OnRondaCambiada -= ActualizarRonda;
        GameManager.OnOroCambiado -= ActualizarDinero;
    }
    void ActualizarVida(int v)    => vidaText.text    = $"Vida: {v}%";
    void ActualizarRonda(int r)   => rondaText.text   = $"Ronda: {r}";
    void ActualizarMinions(int m) => minionsText.text = $"Minions: {m}";
    void ActualizarDinero(int o)  => dineroText.text  = $"Oro: {o}";
}
