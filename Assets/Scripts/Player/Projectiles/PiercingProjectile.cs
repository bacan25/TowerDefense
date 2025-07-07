using UnityEngine;
using System.Collections.Generic;

namespace Player
{
    /// <summary>
    /// Proyectil que puede atravesar múltiples enemigos.
    /// </summary>
    public class PiercingProjectile : MonoBehaviour
    {
        private float velocidad;
        private float daño;
        private int enemigosMaximos;
        private int enemigosAtravesados = 0;
        private float tiempoVida = 5f;
        
        [Header("Efectos")]
        [SerializeField] private ParticleSystem efectoPenetracion;
        [SerializeField] private TrailRenderer trail;
        
        private Rigidbody rb;
        private HashSet<Enemy> enemigosGolpeados = new HashSet<Enemy>();
        
        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            
            // Agregar collider si no tiene
            if (GetComponent<Collider>() == null)
            {
                CapsuleCollider col = gameObject.AddComponent<CapsuleCollider>();
                col.radius = 0.2f;
                col.height = 1f;
                col.direction = 2; // Z-axis
                col.isTrigger = true;
            }
            
            // Configurar trail si existe
            if (trail == null)
            {
                trail = GetComponent<TrailRenderer>();
            }
            if (trail != null)
            {
                trail.time = 0.5f;
                trail.startWidth = 0.3f;
                trail.endWidth = 0.1f;
            }
        }
        
        public void Configurar(float vel, float dmg, int maxEnemigos)
        {
            velocidad = vel;
            daño = dmg;
            enemigosMaximos = maxEnemigos;
            
            // Iniciar movimiento
            rb.linearVelocity = transform.forward * velocidad;
            
            // Destruir después de tiempo de vida
            Destroy(gameObject, tiempoVida);
        }
        
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                Enemy enemigo = other.GetComponent<Enemy>();
                if (enemigo != null && !enemigosGolpeados.Contains(enemigo))
                {
                    // Marcar como golpeado
                    enemigosGolpeados.Add(enemigo);
                    enemigosAtravesados++;
                    
                    // Aplicar daño (reducido por cada enemigo atravesado)
                    float factorReduccion = Mathf.Pow(0.8f, enemigosAtravesados - 1);
                    float dañoActual = daño * factorReduccion;
                    enemigo.RecibirDaño(dañoActual);
                    
                    // Efecto de penetración
                    if (efectoPenetracion != null)
                    {
                        ParticleSystem efecto = Instantiate(efectoPenetracion, other.transform.position, transform.rotation);
                        efecto.Play();
                        Destroy(efecto.gameObject, 1f);
                    }
                    
                    // Crear efecto visual de atravesar
                    CrearEfectoAtraveser(other.transform.position);
                    
                    // Verificar si alcanzamos el límite
                    if (enemigosAtravesados >= enemigosMaximos)
                    {
                        // Crear explosión final
                        CrearExplosionFinal();
                        Destroy(gameObject);
                    }
                }
            }
            else if (other.gameObject.layer != gameObject.layer && !other.isTrigger && !other.CompareTag("Player"))
            {
                // Impacto con pared u otro objeto sólido
                CrearExplosionFinal();
                Destroy(gameObject);
            }
        }
        
        private void CrearEfectoAtraveser(Vector3 posicion)
        {
            // Crear anillo de energía usando un cilindro aplanado
            GameObject anillo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            anillo.transform.position = posicion;
            anillo.transform.rotation = transform.rotation;
            anillo.transform.localScale = new Vector3(1f, 0.1f, 1f); // Aplanar para simular anillo
            
            // Quitar collider
            Destroy(anillo.GetComponent<Collider>());
            
            // Material emisivo
            Renderer renderer = anillo.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Standard"));
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.cyan * 2f);
            renderer.material = mat;
            
            // Animar
            StartCoroutine(AnimarAnillo(anillo));
        }
        
        private System.Collections.IEnumerator AnimarAnillo(GameObject anillo)
        {
            float tiempo = 0f;
            float duracion = 0.3f;
            Vector3 escalaInicial = anillo.transform.localScale;
            
            while (tiempo < duracion)
            {
                tiempo += Time.deltaTime;
                float t = tiempo / duracion;
                
                // Expandir y desvanecer
                anillo.transform.localScale = escalaInicial * (1f + t * 2f);
                
                // Reducir emisión
                Renderer renderer = anillo.GetComponent<Renderer>();
                Color emision = Color.cyan * (2f - t * 2f);
                renderer.material.SetColor("_EmissionColor", emision);
                
                yield return null;
            }
            
            Destroy(anillo);
        }
        
        private void CrearExplosionFinal()
        {
            // Pequeña explosión al final del recorrido
            GameObject explosion = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            explosion.transform.position = transform.position;
            explosion.transform.localScale = Vector3.one * 0.1f;
            
            // Quitar collider
            Destroy(explosion.GetComponent<Collider>());
            
            // Material emisivo
            Renderer renderer = explosion.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Standard"));
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.white * 3f);
            renderer.material = mat;
            
            // Animar expansión
            StartCoroutine(AnimarExplosion(explosion));
        }
        
        private System.Collections.IEnumerator AnimarExplosion(GameObject explosion)
        {
            float tiempo = 0f;
            float duracion = 0.2f;
            
            while (tiempo < duracion)
            {
                tiempo += Time.deltaTime;
                float t = tiempo / duracion;
                
                explosion.transform.localScale = Vector3.one * Mathf.Lerp(0.1f, 2f, t);
                
                Renderer renderer = explosion.GetComponent<Renderer>();
                Color emision = Color.white * (3f - t * 3f);
                renderer.material.SetColor("_EmissionColor", emision);
                
                yield return null;
            }
            
            Destroy(explosion);
        }
        
        void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, transform.forward * 2f);
        }
    }
}
