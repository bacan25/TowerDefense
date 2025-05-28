// ZonasConstruccion.cs
using System.Collections.Generic;
using UnityEngine;

public class ZonasConstruccion : MonoBehaviour
{
    public static ZonasConstruccion Instance { get; private set; }

    [Tooltip("Mallas de las zonas")]
    public List<Renderer> zonas;

    [Tooltip("Color base")]
    public Color colorNormal;

    [Tooltip("Color en preparación")]
    public Color colorHighlight;

    [Tooltip("Color al hacer clic")]
    public Color colorSelected;

    private Renderer selectedZone;

    // Mapea cada zona a la torre actualmente construida en ella
    private Dictionary<Renderer, GameObject> zoneTowerMap = new Dictionary<Renderer, GameObject>();

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    /// <summary>
    /// Ilumina todas las zonas (preparación) o restaura el color base.
    /// </summary>
    public void IluminarTodas(bool on)
    {
        foreach (var r in zonas)
            r.material.color = on ? colorHighlight : colorNormal;

        if (!on)
            ClearSelection();
    }

    /// <summary>
    /// Selecciona una zona al hacer clic: pinta la anterior de highlight y esta de selected.
    /// También guarda la posición central de la zona.
    /// </summary>
    public void SelectZone(Renderer zone)
    {
        if (selectedZone == zone) return;

        if (selectedZone != null)
            selectedZone.material.color = colorHighlight;

        selectedZone = zone;
        selectedZone.material.color = colorSelected;
    }

    /// <summary>
    /// Deselecciona la zona actual (vuelve a highlight).
    /// </summary>
    public void ClearSelection()
    {
        if (selectedZone != null)
            selectedZone.material.color = colorHighlight;
        selectedZone = null;
    }

    /// <summary>
    /// Zona actualmente seleccionada (o null).
    /// </summary>
    public Renderer SelectedZone => selectedZone;

    /// <summary>
    /// Centro de la zona seleccionada (o Vector3.zero si no hay ninguna).
    /// </summary>
    public Vector3 SelectedZoneCenter
        => selectedZone != null
            ? selectedZone.bounds.center
            : Vector3.zero;

    /// <summary>
    /// Registra o reemplaza la torre en la zona dada.
    /// </summary>
    public void PlaceTowerInZone(Renderer zone, GameObject tower)
    {
        if (zoneTowerMap.TryGetValue(zone, out var oldTower) && oldTower != null)
            Destroy(oldTower);

        zoneTowerMap[zone] = tower;
    }
}
