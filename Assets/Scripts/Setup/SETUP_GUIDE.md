# Guía de Configuración de la Escena

## Configuración Automática (Recomendada)

1. **Crear un GameObject vacío** en la escena
2. **Agregar el componente** `SceneSetupHelper`
3. En el Inspector:
   - Click en "1. Crear Materiales de Preview"
   - Click en "2. Configurar Escena Completa"

## Configuración Manual

### 1. Arreglar el Error del Cursor

En tu textura del cursor:
1. Selecciona la textura "Icono (2)" en el Project
2. En el Inspector:
   - Texture Type: **Cursor**
   - Alpha Is Transparency: **✓**
   - Read/Write Enabled: **✓**
   - Generate Mip Maps: **✗**
   - Max Size: **32** o **64**
3. Click Apply

### 2. Crear Layer para Zonas de Construcción

1. Edit > Project Settings > Tags and Layers
2. En Layers, agregar: **ConstructionZone**
3. Asignar este layer a todas las zonas de construcción

### 3. Configurar el Player

En el GameObject Player:
1. Buscar el componente **PlayerShooting**
2. Asignar:
   - **BulletPool**: Arrastra el BulletPool de la escena
   - **FirePoint**: Busca en Player/CameraHolder/FPSCamera/FirePoint
   - **Animator**: El Animator del Player
   - **Sfx**: AudioSource para disparos

### 4. Configurar Audio Listeners

Solo debe haber UN AudioListener activo:
1. En **CameraFPS**: AudioListener activo
2. En **CameraIso**: AudioListener desactivado
3. El AudioListenerManager los manejará automáticamente

### 5. Crear TowerManager

1. Crear GameObject vacío llamado "TowerManager"
2. Agregar componente `TowerManager`
3. Configurar las torres en el array

### 6. Configurar TowerPlacementController

1. Crear GameObject vacío llamado "TowerPlacementController"
2. Agregar componente `TowerPlacementController`
3. Configurar:
   - Construction Zone Layer: ConstructionZone
   - Max Raycast Distance: 100

### 7. Crear Prefabs de Proyectiles Secundarios

Para cada tipo de disparo secundario, crear un prefab con:

#### Proyectil de Ralentización
- GameObject con Rigidbody
- Agregar script `SlowProjectile`
- Agregar efectos visuales (partículas azules)

#### Proyectil de Stun
- GameObject con Rigidbody
- Agregar script `StunProjectile`
- Agregar efectos visuales (partículas amarillas)

#### Proyectil de Área
- GameObject con Rigidbody
- Agregar script `AreaProjectile`
- Agregar efectos visuales (explosión)

#### Proyectil Penetrante
- GameObject con Rigidbody
- Agregar script `PiercingProjectile`
- Agregar TrailRenderer

### 8. Configurar SecondaryFireSystem

En el componente SecondaryFireSystem del Player:
1. Asignar los prefabs de proyectiles creados
2. Configurar cooldown (5 segundos por defecto)
3. Asignar efectos de carga y disparo

## Verificación

Ejecuta el juego y verifica:
- ✓ No hay errores de AudioListener
- ✓ El cursor funciona correctamente
- ✓ PlayerShooting puede disparar
- ✓ Click derecho activa disparo secundario
- ✓ Teclas 1-4 cambian tipo de disparo
- ✓ Las torres muestran preview al seleccionar

## Solución de Problemas

### "NullReferenceException en PlayerShooting"
- Verifica que BulletPool esté asignado
- Verifica que FirePoint esté asignado

### "SecondaryFireSystem: No se asignó firePoint!"
- El script intentará encontrarlo automáticamente
- Si falla, asígnalo manualmente en el Inspector

### "2 audio listeners in the scene"
- El AudioListenerManager debería manejarlo
- Si persiste, desactiva manualmente uno de ellos
