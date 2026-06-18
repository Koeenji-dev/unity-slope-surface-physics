# 02 — Slope & Surface Physics

A focused 2D physics prototype built in Unity 6, demonstrating ground contact detection, surface normal reading, slope angle classification, walkable slope movement, steep-slope behavior, and surface-dependent movement modifiers.

> This is a **technical system prototype**, not a complete game.
> The goal is to build, understand, and demonstrate a reusable slope and surface physics controller for a 2D side-view game.

---

## Project Goal

Design and implement a reusable 2D physics controller that solves slope and surface interaction problems common in platform-style games.

Basic movement and jumping exist only as the minimum infrastructure needed to validate the slope system.

---

## Features

- Explicit ground contact detection using a downward `CapsuleCast`
- Surface normal and ground angle exposed per frame
- Configurable maximum walkable slope angle (default: 45°)
- Movement projected onto the surface tangent for stable slope traversal
- Stable uphill and downhill movement without jitter
- Steep-slope behavior: uphill locomotion is rejected; the player slides downhill under gravity
- Surface modifier system: per-collider components alter movement response without changing the physics body
- Surface types with preset values: Normal, Ice, Mud, Sticky, JumpBoost
- Ice surface: reduced acceleration, strongly reduced deceleration, retained momentum
- Inherited reusable player visual module (Idle, Run, Jump, Fall, facing)
- Inherited Currency system as a secondary integration demonstration
- Debug gizmo layer: contact sphere, normal arrow, tangent arrow, Scene View labels
- Assembly isolation: runtime physics code lives in `KoeenjiDev.SlopeSurfacePhysics`
- New Unity Input System only; no legacy Input Manager

---

## Controls

| Action | Keyboard | Gamepad |
|--------|----------|---------|
| Move left / right | A / D or arrow keys | Left stick |
| Jump | Space | Button South |

---

## Technical Highlights

- `CapsuleCast` ground query matches the player's physical footprint; avoids false positives against walls
- Ground contact state distinguishes `HasGroundContact`, `IsGroundWalkable`, and `IsGrounded` as separate concepts
- `GroundContactData2D` is an immutable readonly struct updated once per `FixedUpdate` — no per-frame heap allocation
- Surface tangent is derived analytically from the surface normal: `tangent = (normal.y, -normal.x)`; no runtime trigonometry
- Slope walkability is a single angle comparison against a serialized threshold
- Surface modifiers use a plain `MonoBehaviour` component on the ground collider; absent modifier defaults to Normal behavior
- `PlayerSlopeController2D` reads multipliers from `GroundContactData2D.SurfaceModifier` and applies them to the resolved acceleration, deceleration, and speed each fixed step
- Ice is implemented through explicit acceleration and deceleration multipliers, not through Physics Material friction
- Gravity is code-controlled (`Rigidbody2D.gravityScale = 0`) for predictable airborne behavior and clean slope adhesion
- Player physics material has friction 0 and bounciness 0; surface response is fully code-driven
- Visual state (`motionSpeed`) uses the scalar projection of velocity onto the slope tangent, so Run remains active on inclines
- `SlopeSurfaceDebugGizmos2D` is a pure debug overlay; it does not participate in gameplay

---

## Architecture Summary

```
PlayerInput
→ PlayerSlopeController2D
→ GroundDetector2D
  └── GroundContactData2D  (contact point, normal, tangent, angle, surface modifier)
→ SurfaceModifier2D        (movement multipliers read per frame from contact data)
→ Rigidbody2D              (receives final velocity each FixedUpdate)
→ PlayerVisualController2D (motion state forwarded after physics step)
```

Secondary independent flow:

```
CurrencyPickup
→ CurrencyCollector
→ CurrencyWallet
→ BalanceChanged event
→ CurrencyDisplay
```

---

## Project Structure

```
Assets/
├── _Project/
│   ├── Demo/
│   │   └── Runtime/           CoinVisualSpin (spin animation helper)
│   ├── Input/
│   │   └── PlayerControls.inputactions
│   ├── Player/
│   │   ├── Prefabs/           Player.prefab
│   │   └── Visual/            Inherited player visual module
│   ├── Scenes/
│   │   └── SlopeSurfacePhysics.unity
│   └── Systems/
│       ├── Currency/          Inherited currency module
│       │   ├── Prefabs/
│       │   ├── Runtime/
│       │   ├── Tests/
│       │   └── UI/
│       └── SlopeSurfacePhysics/
│           ├── Physics/       PlayerNoFriction physics material
│           └── Runtime/
│               ├── GroundContactData2D.cs
│               ├── GroundDetector2D.cs
│               ├── PlayerSlopeController2D.cs
│               ├── SlopeSurfaceDebugGizmos2D.cs
│               └── SurfaceModifier2D.cs
└── ThirdParty/
    ├── Kenney/
    │   └── PlatformerArtDeluxe/   CC0 tilesets (grass and tundra/ice)
    └── OzzbitGames/
        └── FantasyCharacter/      Player sprite sheets (licensed)
```

---

## How to Run

**Requirements:** Unity 6000.3.17f1, Universal 2D template, New Input System package active.

1. Clone or download the repository.
2. Open the project in Unity Hub using Unity version `6000.3.17f1`.
3. Open the scene: `Assets/_Project/Scenes/SlopeSurfacePhysics.unity`.
4. Press **Play**.
5. Use WASD or arrow keys to move and Space to jump.

---

## Testing

### Automated tests

The inherited Currency module includes 9 Edit Mode unit tests. Open the **Test Runner** window (`Window › General › Test Runner`) and run all tests in Edit Mode.

### Manual tests

Open `Documentation/TEST_PLAN.md` for the full manual test matrix covering flat ground, walkable slopes, ice surfaces, surface modifiers, debug visualization, and Currency integration.

---

## Known Issues

See `Documentation/KNOWN_ISSUES.md` for the current list of known issues, accepted limitations, and features intentionally excluded from this prototype.

---

## Future Improvements

The following are potential improvements for a future version, not planned for the current release:

- Automated Edit Mode tests for slope angle and tangent calculations
- Coyote time and jump buffer (intentionally excluded from this prototype)
- Variable jump height
- Moving platform support
- Camera follow script for larger levels
- Additional surface demonstration route with labelled angle markers
- Package extraction for reuse across other projects

---

## Version History

| Tag | Description |
|-----|-------------|
| `v0.5.0-review-debug-polish` | Debug gizmo layer, code review, and polish pass |
| `v0.4.0-surface-modifiers` | Surface modifier system with Normal, Ice, Mud, Sticky, JumpBoost |
| `v0.3.0-slope-movement` | Walkable slope movement, tangent projection, steep-slope behavior |
| `v0.2.0-ground-contact-data` | `GroundContactData2D` struct, extended ground contact information |
| `v0.1.0-basic-player-movement` | Horizontal movement, jump, gravity, visual integration |
| `v0.0.1-foundation` | Project setup, assemblies, input asset, inherited modules |

Full details in `Documentation/CHANGELOG.md`.

---

## Media

Screenshots and GIFs will be added before the v1.0.0 release.

See `Documentation/MEDIA_GUIDE.md` for capture instructions and naming conventions.

---

## Author

**Koeenji Dev** — Unity gameplay systems portfolio  
Repository: [unity-slope-surface-physics](https://github.com/Koeenji-dev/unity-slope-surface-physics)
