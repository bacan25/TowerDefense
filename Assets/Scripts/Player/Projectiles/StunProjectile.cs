using UnityEngine;
using System.Collections;

namespace Player
{
    /// <summary>
    /// Proyectil que aturde a los enemigos impactados.
    /// </summary>
    public class StunProjectile : MonoBehaviour
    {
        private float velocidad;
        private float daño;
        private float duracionStun;
        private float tiempoVida = 5f;
        
        [Header("Efectos")]
        [SerializeField] private ParticleSystem efectoImpacto;
        [SerializeField] private GameObject efectoElectrico;
        
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
                col.radius = 0.25f;
                col.isTrigger = true;
            }
        }
        
        public void Configurar(float vel, float dmg, float stunDuration)
        {
            velocidad = vel;
            daño = dmg;
            duracionStun = stunDuration;
            
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
                    
                    // Aplicar stun
                    EnemyStunEffect stunEffect = enemigo.GetComponent<EnemyStunEffect>();
                    if (stunEffect == null)
                    {
                        stunEffect = enemigo.gameObject.AddComponent<EnemyStunEffect>();
                    }
                    stunEffect.AplicarStun(duracionStun);
                    
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
    /// Componente temporal para aplicar stun a enemigos.
    /// </summary>
    public class EnemyStunEffect : MonoBehaviour
    {
        private Enemy enemigo;
        private IMovementStrategy estrategiaOriginal;
        private Coroutine efectoActual;
        private GameObject efectoVisual;
        
        void Awake()
        {
            enemigo = GetComponent<Enemy>();
            estrategiaOriginal = enemigo.estrategiaMovimiento;
        }
        
        public void AplicarStun(float duracion)
        {
            if (efectoActual != null)
            {
                StopCoroutine(efectoActual);
            }
            efectoActual = StartCoroutine(EfectoStun(duracion));
        }
        
        private IEnumerator EfectoStun(float duracion)
        {
            // Detener movimiento
            enemigo.estrategiaMovimiento = null;
            
            // Crear efecto visual
            if (efectoVisual == null)
            {
                efectoVisual = new GameObject("StunEffect");
                efectoVisual.transform.SetParent(transform);
                efectoVisual.transform.localPosition = Vector3.up * 2f;
                
                // Agregar partículas o sprite de stun
                ParticleSystem ps = efectoVisual.AddComponent<ParticleSystem>();
                var main = ps.main;
                main.startLifetime = 0.5f;
                main.startSpeed = 1f;
                main.startSize = 0.3f;
                main.startColor = Color.yellow;
                
                var shape = ps.shape;
                shape.shapeType = ParticleSystemShapeType.Circle;
                shape.radius = 0.5f;
                
                var emission = ps.emission;
                emission.rateOverTime = 10f;
            }
            
            // Animación de temblor
            Vector3 posicionOriginal = transform.position;
            float tiempoTranscurrido = 0f;
            
            while (tiempoTranscurrido < duracion)
            {
                transform.position = posicionOriginal + Random.insideUnitSphere * 0.1f;
                tiempoTranscurrido += Time.deltaTime;
                yield return null;
            }
            
            // Restaurar posición
            transform.position = posicionOriginal;
            
            // Restaurar movimiento
            enemigo.estrategiaMovimiento = estrategiaOriginal;
            
            // Destruir efecto visual
            if (efectoVisual != null)
            {
                Destroy(efectoVisual);
            }
            
            // Destruir este componente
            Destroy(this);
        }
        
        void OnDestroy()
        {
            // Limpiar efecto visual si el enemigo muere mientras está stunneado
            if (efectoVisual != null)
            {
                Destroy(efectoVisual);
            }
        }
    }
}
