# Changelog — 02 Slope & Surface Physics

All notable changes to this project are documented here.

Format: each entry lists what was added, changed, or fixed relative to the previous version.

---

## [v0.5.0-review-debug-polish] — 2026-06-18

**Commit:** `8a9e0af`

### Added

- `SlopeSurfaceDebugGizmos2D` component: Editor-only gizmo overlay that draws the surface normal arrow, surface tangent arrow, and contact sphere in the Scene View when the Player is selected during Play Mode.
- Scene View label on the Player showing grounded state, walkability, slope angle in degrees, surface type, and all active multipliers when a `SurfaceModifier2D` is detected.
- Contact sphere color coding: green when `IsGrounded` is true, yellow when `HasGroundContact` is true but `IsGrounded` is false.
- `[AIRBORNE]` label when no ground contact exists.

### Changed

- `GroundDetector2D` internal gizmo moved to `SlopeSurfaceDebugGizmos2D` for a cleaner separation between physics behavior and debug display.
- Code review pass: redundant comments removed, naming made consistent, `OnValidate` guards verified.

### Fixed

- No gameplay behavior changes in this release.

---

## [v0.4.0-surface-modifiers] — 2026-06-18

**Commit:** `d3646cd`

### Added

- `SurfaceType2D` enum with five types: `Normal`, `Ice`, `Mud`, `Sticky`, `JumpBoost`.
- `SurfaceModifier2D` MonoBehaviour: placed on ground colliders to override movement multipliers. Exposes `MaxSpeedMultiplier`, `AccelerationMultiplier`, `DecelerationMultiplier`, and `JumpForceMultiplier`.
- Context menu command **Apply Preset Values** on `SurfaceModifier2D` applies recommended defaults for the selected surface type.
- `GroundContactData2D` extended with `SurfaceModifier` field: the `GroundDetector2D` resolves the modifier via `GetComponentInParent<SurfaceModifier2D>()` on the detected collider.
- `GroundContactData2D.HasSurfaceModifier` convenience property.
- `PlayerSlopeController2D` reads multipliers from `ContactData.SurfaceModifier` each fixed step; absent modifier defaults to 1.0 for all multipliers.
- `JumpForceMultiplier` applied to jump speed at the moment the jump executes.

### Changed

- `GroundContactData2D` constructor updated to accept a `SurfaceModifier2D` parameter.
- `GroundDetector2D.UpdateGroundState` updated to perform `GetComponentInParent` lookup on the hit collider.

### Surface preset reference

| Type | Speed × | Accel × | Decel × | Jump × |
|------|---------|---------|---------|--------|
| Normal | 1.00 | 1.00 | 1.00 | 1.00 |
| Ice | 1.05 | 0.35 | 0.15 | 1.00 |
| Mud | 0.60 | 0.55 | 0.80 | 0.85 |
| Sticky | 0.75 | 0.70 | 1.80 | 0.70 |
| JumpBoost | 1.00 | 1.00 | 1.00 | 2.00 |

---

## [v0.3.0-slope-movement] — 2026-06-18

**Commit:** `2b74f75`

### Added

- Slope angle calculation: `Vector2.Angle(hit.normal, Vector2.up)` computed in `GroundDetector2D.UpdateGroundState`.
- Walkability classification: `GroundAngle <= maxWalkableSlopeAngle` (default 45°), exposed as `IsGroundWalkable`.
- Surface tangent derived analytically in `GroundContactData2D`: `Tangent = new Vector2(normal.y, -normal.x)`.
- `PlayerSlopeController2D.ApplyGroundMovement`: when `IsGrounded` is true, movement is projected onto the surface tangent rather than applied as raw horizontal velocity. The tangent is oriented so positive scalar always drives rightward travel.
- `movementSpeed` is the scalar projection of velocity onto the tangent, forwarded to `PlayerVisualController2D` so Run stays active on inclines.
- Uphill speed multiplier: a configurable fraction reduces maximum speed when the movement direction has a positive Y component.
- Steep-slope behavior: when `HasGroundContact` is true but `IsGrounded` is false (angle exceeds limit), `ApplyAirMovement` is called. Gravity pulls the player downward; the zero-friction physics material allows sliding along the surface.
- `MaxWalkableSlopeAngle` public property exposed on `GroundDetector2D`.
- `GroundTangent` convenience property on `GroundDetector2D`.

### Changed

- `PlayerSlopeController2D` movement branch now dispatches based on `IsGrounded` rather than raw contact presence.
- `PlayerSlopeController2D.GetGroundMovementDirection` ensures the tangent X component is always positive.

---

## [v0.2.0-ground-contact-data] — 2026-06-18

