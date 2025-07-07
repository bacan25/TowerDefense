using UnityEngine;

/// <summary>
/// Maneja la vista previa fantasma de torres durante la fase de construcción.
/// Muestra una representación semi-transparente de la torre siguiendo al cursor.
/// </summary>
public class TowerPreview : MonoBehaviour
{
    [Header("Configuración Visual")]
    [Tooltip("Material para la vista previa válida (puede colocarse)")]
    public Material validPreviewMaterial;
    [Tooltip("Material para la vista previa inválida (no puede colocarse)")]
    public Material invalidPreviewMaterial;
    
    private GameObject currentPreview;
    private Renderer[] previewRenderers;
    private TowerManager.ConfigTorre currentConfig;
    private bool isValidPosition = false;
    
    /// <summary>
    /// Crea una vista previa de la torre seleccionada.
    /// </summary>
    public void StartPreview(TowerManager.ConfigTorre config)
    {
        if (currentPreview != null)
            ClearPreview();
            
        currentConfig = config;
        
        // Crear instancia del prefab como vista previa
        currentPreview = Instantiate(config.prefab);
        currentPreview.name = "TowerPreview";
        
        // Desactivar todos los componentes que no sean visuales
        DisableNonVisualComponents(currentPreview);
        
        // Obtener todos los renderers para cambiar el material
        previewRenderers = currentPreview.GetComponentsInChildren<Renderer>();
        
        // Aplicar material de vista previa inicial
        SetPreviewMaterial(false);
    }
    
    /// <summary>
    /// Actualiza la posición de la vista previa siguiendo al cursor.
    /// </summary>
    public void UpdatePreviewPosition(Vector3 worldPosition, bool isValid)
    {
        if (currentPreview == null) return;
        
        currentPreview.transform.position = worldPosition;
        
        // Actualizar material si cambió el estado de validez
        if (isValid != isValidPosition)
        {
            isValidPosition = isValid;
            SetPreviewMaterial(isValid);
        }
    }
    
    /// <summary>
    /// Limpia la vista previa actual.
    /// </summary>
    public void ClearPreview()
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }
    }
    
    /// <summary>
    /// Devuelve si hay una vista previa activa.
    /// </summary>
    public bool HasPreview => currentPreview != null;
    
    /// <summary>
    /// Devuelve la configuración de la torre en preview.
    /// </summary>
    public TowerManager.ConfigTorre GetCurrentConfig() => currentConfig;
    
    private void DisableNonVisualComponents(GameObject obj)
    {
        // Desactivar scripts
        MonoBehaviour[] scripts = obj.GetComponentsInChildren<MonoBehaviour>();
        foreach (var script in scripts)
        {
            script.enabled = false;
        }
        
        // Desactivar colliders
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
        
        // Desactivar rigidbodies
        Rigidbody[] rigidbodies = obj.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = true;
        }
        
        // Desactivar audio sources
        AudioSource[] audioSources = obj.GetComponentsInChildren<AudioSource>();
        foreach (var audio in audioSources)
        {
            audio.enabled = false;
        }
    }
    
    private void SetPreviewMaterial(bool isValid)
    {
        if (previewRenderers == null) return;
        
        Material materialToUse = isValid ? validPreviewMaterial : invalidPreviewMaterial;
        
        foreach (var renderer in previewRenderers)
        {
            Material[] materials = new Material[renderer.materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = materialToUse;
            }
            renderer.materials = materials;
        }
    }
}
