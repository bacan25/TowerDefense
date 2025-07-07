using UnityEngine;
using System.Collections.Generic;

namespace Player
{
    /// <summary>
    /// Proyectil que explota al impactar, causando daño en área.
    /// </summary>
    public class AreaProjectile : MonoBehaviour
    {
        private float velocidad;
        private float daño;
        private float radioExplosion;
        private float tiempoVida = 5f;
        
        [Header("Efectos")]
        [SerializeField] private ParticleSystem efectoExplosion;
        [SerializeField] private GameObject modeloProyectil;
        
        private Rigidbody rb;
        private bool haExplotado = false;
        
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
                col.radius = 0.3f;
                col.isTrigger = true;
            }
        }
        
        public void Configurar(float vel, float dmg, float radius)
        {
            velocidad = vel;
            daño = dmg;
            radioExplosion = radius;
            
            // Iniciar movimiento
            rb.linearVelocity = transform.forward * velocidad;
            
            // Destruir después de tiempo de vida
            Destroy(gameObject, tiempoVida);
        }
        
        void OnTriggerEnter(Collider other)
        {
            // Solo explotar una vez
            if (haExplotado) return;
            
            // Explotar al tocar enemigo o superficie sólida
            if (other.CompareTag("Enemy") || (other.gameObject.layer != gameObject.layer && !other.isTrigger))
            {
                Explotar();
            }
        }
        
        private void Explotar()
        {
            haExplotado = true;
            
            // Detener movimiento
            rb.linearVelocity = Vector3.zero;
            
            // Ocultar modelo
            if (modeloProyectil != null)
            {
                modeloProyectil.SetActive(false);
            }
            
            // Buscar enemigos en el área
            Collider[] colliders = Physics.OverlapSphere(transform.position, radioExplosion);
            HashSet<Enemy> enemigosAfectados = new HashSet<Enemy>();
            
            foreach (Collider col in colliders)
            {
                if (col.CompareTag("Enemy"))
                {
                    Enemy enemigo = col.GetComponent<Enemy>();
                    if (enemigo != null && !enemigosAfectados.Contains(enemigo))
                    {
                        enemigosAfectados.Add(enemigo);
                        
                        // Calcular daño basado en distancia
                        float distancia = Vector3.Distance(transform.position, enemigo.transform.position);
                        float factorDistancia = 1f - (distancia / radioExplosion);
                        float dañoFinal = daño * factorDistancia;
                        
                        // Aplicar daño
                        enemigo.RecibirDaño(dañoFinal);
                        
                        // Aplicar fuerza de explosión
                        Rigidbody rbEnemigo = enemigo.GetComponent<Rigidbody>();
                        if (rbEnemigo != null)
                        {
                            Vector3 direccion = (enemigo.transform.position - transform.position).normalized;
                            rbEnemigo.AddForce(direccion * 500f * factorDistancia, ForceMode.Impulse);
                        }
                    }
                }
            }
            
            // Efectos visuales
            if (efectoExplosion != null)
            {
                ParticleSystem explosion = Instantiate(efectoExplosion, transform.position, Quaternion.identity);
                
                // Ajustar tamaño según radio
                var main = explosion.main;
                main.startSize = radioExplosion * 0.5f;
                
                explosion.Play();
                Destroy(explosion.gameObject, 3f);
            }
            
            // Crear onda expansiva visual
            CrearOndaExpansiva();
            
            // Destruir proyectil después de un momento
            Destroy(gameObject, 0.1f);
        }
        
        private void CrearOndaExpansiva()
        {
            GameObject onda = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            onda.transform.position = transform.position;
            onda.transform.localScale = Vector3.one * 0.1f;
            
            // Quitar collider
            Destroy(onda.GetComponent<Collider>());
            
            // Material transparente
            Renderer renderer = onda.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Standard"));
            mat.SetFloat("_Mode", 3); // Transparent
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
            mat.color = new Color(1f, 0.5f, 0f, 0.5f);
            renderer.material = mat;
            
            // Animar expansión
            StartCoroutine(AnimarOnda(onda, radioExplosion * 2f));
        }
        
        private System.Collections.IEnumerator AnimarOnda(GameObject onda, float tamañoFinal)
        {
            float tiempo = 0f;
            float duracion = 0.5f;
            Renderer renderer = onda.GetComponent<Renderer>();
            
            while (tiempo < duracion)
            {
                tiempo += Time.deltaTime;
                float t = tiempo / duracion;
                
                // Expandir
                onda.transform.localScale = Vector3.one * Mathf.Lerp(0.1f, tamañoFinal, t);
                
                // Desvanecer
                Color color = renderer.material.color;
                color.a = Mathf.Lerp(0.5f, 0f, t);
                renderer.material.color = color;
                
                yield return null;
            }
            
            Destroy(onda);
        }
        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radioExplosion);
        }
    }
}
