using UnityEngine;
using System.Collections.Generic;

namespace ElementalSystem
{
    /// <summary>
    /// Ejemplo de implementación del sistema elemental en torres.
    /// Las torres existentes pueden heredar de esta clase o implementar IElementalUpgradeable directamente.
    /// </summary>
    public class ElementalTower : MonoBehaviour, IElementalUpgradeable
    {
        [Header("Estado Elemental")]
        [SerializeField] private ElementType elementoActual = ElementType.None;
        [SerializeField] private int nivelElemental = 0;
        [SerializeField] private ElementalUpgrade mejorActual;
        
        [Header("Referencias")]
        [SerializeField] private Tower torreBase; // Referencia al script Tower existente
        [SerializeField] private Transform puntoEfectoElemental;
        
        [Header("Efectos Visuales")]
        private GameObject efectoElementalActivo;
        private ParticleSystem[] sistemasParticulas;
        
        // Cache de stats originales
        private float rangoOriginal;
        private float tiempoDisparoOriginal;
        private int dañoOriginal;
        
        void Awake()
        {
            if (torreBase == null)
                torreBase = GetComponent<Tower>();
                
            if (torreBase != null)
            {
                // Guardar stats originales
                rangoOriginal = torreBase.rango;
                tiempoDisparoOriginal = torreBase.tiempoDisparo;
                dañoOriginal = torreBase.daño;
            }
        }
        
        #region IElementalUpgradeable Implementation
        
        public ElementType? GetElementoActual()
        {
            return elementoActual == ElementType.None ? null : (ElementType?)elementoActual;
        }
        
        public int GetNivelElemental()
        {
            return nivelElemental;
        }
        
        public bool AplicarElemento(ElementData elemento)
        {
            if (elemento == null || elementoActual != ElementType.None)
                return false;
                
            elementoActual = elemento.tipo;
            nivelElemental = 1;
            
            // Aplicar mejora nivel 1
            var mejoraNivel1 = elemento.mejoras.Find(m => m.nivel == 1);
            if (mejoraNivel1 != null)
            {
                AplicarMejora(mejoraNivel1);
            }
            
            // Aplicar efectos visuales
            AplicarEfectosVisuales(elemento);
            
            return true;
        }
        
        public bool AplicarMejora(ElementalUpgrade mejora)
        {
            if (mejora == null) return false;
            
            // Verificar que la mejora sea del elemento correcto
            if (elementoActual != ElementType.None && mejora.tipoElemento != elementoActual)
                return false;
                
            // Si es la primera mejora, establecer el elemento
            if (elementoActual == ElementType.None)
            {
                elementoActual = mejora.tipoElemento;
            }
            
            mejorActual = mejora;
            nivelElemental = mejora.nivel;
            
            // Aplicar modificadores a la torre
            AplicarModificadores(mejora);
            
            // Actualizar efectos visuales
            ActualizarEfectosVisuales();
            
            return true;
        }
        
        public void RemoverElemento()
        {
            elementoActual = ElementType.None;
            nivelElemental = 0;
            mejorActual = null;
            
            // Restaurar stats originales
            if (torreBase != null)
            {
                torreBase.rango = rangoOriginal;
                torreBase.tiempoDisparo = tiempoDisparoOriginal;
                torreBase.daño = dañoOriginal;
            }
            
            // Limpiar efectos visuales
            LimpiarEfectosVisuales();
        }
        
        #endregion
        
        #region Aplicación de Modificadores
        
        private void AplicarModificadores(ElementalUpgrade mejora)
        {
            if (torreBase == null) return;
            
            // Aplicar modificadores de stats
            torreBase.rango = rangoOriginal * mejora.multiplicadorRango;
            torreBase.tiempoDisparo = tiempoDisparoOriginal / mejora.multiplicadorVelocidad; // Menor tiempo = más rápido
            torreBase.daño = Mathf.RoundToInt(dañoOriginal * mejora.multiplicadorDaño);
            
            // Los efectos adicionales se aplicarían a través del sistema de balas
        }
        
        /// <summary>
        /// Método para ser llamado cuando la torre dispara, para aplicar efectos elementales a las balas.
        /// </summary>
        public void OnTowerShoot(GameObject bullet)
        {
            if (mejorActual == null || bullet == null) return;
            
            // Buscar o agregar componente elemental al proyectil
            ElementalBullet elementalBullet = bullet.GetComponent<ElementalBullet>();
            if (elementalBullet == null)
            {
                elementalBullet = bullet.AddComponent<ElementalBullet>();
            }
            
            // Configurar el proyectil con los datos elementales
            elementalBullet.Configurar(elementoActual, mejorActual);
        }
        
