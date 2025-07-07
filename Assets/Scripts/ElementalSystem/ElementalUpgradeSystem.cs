using UnityEngine;
using System;
using System.Collections.Generic;

namespace ElementalSystem
{
    /// <summary>
    /// Sistema de mejoras elementales para torres y jugador.
    /// Diseñado para ser extensible y flexible para futuros sprints.
    /// </summary>
    public class ElementalUpgradeSystem : MonoBehaviour
    {
        public static ElementalUpgradeSystem Instance { get; private set; }
        
        // Eventos para notificar cambios
        public static event Action<GameObject, ElementType> OnElementApplied;
        public static event Action<GameObject, ElementalUpgrade> OnUpgradeApplied;
        
        [Header("Configuración de Elementos")]
        [SerializeField] private ElementDatabase elementDatabase;
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Aplica un elemento a una torre o al jugador.
        /// </summary>
        public bool AplicarElemento(GameObject objetivo, ElementType elemento)
        {
            if (objetivo == null) return false;
            
            // Verificar si el objetivo puede recibir elementos
            IElementalUpgradeable upgradeable = objetivo.GetComponent<IElementalUpgradeable>();
            if (upgradeable == null)
            {
                Debug.LogWarning($"{objetivo.name} no implementa IElementalUpgradeable");
                return false;
            }
            
            // Obtener datos del elemento
            ElementData elementData = elementDatabase.GetElementData(elemento);
            if (elementData == null)
            {
                Debug.LogError($"No se encontraron datos para el elemento {elemento}");
                return false;
            }
            
            // Aplicar elemento
            bool exito = upgradeable.AplicarElemento(elementData);
            
            if (exito)
            {
                OnElementApplied?.Invoke(objetivo, elemento);
                Debug.Log($"Elemento {elemento} aplicado a {objetivo.name}");
            }
            
            return exito;
        }
        
        /// <summary>
        /// Aplica una mejora completa (elemento + nivel) a un objetivo.
        /// </summary>
        public bool AplicarMejora(GameObject objetivo, ElementalUpgrade mejora)
        {
            if (objetivo == null || mejora == null) return false;
            
            IElementalUpgradeable upgradeable = objetivo.GetComponent<IElementalUpgradeable>();
            if (upgradeable == null) return false;
            
            bool exito = upgradeable.AplicarMejora(mejora);
            
            if (exito)
            {
                OnUpgradeApplied?.Invoke(objetivo, mejora);
                Debug.Log($"Mejora {mejora.nombre} nivel {mejora.nivel} aplicada a {objetivo.name}");
            }
            
            return exito;
        }
        
        /// <summary>
        /// Obtiene las mejoras disponibles para un objetivo específico.
        /// </summary>
        public List<ElementalUpgrade> ObtenerMejorasDisponibles(GameObject objetivo)
        {
            List<ElementalUpgrade> mejorasDisponibles = new List<ElementalUpgrade>();
            
            IElementalUpgradeable upgradeable = objetivo.GetComponent<IElementalUpgradeable>();
            if (upgradeable == null) return mejorasDisponibles;
            
            // Obtener elemento actual del objetivo
            ElementType? elementoActual = upgradeable.GetElementoActual();
            
            if (elementoActual.HasValue)
            {
                // Si ya tiene elemento, obtener mejoras del mismo tipo
                ElementData elementData = elementDatabase.GetElementData(elementoActual.Value);
                if (elementData != null)
                {
                    int nivelActual = upgradeable.GetNivelElemental();
                    
                    // Agregar mejoras de niveles superiores
                    foreach (var mejora in elementData.mejoras)
                    {
                        if (mejora.nivel > nivelActual)
                        {
                            mejorasDisponibles.Add(mejora);
                        }
                    }
                }
            }
            else
            {
                // Si no tiene elemento, mostrar todas las opciones base
                foreach (ElementType tipo in Enum.GetValues(typeof(ElementType)))
                {
                    if (tipo == ElementType.None) continue;
                    
                    ElementData elementData = elementDatabase.GetElementData(tipo);
                    if (elementData != null && elementData.mejoras.Count > 0)
                    {
                        // Agregar la mejora de nivel 1 de cada elemento
                        var mejoraNivel1 = elementData.mejoras.Find(m => m.nivel == 1);
                        if (mejoraNivel1 != null)
                        {
                            mejorasDisponibles.Add(mejoraNivel1);
                        }
                    }
                }
            }
            
            return mejorasDisponibles;
        }
        
        /// <summary>
        /// Calcula el costo de una mejora específica.
        /// </summary>
        public int CalcularCostoMejora(ElementalUpgrade mejora)
        {
            if (mejora == null) return 0;
            
            // Fórmula base: costo = costoBase * nivel * multiplicadorElemento
            int costoBase = 50;
            float multiplicadorNivel = mejora.nivel;
            float multiplicadorElemento = 1f;
            
            // Ajustar multiplicador según elemento
            switch (mejora.tipoElemento)
            {
                case ElementType.Fuego:
                    multiplicadorElemento = 1.2f; // Más caro por daño alto
                    break;
                case ElementType.Hielo:
                    multiplicadorElemento = 1.1f;
                    break;
                case ElementType.Rayo:
                    multiplicadorElemento = 1.3f; // Más caro por cadena
                    break;
                case ElementType.Veneno:
                    multiplicadorElemento = 1.0f;
                    break;
                case ElementType.Sagrado:
                    multiplicadorElemento = 1.5f; // Más caro por curación
                    break;
                case ElementType.Oscuro:
                    multiplicadorElemento = 1.4f;
                    break;
            }
            
            return Mathf.RoundToInt(costoBase * multiplicadorNivel * multiplicadorElemento);
        }
        
        /// <summary>
        /// Verifica si dos elementos son compatibles para fusión.
        /// </summary>
        public bool SonElementosCompatibles(ElementType elemento1, ElementType elemento2)
        {
            // Definir compatibilidades (para futuras fusiones)
            Dictionary<ElementType, List<ElementType>> compatibilidades = new Dictionary<ElementType, List<ElementType>>
            {
                { ElementType.Fuego, new List<ElementType> { ElementType.Rayo, ElementType.Oscuro } },
                { ElementType.Hielo, new List<ElementType> { ElementType.Sagrado, ElementType.Rayo } },
                { ElementType.Rayo, new List<ElementType> { ElementType.Fuego, ElementType.Hielo } },
                { ElementType.Veneno, new List<ElementType> { ElementType.Oscuro, ElementType.Fuego } },
                { ElementType.Sagrado, new List<ElementType> { ElementType.Hielo, ElementType.Rayo } },
                { ElementType.Oscuro, new List<ElementType> { ElementType.Veneno, ElementType.Fuego } }
            };
            
            if (compatibilidades.ContainsKey(elemento1))
            {
                return compatibilidades[elemento1].Contains(elemento2);
            }
            
            return false;
        }
    }
}
