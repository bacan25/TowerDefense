// ZonasConstruccion.cs
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ZonasConstruccion : MonoBehaviour
{
    public static ZonasConstruccion Instance { get; private set; }
    public List<Renderer> zonas;            // mallas de las zonas
    public Color colorNormal;               // color base
    public Color colorHighlight;            // color en preparación
    public Color colorSelected;             // color al hacer clic

    [HideInInspector] public Vector3 UltimaPosicionClick;
    private Renderer selectedZone;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    /// <summary>
    /// Ilumina todas las zonas con colorHighlight o las restaura a colorNormal.
    /// </summary>
    public void IluminarTodas(bool on)
    {
        foreach (var r in zonas)
            r.material.color = on ? colorHighlight : colorNormal;

        // si apagamos la iluminación, limpiamos la selección
        if (!on)
            ClearSelection();
    }

    /// <summary>
    /// Marca la zona clicada en colorSelected y restaura la anterior.
    /// </summary>
    public void SelectZone(Renderer zone)
    {
        if (selectedZone == zone) return;

        // restaura la previa
        if (selectedZone != null)
            selectedZone.material.color = colorHighlight;

        selectedZone = zone;
        selectedZone.material.color = colorSelected;
    }

    /// <summary>
    /// Quita la selección actual (vuelve a highlight).
    /// </summary>
    public void ClearSelection()
    {
        if (selectedZone != null)
            selectedZone.material.color = colorHighlight;
        selectedZone = null;
    }
}
