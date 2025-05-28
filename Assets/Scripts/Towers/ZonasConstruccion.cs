using System.Collections.Generic;  // Para List<T>
using System.Linq;                 // Para usar Any(), si lo necesitas
using UnityEngine;

public class ZonasConstruccion : MonoBehaviour
{
    public static ZonasConstruccion Instance { get; private set; }
    public List<Renderer> zonas;            // colliders con malla para mostrar
    public Color colorNormal, colorHighlight;
    [HideInInspector] public Vector3 UltimaPosicionClick;

    void Awake() { Instance = this; }

    public void Iluminar(bool on)
    {
        foreach (var r in zonas)
            r.material.color = on ? colorHighlight : colorNormal;
    }

    void Update()
    {
        if (!GameManager.Instance.FaseConstruccion) return;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (zonas.Any(z => z.transform == hit.collider.transform))
                    UltimaPosicionClick = hit.point;
            }
        }
    }
}
