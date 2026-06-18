# Architecture — 02 Slope & Surface Physics

**Project:** 02 — Slope & Surface Physics  
**Unity version:** 6000.3.17f1  
**Assembly:** `KoeenjiDev.SlopeSurfacePhysics`

---

## Purpose

This document describes the runtime architecture of the slope and surface physics system.

The system is designed to:

- be proportional to the problem it solves;
- maintain one clear responsibility per component;
- use explicit serialized references instead of scene searches;
- separate reusable physics code from demo-only code;
- remain understandable by a developer new to the codebase.

---

## High-Level Flow

```
PlayerInput (Unity component)
  |
  | OnMove / OnJump callbacks
  v
PlayerSlopeController2D
  |
  | calls UpdateGroundState() each FixedUpdate
  v
GroundDetector2D
  |
  | CapsuleCast → produces
  v
GroundContactData2D  ←  SurfaceModifier2D (from detected collider)
  |
  | walkability, tangent, angle, modifiers
  v
PlayerSlopeController2D  (resolves velocity)
  |
  | linearVelocity =
  v
Rigidbody2D
  |
  | after velocity is applied
  v
PlayerVisualController2D  (SetMotionState, SetFacingDirection)
```

The secondary Currency flow is fully independent and does not interact with the physics path.

---

## Main Runtime Modules

### PlayerSlopeController2D

**File:** `Assets/_Project/Systems/SlopeSurfacePhysics/Runtime/PlayerSlopeController2D.cs`  
**Namespace:** `KoeenjiDev.SlopeSurfacePhysics`  
**Type:** `MonoBehaviour` (sealed)

Owns all movement decisions. Receives input callbacks from the `PlayerInput` component, queries the `GroundDetector2D`, calculates the final velocity for the current fixed step, writes it to `Rigidbody2D.linearVelocity`, and forwards the resulting motion state to `PlayerVisualController2D`.

**Serialized references:** `Rigidbody2D body`, `GroundDetector2D groundDetector`, `PlayerVisualController2D playerVisual`

**Configuration groups:** Ground Movement, Jump, Gravity

**Input entry points:**

```csharp
public void OnMove(InputAction.CallbackContext context)
public void OnJump(InputAction.CallbackContext context)
```

**Must not:** execute physics casts directly, own Currency state, modify Animator parameters, search scene objects.

---

### GroundDetector2D

**File:** `Assets/_Project/Systems/SlopeSurfacePhysics/Runtime/GroundDetector2D.cs`  
**Namespace:** `KoeenjiDev.SlopeSurfacePhysics`  
**Type:** `MonoBehaviour` (sealed)

Performs a short downward `CapsuleCast` each time `UpdateGroundState()` is called. Reports contact information through a `GroundContactData2D` snapshot and three boolean state properties.

**Serialized references:** `Rigidbody2D body`, `CapsuleCollider2D playerCollider`, `Transform groundProbe`, `LayerMask groundLayers`

**Configuration:** `groundCheckDistance`, `maxWalkableSlopeAngle`, `groundedNormalVelocityTolerance`

**Read-only outputs:**

| Property | Type | Meaning |
|----------|------|---------|
| `HasGroundContact` | `bool` | A collider was detected below the Player |
| `IsGroundWalkable` | `bool` | The detected angle is within the walkable limit |
| `IsGrounded` | `bool` | Contact is walkable and the Player is not moving away from it |
| `MaxWalkableSlopeAngle` | `float` | Serialized walkable limit in degrees |
| `ContactData` | `GroundContactData2D` | Full snapshot from the most recent cast |
| `ContactPoint` | `Vector2` | Pass-through from `ContactData` |
| `ContactNormal` | `Vector2` | Pass-through from `ContactData` |
| `GroundTangent` | `Vector2` | Pass-through from `ContactData` |
| `GroundAngle` | `float` | Pass-through from `ContactData` |
| `GroundCollider` | `Collider2D` | Pass-through from `ContactData` |

**Must not:** read input, apply velocity, modify Currency, update visuals.

---

### GroundContactData2D

**File:** `Assets/_Project/Systems/SlopeSurfacePhysics/Runtime/GroundContactData2D.cs`  
**Namespace:** `KoeenjiDev.SlopeSurfacePhysics`  
**Type:** `readonly struct`

Immutable snapshot of one ground contact detection result. Produced by `GroundDetector2D.UpdateGroundState()` and consumed by `PlayerSlopeController2D` during the same fixed step.

No heap allocation occurs because this is a value type updated by assignment.

The surface tangent is derived analytically at construction time:

```csharp
Tangent = new Vector2(normal.y, -normal.x);
```

`GroundContactData2D.None` is a safe no-contact sentinel value.

**Key fields:**

