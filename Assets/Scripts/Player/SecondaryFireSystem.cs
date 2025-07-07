using UnityEngine;
using System.Collections;

namespace Player
{
    /// <summary>
    /// Sistema de disparo secundario con habilidades especiales.
    /// </summary>
    public class SecondaryFireSystem : MonoBehaviour
    {
        public enum TipoDisparoSecundario
        {
            Ralentizacion,
            Stun,
            AreaDamage,
            Penetrante
        }
        
        [Header("Configuración General")]
        [SerializeField] private TipoDisparoSecundario tipoActual = TipoDisparoSecundario.Ralentizacion;
        [SerializeField] private float cooldownSecundario = 5f;
        [SerializeField] private Transform firePoint;
        [SerializeField] private LayerMask enemyLayer;
        
        [Header("Prefabs de Proyectiles")]
        [SerializeField] private GameObject proyectilRalentizacion;
        [SerializeField] private GameObject proyectilStun;
        [SerializeField] private GameObject proyectilArea;
        [SerializeField] private GameObject proyectilPenetrante;
        
        [Header("Efectos Visuales")]
        [SerializeField] private GameObject efectoCarga;
        [SerializeField] private GameObject efectoDisparo;
        
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip sonidoCarga;
        [SerializeField] private AudioClip sonidoDisparo;
        
        private float ultimoDisparo = -10f;
        private bool cargando = false;
        private Coroutine rutinaCarga;
        
        // Eventos para UI
        public static event System.Action<float> OnCooldownChanged;
        public static event System.Action<TipoDisparoSecundario> OnTipoDisparoChanged;
        
        void Start()
        {
            if (firePoint == null)
            {
                Debug.LogError("SecondaryFireSystem: No se asignó firePoint!");
                enabled = false;
            }
            
            OnTipoDisparoChanged?.Invoke(tipoActual);
        }
        
        void Update()
        {
            // Actualizar cooldown para UI
            float tiempoRestante = Mathf.Max(0, cooldownSecundario - (Time.time - ultimoDisparo));
            float porcentaje = 1f - (tiempoRestante / cooldownSecundario);
            OnCooldownChanged?.Invoke(porcentaje);
            
            // Cambiar tipo de disparo con teclas numéricas
            if (Input.GetKeyDown(KeyCode.Alpha1)) CambiarTipoDisparo(TipoDisparoSecundario.Ralentizacion);
            if (Input.GetKeyDown(KeyCode.Alpha2)) CambiarTipoDisparo(TipoDisparoSecundario.Stun);
            if (Input.GetKeyDown(KeyCode.Alpha3)) CambiarTipoDisparo(TipoDisparoSecundario.AreaDamage);
            if (Input.GetKeyDown(KeyCode.Alpha4)) CambiarTipoDisparo(TipoDisparoSecundario.Penetrante);
        }
        
        /// <summary>
        /// Intenta realizar un disparo secundario.
        /// </summary>
        public void IntentarDisparoSecundario()
        {
            if (Time.time - ultimoDisparo < cooldownSecundario)
            {
                Debug.Log($"Disparo secundario en cooldown. Tiempo restante: {cooldownSecundario - (Time.time - ultimoDisparo):F1}s");
                return;
            }
            
            if (!cargando)
            {
                rutinaCarga = StartCoroutine(CargarYDisparar());
            }
        }
        
        private IEnumerator CargarYDisparar()
        {
            cargando = true;
            
            // Reproducir sonido de carga
            if (audioSource && sonidoCarga)
            {
                audioSource.PlayOneShot(sonidoCarga);
            }
            
            // Mostrar efecto de carga
            if (efectoCarga)
            {
                efectoCarga.SetActive(true);
                efectoCarga.transform.position = firePoint.position;
            }
            
            // Tiempo de carga
            yield return new WaitForSeconds(0.5f);
            
            // Ejecutar disparo según tipo
            switch (tipoActual)
            {
                case TipoDisparoSecundario.Ralentizacion:
                    DispararRalentizacion();
                    break;
                case TipoDisparoSecundario.Stun:
                    DispararStun();
                    break;
                case TipoDisparoSecundario.AreaDamage:
                    DispararAreaDamage();
                    break;
                case TipoDisparoSecundario.Penetrante:
                    DispararPenetrante();
                    break;
            }
            
            // Efectos post-disparo
            if (efectoDisparo)
            {
                GameObject efecto = Instantiate(efectoDisparo, firePoint.position, firePoint.rotation);
                Destroy(efecto, 2f);
            }
            
            if (audioSource && sonidoDisparo)
            {
                audioSource.PlayOneShot(sonidoDisparo);
            }
            
            // Ocultar efecto de carga
            if (efectoCarga)
            {
                efectoCarga.SetActive(false);
            }
            
            ultimoDisparo = Time.time;
            cargando = false;
        }
        
        private void DispararRalentizacion()
        {
            if (proyectilRalentizacion)
            {
                GameObject proyectil = Instantiate(proyectilRalentizacion, firePoint.position, firePoint.rotation);
                SlowProjectile slow = proyectil.GetComponent<SlowProjectile>();
                if (slow == null)
                {
                    slow = proyectil.AddComponent<SlowProjectile>();
                }
                slow.Configurar(15f, 50f, 0.5f, 3f); // velocidad, daño, factor ralentización, duración
            }
        }
        
        private void DispararStun()
        {
            if (proyectilStun)
            {
                GameObject proyectil = Instantiate(proyectilStun, firePoint.position, firePoint.rotation);
                StunProjectile stun = proyectil.GetComponent<StunProjectile>();
                if (stun == null)
                {
                    stun = proyectil.AddComponent<StunProjectile>();
                }
                stun.Configurar(20f, 30f, 2f); // velocidad, daño, duración stun
            }
        }
        
        private void DispararAreaDamage()
        {
            if (proyectilArea)
            {
                GameObject proyectil = Instantiate(proyectilArea, firePoint.position, firePoint.rotation);
                AreaProjectile area = proyectil.GetComponent<AreaProjectile>();
                if (area == null)
                {
                    area = proyectil.AddComponent<AreaProjectile>();
                }
                area.Configurar(10f, 75f, 5f); // velocidad, daño, radio explosión
            }
        }
        
        private void DispararPenetrante()
        {
            if (proyectilPenetrante)
            {
                GameObject proyectil = Instantiate(proyectilPenetrante, firePoint.position, firePoint.rotation);
                PiercingProjectile piercing = proyectil.GetComponent<PiercingProjectile>();
                if (piercing == null)
                {
                    piercing = proyectil.AddComponent<PiercingProjectile>();
                }
                piercing.Configurar(25f, 40f, 5); // velocidad, daño, enemigos máximos
            }
        }
        
        public void CambiarTipoDisparo(TipoDisparoSecundario nuevoTipo)
        {
            tipoActual = nuevoTipo;
            OnTipoDisparoChanged?.Invoke(tipoActual);
            Debug.Log($"Tipo de disparo secundario cambiado a: {tipoActual}");
        }
        
        public float GetCooldownRestante()
        {
            return Mathf.Max(0, cooldownSecundario - (Time.time - ultimoDisparo));
        }
        
        public bool EstaDisponible()
        {
            return Time.time - ultimoDisparo >= cooldownSecundario && !cargando;
        }
    }
}
