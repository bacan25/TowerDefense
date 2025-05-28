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
    /// Se dispara cada vez que cambia el número de minions restantes por spawnear.
    /// Pasa como parámetro cuántos quedan por crear.
    /// </summary>
    public static event Action<int> OnMinionsRemainingChanged;
    public static event Action<int> OnEnemigosVivosChanged;

    /// <summary>
    /// Se dispara al iniciar cada nueva ronda.
    /// Pasa como parámetro el número de ronda (0-basado).
    /// </summary>
    public static event Action<int> OnRondaCambiada;

    [System.Serializable]
    public class Oleada
    {
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

    public int EnemigosVivos => enemiesContainer.childCount;


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
    public static void RaiseEnemigosVivosChanged(int vivos)
    {
        OnEnemigosVivosChanged?.Invoke(vivos);
    }

    private IEnumerator IniciarOleadaRoutine()
    {
        if (indiceOleadaActual >= oleadas.Length)
            yield break;

        var oleada = oleadas[indiceOleadaActual];

        // Aumento de dificultad opcional
        if (indiceOleadaActual != 0)
            oleada.prefabEnemigo.GetComponent<Enemy>().saludMax += indiceOleadaActual * 2;

        int total = oleada.cantidad;
        for (int i = 0; i < total; i++)
        {

            // Spawn
            var spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            Instantiate(oleada.prefabEnemigo, spawnPoint.position, spawnPoint.rotation, enemiesContainer);
            OnEnemigosVivosChanged?.Invoke(enemiesContainer.childCount);


            // Notifica cuántos quedan por spawnear
            OnMinionsRemainingChanged?.Invoke(total - i - 1);

            yield return new WaitForSeconds(oleada.intervalo);
        }

        // Espera hasta que todos los enemigos instanciados hayan sido destruidos
        yield return new WaitUntil(() => enemiesContainer.childCount == 0);

        // Incrementa el índice y notifica nueva ronda
        OnEnemigosVivosChanged?.Invoke(0);
        indiceOleadaActual++;
        OnRondaCambiada?.Invoke(indiceOleadaActual);

        // Vuelta a fase de preparación
        GameManager.Instance.IniciarPreparacion();
    }
}
