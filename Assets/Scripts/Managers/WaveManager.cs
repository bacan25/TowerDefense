using System;
using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    public static event Action<int> OnMinionsRemainingChanged;
    public static event Action<int> OnEnemigosVivosChanged;
    public static event Action<int> OnRondaCambiada;

    [Serializable]
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
    private int vivosActuales = 0;

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
        if (indiceOleadaActual >= oleadas.Length)
            yield break;

        var oleada = oleadas[indiceOleadaActual];
        int total = oleada.cantidad;

        // Opcional: escalar salud
        if (indiceOleadaActual > 0)
        {
            var ejemplo = oleada.prefabEnemigo.GetComponent<Enemy>();
            if (ejemplo != null)
                ejemplo.saludMax += indiceOleadaActual * 2;
        }

        for (int i = 0; i < total; i++)
        {
            // 1) Selecciona spawn
            Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

            // 2) Instancia dentro de "enemiesContainer"
            Instantiate(
                oleada.prefabEnemigo,
                spawnPoint.position,
                spawnPoint.rotation,
                enemiesContainer
            );

            // 3) Actualiza y notifica vivos
            vivosActuales++;
            OnEnemigosVivosChanged?.Invoke(vivosActuales);

            // 4) Notifica cuántos faltan por spawnear
            OnMinionsRemainingChanged?.Invoke(total - i - 1);

            yield return new WaitForSeconds(oleada.intervalo);
        }

        // Espera a que todos mueran (vivosActuales llegue a 0)
        yield return new WaitUntil(() => vivosActuales == 0);

        // Nueva ronda
        indiceOleadaActual++;
        OnRondaCambiada?.Invoke(indiceOleadaActual);

        // Vuelta a fase de preparación
        GameManager.Instance.IniciarPreparacion();
    }

    /// <summary>
    /// Llamar desde Enemy.Morir() justo antes de Destroy(...)
    /// </summary>
    public void EnemigoMuerto()
    {
        vivosActuales = Mathf.Max(0, vivosActuales - 1);
        OnEnemigosVivosChanged?.Invoke(vivosActuales);
    }
}
