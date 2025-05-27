using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Controla la aparición de oleadas de enemigos.
/// </summary>
public class WaveManager : MonoBehaviour 
{
    public static WaveManager Instance { get; private set; }

    /// <summary>
    /// Se dispara cada vez que cambia el número de minions restantes.
    /// Pasa como parámetro cuántos enemigos quedan por spawnear en la oleada actual.
    /// </summary>
    public static event Action<int> OnMinionsRemainingChanged;

    /// <summary>
    /// Se dispara al iniciar cada nueva ronda.
    /// Pasa como parámetro el número de ronda (0-basado).
    /// </summary>
    public static event Action<int> OnRondaCambiada;

    [System.Serializable]
    public class Oleada {
        public GameObject prefabEnemigo;
        public int cantidad = 35;
        public float intervalo = 1f;
    }

    [Tooltip("Configuración de cada oleada.")]
    public Oleada[] oleadas;
    [Tooltip("Puntos de spawn para los enemigos.")]
    public Transform[] spawnPoints;

    [SerializeField] private Transform enemiesContainer;
    private int indiceOleadaActual = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Inicia la corrutina de spawn para la oleada actual.
    /// </summary>
    public void ComenzarOleada()
    {
        StartCoroutine(IniciarOleadaRoutine());
    }

    private IEnumerator IniciarOleadaRoutine()
    {
        if (indiceOleadaActual >= oleadas.Length) yield break;
        Oleada oleada = oleadas[indiceOleadaActual];

        // Aumento de dificultad opcional
        if (indiceOleadaActual != 0)
            oleada.prefabEnemigo.GetComponent<Enemy>().saludMax += indiceOleadaActual * 2;

        int total = oleada.cantidad;
        for (int i = 0; i < total; i++)
        {
            // Spawn
            Transform spawn = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            Instantiate(oleada.prefabEnemigo, spawn.position, spawn.rotation, enemiesContainer);

            // Disparo de evento: quedan (total - i - 1) por crear
            OnMinionsRemainingChanged?.Invoke(total - i - 1);

            yield return new WaitForSeconds(oleada.intervalo);
        }

        // Pequeña espera antes de pasar a preparación
        yield return new WaitForSeconds(3f);

        // Incrementa el índice y avisa de la nueva ronda
        indiceOleadaActual++;
        OnRondaCambiada?.Invoke(indiceOleadaActual);

        // Vuelta a fase de preparación
        GameManager.Instance.IniciarPreparacion();
    }
}
