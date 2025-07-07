using UnityEngine;

/// <summary>
/// Gestiona los AudioListeners para asegurar que solo haya uno activo a la vez.
/// </summary>
public class AudioListenerManager : MonoBehaviour
{
    private AudioListener fpsListener;
    private AudioListener isoListener;
    
    void Start()
    {
        // Buscar los listeners en las cámaras
        GameObject cameraFPS = GameObject.Find("CameraFPS");
        GameObject cameraIso = GameObject.Find("CameraIso");
        
        if (cameraFPS != null)
            fpsListener = cameraFPS.GetComponentInChildren<AudioListener>();
            
        if (cameraIso != null)
            isoListener = cameraIso.GetComponentInChildren<AudioListener>();
            
        // Suscribirse al evento de cambio de fase
        GameManager.OnFaseConstruccionChanged += OnFaseChanged;
        
        // Configurar estado inicial
        OnFaseChanged(GameManager.Instance.FaseConstruccion);
    }
    
    void OnDestroy()
    {
        GameManager.OnFaseConstruccionChanged -= OnFaseChanged;
    }
    
    private void OnFaseChanged(bool enConstruccion)
    {
        if (enConstruccion)
        {
            // Fase de construcción: activar ISO listener
            if (isoListener != null) isoListener.enabled = true;
            if (fpsListener != null) fpsListener.enabled = false;
        }
        else
        {
            // Fase de combate: activar FPS listener
            if (fpsListener != null) fpsListener.enabled = true;
            if (isoListener != null) isoListener.enabled = false;
        }
    }
}