        #endregion
        
        #region Efectos Visuales
        
        private void AplicarEfectosVisuales(ElementData elemento)
        {
            if (elemento.efectoVisualPrefab != null && puntoEfectoElemental != null)
            {
                efectoElementalActivo = Instantiate(elemento.efectoVisualPrefab, puntoEfectoElemental);
                efectoElementalActivo.transform.localPosition = Vector3.zero;
                
                // Ajustar colores
                sistemasParticulas = efectoElementalActivo.GetComponentsInChildren<ParticleSystem>();
                foreach (var ps in sistemasParticulas)
                {
                    var main = ps.main;
                    main.startColor = new ParticleSystem.MinMaxGradient(elemento.colorPrimario, elemento.colorSecundario);
                }
            }
        }
        
        private void ActualizarEfectosVisuales()
        {
            if (sistemasParticulas == null) return;
            
            // Escalar efectos según nivel
            float escala = 1f + (nivelElemental - 1) * 0.3f;
            foreach (var ps in sistemasParticulas)
            {
                var main = ps.main;
                main.startSizeMultiplier = escala;
                
                var emission = ps.emission;
                emission.rateOverTimeMultiplier = escala;
            }
        }
        
        private void LimpiarEfectosVisuales()
        {
            if (efectoElementalActivo != null)
            {
                Destroy(efectoElementalActivo);
            }
        }
        
        #endregion
        
        #region UI Integration
        
        /// <summary>
        /// Obtiene información formateada para mostrar en UI.
        /// </summary>
        public string GetElementalInfo()
        {
            if (elementoActual == ElementType.None)
                return "Sin elemento";
                
            string info = $"<color=#{ColorUtility.ToHtmlStringRGB(GetElementColor())}>{elementoActual}</color>";
            
            if (mejorActual != null)
            {
                info += $" Nv.{nivelElemental}\n";
                info += $"{mejorActual.nombre}";
            }
            
            return info;
        }
        
        private Color GetElementColor()
        {
            var elementData = ElementalUpgradeSystem.Instance?.GetComponent<ElementDatabase>()?.GetElementData(elementoActual);
            return elementData?.colorPrimario ?? Color.white;
        }
        
        #endregion
    }
    
    /// <summary>
    /// Componente para aplicar efectos elementales a las balas.
    /// </summary>
    public class ElementalBullet : MonoBehaviour
    {
        private ElementType tipo;
        private ElementalUpgrade mejora;
        private TrailRenderer trail;
        
        public void Configurar(ElementType tipo, ElementalUpgrade mejora)
        {
            this.tipo = tipo;
            this.mejora = mejora;
            
            AplicarEfectosVisuales();
        }
        
        private void AplicarEfectosVisuales()
        {
            // Agregar o modificar trail
            trail = GetComponent<TrailRenderer>();
            if (trail == null)
            {
                trail = gameObject.AddComponent<TrailRenderer>();
            }
            
            // Configurar trail según elemento
            ConfigurarTrailPorElemento();
        }
        
        private void ConfigurarTrailPorElemento()
        {
            trail.time = 0.3f;
            trail.startWidth = 0.1f * (1f + mejora.nivel * 0.2f);
            trail.endWidth = 0.01f;
            
            // Colores según elemento
            Color startColor = Color.white;
            Color endColor = Color.white;
            
            switch (tipo)
            {
                case ElementType.Fuego:
                    startColor = new Color(1f, 0.5f, 0f);
                    endColor = new Color(1f, 0f, 0f);
                    break;
                case ElementType.Hielo:
                    startColor = new Color(0.5f, 0.8f, 1f);
                    endColor = new Color(0.8f, 0.9f, 1f);
                    break;
                case ElementType.Rayo:
                    startColor = new Color(1f, 1f, 0.5f);
                    endColor = new Color(0.8f, 0.8f, 1f);
                    break;
            }
            
            trail.startColor = startColor;
            trail.endColor = endColor;
        }
        
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                // Aplicar efectos elementales según tipo
                AplicarEfectoElemental(other.gameObject);
            }
        }
        
        private void AplicarEfectoElemental(GameObject enemigo)
        {
            // Aquí se implementarían los efectos específicos de cada elemento
            // Por ejemplo: quemar, ralentizar, electrocutar, etc.
            
            switch (tipo)
            {
                case ElementType.Fuego:
                    // Aplicar DoT de fuego
                    break;
                case ElementType.Hielo:
                    // Aplicar ralentización
                    break;
                case ElementType.Rayo:
                    // Buscar enemigos cercanos para cadena
                    break;
            }
        }
    }
}
