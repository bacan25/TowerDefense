using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace ElementalSystem
{
    /// <summary>
    /// Base de datos centralizada para todos los elementos y sus mejoras.
    /// </summary>
    [CreateAssetMenu(fileName = "ElementDatabase", menuName = "ElementalSystem/Element Database")]
    public class ElementDatabase : ScriptableObject
    {
        [SerializeField] private List<ElementData> elementos = new List<ElementData>();
        
        private Dictionary<ElementType, ElementData> elementCache;
        
        void OnEnable()
        {
            RebuildCache();
        }
        
        private void RebuildCache()
        {
            elementCache = new Dictionary<ElementType, ElementData>();
            foreach (var elemento in elementos)
            {
                if (!elementCache.ContainsKey(elemento.tipo))
                {
                    elementCache[elemento.tipo] = elemento;
                }
            }
        }
        
        /// <summary>
        /// Obtiene los datos de un elemento específico.
        /// </summary>
        public ElementData GetElementData(ElementType tipo)
        {
            if (elementCache == null) RebuildCache();
            
            if (elementCache.TryGetValue(tipo, out ElementData data))
            {
                return data;
            }
            
            Debug.LogWarning($"No se encontraron datos para el elemento {tipo}");
            return null;
        }
        
        /// <summary>
        /// Obtiene todas las mejoras de un nivel específico para un elemento.
        /// </summary>
        public List<ElementalUpgrade> GetUpgradesByLevel(ElementType tipo, int nivel)
        {
            ElementData data = GetElementData(tipo);
            if (data == null) return new List<ElementalUpgrade>();
            
            return data.mejoras.Where(m => m.nivel == nivel).ToList();
        }
        
        /// <summary>
        /// Obtiene la mejora máxima disponible para un elemento.
        /// </summary>
        public ElementalUpgrade GetMaxUpgrade(ElementType tipo)
        {
            ElementData data = GetElementData(tipo);
            if (data == null || data.mejoras.Count == 0) return null;
            
            return data.mejoras.OrderByDescending(m => m.nivel).FirstOrDefault();
        }
        
        /// <summary>
        /// Inicializa la base de datos con valores por defecto.
        /// </summary>
        [ContextMenu("Initialize Default Elements")]
        public void InitializeDefaults()
        {
            elementos.Clear();
            
            // Fuego
            var fuego = new ElementData
            {
                tipo = ElementType.Fuego,
                nombre = "Fuego",
                descripcion = "Inflige daño con el tiempo mediante quemaduras",
                colorPrimario = new Color(1f, 0.4f, 0f),
                colorSecundario = new Color(1f, 0.8f, 0f),
                efectosBase = ElementalEffect.DamageOverTime | ElementalEffect.Burn,
                mejoras = new List<ElementalUpgrade>
                {
                    new ElementalUpgrade
                    {
                        nombre = "Llama Menor",
                        descripcion = "Añade daño de fuego básico",
                        nivel = 1,
                        tipoElemento = ElementType.Fuego,
                        multiplicadorDaño = 1.15f,
                        duracionEfecto = 3f,
                        potenciaEfecto = 5f
                    },
                    new ElementalUpgrade
                    {
                        nombre = "Llama Ardiente",
                        descripcion = "Aumenta el daño y duración de la quemadura",
                        nivel = 2,
                        tipoElemento = ElementType.Fuego,
                        multiplicadorDaño = 1.3f,
                        duracionEfecto = 4f,
                        potenciaEfecto = 10f,
                        probabilidadCritico = 0.1f
                    },
                    new ElementalUpgrade
                    {
                        nombre = "Infierno",
                        descripcion = "Quemaduras devastadoras con explosiones",
                        nivel = 3,
                        tipoElemento = ElementType.Fuego,
                        multiplicadorDaño = 1.5f,
                        duracionEfecto = 5f,
                        potenciaEfecto = 15f,
                        radioExplosion = 3f,
                        probabilidadCritico = 0.2f
                    }
                }
            };
            elementos.Add(fuego);
            
            // Hielo
            var hielo = new ElementData
            {
                tipo = ElementType.Hielo,
                nombre = "Hielo",
                descripcion = "Ralentiza y congela a los enemigos",
                colorPrimario = new Color(0.5f, 0.8f, 1f),
                colorSecundario = new Color(0.8f, 0.9f, 1f),
                efectosBase = ElementalEffect.Slow | ElementalEffect.Freeze,
                mejoras = new List<ElementalUpgrade>
                {
                    new ElementalUpgrade
                    {
                        nombre = "Escarcha",
                        descripcion = "Ralentiza levemente a los enemigos",
                        nivel = 1,
                        tipoElemento = ElementType.Hielo,
                        multiplicadorDaño = 1.1f,
                        multiplicadorVelocidad = 0.8f,
                        duracionEfecto = 2f,
                        potenciaEfecto = 0.3f
                    },
                    new ElementalUpgrade
                    {
                        nombre = "Congelación",
                        descripcion = "Mayor ralentización y posibilidad de congelar",
                        nivel = 2,
                        tipoElemento = ElementType.Hielo,
                        multiplicadorDaño = 1.2f,
                        multiplicadorVelocidad = 0.7f,
                        duracionEfecto = 3f,
                        potenciaEfecto = 0.5f,
                        efectosAdicionales = ElementalEffect.Stun
                    },
                    new ElementalUpgrade
                    {
                        nombre = "Ventisca",
                        descripcion = "Congela en área y fragmentos de hielo",
                        nivel = 3,
                        tipoElemento = ElementType.Hielo,
                        multiplicadorDaño = 1.35f,
                        multiplicadorVelocidad = 0.6f,
                        duracionEfecto = 4f,
                        potenciaEfecto = 0.7f,
                        radioExplosion = 4f,
                        objetivosAdicionales = 2
                    }
                }
            };
            elementos.Add(hielo);
            
            // Rayo
            var rayo = new ElementData
            {
                tipo = ElementType.Rayo,
                nombre = "Rayo",
                descripcion = "Daño en cadena que salta entre enemigos",
                colorPrimario = new Color(0.8f, 0.8f, 1f),
                colorSecundario = new Color(1f, 1f, 0.5f),
                efectosBase = ElementalEffect.ChainDamage | ElementalEffect.Shock,
                mejoras = new List<ElementalUpgrade>
                {
                    new ElementalUpgrade
                    {
                        nombre = "Chispa",
                        descripcion = "Pequeños saltos eléctricos",
                        nivel = 1,
                        tipoElemento = ElementType.Rayo,
                        multiplicadorDaño = 1.05f,
                        multiplicadorVelocidad = 1.1f,
                        objetivosAdicionales = 1,
                        potenciaEfecto = 0.8f
                    },
                    new ElementalUpgrade
                    {
                        nombre = "Relámpago",
                        descripcion = "Cadenas más largas y daño aumentado",
                        nivel = 2,
                        tipoElemento = ElementType.Rayo,
                        multiplicadorDaño = 1.2f,
                        multiplicadorVelocidad = 1.2f,
                        objetivosAdicionales = 3,
                        potenciaEfecto = 0.7f,
                        probabilidadCritico = 0.15f
                    },
                    new ElementalUpgrade
                    {
                        nombre = "Tormenta Eléctrica",
                        descripcion = "Cadenas masivas con mini-stuns",
                        nivel = 3,
                        tipoElemento = ElementType.Rayo,
                        multiplicadorDaño = 1.4f,
                        multiplicadorVelocidad = 1.3f,
                        objetivosAdicionales = 5,
                        potenciaEfecto = 0.6f,
                        efectosAdicionales = ElementalEffect.Stun,
                        duracionEfecto = 0.5f,
                        probabilidadCritico = 0.25f
                    }
                }
            };
            elementos.Add(rayo);
            
            // Guardar cambios
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
    }
}
