# Sistema de Mejoras Elementales

## Descripción General
El sistema de mejoras elementales es una arquitectura flexible y extensible diseñada para agregar profundidad estratégica al juego mediante la personalización de torres y habilidades del jugador con diferentes elementos.

## Arquitectura

### Componentes Principales

1. **ElementalUpgradeSystem** - Sistema central que gestiona todas las mejoras
2. **ElementDatabase** - ScriptableObject que almacena configuraciones de elementos
3. **IElementalUpgradeable** - Interfaz para objetos que pueden recibir mejoras
4. **ElementalTower** - Implementación ejemplo para torres
5. **ElementalProjectile** - Base para proyectiles con efectos elementales

### Elementos Disponibles

- **Fuego** 🔥: Daño con el tiempo (DoT) y quemaduras
- **Hielo** ❄️: Ralentización y congelación
- **Rayo** ⚡: Daño en cadena entre enemigos
- **Veneno** ☠️: DoT y reducción de armadura
- **Sagrado** ✨: Daño extra a no-muertos, curación
- **Oscuro** 🌑: Robo de vida y debuffs

## Uso del Sistema

### 1. Configuración Inicial

```csharp
// En el GameManager o similar
void Start()
{
    // El sistema es un singleton
    ElementalUpgradeSystem.Instance.Initialize();
}
```

### 2. Aplicar Elemento a Torre

```csharp
// Obtener torre objetivo
GameObject torre = // ... tu torre
ElementType elemento = ElementType.Fuego;

// Aplicar elemento
bool exito = ElementalUpgradeSystem.Instance.AplicarElemento(torre, elemento);
```

### 3. Mejorar Torre Existente

```csharp
// Obtener mejoras disponibles
List<ElementalUpgrade> mejoras = ElementalUpgradeSystem.Instance.ObtenerMejorasDisponibles(torre);

// Aplicar mejora específica
if (mejoras.Count > 0)
{
    ElementalUpgrade mejora = mejoras[0];
    int costo = ElementalUpgradeSystem.Instance.CalcularCostoMejora(mejora);
    
    if (GameManager.Instance.Oro >= costo)
    {
        GameManager.Instance.GastarOro(costo);
        ElementalUpgradeSystem.Instance.AplicarMejora(torre, mejora);
    }
}
```

### 4. Implementar en Torre Existente

```csharp
public class Tower : MonoBehaviour, IElementalUpgradeable
{
    private ElementType? elementoActual;
    private int nivelElemental;
    
    // Implementar interfaz
    public ElementType? GetElementoActual() => elementoActual;
    public int GetNivelElemental() => nivelElemental;
    
    public bool AplicarElemento(ElementData elemento)
    {
        // Tu lógica aquí
    }
    
    public bool AplicarMejora(ElementalUpgrade mejora)
    {
        // Tu lógica aquí
    }
    
    public void RemoverElemento()
    {
        // Tu lógica aquí
    }
}
```

## Extensibilidad

### Agregar Nuevo Elemento

1. Añadir entrada en `ElementType` enum
2. Crear configuración en `ElementDatabase`
3. Implementar efectos específicos

### Agregar Nuevos Efectos

1. Añadir flag en `ElementalEffect` enum
2. Implementar lógica en `ElementalProjectile`
3. Configurar en las mejoras correspondientes

### Sistema de Fusión (Futuro)

El sistema está preparado para fusiones elementales:

```csharp
bool compatible = ElementalUpgradeSystem.Instance.SonElementosCompatibles(
    ElementType.Fuego, 
    ElementType.Rayo
);
// Resultado: true (Fuego + Rayo = Plasma?)
```

## Integración con UI

### Mostrar Información

```csharp
ElementalTower torre = GetComponent<ElementalTower>();
string info = torre.GetElementalInfo();
// Resultado: "Fuego Nv.2\nLlama Ardiente"
```

### Eventos

```csharp
// Suscribirse a eventos
ElementalUpgradeSystem.OnElementApplied += OnElementoAplicado;
ElementalUpgradeSystem.OnUpgradeApplied += OnMejoraAplicada;

void OnElementoAplicado(GameObject obj, ElementType tipo)
{
    // Actualizar UI
}
```

## Consideraciones de Balance

### Costos
- Base: 50 oro
- Multiplicadores por elemento (1.0x - 1.5x)
- Incremento por nivel

### Poder
- Nivel 1: +15% daño base
- Nivel 2: +30% daño, efectos adicionales
- Nivel 3: +50% daño, efectos masivos

## Próximos Pasos

1. **Sprint 2**: Implementar efectos elementales completos
2. **Sprint 3**: Sistema de fusión de elementos
3. **Sprint 4**: Elementos raros y legendarios
4. **Sprint 5**: Sinergias entre torres

## Notas para Desarrolladores

- El sistema usa ScriptableObjects para fácil configuración
- Los efectos visuales se aplican automáticamente
- Compatible con el sistema de pooling existente
- Preparado para guardado/carga de estados

## Ejemplos de Combinaciones

- **Fuego + Rayo** = Plasma (daño masivo en área)
- **Hielo + Sagrado** = Cristal Divino (congelación purificadora)
- **Veneno + Oscuro** = Plaga (propagación entre enemigos)

El sistema está diseñado para crecer con el juego y permitir estrategias complejas y emergentes.