| Field | Type | Description |
|-------|------|-------------|
| `HasHit` | `bool` | True when a ground collider was detected |
| `Point` | `Vector2` | World-space contact point |
| `Normal` | `Vector2` | World-space surface normal |
| `Tangent` | `Vector2` | Rightward surface tangent, derived from normal |
| `Angle` | `float` | Degrees between normal and `Vector2.up` |
| `Distance` | `float` | Cast distance to contact |
| `Collider` | `Collider2D` | Detected collider |
| `Rigidbody` | `Rigidbody2D` | Rigidbody on the detected collider, if any |
| `SurfaceModifier` | `SurfaceModifier2D` | Found via `GetComponentInParent`; null means Normal |
| `HasSurfaceModifier` | `bool` | True when a modifier was found |

---

### SurfaceModifier2D

**File:** `Assets/_Project/Systems/SlopeSurfacePhysics/Runtime/SurfaceModifier2D.cs`  
**Namespace:** `KoeenjiDev.SlopeSurfacePhysics`  
**Type:** `MonoBehaviour` (sealed, `DisallowMultipleComponent`)

Placed on a ground `GameObject` to override movement behavior for the Player standing on that collider. All multipliers are read-only from the outside. Absent modifier on a collider is treated as Normal (all multipliers = 1.0).

**Surface types:**

| Type | Speed × | Accel × | Decel × | Jump × | Behavior |
|------|---------|---------|---------|--------|----------|
| Normal | 1.00 | 1.00 | 1.00 | 1.00 | Baseline |
| Ice | 1.05 | 0.35 | 0.15 | 1.00 | Slow accel, very slow decel, retained momentum |
| Mud | 0.60 | 0.55 | 0.80 | 0.85 | Reduced top speed and acceleration |
| Sticky | 0.75 | 0.70 | 1.80 | 0.70 | Strong deceleration, reduced jump |
| JumpBoost | 1.00 | 1.00 | 1.00 | 2.00 | Doubled jump force when input is pressed |

A context menu item **Apply Preset Values** sets the preset multipliers for the selected type.

**Must not:** move the Player, read input, execute physics casts, own global state, depend on `PlayerVisualController2D`.

---

### SlopeSurfaceDebugGizmos2D

**File:** `Assets/_Project/Systems/SlopeSurfacePhysics/Runtime/SlopeSurfaceDebugGizmos2D.cs`  
**Namespace:** `KoeenjiDev.SlopeSurfacePhysics`  
**Type:** `MonoBehaviour` (sealed, `RequireComponent(typeof(GroundDetector2D))`)

Pure debug overlay. Reads from `GroundDetector2D` in `OnDrawGizmosSelected` and draws:

- A sphere at the contact point (green when grounded, yellow when contact exists but not grounded)
- A blue arrow for the surface normal
- A red arrow for the surface tangent
- A Scene View label with grounded state, walkability, slope angle, surface type, and multipliers

Geometric gizmos are only drawn at runtime with an active contact. The label is only drawn in Play Mode using `UnityEditor.Handles.Label`. Nothing from this component appears in the Game View.

**Must not:** affect gameplay, write to physics state, affect build output.

---

## Player Movement

`PlayerSlopeController2D.FixedUpdate` runs the following sequence each physics step:

1. Call `groundDetector.UpdateGroundState()`.
2. Read `body.linearVelocity`.
3. Determine if jump was requested and ground state allows it.
4. Branch on `groundDetector.IsGrounded && !jumped`:
   - **Grounded:** call `ApplyGroundMovement` — projects movement onto slope tangent, reads surface multipliers, applies `MoveTowards` acceleration or deceleration.
   - **Not grounded (airborne or steep contact):** call `ApplyAirMovement` — applies horizontal control at air rate, applies gravity each step, clamps fall speed.
5. Assign `body.linearVelocity`.
6. Consume jump request.
7. Call `UpdateVisualState`.

On steep slopes (angle > `maxWalkableSlopeAngle`), `IsGrounded` is false. The player enters air movement, gravity pulls them downward, and the zero-friction physics material allows them to slide along the surface under natural physics simulation.

---

## Ground Detection

The `CapsuleCast` origin is `playerCollider.bounds.center`. The cast size matches the collider's world-space dimensions, accounting for lossy scale. Cast direction is `Vector2.down`. Cast distance is `groundCheckDistance` beyond the capsule base.

Because the cast uses the capsule shape, angled walls produce normals that point mostly sideways. `Vector2.Angle(normal, Vector2.up)` for a near-vertical wall returns approximately 90°, which exceeds the walkable limit and is correctly rejected as ground.

`IsGrounded` adds a velocity check: the component of `Rigidbody2D.linearVelocity` along the contact normal must not exceed `groundedNormalVelocityTolerance`. This prevents the grounded state from persisting during the first frames of a jump.

