using UnityEngine;

/// <summary>
/// Controlador mejorado para la colocación de torres con sistema de preview.
/// Reemplaza al LaserAimer para un flujo más intuitivo.
/// </summary>
public class TowerPlacementController : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Distancia máxima del raycast")]
    public float maxRaycastDistance = 100f;
    [Tooltip("Layer de las zonas de construcción")]
    public LayerMask constructionZoneLayer;
    [Tooltip("Offset vertical para el preview de torres")]
    public float towerHeightOffset = 0.1f;
    
    private Camera mainCamera;
    private TowerManager towerManager;
    private ZonasConstruccion zonasConstruccion;
    private Renderer currentHoveredZone;
    private bool canPlace = false;
    
    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("TowerPlacementController: No se encontró la cámara principal.");
            enabled = false;
            return;
        }
        
        towerManager = TowerManager.Instance;
        if (towerManager == null)
        {
            Debug.LogError("TowerPlacementController: No se encontró TowerManager.");
            enabled = false;
            return;
        }
        
        zonasConstruccion = ZonasConstruccion.Instance;
        if (zonasConstruccion == null)
        {
            Debug.LogError("TowerPlacementController: No se encontró ZonasConstruccion.");
            enabled = false;
            return;
        }
    }
    
    void Update()
    {
        // Solo procesar si estamos en fase de construcción y colocando una torre
        if (!GameManager.Instance.FaseConstruccion || !towerManager.EstaColocandoTorre())
            return;
            
        HandleTowerPlacement();
    }
    
    private void HandleTowerPlacement()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        // Verificar si el cursor está sobre una zona de construcción
        if (Physics.Raycast(ray, out hit, maxRaycastDistance, constructionZoneLayer))
        {
            Renderer zoneRenderer = hit.collider.GetComponent<Renderer>();
            
            if (zoneRenderer != null && zonasConstruccion.zonas.Contains(zoneRenderer))
            {
                // Actualizar zona hover
                if (currentHoveredZone != zoneRenderer)
                {
                    // Restaurar color de la zona anterior
                    if (currentHoveredZone != null)
                    {
                        currentHoveredZone.material.color = zonasConstruccion.colorHighlight;
                    }
                    
                    currentHoveredZone = zoneRenderer;
                }
                
                // Verificar si la zona ya tiene una torre
                bool zoneOccupied = zonasConstruccion.IsZoneOccupied(zoneRenderer);
                canPlace = !zoneOccupied && GameManager.Instance.Oro >= towerManager.GetConfig(towerManager.GetSelectedType()).costo;
                
                // Actualizar color de la zona actual
                if (canPlace)
                {
                    currentHoveredZone.material.color = zonasConstruccion.colorSelected;
                }
                else
                {
                    currentHoveredZone.material.color = Color.red; // Zona inválida
                }
                
                // Actualizar posición del preview
                Vector3 previewPosition = zoneRenderer.bounds.center;
                previewPosition.y += towerHeightOffset;
                towerManager.ActualizarPreview(previewPosition, canPlace);
                
                // Manejar clicks
                if (Input.GetMouseButtonDown(0) && canPlace)
                {
                    PlaceTower(zoneRenderer);
                }
            }
        }
        else
        {
            // No estamos sobre una zona válida
            if (currentHoveredZone != null)
            {
                currentHoveredZone.material.color = zonasConstruccion.colorHighlight;
                currentHoveredZone = null;
            }
            
            canPlace = false;
            towerManager.ActualizarPreview(Vector3.zero, false);
        }
        
        // Cancelar con clic derecho o ESC
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }
    }
    
    private void PlaceTower(Renderer zone)
    {
        // Seleccionar la zona
        zonasConstruccion.SelectZone(zone);
        
        // Colocar la torre
        if (towerManager.ColocarTorreSeleccionada())
        {
            // Reproducir sonido de construcción
            var audioSources = FindObjectsOfType<AudioSource>();
            foreach (var audio in audioSources)
            {
                if (audio.gameObject.name.Contains("Construccion") || audio.gameObject.name.Contains("Build"))
                {
                    audio.Play();
                    break;
                }
            }
            
            // Limpiar estado
            currentHoveredZone = null;
        }
    }
    
    private void CancelPlacement()
    {
        towerManager.CancelarConstruccion();
        
        if (currentHoveredZone != null)
        {
            currentHoveredZone.material.color = zonasConstruccion.colorNormal;
            currentHoveredZone = null;
        }
    }
    
    void OnDisable()
    {
        // Limpiar al desactivar
        if (currentHoveredZone != null)
        {
            currentHoveredZone.material.color = zonasConstruccion.colorNormal;
            currentHoveredZone = null;
        }
    }
}
