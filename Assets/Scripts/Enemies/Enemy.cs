using System;
using UnityEngine;

/// <summary>
/// Controla el comportamiento básico de un enemigo en la ruta hacia el núcleo.
/// </summary>
public class Enemy : MonoBehaviour
{
    [Tooltip("Velocidad de movimiento del enemigo.")]
    public float velocidad = 3f;
    [Tooltip("Salud máxima del enemigo.")]
    public int saludMax = 20;
    [Tooltip("Daño que inflige al núcleo al llegar.")]
    public int dañoAlNucleo = 5;
    [Tooltip("Oro que otorga al morir.")]
    public int recompensaOro = 10;

    private int saludActual;
    /// <summary>
    /// Salud actual del enemigo (solo lectura).
    /// </summary>
    public int Salud => saludActual;

    /// <summary>
    /// Estrategia de movimiento (puede asignarse dinámicamente).
    /// </summary>
    public IMovementStrategy estrategiaMovimiento;

    private Transform objetivoActualDelCamino;
    private int indiceWaypoint = 0;

    void Start()
    {
        saludActual = saludMax;
        objetivoActualDelCamino = PathManager.Instance.GetWaypoint(indiceWaypoint);
    }

    void Update()
    {
        if (estrategiaMovimiento != null)
            estrategiaMovimiento.Mover(this);
        else
            MoverPorDefecto();
    }

    void MoverPorDefecto()
    {
        if (objetivoActualDelCamino == null) return;
        transform.position = Vector3.MoveTowards(
            transform.position,
            objetivoActualDelCamino.position,
            velocidad * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, objetivoActualDelCamino.position) < 0.1f)
        {
            indiceWaypoint++;
            objetivoActualDelCamino = PathManager.Instance.GetWaypoint(indiceWaypoint);
            if (objetivoActualDelCamino == null)
                LlegarAlNucleo();
        }
    }

    public void RecibirDaño(float cantidad)
    {
        saludActual -= Mathf.CeilToInt(cantidad);
        if (saludActual <= 0)
            Morir();
    }

    void LlegarAlNucleo()
    {
        CoreHealth core = FindObjectOfType<CoreHealth>();
        core?.AplicarDaño(dañoAlNucleo);
        Morir(true);
    }

    void Morir(bool alcanzóNucleo = false)
    {
        if (!alcanzóNucleo)
            GameManager.Instance.RecompensaPorEnemigo(recompensaOro);
        Destroy(gameObject);
    }
}
