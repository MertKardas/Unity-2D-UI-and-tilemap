# Unity 2D Project - Memory Bank
**Project Scale Reference & Architecture Guide**

---

## Table of Contents
1. [Project Overview](#project-overview)
2. [Core Architecture](#core-architecture)
3. [Systems Documentation](#systems-documentation)
4. [Data Persistence & Memory](#data-persistence--memory)
5. [Scalability Guidelines](#scalability-guidelines)
6. [File Structure Reference](#file-structure-reference)
7. [Design Patterns](#design-patterns)
8. [Future Considerations](#future-considerations)

---

## Project Overview

### Current Scale
- **Type**: Unity 2D Action/Adventure Game
- **Scenes**: 2 (MainMenu, Episode01)
- **Player System**: Component-based architecture with hierarchical state machine
- **Inventory**: 5-slot system with stackable items support
- **Save System**: JSON-based multi-slot save/load with metadata
- **Items**: Expandable ScriptableObject-based item database

### Technology Stack
- **Engine**: Unity (2D)
- **Language**: C#
- **Serialization**: Newtonsoft.Json
- **Asset Loading**: Addressables
- **State Management**: Custom Hierarchical State Machine
- **Persistence**: JSON file-based save system

---

## Core Architecture

### Design Philosophy
The project follows a **modular, component-based architecture** designed for scalability:
- **Separation of Concerns**: Each system handles one responsibility
- **Singleton Managers**: Core systems accessible globally
- **Component Composition**: Player built from reusable components
- **Interface-Driven**: `ISavable`, `ICollectable`, `IInteractable` for polymorphism
- **Data-Driven Design**: ScriptableObjects for game data configuration

### System Hierarchy

```
GameManager (Singleton)
├── SaveManager (Singleton)
│   ├── SaveService (File I/O)
│   └── SceneSaveLoader (Auto save/load)
├── AudioManager (Singleton)
├── InputManager (Singleton)
└── ItemDataBase (Singleton)

PlayerController (MonoBehaviour + ISavable)
├── MovementComponent
├── HealthComponent
├── AttackComponent
├── InteractionComponent
├── InventoryComponent
├── VisualComponent
└── StateMachine
    ├── LocomotionState
    │   ├── IdleState
    │   └── MoveState
    ├── TakingDamageState
    └── DeathState
```

---

## Systems Documentation

### 1. Save System
**Location**: `/Assets/Scripts/Systems/SaveSystem/`

#### Components
- **SaveManager**: Central coordinator for all save operations
  - Manages current game state cache
  - Provides QuickSave/QuickLoad API
  - Handles save slot operations

- **SaveService**: Low-level file I/O handler
  - JSON serialization/deserialization
  - File path management: `Application.persistentDataPath/Saves/`
  - Creates save folders with two files:
    - `savedata.json` - Complete game state
    - `metadata.meta` - Save metadata (timestamp, scene, version)

- **SceneSaveLoader**: Automatic save/load integration
  - Auto-saves on scene unload (if not game-over)
  - Auto-loads on scene start
  - Finds and restores all `ISavable` entities
  - Test quick-save at 5 seconds after game start

#### Save Data Structure
```csharp
GameSaveData
├── SceneName: string
├── SaveName: string
└── DataDict: Dictionary<string, object>
    └── [EntityUniqueId]: PlayerSaveData
        ├── StateData (hierarchical state info)
        ├── position: float[2]
        ├── velocity: float[2]
        ├── MaxSpeed, acceleration, deceleration
        ├── health, maxHealth
        ├── coin
        └── items: List<InventoryItem>

MetaData
├── SaveName, DisplayName
├── SaveTime: DateTime
├── SceneName
└── GameVersion
```

#### Save/Load Flow
**Save**:
1. Scene unload detected → `SceneSaveLoader.CollectData()`
2. Find all `ISavable` objects in scene
3. Call `CaptureState()` on each entity
4. Store in `GameSaveData.DataDict[UniqueId]`
5. `SaveService` serializes to JSON with UTF-8 encoding
6. Write to persistent data path

**Load**:
1. User selects save slot or quick-loads
2. `SaveService` deserializes JSON files
3. Scene loads
4. `SceneSaveLoader` distributes data to entities
5. Each `ISavable.RestoreState(data)` called

#### Scalability
- **Multi-Entity Support**: Automatically handles all `ISavable` objects
- **Extensible Data**: Dictionary allows any serializable data
- **Multiple Save Slots**: Folder-based organization
- **Version Tracking**: GameVersion in metadata for migration support

---

### 2. Inventory System
**Location**: `/Assets/Scripts/Player/InventoryComponent.cs`

#### Features
- **Configurable Slot Count**: Currently 5 slots
- **Smart Stacking**:
  - Auto-stacks items up to `ItemData.StackSize`
  - Creates new stacks when full
  - Falls back to non-stackable behavior
- **Non-Stackable Items**: Each occupies full slot
- **Coin System**: Separate currency tracking
- **Event-Driven Updates**: Real-time UI synchronization

#### Item Data Model
```csharp
ItemData (ScriptableObject)
├── ID: string (unique identifier)
├── ItemName: string
├── Description: string
├── Icon: Sprite
├── Prefab: GameObject
├── IsStackable: bool
└── StackSize: int (max stack count)

InventoryItem (Serializable)
├── item: string (ItemData ID reference)
├── quantity: int
└── itemData: ItemData (resolved at runtime)
```

#### Item Management
- **ItemDataBase** (Singleton): Loads all items via Addressables
- Items defined as **ScriptableObjects** in `/Assets/ScriptableObjects/Items/`
- Runtime lookup by ID: `ItemDataBase.Instance.GetItem(id)`
- Label-based loading: All items tagged with "Item"

#### Events
```csharp
public event Action<int> OnCoinChanged;
public event Action<int> OnCoinChangedAmount;
public event Action<List<InventoryItem>> OnInventoryChanged;
```

#### Inventory UI
**Location**: `/Assets/Scripts/UI/InventoryUI/InventoryUI.cs`
- Dynamically creates UI slots matching inventory size
- Updates on `OnInventoryChanged` events
- Displays item icons and quantities
- Connected via PlayerController reference

#### Scalability
- **Expandable Slots**: Change `SlotCount` in inspector
- **Unlimited Item Types**: Add ScriptableObjects with "Item" label
- **Equipment Systems**: Extend `ItemData` with equipment properties
- **Crafting Ready**: Stack system supports material counting

---

### 3. Player System
**Location**: `/Assets/Scripts/Player/`

#### Component-Based Architecture
**PlayerController** acts as coordinator for modular components:

| Component | Responsibility | Key Features |
|-----------|----------------|--------------|
| **MovementComponent** | Character movement | Rigidbody2D physics, acceleration/deceleration |
| **HealthComponent** | HP management | Current/max health, damage/heal events |
| **AttackComponent** | Combat actions | Attack logic, cooldowns |
| **InteractionComponent** | World interaction | IInteractable detection, pickup logic |
| **InventoryComponent** | Item storage | See Inventory System above |
| **VisualComponent** | Sprite rendering | Animator, sprite flipping |

#### Hierarchical State Machine
**Location**: `/Assets/Scripts/Player/States/`

```
PlayerController.StateMachine
├── LocomotionState (SuperState)
│   ├── IdleState (default)
│   └── MoveState
├── TakingDamageState (interrupt state)
└── DeathState (final state)
```

**State Machine Features**:
- **Hierarchical States**: SuperStates contain nested states
- **State Persistence**: Full state tree saved/loaded
- **Dynamic Transitions**: States can change substates
- **Reusable Framework**: Generic `StateMachine<T>` in `/Assets/Scripts/Systems/StateMachine/`

#### State Persistence
```csharp
StateData
├── stateTypeName: string (full type name for reflection)
├── customData: Dictionary<string, object>
└── nestedStateData: StateData (recursive for hierarchies)
```

#### Scalability
- **Add Components**: Create new `IComponent` implementations
- **New States**: Inherit from `State` or `SuperState`
- **AI Reusability**: Same state machine for NPCs/enemies
- **Modular Abilities**: Each ability can be a component or state

---

### 4. Audio System
**Location**: `/Assets/Scripts/Systems/Audio System/`

#### Features
- **Singleton Pattern**: Globally accessible `AudioManager`
- **Data-Driven**: ScriptableObject-based audio data
- **Configurable**: Volume, pitch, looping settings per clip

#### Scalability
- **Audio Pools**: Extend for object pooling of AudioSources
- **Music Layers**: Add system for layered/adaptive music
- **3D Audio**: Spatial audio for world sounds

---

### 5. Input System
**Location**: `/Assets/Scripts/Systems/Input System/`

#### Features
- **Centralized Input**: Single point of input handling
- **Abstraction Layer**: Separates input detection from game logic
- **Event-Based**: Components subscribe to input events

#### Scalability
- **Rebinding**: Easy to add key/button remapping
- **Input Modes**: Add gamepad, touch, or keyboard variants
- **Action Maps**: Organize inputs by context (gameplay, UI, menu)

---

## Data Persistence & Memory

### ISavable Interface Pattern
**Location**: `/Assets/Scripts/Systems/SaveSystem/ISavable.cs`

```csharp
public interface ISavable {
    string UniqueId { get; }           // Persistent GUID
    object CaptureState();             // Serialize state
    void RestoreState(object data);    // Deserialize state
}
```

#### Implementation Guidelines
1. **Unique IDs**: Use `SaveableEntity` component to auto-generate GUIDs
2. **Serializable Data**: Return classes marked `[Serializable]`
3. **Runtime References**: Use `[JsonIgnore]` for non-serializable fields
4. **Restoration Logic**: Rebuild runtime state from saved data

#### Example: PlayerController Implementation
```csharp
public object CaptureState() {
    return new PlayerSaveData {
        StateData = StateMachine.CaptureState(),
        position = new float[] { transform.position.x, transform.position.y },
        velocity = new float[] { _rb.velocity.x, _rb.velocity.y },
        MaxSpeed = movementComponent.MaxSpeed,
        health = healthComponent.CurrentHealth,
        coin = inventoryComponent.Coin,
        items = inventoryComponent.Items
    };
}

public void RestoreState(object data) {
    var saveData = (PlayerSaveData)data;
    transform.position = new Vector3(saveData.position[0], saveData.position[1], 0);
    _rb.velocity = new Vector2(saveData.velocity[0], saveData.velocity[1]);
    healthComponent.CurrentHealth = saveData.health;
    inventoryComponent.Coin = saveData.coin;
    inventoryComponent.RestoreInventory(saveData.items);
    StateMachine.RestoreState(saveData.StateData);
}
```

### Memory Optimization Strategies
1. **Reference Caching**: Store frequently accessed references in Awake/Start
2. **Object Pooling**: Reuse GameObjects instead of Instantiate/Destroy
3. **Addressables**: Load assets on-demand, unload when not needed
4. **Sprite Atlases**: Combine textures to reduce draw calls
5. **Event Unsubscription**: Always unsubscribe in OnDestroy to prevent leaks

---

## Scalability Guidelines

### Adding New Saveable Entities
1. Implement `ISavable` interface
2. Add `SaveableEntity` component for unique ID
3. Define serializable data class
4. Implement `CaptureState()` and `RestoreState()`
5. System automatically includes in save/load

### Expanding Inventory
1. **More Slots**: Change `InventoryComponent.SlotCount`
2. **New Item Types**: Create ItemData ScriptableObject with "Item" label
3. **Equipment System**:
   - Extend ItemData with `ItemType` enum (Weapon, Armor, Consumable)
   - Create `EquipmentComponent` with equipment slots
   - Add stat modifiers to ItemData
4. **Crafting**:
   - Create `CraftingRecipe` ScriptableObject
   - Implement `CraftingSystem` to check/consume materials
   - Link to UI with recipe book

### Adding NPCs/Enemies
1. **Reuse Player Architecture**:
   - Same component pattern (Health, Movement, Attack)
   - Same state machine framework
   - Implement `ISavable` for persistence
2. **AI Controller**: Replace PlayerController input with AI logic
3. **Enemy Data**: ScriptableObject for stats, behaviors
4. **Spawn Management**: Spawn system with pooling

### Multi-Scene Architecture
1. **Scene Additive Loading**: Load multiple scenes for layers (UI, gameplay, background)
2. **Persistent Scene**: Core managers in DontDestroyOnLoad scene
3. **Scene Transitions**: SaveManager handles cross-scene saves
4. **Scene References**: Use scene names, not indices

### Performance Scaling
| Aspect | Current | Small Scale (10-20 entities) | Medium Scale (50-100) | Large Scale (200+) |
|--------|---------|------------------------------|------------------------|---------------------|
| **Save/Load** | JSON per-frame | JSON on demand | JSON + compression | Binary + streaming |
| **Item Lookup** | Dictionary O(1) | Same | Same | Same |
| **Entity Finding** | FindObjectsByType | Cached registry | Manager registry | ECS/Job System |
| **Rendering** | Unity default | Sprite atlases | Batching + culling | SRP Batching |
| **Physics** | Full 2D | Layer masks | Fixed timestep tuning | Spatial partitioning |

---

## File Structure Reference

### Core Directories
```
Assets/
├── Scenes/
│   ├── MainMenu.unity
│   └── Episode(01).unity
│
├── Scripts/
│   ├── Systems/                    # Singleton managers
│   │   ├── SaveSystem/
│   │   │   ├── SaveManager.cs
│   │   │   ├── SaveService.cs
│   │   │   ├── SceneSaveLoader.cs
│   │   │   ├── SaveableEntity.cs
│   │   │   ├── ISavable.cs
│   │   │   └── Models/
│   │   │       ├── GameSaveData.cs
│   │   │       ├── MetaData.cs
│   │   │       ├── PlayerSaveData.cs
│   │   │       └── StateData.cs
│   │   ├── Audio System/
│   │   │   ├── AudioManager.cs
│   │   │   └── AudioData.cs
│   │   ├── Input System/
│   │   │   └── InputManager.cs
│   │   ├── StateMachine/
│   │   │   ├── StateMachine.cs
│   │   │   ├── State.cs
│   │   │   └── SuperState.cs
│   │   ├── GameManager.cs
│   │   └── Singleton.cs
│   │
│   ├── Player/
│   │   ├── PlayerController.cs     # Main coordinator
│   │   ├── Components/
│   │   │   ├── AttackComponent.cs
│   │   │   ├── HealthComponent.cs
│   │   │   ├── MovementComponent.cs
│   │   │   ├── InteractionComponent.cs
│   │   │   ├── InventoryComponent.cs
│   │   │   └── VisualComponent.cs
│   │   └── States/
│   │       ├── LocomotionState.cs
│   │       ├── MoveState.cs
│   │       ├── IdleState.cs
│   │       ├── TakingDamageState.cs
│   │       └── DeathState.cs
│   │
│   ├── UI/
│   │   ├── InventoryUI/
│   │   │   ├── InventoryUI.cs
│   │   │   └── InventorySlot.cs
│   │   ├── Main Menu UI/
│   │   └── In Game Menu UI/
│   │
│   ├── Interfaces/
│   │   ├── ICollectable.cs
│   │   └── IInteractable.cs
│   │
│   └── Item system/
│       ├── ItemData.cs
│       └── ItemDataBase.cs
│
├── ScriptableObjects/
│   └── Items/
│       └── Key_01.asset            # Example item
│
├── Prefabs/                         # Reusable GameObjects
├── Resources/                       # Runtime loadable assets
└── Materials/
```

### Save File Structure
```
Application.persistentDataPath/Saves/
├── Save1/
│   ├── savedata.json               # GameSaveData
│   └── metadata.meta               # MetaData
├── Save2/
│   ├── savedata.json
│   └── metadata.meta
└── Save3/
    ├── savedata.json
    └── metadata.meta
```

---

## Design Patterns

### 1. Singleton Pattern
**Usage**: Manager classes (SaveManager, AudioManager, ItemDataBase)

**Implementation**:
```csharp
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    private static T _instance;
    public static T Instance {
        get {
            if (_instance == null) {
                _instance = FindAnyObjectByType<T>();
                if (_instance == null) {
                    GameObject go = new GameObject(typeof(T).Name);
                    _instance = go.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
}
```

**Pros**: Global access, single instance guarantee
**Cons**: Tight coupling, testing difficulty
**Best For**: Manager systems, global state

---

### 2. Component Pattern
**Usage**: Player system (MovementComponent, HealthComponent, etc.)

**Benefits**:
- **Modularity**: Each component handles one concern
- **Reusability**: Share components across entities
- **Testability**: Test components in isolation
- **Flexibility**: Add/remove features dynamically

**Example**:
```csharp
public class PlayerController : MonoBehaviour {
    public MovementComponent movementComponent;
    public HealthComponent healthComponent;
    public InventoryComponent inventoryComponent;

    void Awake() {
        // Components work together but remain independent
    }
}
```

---

### 3. State Pattern
**Usage**: Player state machine, hierarchical states

**Benefits**:
- **Clear Logic**: Each state encapsulates its behavior
- **Easy Transitions**: Centralized state change logic
- **Hierarchical**: SuperStates contain substates
- **Saveable**: State tree can be serialized

**Structure**:
```csharp
public abstract class State {
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void LogicUpdate() { }
    public virtual void PhysicsUpdate() { }
}

public abstract class SuperState : State {
    protected State currentSubState;
    protected StateMachine subStateMachine;
}
```

---

### 4. Observer Pattern
**Usage**: Event system (OnInventoryChanged, OnCoinChanged)

**Benefits**:
- **Decoupling**: Publishers don't know subscribers
- **Flexibility**: Add/remove listeners dynamically
- **Reactivity**: UI updates automatically

**Example**:
```csharp
// Publisher
public event Action<List<InventoryItem>> OnInventoryChanged;
OnInventoryChanged?.Invoke(Items);

// Subscriber
inventoryComponent.OnInventoryChanged += UpdateUI;
```

---

### 5. Factory Pattern (Implicit)
**Usage**: ItemDataBase loading items, instantiating prefabs

**Future Expansion**:
```csharp
public class EntityFactory {
    public Enemy CreateEnemy(EnemyType type) {
        var data = Resources.Load<EnemyData>($"Enemies/{type}");
        var prefab = Instantiate(data.Prefab);
        return prefab.GetComponent<Enemy>();
    }
}
```

---

## Future Considerations

### Roadmap Alignment
Based on project README future plans:

#### 1. Design Levels
**Architecture Support**:
- ✅ Scene-based save/load ready
- ✅ Modular player system works in any scene
- ⚠️ **Need**: Tilemap generation system, level data format

**Recommendations**:
- Create `LevelData` ScriptableObject for level configurations
- Implement tilemap serialization for custom levels
- Add `LevelManager` for level progression tracking

---

#### 2. More Complex Inventory System
**Current Foundation**:
- ✅ Stackable items
- ✅ ScriptableObject-based items
- ✅ UI integration

**Missing Features**:
- ❌ Equipment slots (weapon, armor, accessories)
- ❌ Item categories/filtering
- ❌ Crafting system
- ❌ Item durability/stats
- ❌ Drag-and-drop UI

**Implementation Path**:
1. Extend `ItemData` with `ItemType` enum and stat modifiers
2. Create `EquipmentComponent` with typed slots
3. Implement `CraftingRecipe` and `CraftingSystem`
4. Update UI for drag-and-drop and equipment display

---

#### 3. NPC and Battle System
**Architecture Readiness**:
- ✅ Component system reusable for NPCs/enemies
- ✅ State machine framework ready
- ✅ Health/Attack components exist
- ⚠️ **Need**: AI controller, combat stats, damage calculation

**Implementation Path**:
1. **NPC System**:
   - Create `NPCController` using same component pattern
   - Implement `DialogueComponent` and `QuestComponent`
   - Add `DialogueSystem` for conversations

2. **Battle System**:
   - Extend `AttackComponent` with combat stats (attack, defense)
   - Create `CombatManager` for turn-based or real-time combat
   - Implement damage calculation with stats/resistances
   - Add `EnemyAI` states (Patrol, Chase, Attack, Retreat)

3. **Enemy Management**:
   - Create `EnemyData` ScriptableObject
   - Implement `EnemySpawner` with wave system
   - Add object pooling for performance

---

### Technical Debt Prevention

#### 1. Testing Strategy
**Current State**: No automated tests
**Recommendation**:
- Add Unity Test Framework
- Unit test SaveService serialization
- Integration test save/load flow
- Play mode tests for combat/inventory

#### 2. Code Documentation
**Current State**: Minimal XML comments
**Recommendation**:
- XML comments for public APIs
- Architecture diagrams in `/Documentation/`
- Component interaction flowcharts

#### 3. Version Control
**Current State**: Git repository active
**Recommendation**:
- Use Git LFS for large assets
- Implement branching strategy (feature/bugfix branches)
- Add .gitattributes for Unity scene merging

#### 4. Performance Profiling
**Recommendation**:
- Profile save/load with 100+ entities
- Test inventory with 1000+ items
- Benchmark state machine with nested hierarchies

---

### Architectural Evolution Path

#### Phase 1: Current (MVP)
- Single player, basic inventory, scene saves
- **Entity Count**: 1 player
- **Save System**: JSON per scene

#### Phase 2: Enhanced Gameplay (Next)
- NPCs, enemies, combat, crafting
- **Entity Count**: 10-20 entities
- **Save System**: Compressed JSON, entity registry

#### Phase 3: Full Game
- Multiple levels, complex quests, equipment
- **Entity Count**: 50-100 entities
- **Save System**: Binary + streaming, incremental saves

#### Phase 4: Optimization (If Needed)
- Mobile port, large-scale battles
- **Entity Count**: 200+ entities
- **Save System**: ECS/Job System, cloud saves

---

## Key Metrics & Limits

### Current Capacities
| Resource | Current | Max Tested | Recommended Limit |
|----------|---------|------------|-------------------|
| **Inventory Slots** | 5 | - | 50 (UI performance) |
| **Save Slots** | Unlimited | 3 | 10 (disk space) |
| **Items in Database** | 1 | - | 500 (memory) |
| **Saveable Entities/Scene** | 1 | - | 100 (JSON size) |
| **State Hierarchy Depth** | 2 | 2 | 5 (recursion) |

### Performance Targets
- **Save Time**: < 100ms for 50 entities
- **Load Time**: < 500ms for scene restore
- **UI Update**: < 16ms per frame (60 FPS)
- **Item Lookup**: O(1) dictionary access

---

## Quick Reference

### Common Tasks

#### Add New Item
1. Create ItemData ScriptableObject in `/Assets/ScriptableObjects/Items/`
2. Set ID, name, description, icon, prefab
3. Mark as stackable and set stack size if applicable
4. Tag asset with "Item" label
5. ItemDataBase auto-loads on game start

#### Add New Player State
1. Create class inheriting `State` or `SuperState` in `/Assets/Scripts/Player/States/`
2. Override `Enter()`, `Exit()`, `LogicUpdate()`, `PhysicsUpdate()` as needed
3. Add transition logic in parent state or PlayerController
4. Test state transitions and persistence

#### Create Save Slot
1. Call `SaveManager.Instance.Save("SlotName", currentGameSaveData)`
2. System creates folder, serializes data, writes metadata
3. Load via `SaveManager.Instance.Load("SlotName")`

#### Add Saveable Entity
1. Implement `ISavable` on MonoBehaviour
2. Add `SaveableEntity` component to GameObject
3. Define serializable data class
4. Implement `CaptureState()` → return data
5. Implement `RestoreState(data)` → rebuild state

---

## Conclusion

This Memory Bank serves as the **single source of truth** for the Unity 2D project's architecture at scale. It documents:

✅ **Current Systems**: All implemented features and their interactions
✅ **Data Flow**: How data persists and moves through the game
✅ **Scalability**: How to expand each system as the project grows
✅ **Best Practices**: Patterns and guidelines for consistent development

**Maintenance**: Update this document when:
- New systems are added
- Architecture patterns change
- Performance limits are discovered
- Future roadmap evolves

**Contact/Issues**: Track architectural decisions and improvements in version control commit messages and project documentation.

---

**Last Updated**: 2026-01-17
**Project Version**: Pre-Alpha
**Document Version**: 1.0
