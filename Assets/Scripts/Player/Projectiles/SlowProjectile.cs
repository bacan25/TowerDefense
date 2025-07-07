using UnityEngine;
using System.Collections;

namespace Player
{
    /// <summary>
    /// Proyectil que ralentiza a los enemigos impactados.
    /// </summary>
    public class SlowProjectile : MonoBehaviour
    {
        private float velocidad;
        private float daño;
        private float factorRalentizacion;
        private float duracionRalentizacion;
        private float tiempoVida = 5f;
        
        [Header("Efectos")]
        [SerializeField] private ParticleSystem efectoImpacto;
        [SerializeField] private TrailRenderer trail;
        
        private Rigidbody rb;
        
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
                SphereCollider col = gameObject.AddComponent<SphereCollider>();
                col.radius = 0.2f;
                col.isTrigger = true;
            }
        }
        
        public void Configurar(float vel, float dmg, float slowFactor, float slowDuration)
        {
            velocidad = vel;
            daño = dmg;
            factorRalentizacion = Mathf.Clamp01(slowFactor);
            duracionRalentizacion = slowDuration;
            
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
                if (enemigo != null)
                {
                    // Aplicar daño
                    enemigo.RecibirDaño(daño);
                    
                    // Aplicar ralentización
                    EnemySlowEffect slowEffect = enemigo.GetComponent<EnemySlowEffect>();
                    if (slowEffect == null)
                    {
                        slowEffect = enemigo.gameObject.AddComponent<EnemySlowEffect>();
                    }
                    slowEffect.AplicarRalentizacion(factorRalentizacion, duracionRalentizacion);
                    
                    // Efectos visuales
                    if (efectoImpacto != null)
                    {
                        ParticleSystem efecto = Instantiate(efectoImpacto, transform.position, Quaternion.identity);
                        efecto.Play();
                        Destroy(efecto.gameObject, 2f);
                    }
                    
                    // Destruir proyectil
                    Destroy(gameObject);
                }
            }
            else if (other.gameObject.layer != gameObject.layer && !other.isTrigger)
            {
                // Impacto con pared u otro objeto
                Destroy(gameObject);
            }
        }
    }
    
    /// <summary>
    /// Componente temporal para aplicar ralentización a enemigos.
    /// </summary>
    public class EnemySlowEffect : MonoBehaviour
    {
        private Enemy enemigo;
        private float velocidadOriginal;
        private Coroutine efectoActual;
        
        void Awake()
        {
            enemigo = GetComponent<Enemy>();
            velocidadOriginal = enemigo.velocidad;
        }
        
        public void AplicarRalentizacion(float factor, float duracion)
        {
            if (efectoActual != null)
            {
                StopCoroutine(efectoActual);
            }
            efectoActual = StartCoroutine(EfectoRalentizacion(factor, duracion));
        }
        
        private IEnumerator EfectoRalentizacion(float factor, float duracion)
        {
            // Aplicar ralentización
            enemigo.velocidad = velocidadOriginal * (1f - factor);
            
            // Efecto visual (cambiar color)
            Renderer renderer = GetComponentInChildren<Renderer>();
            Color colorOriginal = Color.white;
            if (renderer != null)
            {
                colorOriginal = renderer.material.color;
                renderer.material.color = Color.cyan;
            }
            
            yield return new WaitForSeconds(duracion);
            
            // Restaurar velocidad
            enemigo.velocidad = velocidadOriginal;
            
            // Restaurar color
            if (renderer != null)
            {
                renderer.material.color = colorOriginal;
            }
            
            // Destruir este componente
            Destroy(this);
        }
    }
}
