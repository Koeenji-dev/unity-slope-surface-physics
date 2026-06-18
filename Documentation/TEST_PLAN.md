# Test Plan — 02 Slope & Surface Physics

**Version:** v0.5.0 (Phase 02.5 — Review, Debug & Polish)  
**Last updated:** 2026-06-18  
**Tester:** Koeenji  
**Unity version:** 6000.3.17f1

---

## How to Use

1. Open the scene `Assets/_Project/Scenes/SlopeSurfacePhysics.unity`.
2. Press **Play**.
3. Execute each test case below.
4. Fill in **Observed Result** and set **Status** to `pass`, `fail`, or `skip`.
5. Add notes about any unexpected behavior.

**Status values:**

| Value | Meaning |
|-------|---------|
| `pending` | Not yet executed |
| `pass` | Observed result matches expected result |
| `fail` | Observed result does not match expected result |
| `skip` | Intentionally skipped with justification |

---

## Test Cases

### T-01 — Compile Without Errors

| Field | Value |
|-------|-------|
| **Setup** | Open the project in Unity 6000.3.17f1. |
| **Steps** | 1. Wait for Unity to finish importing and compiling. 2. Check the Console window. |
| **Expected Result** | 0 compile errors. Assembly `KoeenjiDev.SlopeSurfacePhysics` resolves successfully. |
| **Observed Result** | Pending manual validation |
| **Status** | `pending` |
| **Notes** | — |

---

### T-02 — Console Without New Errors

| Field | Value |
|-------|-------|
| **Setup** | Scene `SlopeSurfacePhysics.unity` open. |
| **Steps** | 1. Enter Play Mode. 2. Do not press any input. 3. Wait 5 seconds. 4. Check the Console. |
| **Expected Result** | 0 errors. 0 new warnings introduced by the system scripts. |
| **Observed Result** | Pending manual validation |
| **Status** | `pending` |
| **Notes** | Pre-existing Unity warnings unrelated to this project are acceptable. |

---

### T-03 — Base Movement on Normal Surface

| Field | Value |
|-------|-------|
| **Setup** | Player standing on a flat Normal surface. |
| **Steps** | 1. Press and hold right arrow. 2. Observe Player accelerates to maximum speed. 3. Release input. 4. Observe Player decelerates to a stop. |
| **Expected Result** | Smooth acceleration to max speed. Clean deceleration to zero. Run animation active while moving. Idle animation when stopped. |
| **Observed Result** | Pending manual validation |
| **Status** | `pending` |
| **Notes** | — |

---

### T-04 — Jump on Normal Surface

| Field | Value |
|-------|-------|
| **Setup** | Player standing on a flat Normal surface. |
| **Steps** | 1. Press Space. 2. Observe Player leaves the ground and rises. 3. Observe Player falls and lands cleanly. |
| **Expected Result** | Jump animation on ascent. Fall animation on descent. Clean landing with Idle or Run animation restoring correctly. No double-jump. |
| **Observed Result** | Pending manual validation |
| **Status** | `pending` |
| **Notes** | — |

---

### T-05 — Walkable Slope — Uphill and Downhill

| Field | Value |
|-------|-------|
| **Setup** | A walkable slope (angle ≤ 45°) exists in the scene. |
| **Steps** | 1. Walk uphill. Observe Player climbs smoothly. 2. Walk downhill. Observe Player descends smoothly. 3. Stop on the slope and release input. |
| **Expected Result** | Player stays on slope without floating or sinking. Run animation active while moving. No visible jitter. Player stops on slope without sliding away. |
| **Observed Result** | Pending manual validation |
| **Status** | `pending` |
| **Notes** | Test with at least one slope between 20° and 45°. |

---

### T-06 — Transition Flat to Slope

| Field | Value |
|-------|-------|
| **Setup** | Flat Normal surface adjacent to a walkable slope. |
| **Steps** | 1. Walk from flat ground toward the slope. 2. Observe the transition onto the slope. |
| **Expected Result** | No gap, float, or bounce at the transition point. Player moves seamlessly from flat to slope. `IsGrounded` remains true throughout. |
| **Observed Result** | Pending manual validation |
| **Status** | `pending` |
| **Notes** | — |

---

### T-07 — Transition Slope to Flat

| Field | Value |
|-------|-------|
| **Setup** | A walkable slope adjacent to flat Normal ground. |
| **Steps** | 1. Walk from the slope onto flat ground. 2. Observe the transition. |
| **Expected Result** | No bounce or airborne flash at the transition. `IsGrounded` remains true. Movement continues smoothly on flat ground. |
| **Observed Result** | Pending manual validation |
| **Status** | `pending` |
| **Notes** | — |

---

### T-08 — Ice Surface Sliding

| Field | Value |
|-------|-------|
| **Setup** | A flat or sloped surface with `SurfaceModifier2D` set to `Ice`. |
| **Steps** | 1. Walk onto the Ice surface. 2. Press right to accelerate. 3. Release input. Observe deceleration. 4. Reverse direction. Observe reversal speed. |
| **Expected Result** | Noticeably slower acceleration. Noticeably slower deceleration (momentum retained for longer). Slower direction reversal than on Normal surface. `SlopeSurfaceDebugGizmos2D` shows `Surface: Ice`. |
| **Observed Result** | Pending manual validation |
| **Status** | `pending` |
| **Notes** | Ice does not rely on Physics Material friction. |

