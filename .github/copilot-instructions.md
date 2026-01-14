# AI Coding Agent Instructions for Sharkadula

**Project:** 2D Top-Down RPG (Unity) | **Architecture:** Modular Component-Based + State Machine + Singleton Systems

## 🏗 Architecture Overview

This project uses **three core architectural patterns**:

### 1. **Component-Based Player Architecture**
The player is NOT a monolithic controller. Instead, [PlayerController.cs](../Assets/Scripts/Player/PlayerController.cs) orchestrates specialized components:
- `MovementComponent` - 4-directional movement
- `HealthComponent` - Health, damage, death
- `AttackComponent` - Combat logic
- `InteractionComponent` - World interaction
- `InventoryComponent` - Item management
- `PlayerVisualComponent` - Animations, sprites

All components inherit `IComponent` and implement `Initialize(PlayerController)`. This pattern separates concerns and makes systems testable.

### 2. **Hierarchical State Machine**
[StateMachine.cs](../Assets/Scripts/Systems/StateMachine/StateMachine.cs) is a generic, non-MonoBehaviour state manager:
- `StateMachine<T>` manages states for any entity (players, enemies, UI flow)
- States inherit `State<T>` and implement `Enter()`, `Update()`, `FixedUpdate()`, `Exit()`
- Player states: `LocomotionState`, `TakingDamageState`, `DeathState` (in [States/](../Assets/Scripts/Player/States/))
- **Critical:** State transitions happen via `Machine.SetState()`, never directly in Update loops

### 3. **Singleton Global Systems**
All managers inherit [Singleton.cs](../Assets/Scripts/Systems/Singleton.cs):
- `InputManager` - Event-based input handling + ReadInput<T>() queries
- `AudioManager` - GC-friendly pooling, Audio Mixer integration, unscaled UI audio support
- `GameManager` - Game-wide orchestration
- `SaveManager` - Persistence layer
- `ItemDataBase` - Central item registry

Singletons use `DontDestroyOnLoad()` and thread-safe lazy initialization.

## 🔄 Critical Data Flows

### Input → Action Flow
1. `InputManager.Subscribe(InputType.X, callback, phase)` registers listeners
2. Alternatively, read current state: `InputManager.Instance.ReadInput<Vector2>(InputType.Move)`
3. InputSystem_Actions.cs (auto-generated from .inputactions) wraps Unity Input System

### Audio System (Zero-GC Design)
1. Audio behavior lives in `AudioData` ScriptableObjects (data-driven)
2. `AudioManager.PlaySound(audioData)` pools `AudioSource` instances
3. Internal tracking via `ActiveSound` struct prevents allocation on Update loops
4. Separate mixer groups: Music, SFX, Ambience, UI (with unscaled volume for UI)

### Save/Load System
- Entities implement `ISavable` interface (see [PlayerController.cs](../Assets/Scripts/Player/PlayerController.cs) for pattern)
- Each savable entity has unique ID via `SaveableEntity` component
- SaveManager coordinates serialization

## 📋 Project Conventions

### File Organization
```
Scripts/
├── Player/          # PlayerController + all player components
├── Systems/         # Singletons, State Machine, Audio, Input, Save
├── UI/              # UI prefabs and scripts
├── Behaviour/       # Enemy, NPC, interactive object scripts
├── Data/            # ScriptableObjects definitions
└── ...
```

### Naming Patterns
- **Components:** `XyzComponent` (e.g., `HealthComponent`, `MovementComponent`)
- **States:** `XyzState` (e.g., `LocomotionState`)
- **Managers:** `XyzManager` inheriting `Singleton<XyzManager>`
- **Interfaces:** `IXyz` (e.g., `IComponent`, `IDamagable`, `IInteractable`)
- **Data classes:** `XyzData` (e.g., `ItemData`, `AudioData`)

### Component Initialization Pattern
```csharp
public class MyComponent : MonoBehaviour, IComponent {
    private PlayerController _controller;
    
    public void Initialize(PlayerController controller) {
        _controller = controller;
        // Setup references to other components via controller
    }
}
```

### State Pattern Usage
```csharp
public class MyState : State<PlayerController> {
    public override void Enter() { /* Setup */ }
    public override void Update() { /* Called each frame */ }
    public override void FixedUpdate() { /* Physics frame */ }
    public override void Exit() { /* Cleanup */ }
}
```

## 🛠 Key Build & Workflow Commands

- **Build:** Use Unity Editor build menu (project configured for 2D pipeline)
- **Scripts:** C# 7.3+ language version (check project settings)
- **Dependencies:** NaughtyAttributes (validation), InputSystem (input binding)
- **Assets:** Addressable Assets system partially set up (see AddressableAssetsData/)

## ⚠️ Critical Gotchas

1. **Audio Manager GC:** Uses struct pooling intentionally. Never allocate strings in `Update()` loops for audio logging.
2. **Input System:** Already enabled in InputManager.Awake(). Don't re-enable elsewhere.
3. **State Machine:** Always call `Machine.SetState()` from within states or specific callbacks—never from arbitrary Update methods.
4. **Singletons:** Already DontDestroyOnLoad(). Avoid re-instantiating managers in different scenes.
5. **Save System:** All savable entities must have unique IDs and ISavable implementation.

## 🔗 Integration Points to Remember

- **Player ↔ Health:** HealthComponent talks to OnDamage events, triggers TakingDamageState
- **Input ↔ Movement:** MovementComponent polls InputManager.ReadInput<Vector2>(InputType.Move)
- **Audio ↔ Actions:** Components call AudioManager.Instance.PlaySound() on events
- **World ↔ Interaction:** InteractionComponent uses IInteractable interface for chests, traps, NPCs
- **Inventory ↔ World:** Dropped items (DropObject.cs) interact with InventoryComponent

## 💡 When Adding New Features

1. **New player ability?** Create XyzComponent, add to PlayerController references, integrate in relevant state
2. **New enemy type?** Subclass or create parallel component system, use StateMachine for AI states
3. **New audio type?** Create AudioData ScriptableObject, call AudioManager.PlaySound()
4. **New save data?** Implement ISavable in target class, register with SaveManager
5. **New input binding?** Add to InputSystem_Actions via Input Debugger, map in InputManager.InitActionMap()