---

## Slope Handling

Given a detected surface normal **n**:

- **Slope angle:** `Vector2.Angle(n, Vector2.up)` — angle in degrees from vertical
- **Walkability:** `angle <= maxWalkableSlopeAngle`
- **Tangent:** `(n.y, −n.x)` — always points in the positive-X direction

`PlayerSlopeController2D.GetGroundMovementDirection()` ensures the tangent's X component is always positive, so a positive `movementSpeed` scalar always moves the player to the right and a negative one to the left, regardless of which side of a slope they are on.

The `motionSpeed` forwarded to the visual module is `|scalar projection of velocity onto tangent|`, not `|velocity.x|`. This keeps the Run animation active while moving on inclines.

---

## Surface Modifiers

When `GroundDetector2D` executes the `CapsuleCast`, it calls `GetComponentInParent<SurfaceModifier2D>()` on the detected collider. The result is stored in `GroundContactData2D.SurfaceModifier`.

`PlayerSlopeController2D.ApplyGroundMovement` reads three multipliers from this reference (or uses 1.0 defaults if null) and applies them to the base speed, acceleration, and deceleration values before resolving the velocity for that step.

`ApplyAirMovement` reads `JumpForceMultiplier` at the moment the jump executes, allowing surfaces like JumpBoost to affect jump height.

---

## Debug and Polish Layer

`SlopeSurfaceDebugGizmos2D` is attached alongside `GroundDetector2D` on the Player root. It has no `Update`, no event subscriptions, and no gameplay code. It is driven entirely by `OnDrawGizmosSelected`.

The dependency direction is:

```
SlopeSurfaceDebugGizmos2D → GroundDetector2D
```

No runtime physics component depends on the debug layer.

---

## Input System

Input asset: `Assets/_Project/Input/PlayerControls.inputactions`  
Action map: `Player`

| Action | Type | Bindings |
|--------|------|----------|
| `Move` | Value / Vector2 | WASD + Arrow Keys + Gamepad Left Stick |
| `Jump` | Button | Space + Gamepad Button South |

`PlayerInput` component behavior: **Invoke Unity Events**  
Callbacks wired in the Inspector: `Move → OnMove`, `Jump → OnJump`

No Control Schemes. No generated C# class. Input is read through callback contexts only.

---

## Scene Dependencies

Scene: `Assets/_Project/Scenes/SlopeSurfacePhysics.unity`

The Player prefab (`Assets/_Project/Player/Prefabs/Player.prefab`) contains:

```
Player (root)
├── Rigidbody2D               Dynamic, Gravity Scale 0, Continuous, Interpolate, Freeze Rotation Z
├── CapsuleCollider2D          Vertical, Size (0.6, 1.4), PlayerNoFriction material
├── PlayerInput                PlayerControls asset, Player map, Invoke Unity Events
├── PlayerSlopeController2D    serialized refs: body, groundDetector, playerVisual
├── GroundDetector2D           serialized refs: body, playerCollider, groundProbe, groundLayers
├── SlopeSurfaceDebugGizmos2D  requires GroundDetector2D
├── CurrencyWallet
├── CurrencyCollector          ref: CurrencyWallet on same GameObject
├── GroundProbe (child)        local Y −0.75, no components
└── Visual (child)
    └── PlayerVisual (nested prefab)
        └── Renderer
```

All serialized references are set in the prefab. No `FindObjectOfType` or `GameObject.Find` is used anywhere in the project.

---

## Extensibility Notes

**Adding a new surface type:**

1. Add a new `SurfaceType2D` enum value.
2. Add a preset case in `SurfaceModifier2D.ApplyPresetValues`.
3. Place a `SurfaceModifier2D` component on the desired ground collider and set its type.
4. No changes to `PlayerSlopeController2D` or `GroundDetector2D` are required.

**Changing the maximum walkable slope:**

Adjust `maxWalkableSlopeAngle` on the `GroundDetector2D` component in the Inspector. The same value is exposed as a public property for any code that needs to read it.

**Extending ground contact data:**

Add fields to `GroundContactData2D` and update the constructor. All consumers receive the updated struct by value.

---

## What This Project Intentionally Does Not Include

The following are excluded by design to keep the project focused on its stated technical scope:

- Dash, stamina, wall interaction, climbing, ladders
- Coyote time, jump buffering, variable jump height, double jump
- Moving or disappearing platforms
- Enemies, health, damage, combat
- Checkpoints, save and load, inventory
- ScriptableObject-based surface database
- Complex Tilemap environments
- Cinemachine
- Procedural terrain generation
- Multiplayer or networking
- Generic player state machine framework