---

### T-09 — Mud Slowdown

| Field | Value |
|-------|-------|
| **Setup** | A surface with `SurfaceModifier2D` set to `Mud`. |
| **Steps** | 1. Walk onto the Mud surface. 2. Press right. Observe reduced max speed. 3. Release input. Observe deceleration. |
| **Expected Result** | Noticeably lower maximum speed (~60% of Normal). Slightly slower acceleration. `SlopeSurfaceDebugGizmos2D` shows `Surface: Mud`. |
| **Observed Result** | Pending manual validation |
| **Status** | `pending` |
| **Notes** | — |

---

### T-10 — Sticky Strong Deceleration

| Field | Value |
|-------|-------|
| **Setup** | A surface with `SurfaceModifier2D` set to `Sticky`. |
| **Steps** | 1. Accelerate on Normal ground. 2. Walk onto Sticky surface. 3. Release input. Observe deceleration. |
| **Expected Result** | Player decelerates noticeably faster on Sticky than on Normal (deceleration ×1.80). `SlopeSurfaceDebugGizmos2D` shows `Surface: Sticky`. |
| **Observed Result** | Pending manual validation |
| **Status** | `pending` |
| **Notes** | — |

---

### T-11 — JumpBoost Only When Jump Is Pressed

| Field | Value |
|-------|-------|
| **Setup** | A surface with `SurfaceModifier2D` set to `JumpBoost`. |
| **Steps** | 1. Walk onto the JumpBoost surface. 2. Observe horizontal movement (no change expected). 3. Press Space. Observe jump height. |
| **Expected Result** | No automatic bounce when walking over the surface. Jump height is noticeably higher than Normal (×2.00 jump force). `SlopeSurfaceDebugGizmos2D` shows `Surface: JumpBoost`. |
| **Observed Result** | Pending manual validation |
| **Status** | `pending` |
| **Notes** | The multiplier is applied only when the jump input is pressed. |

---

### T-12 — No Automatic Trampoline Behavior

| Field | Value |
|-------|-------|
| **Setup** | A surface with `SurfaceModifier2D` set to `JumpBoost`. |
| **Steps** | 1. Walk onto the surface. 2. Do not press Space. 3. Stand still on the surface for several seconds. |
| **Expected Result** | Player does not bounce or receive vertical force without pressing Jump. Standing and walking on JumpBoost surface behaves like Normal ground for horizontal movement. |
| **Observed Result** | Pending manual validation |
| **Status** | `pending` |
| **Notes** | This confirms JumpBoost is input-driven and not collision-driven. |

---

### T-13 — Fallback Surface With No Modifier

| Field | Value |
|-------|-------|
| **Setup** | A ground collider with no `SurfaceModifier2D` component attached (plain Normal geometry). |
| **Steps** | 1. Walk onto plain ground. 2. Observe movement behavior. 3. Observe `SlopeSurfaceDebugGizmos2D` label. |
| **Expected Result** | Player moves with standard acceleration, deceleration, and max speed (multipliers default to 1.0). `SlopeSurfaceDebugGizmos2D` shows `Surface: Normal`. No errors in Console. |
| **Observed Result** | Pending manual validation |
| **Status** | `pending` |
| **Notes** | `GroundContactData2D.HasSurfaceModifier` is false. All multipliers read as 1.0f. |

---

### T-14 — Debug Normal / Tangent / Slope Angle Visibility

| Field | Value |
|-------|-------|
| **Setup** | `SlopeSurfaceDebugGizmos2D` attached to the Player. Scene View open. Player selected. |
| **Steps** | 1. Enter Play Mode with the Scene View visible. 2. Walk the Player onto a slope. 3. Observe the Scene View. |
| **Expected Result** | A blue arrow shows the ground normal. A red arrow shows the ground tangent. A text label shows slope angle in degrees, surface type, and grounded state. Contact sphere is green when grounded, yellow when contact exists but not grounded. In Edit Mode no geometric gizmos are shown. |
| **Observed Result** | Pending manual validation |
| **Status** | `pending` |
| **Notes** | Label uses `UnityEditor.Handles.Label`. No runtime UI is displayed in Game View. |

---

## Acceptance Criteria Reference

| AC | Description | Covered By |
|----|-------------|------------|
| AC-06 | Ground contact detected explicitly | T-03, T-05, T-13 |
| AC-07 | Ground normal exposed | T-14 |
| AC-08 | Ground angle exposed | T-14 |
| AC-09 | Flat ground behaves correctly | T-03, T-13 |
| AC-10 | Walkable slopes follow surface tangent | T-05 |
| AC-11 | Maximum walkable angle consistent | T-05 |
| AC-16 | Simple jump works | T-04, T-11 |
| AC-18 | Normal surfaces accelerate/decelerate correctly | T-03 |
| AC-19 | Ice preserves momentum | T-08 |
| AC-20 | Ice affects slope behavior | T-08 |
| AC-29 | No Missing Scripts | T-01 |
| AC-30 | No Missing References | T-01 |
| AC-31 | Console at 0 errors | T-02 |
