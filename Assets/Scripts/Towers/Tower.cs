using UnityEngine;
using System.Collections.Generic;
using Player;


/// <summary>
/// Controla el comportamiento de una torre defensiva.
/// </summary>
public class Tower : MonoBehaviour
{
    public float rango = 5f;
    public int daño = 10;
    public float tiempoDisparo = 1f;

    [Header("Projectile Setup")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private BulletPool bulletPool;

    private float temporizador;
    private Enemy objetivoActual;
    private List<Enemy> enemigosEnRango = new List<Enemy>();
    public ITargetSelectionStrategy estrategiaObjetivo = new SeleccionarMasCercano();

    void Update()
    {
        // (presupongo que ya tienes ActualizarEnemigosEnRango() e ITargetSelectionStrategy)
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
        // 1) Pedir bala al pool, ya viene con dmg asignado
        GameObject projObj = bulletPool.GetBullet(daño);

        // 2) Posicionar y resetear rotación
        projObj.transform.position = firePoint.position;
        projObj.transform.rotation = Quaternion.identity;

        // 3) Darle el target
        projObj.GetComponent<BulletPlayer>().SetTarget(enemigo.transform);

        // 4) Activar la bala
        projObj.SetActive(true);
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
