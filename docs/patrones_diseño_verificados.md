# Verificación de Patrones de Diseño - Tower Defense

Este documento confirma la implementación de los siguientes patrones de diseño en el proyecto:

## 1. Patrón Singleton

El patrón Singleton se ha implementado correctamente para los managers globales del juego:

### GameManager (Assets/Scripts/Managers/GameManager.cs)
```csharp
public static GameManager Instance { get; private set; }

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
        return;
    }
}
```
- **Propósito**: Gestiona el estado global del juego, fases, economía y eventos principales.
- **Características**: 
  - Instancia única accesible globalmente
  - Persiste entre escenas con `DontDestroyOnLoad`

### WaveManager (Assets/Scripts/Managers/WaveManager.cs)
```csharp
public static WaveManager Instance { get; private set; }

void Awake()
{
    if (Instance == null) Instance = this;
    else Destroy(gameObject);
}
```
- **Propósito**: Gestiona las oleadas de enemigos y su spawn.

### AudioManager (Assets/Scripts/Managers/AudioManager.cs)
```csharp
public static AudioManager Instance { get; private set; }
```
- **Propósito**: Centraliza la reproducción de efectos de sonido.

### Otros Singletons implementados:
- **UIManager**: Gestión de la interfaz de usuario
- **TowerManager**: Gestión de torres disponibles
- **CoreHealth**: Salud del núcleo
- **PathManager**: Gestión de waypoints

## 2. Patrón Strategy

El patrón Strategy se ha implementado para dos propósitos principales:

### Selección de Objetivos de Torres (Assets/Scripts/ITargetSelectionStrategy.cs)
```csharp
public interface ITargetSelectionStrategy
{
    Enemy SeleccionarObjetivo(Tower torre, List<Enemy> enemigosEnRango);
}

// Estrategias concretas:
public class SeleccionarMasCercano : ITargetSelectionStrategy 
{
    // Selecciona el enemigo más cercano
}

public class SeleccionarMasDebil : ITargetSelectionStrategy 
{
    // Selecciona el enemigo con menor salud
}
```

### Movimiento de Enemigos (Assets/Scripts/IMovementStrategy.cs)
```csharp
public interface IMovementStrategy
{
    void Mover(Enemy enemigo);
}
```

**Uso en Tower.cs:**
```csharp
public ITargetSelectionStrategy estrategiaObjetivo = new SeleccionarMasCercano();
objetivoActual = estrategiaObjetivo.SeleccionarObjetivo(this, enemigosEnRango);
```

**Uso en Enemy.cs:**
```csharp
public IMovementStrategy estrategiaMovimiento;

void Update()
{
    if (estrategiaMovimiento != null)
        estrategiaMovimiento.Mover(this);
    else
        MoverPorDefecto();
}
```

## 3. Patrón Observer

El patrón Observer se implementa extensivamente mediante eventos de C# para notificar cambios de estado:

### GameManager - Eventos de Estado
```csharp
// Notificación de cambios en el oro
public static event Action<int> OnOroCambiado;

// Notificación de cambios de fase
public static event Action<bool> OnFaseConstruccionChanged;
```

### WaveManager - Eventos de Oleada
```csharp
public static event Action<int> OnMinionsRemainingChanged;
public static event Action<int> OnEnemigosVivosChanged;
public static event Action<int> OnRondaCambiada;
```

### CoreHealth - Eventos de Vida
```csharp
public static event Action<int> OnVidaNucleoCambiada;
```

### Ejemplo de Suscripción (UIManager.cs)
```csharp
void OnEnable()
{
    GameManager.OnOroCambiado += UpdateOro;
    CoreHealth.OnVidaNucleoCambiada += UpdateVida;
    WaveManager.OnMinionsRemainingChanged += UpdateMinions;
    WaveManager.OnRondaCambiada += UpdateRonda;
    WaveManager.OnEnemigosVivosChanged += UpdateVivos;
}

void OnDisable()
{
    // Desuscripción para evitar memory leaks
    GameManager.OnOroCambiado -= UpdateOro;
    // ... etc
}
```

## 4. Patrón Object Pooling

El patrón Object Pooling se implementa para la gestión eficiente de proyectiles:

### BulletPool (Assets/Scripts/Player/BulletPool.cs)
```csharp
public class BulletPool : MonoBehaviour
{
    private List<GameObject> bullets = new List<GameObject>();

    void Start()
    {
        // Pre-instancia objetos al inicio
        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(bulletPrefab, bulletsParent);
            obj.SetActive(false);
            bullets.Add(obj);
        }
    }

    public GameObject GetBullet(int givenDmg)
    {
        // Busca bala inactiva para reutilizar
        foreach (var bullet in bullets)
        {
            if (!bullet.activeInHierarchy)
            {
                var bp = bullet.GetComponent<BulletPlayer>();
                bp.dmg = givenDmg;
                return bullet;
            }
        }

        // Si no hay disponibles, crea una nueva
        var newBullet = Instantiate(bulletPrefab, bulletsParent);
        bullets.Add(newBullet);
        return newBullet;
    }
}
```

### Uso en Tower.cs:
```csharp
void DispararA(Enemy enemigo)
{
    // Obtiene bala del pool en lugar de instanciar
    GameObject projObj = bulletPool.GetBullet(daño);
    
    // Configura y activa
    projObj.transform.position = firePoint.position;
    projObj.GetComponent<BulletPlayer>().SetTarget(enemigo.transform);
    projObj.SetActive(true);
}
```

### Comportamiento del Proyectil:
```csharp
// En BulletPlayer.cs
void OnTriggerEnter(Collider col)
{
    // Al impactar, se desactiva (no se destruye)
    gameObject.SetActive(false);
}
```

## Conclusiones

Los cuatro patrones de diseño solicitados están correctamente implementados:

1. **Singleton**: Garantiza instancias únicas de managers globales con acceso centralizado.
2. **Strategy**: Permite cambiar algoritmos de selección de objetivos y movimiento en tiempo de ejecución.
3. **Observer**: Desacopla la comunicación entre componentes mediante eventos.
4. **Object Pooling**: Optimiza el rendimiento reutilizando proyectiles en lugar de crear/destruir constantemente.

Esta arquitectura proporciona:
- **Modularidad**: Los componentes están desacoplados
- **Extensibilidad**: Fácil agregar nuevas estrategias o suscriptores
- **Rendimiento**: Optimización mediante pooling
- **Mantenibilidad**: Código organizado y responsabilidades bien definidas
