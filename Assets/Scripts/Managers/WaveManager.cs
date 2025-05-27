using UnityEngine;
using System.Collections;

/// <summary>
/// Controla la aparición de oleadas de enemigos.
/// </summary>
public class WaveManager : MonoBehaviour 
{
    public static WaveManager Instance { get; private set; }

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

    [SerializeField]private Transform enemiesContainer;

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

        if(indiceOleadaActual != 0)
        {
            oleada.prefabEnemigo.GetComponent<Enemy>().saludMax += indiceOleadaActual * 2;
        }

        for (int i = 0; i < oleada.cantidad; i++)
        {
            Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(oleada.prefabEnemigo, spawn.position, spawn.rotation,enemiesContainer);
            yield return new WaitForSeconds(oleada.intervalo);
        }

        yield return new WaitForSeconds(3f);
        indiceOleadaActual++;
        GameManager.Instance.IniciarPreparacion();
    }
}
