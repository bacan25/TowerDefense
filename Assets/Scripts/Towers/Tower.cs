using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controla el comportamiento de una torre defensiva.
/// </summary>
public class Tower : MonoBehaviour
{
    [Tooltip("Radio de detección de enemigos.")]
    public float rango = 5f;
    [Tooltip("Daño que causa cada disparo.")]
    public float daño = 10f;
    [Tooltip("Tiempo en segundos entre disparos.")]
    public float tiempoDisparo = 1f;

    private float temporizador;
    private Enemy objetivoActual;
    private List<Enemy> enemigosEnRango = new List<Enemy>();

    [Tooltip("Estrategia para selección de objetivos.")]
    public ITargetSelectionStrategy estrategiaObjetivo = new SeleccionarMasCercano();

    void Update()
    {
        ActualizarEnemigosEnRango();

        if (objetivoActual == null || !enemigosEnRango.Contains(objetivoActual))
            objetivoActual = estrategiaObjetivo.SeleccionarObjetivo(this, enemigosEnRango);

        if (objetivoActual != null)
        {
            temporizador += Time.deltaTime;
            if (temporizador >= tiempoDisparo)
            {
                DispararA(objetivoActual);
                temporizador = 0f;
            }
        }
    }

    void DispararA(Enemy enemigo)
    {
        enemigo?.RecibirDaño(daño);
        // Aquí podrías instanciar un proyectil o efecto visual
    }

    void ActualizarEnemigosEnRango()
    {
        enemigosEnRango.Clear();
        Collider[] hits = Physics.OverlapSphere(transform.position, rango);
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<Enemy>(out Enemy enemigo))
                enemigosEnRango.Add(enemigo);
        }
    }

    // Para visualizar el rango en el editor (opcional)
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, rango);
    }
}
