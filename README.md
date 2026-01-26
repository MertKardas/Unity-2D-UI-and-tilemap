#  Sharkadula – 2D Top-Down RPG

A **2D Top-Down RPG** project developed in **Unity**, built upon principles of **modular architecture**, **clean code**, and **high reusability**. The game features **4-directional movement mechanics** set within a **dark, atmospheric Vampire/Dungeon world**.

[![Demo](intro/intro.png)](https://www.youtube.com/watch?v=s4vGGgpIS2c)

## 🎯 Quick Summary

A complete dungeon-crawler foundation featuring:

| Category | Features |
|----------|----------|
| **Core** | Game state management, Save/Load system, Input handling |
| **Player** | Modular components (Movement, Health, Inventory, Interaction) |
| **Audio** | Pooled audio system with mixer groups, fade effects, volume persistence |
| **Gameplay** | Item collection, locked doors, treasure chests, spike traps |
| **AI** | Enemy pathfinding with Unity Behavior graphs |
| **UI** | Pause menu, settings, inventory display, death/end screens |
| **Persistence** | Full JSON-based save system with multiple slots |

**Tech Stack:** Unity 2D • New Input System • LeanTween • Addressables • Newtonsoft.Json • Unity Behavior

## 📐 Design Patterns Used

* **Singleton** – Central systems (`GameManager`, `AudioManager`, `InputManager`, `SaveManager`, `ItemDataBase`) with global access and single source of truth.

* **State Machine** – Hierarchical state machine for player locomotion with parent/child states (Idle, Move, Run, TakingDamage, Death).

* **Component Pattern** – Player logic split into focused components via `IComponent` interface.

* **Observer Pattern** – Events throughout (`OnHealthChanged`, `OnInventoryChanged`, `OnGamePaused`, etc.).

* **Object Pool** – Zero-allocation AudioSource pooling in AudioManager.

* **Interface Segregation** – `ISavable`, `IInteractable`, `ICollectable`, `IDamageable` for clean contracts.

---

## 🧩 Modular Player Controller

Instead of relying on a monolithic controller script, the player logic is divided into **small, focused components**:

| Component | Responsibility |
|-----------|----------------|
| `MovementComponent` | Physics-based movement with acceleration/deceleration, walk & run speeds |
| `HealthComponent` | Health tracking, damage events, death triggers |
| `AttackComponent` | Attack input handling |
| `InteractionComponent` | Detects nearby interactables, triggers interactions |
| `InventoryComponent` | 5-slot inventory, stackable items, coin system |
| `PlayerVisualComponent` | Animator control, 4-direction sprites, footstep sounds |

---

## ⚙️ Core Systems

### 🎮 Game Manager

* Manages game states: `Playing`, `Paused`, `Gameover`, `EndGame`
* Time scale manipulation for pause
* Events: `OnGamePaused`, `OnGameStarted`, `OnGameEnded`, `OnGameover`
* Quick Save / Quick Load integration

---

### 🔊 Audio Manager

* Zero-allocation, GC-friendly **Object Pooling** (20-64 AudioSources)
* Integrated with **Unity Audio Mixer**
* Grouped audio control: Master, SFX, Music, Ambience, UI
* **Pause/Resume** per audio type
* **Fade methods**: `FadeOutSound`, `FadeOutMusic`, `CrossfadeMusic`
* UI sounds continue during game pause (`ignoreListenerPause`)
* Volume persistence via PlayerPrefs
* `isPlaying`-based auto-release with safety buffer

---

### 🎹 Input Manager

* Wrapper around Unity's new Input System
* Input types: Move, Attack, Interact, Run, Cancel
* Subscribe/Unsubscribe pattern for callbacks
* Enable/disable player or UI input independently

```csharp
InputManager.Instance.Subscribe(InputType.Interact, OnInteract);
Vector2 moveDir = InputManager.Instance.ReadInput<Vector2>(InputType.Move);
```

---

### 💾 Save System

Complete persistent save/load architecture:

| Class | Role |
|-------|------|
| `SaveManager` | Singleton coordinating all save operations, quick save/load |
| `SaveService` | JSON serialization via Newtonsoft.Json, file I/O |
| `SaveableEntity` | Assigns unique IDs to saveable objects |
| `ISavable` | Interface with `CaptureState()` / `RestoreState()` |
| `GameSaveData` | Main container with per-scene data dictionary |
| `MetaData` | Save slot info (timestamps, display names) |

**Saveable objects:** Player (position, health, inventory, state), Doors, Chests, Traps, Enemies, Drop Items

---

### 📦 Item & Inventory System

* **ItemData** – ScriptableObject defining item properties (ID, name, icon, stackable, stack size)
* **ItemDataBase** – Singleton loading items via Addressables with "Item" label
* **InventoryComponent** – 5-slot inventory with coin system
* Events: `OnCoinChanged`, `OnInventoryChanged`

```csharp
inventory.AddItem(itemData, quantity);
inventory.SpendMoney(cost);
```

---

### 🚪 Interaction System

* **IInteractable** interface with `CanInteract`, `Interact()`, `SetInRange()`
* **InteractableBase** – Abstract base managing range state

| Interactable | Behavior |
|--------------|----------|
| `Door` | Locked/unlocked, requires key item, audio feedback, saves state |
| `Chest` | Gives coins, open animation, one-time use, saves state |
| `DropObject` | Collectible items, plays sound, disables on pickup |

---

### 👾 Enemy System

* **EnemyController** – Uses Unity Behavior package for AI
* NavMeshAgent-based pathfinding
* Configurable via **EnemyData** ScriptableObject (FollowRange, AttackRange, MaxHealth)
* Saveable (position + health)

---

### ⚡ Hazards

* **Trap** – Spike traps with timed activation cycle
* Damages `IDamageable` objects on collision
* Saves activation state

---

## 🖥️ UI Systems

### In-Game UI

| Panel | Features |
|-------|----------|
| `GameUI` | Main controller, panel transitions with LeanTween |
| `StatsPanelUI` | Health, coins display |
| `PauseMenu` | Resume, Settings, Main Menu, Quit |
| `SettingsMenu` | Volume sliders with persistence |
| `InventoryUI` | 5-slot display with real-time updates |
| `DeathMenuUI` | Restart, Main Menu options |
| `EndGameUI` | Fade to black on completion |

### Main Menu UI

* `UIManager` – Menu flow management
* `SavePanelUI` – Save slot selection, load/delete saves

---

## 🎞 Animation System

* Uses **Blend Trees** for smooth **4-directional movement**
* Smart diagonal handling (prioritizes horizontal/vertical)
* Animation parameters: InputX, InputY, Move, MoveSpeed, TakeDamage, Death
* Highly scalable for future states

---

## 🗺 Environment & Lighting

* **Layered Tilemap Architecture** – Easy sorting and collision
* **2D Lighting System** – Dynamic lighting for dark atmosphere

---

## 🔁 Player State Machine

Hierarchical state machine structure:

```
LocomotionState (Parent)
├── IdleState
├── MoveState
└── RunState

TakingDamageState
DeathState
```

```csharp
public override void CheckTransitions() {
    base.CheckTransitions();
    if (_input != Vector2.zero) {
        var state = _machine.GetState<MoveState>();
        _machine.SetState(state);
    }
}
```

---

## 🛠 Libraries & Tools

| Library | Usage |
|---------|-------|
| **LeanTween** | UI animations, fade effects, delayed calls |
| **NaughtyAttributes** | Inspector enhancements, [Scene] attribute |
| **Newtonsoft.Json** | Save system serialization |
| **Addressables** | Item database loading |
| **Unity Behavior** | Enemy AI decision graphs |
| **TextMesh Pro** | UI text rendering |

---

## 🎨 Used Assets

* **Free Vampire 4-Direction Pixel Character Sprite Pack**
* **Dungeon Asset Pack**

---

## Future Plans

* Design more levels
* Combat system expansion
* NPC dialogue system
