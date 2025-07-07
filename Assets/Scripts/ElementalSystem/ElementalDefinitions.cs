using UnityEngine;
using System;
using System.Collections.Generic;

namespace ElementalSystem
{
    /// <summary>
    /// Tipos de elementos disponibles en el juego.
    /// </summary>
    public enum ElementType
    {
        None,
        Fuego,      // Daño con el tiempo (DoT)
        Hielo,      // Ralentización y posible congelación
        Rayo,       // Daño en cadena
        Veneno,     // DoT y reducción de armadura
        Sagrado,    // Daño extra a no-muertos, curación aliados
        Oscuro      // Robo de vida, debuff
    }
    
    /// <summary>
    /// Efectos especiales que pueden aplicar los elementos.
    /// </summary>
    [Flags]
    public enum ElementalEffect
    {
        None = 0,
        DamageOverTime = 1 << 0,
        Slow = 1 << 1,
        Stun = 1 << 2,
        ChainDamage = 1 << 3,
        ArmorReduction = 1 << 4,
        LifeSteal = 1 << 5,
        Heal = 1 << 6,
        Burn = 1 << 7,
        Freeze = 1 << 8,
        Shock = 1 << 9,
        Poison = 1 << 10,
        Purify = 1 << 11,
        Curse = 1 << 12
    }
    
    /// <summary>
    /// Datos de configuración para cada elemento.
    /// </summary>
    [Serializable]
    public class ElementData
    {
        public ElementType tipo;
        public string nombre;
        public string descripcion;
        public Color colorPrimario;
        public Color colorSecundario;
        public Sprite icono;
        public GameObject efectoVisualPrefab;
        public ElementalEffect efectosBase;
        public List<ElementalUpgrade> mejoras = new List<ElementalUpgrade>();
    }
    
    /// <summary>
    /// Representa una mejora elemental específica.
    /// </summary>
    [Serializable]
    public class ElementalUpgrade
    {
        public string nombre;
        public string descripcion;
        public int nivel;
        public ElementType tipoElemento;
        
        [Header("Modificadores de Stats")]
        public float multiplicadorDaño = 1f;
        public float multiplicadorVelocidad = 1f;
        public float multiplicadorRango = 1f;
        
        [Header("Efectos Adicionales")]
        public ElementalEffect efectosAdicionales;
        public float duracionEfecto = 0f;
        public float potenciaEfecto = 0f;
        
        [Header("Propiedades Especiales")]
        public int objetivosAdicionales = 0; // Para efectos en cadena
        public float radioExplosion = 0f;
        public float probabilidadCritico = 0f;
        public float multiplicadorCritico = 1.5f;
    }
    
    /// <summary>
    /// Interfaz que deben implementar los objetos que pueden recibir mejoras elementales.
    /// </summary>
    public interface IElementalUpgradeable
    {
        ElementType? GetElementoActual();
        int GetNivelElemental();
        bool AplicarElemento(ElementData elemento);
        bool AplicarMejora(ElementalUpgrade mejora);
        void RemoverElemento();
    }
    
    /// <summary>
    /// Componente base para aplicar efectos elementales a proyectiles.
    /// </summary>
    public abstract class ElementalProjectile : MonoBehaviour
    {
        protected ElementType tipoElemento;
        protected ElementalUpgrade mejora;
        protected float dañoBase;
        
        public virtual void Configurar(ElementType tipo, ElementalUpgrade mejora, float dañoBase)
        {
            this.tipoElemento = tipo;
            this.mejora = mejora;
            this.dañoBase = dañoBase;
            
            AplicarEfectosVisuales();
        }
        
        protected abstract void AplicarEfectosVisuales();
        protected abstract void OnImpacto(GameObject objetivo);
        
        protected virtual float CalcularDañoFinal()
        {
            float dañoFinal = dañoBase;
            
            if (mejora != null)
            {
                dañoFinal *= mejora.multiplicadorDaño;
                
                // Probabilidad de crítico
                if (UnityEngine.Random.Range(0f, 1f) < mejora.probabilidadCritico)
                {
                    dañoFinal *= mejora.multiplicadorCritico;
                    OnCritico();
                }
            }
            
            return dañoFinal;
        }
        
        protected virtual void OnCritico()
        {
            // Override para efectos de crítico
        }
    }
    
    /// <summary>
    /// Datos para persistir el estado elemental de un objeto.
    /// </summary>
    [Serializable]
    public class ElementalSaveData
    {
        public string objetoId;
        public ElementType tipoElemento;
        public int nivelElemental;
        public List<string> mejorasAplicadas = new List<string>();
    }
}
