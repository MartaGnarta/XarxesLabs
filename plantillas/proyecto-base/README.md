# Proyecto Base Unity - Networking

Este es un proyecto Unity preconfigurado con los elementos básicos para desarrollo de aplicaciones de red.

## 🔧 Configuración Incluida

### Unity Packages
- Unity Netcode for GameObjects
- Unity Transport Package
- Unity Services Core
- Input System (New)

### Estructura del Proyecto
```
Assets/
├── Scenes/
│   ├── MainMenu.unity        # Menú principal
│   ├── Lobby.unity           # Sala de espera
│   └── Game.unity            # Escena de juego
├── Scripts/
│   ├── Network/              # Scripts de networking
│   │   ├── NetworkManager/   # Gestor de red
│   │   ├── Player/          # Scripts del jugador
│   │   └── Utils/           # Utilidades de red
│   ├── UI/                  # Scripts de interfaz
│   └── Game/                # Lógica de juego
├── Prefabs/
│   ├── NetworkPlayer.prefab  # Prefab del jugador en red
│   └── UI/                  # Prefabs de UI
└── Materials/               # Materiales básicos
```

## 🎮 Funcionalidades Base

### Network Manager
- Configuración básica de cliente/servidor
- Gestión de conexiones
- Spawn de jugadores
- Callbacks de conexión/desconexión

### Player Controller
- Movimiento básico sincronizado
- Input handling
- Network variables básicas
- RPCs de ejemplo

### UI Sistema
- Menú de conexión
- Lobby básico
- HUD en juego
- Gestión de estados

## 🚀 Cómo Usar Este Template

### 1. Crear Nuevo Proyecto
```bash
# Copia toda la carpeta del template
cp -r proyecto-base/ tu-nueva-practica/
cd tu-nueva-practica/
```

### 2. Configurar Unity
1. Abre Unity Hub
2. Añade el proyecto desde la carpeta copiada
3. Abre el proyecto
4. Verifica que todos los packages estén instalados

### 3. Personalizar
1. Renombra el proyecto en Project Settings
2. Modifica los scripts según tus necesidades
3. Añade tu lógica específica
4. Actualiza las escenas según el enunciado

## 📝 Scripts Importantes

### `NetworkManagerScript.cs`
Gestiona las conexiones y el estado de la red.

```csharp
public class NetworkManagerScript : NetworkBehaviour
{
    // Configuración básica de red
    // Callbacks de conexión
    // Gestión de jugadores
}
```

### `NetworkPlayer.cs`
Controlador base del jugador en red.

```csharp
public class NetworkPlayer : NetworkBehaviour
{
    // Variables de red básicas
    // Control de movimiento
    // RPCs de ejemplo
}
```

### `UIManager.cs`
Gestiona la interfaz de usuario.

```csharp
public class UIManager : MonoBehaviour
{
    // Estados de UI
    // Conexión/desconexión
    // Feedback al usuario
}
```

## 🔧 Configuración de Red

### Network Manager Component
- **Player Prefab**: NetworkPlayer
- **Connection Approval**: Habilitado
- **Max Connections**: 10 (configurable)
- **Transport**: Unity Transport

### Input System
- Configurado para usar el New Input System
- Mapas de input básicos incluidos
- Soporte para múltiples dispositivos

## 📋 Lista de Tareas Típicas

### Para Cada Nueva Práctica
- [ ] Revisar objetivos de aprendizaje
- [ ] Adaptar scripts base según necesidades
- [ ] Modificar UI según requerimientos
- [ ] Añadir lógica específica del ejercicio
- [ ] Configurar testing apropiado
- [ ] Actualizar documentación

### Testing Básico
- [ ] Test de conexión local
- [ ] Test con múltiples clientes
- [ ] Verificar sincronización
- [ ] Probar reconexión
- [ ] Validar UI

## 🎯 Extensiones Comunes

### Para Diferentes Tipos de Prácticas

#### Chat/Messaging
- Añadir `ChatManager.cs`
- UI de chat en overlay
- Serialización de mensajes

#### Juegos de Movimiento
- Physics networking
- Collision detection
- Smooth movement interpolation

#### Turn-Based Games
- Turn manager
- State synchronization
- Timer systems

#### Real-Time Games
- High-frequency updates
- Prediction systems
- Lag compensation

## 📚 Referencias Útiles

- [Unity Netcode Documentation](https://docs-multiplayer.unity3d.com/)
- [Unity Transport Documentation](https://docs.unity3d.com/Packages/com.unity.transport@latest)
- [Input System Guide](https://docs.unity3d.com/Packages/com.unity.inputsystem@latest)

---

*Template actualizado para Unity 2022.3 LTS*