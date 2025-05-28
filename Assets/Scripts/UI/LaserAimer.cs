// LaserAimer.cs
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserAimer : MonoBehaviour
{
    public float maxDistance = 50f;
    public LayerMask layerMask;

    private LineRenderer lr;
    private Camera cam;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        cam = Camera.main;
        lr.positionCount = 2;
        lr.enabled = false;
    }

    void OnEnable()
    {
        GameManager.OnFaseConstruccionChanged += HandlePhaseChanged;
    }

    void OnDisable()
    {
        GameManager.OnFaseConstruccionChanged -= HandlePhaseChanged;
    }

    void HandlePhaseChanged(bool enConstruccion)
    {
        lr.enabled = enConstruccion;
        if (!enConstruccion)
            ZonasConstruccion.Instance.IluminarTodas(false);
    }

    void Update()
    {
        if (!lr.enabled) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        lr.SetPosition(0, ray.origin);
        lr.SetPosition(1, ray.origin + ray.direction * maxDistance);

        // Clic izquierdo → intentar seleccionar zona
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
            {
                var rend = hit.collider.GetComponent<Renderer>();
                if (rend != null && ZonasConstruccion.Instance.zonas.Contains(rend))
                {
                    // Solo marcamos la zona seleccionada
                    ZonasConstruccion.Instance.SelectZone(rend);
                    // Abrimos el panel de confirmación
                    FindObjectOfType<HUDController_Iso>()?.MostrarConfirmaciónConstrucción();
                }
            }
        }

        // Clic derecho → cancelar todo
        if (Input.GetMouseButtonDown(1))
        {
            lr.enabled = false;
            ZonasConstruccion.Instance.IluminarTodas(false);
            FindObjectOfType<HUDController_Iso>()?.CancelarConstruccion();
        }
    }
}