**Commit:** `a42f81d`

### Added

- `GroundContactData2D` readonly struct: immutable snapshot produced once per `UpdateGroundState` call. Fields: `HasHit`, `Point`, `Normal`, `Tangent`, `Angle`, `Distance`, `Collider`, `Rigidbody`, `SurfaceModifier`.
- `GroundContactData2D.None` sentinel value for the no-contact state.
- `GroundDetector2D` extended with `ContactData` property.
- Convenience pass-through properties on `GroundDetector2D`: `ContactPoint`, `ContactNormal`, `GroundAngle`, `GroundDistance`, `GroundCollider`, `GroundRigidbody`.
- `groundedNormalVelocityTolerance` configuration field on `GroundDetector2D`: prevents `IsGrounded` from persisting during the first frames of a jump by checking the velocity component along the contact normal.
- `PlayerSlopeController2D` updated to query `ContactData` instead of individual properties.

### Changed

- `GroundDetector2D.UpdateGroundState` now constructs a `GroundContactData2D` from the `RaycastHit2D` result.
- Initial `GroundDetector2D` gizmos added for cast direction, contact point, normal, and tangent (later moved to `SlopeSurfaceDebugGizmos2D` in v0.5.0).

---

## [v0.1.0-basic-player-movement] — 2026-06-18

**Commit:** `ead3baa`

### Added

- `PlayerSlopeController2D` MonoBehaviour: receives `OnMove` and `OnJump` callbacks from `PlayerInput`.
- Horizontal acceleration and deceleration using `Mathf.MoveTowards` each fixed step.
- Code-controlled gravity: `Rigidbody2D.gravityScale` is 0; gravity is applied explicitly as `velocity.y -= gravityAcceleration * Time.fixedDeltaTime`.
- Fall speed clamped to `maximumFallSpeed`.
- Simple global vertical jump: assigns `jumpSpeed` to `velocity.y` when grounded and jump was requested.
- Reduced air control: same acceleration and deceleration rates apply in the air; no separate air-only values in this version.
- `GroundDetector2D` MonoBehaviour (initial version): downward `CapsuleCast` using the `CapsuleCollider2D` world dimensions, filtered by `groundLayers` LayerMask. Exposes `HasGroundContact`, `IsGrounded`, and basic contact properties.
- `PlayerVisualController2D` integration: `SetMotionState(|movementSpeed|, velocity.y, IsGrounded)` and `SetFacingDirection(horizontalInput)` called after each fixed step.
- `PlayerInput` events wired in the Inspector: `Move → OnMove`, `Jump → OnJump`.

### Configuration defaults

| Parameter | Value |
|-----------|-------|
| `maximumSpeed` | 7 u/s |
| `groundAcceleration` | 45 u/s² |
| `groundDeceleration` | 55 u/s² |
| `jumpSpeed` | 12 u/s |
| `gravityAcceleration` | 35 u/s² |
| `maximumFallSpeed` | 20 u/s |

---

## [v0.0.1-foundation] — 2026-06-17

**Commit:** `36ceb45`

### Added

- Unity project created with Unity `6000.3.17f1`, Universal 2D template, New Input System active.
- Runtime Assembly Definition `KoeenjiDev.SlopeSurfacePhysics` with reference to `KoeenjiDev.PlayerVisual`.
- Input Actions asset: `Assets/_Project/Input/PlayerControls.inputactions` — Action Map `Player`, actions `Move` (Value/Vector2) and `Jump` (Button). Bindings: WASD + Gamepad Left Stick for Move; Space + Gamepad Button South for Jump.
- Inherited `KoeenjiDev.PlayerVisual` module (controlled copy preserving `.meta` files).
- Inherited `KoeenjiDev.CurrencySystem` module (controlled copy preserving `.meta` files).
- Kenney Platformer Art Deluxe subset: 13 grass and 14 tundra/ice CC0 sprites.
- Physics Material 2D `PlayerNoFriction`: Friction 0, Bounciness 0.
- Player prefab: `Rigidbody2D` Dynamic (Gravity Scale 0, Continuous, Interpolate, Freeze Rotation Z), vertical `CapsuleCollider2D` (0.6 × 1.4), `PlayerInput`, `CurrencyWallet`, `CurrencyCollector`.
- Layers: Player (6), Ground (7).
- Base scene `SlopeSurfacePhysics.unity` with canonical hierarchy.
- `GroundProbe` child transform at local Y −0.75.
- `PlayerVisual` nested prefab inside `Visual` child.
- `.gitignore` (Unity template) and local exclusions configured.
- Git repository initialized and connected to remote.
