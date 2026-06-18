# Test Plan — 02 Slope & Surface Physics

**Version:** v0.5.0 (Review, Debug & Polish)  
**Last updated:** 2026-06-18  
**Tester:** Koeenji  
**Unity version:** 6000.3.17f1

---

## Scope

This plan covers validation of the slope and surface physics system at the current release state.

Validation categories:

- Manual Play Mode test cases
- Automated Edit Mode tests (inherited Currency module)
- Regression checklist
- Console validation
- Git validation

---

## Manual Test Environment

1. Open the project in Unity `6000.3.17f1`.
2. Open the scene `Assets/_Project/Scenes/SlopeSurfacePhysics.unity`.
3. Open the Scene View alongside the Game View to observe gizmos.
4. Select the Player in the Hierarchy so `SlopeSurfaceDebugGizmos2D` is visible.
5. Press **Play**.
6. Execute each test case below.

**Status values:**

| Value | Meaning |
|-------|---------|
| `pending` | Not yet executed |
| `pass` | Observed result matches expected result |
| `fail` | Observed result does not match expected result |
| `skip` | Intentionally skipped with justification |

---

## Manual Test Cases

### T-01 — Compile Without Errors

| Field | Value |
|-------|-------|
| **Setup** | Open the project in Unity 6000.3.17f1. |
| **Steps** | 1. Wait for Unity to finish importing and compiling. 2. Check the Console window. |
| **Expected Result** | 0 compile errors. Assembly `KoeenjiDev.SlopeSurfacePhysics` resolves successfully. No Missing Scripts. No Missing References. |
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
| **Steps** | 1. Press and hold right. 2. Observe Player accelerates to maximum speed. 3. Release input. 4. Observe Player decelerates to a stop. |
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

### T-08 — Ice Surface

| Field | Value |
|-------|-------|
| **Setup** | A flat or sloped surface with `SurfaceModifier2D` set to `Ice`. |
| **Steps** | 1. Walk onto the Ice surface. 2. Press right to accelerate. 3. Release input. Observe deceleration. 4. Reverse direction. Observe reversal speed. |
| **Expected Result** | Noticeably slower acceleration. Noticeably slower deceleration (momentum retained for longer). Slower direction reversal than on Normal surface. `SlopeSurfaceDebugGizmos2D` shows `Surface: Ice`. |
| **Observed Result** | Pending manual validation |
| **Status** | `pending` |
| **Notes** | Ice does not rely on Physics Material friction. |

---

### T-09 — Mud Surface

| Field | Value |
|-------|-------|
| **Setup** | A surface with `SurfaceModifier2D` set to `Mud`. |
| **Steps** | 1. Walk onto the Mud surface. 2. Press right. Observe reduced max speed. 3. Release input. Observe deceleration. |
| **Expected Result** | Noticeably lower maximum speed (~60% of Normal). Slightly slower acceleration. `SlopeSurfaceDebugGizmos2D` shows `Surface: Mud`. |
| **Observed Result** | Pending manual validation |
| **Status** | `pending` |
| **Notes** | — |

---

### T-10 — Sticky Surface

| Field | Value |
|-------|-------|
| **Setup** | A surface with `SurfaceModifier2D` set to `Sticky`. |
| **Steps** | 1. Accelerate on Normal ground. 2. Walk onto Sticky surface. 3. Release input. Observe deceleration. |
| **Expected Result** | Player decelerates noticeably faster on Sticky than on Normal (deceleration ×1.80). `SlopeSurfaceDebugGizmos2D` shows `Surface: Sticky`. |
| **Observed Result** | Pending manual validation |
| **Status** | `pending` |
| **Notes** | — |

---

### T-11 — JumpBoost Surface

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
| **Setup** | A ground collider with no `SurfaceModifier2D` component attached. |
| **Steps** | 1. Walk onto plain ground. 2. Observe movement behavior. 3. Observe `SlopeSurfaceDebugGizmos2D` label. |
| **Expected Result** | Player moves with standard acceleration, deceleration, and max speed. `SlopeSurfaceDebugGizmos2D` shows `Surface: Normal`. No errors in Console. |
| **Observed Result** | Pending manual validation |
| **Status** | `pending` |
| **Notes** | All multipliers default to 1.0. |

---

### T-14 — Debug Gizmo Visibility

| Field | Value |
|-------|-------|
| **Setup** | `SlopeSurfaceDebugGizmos2D` attached to the Player. Scene View open. Player selected. |
| **Steps** | 1. Enter Play Mode with the Scene View visible. 2. Walk the Player onto a slope. 3. Observe the Scene View. |
| **Expected Result** | Blue arrow shows ground normal. Red arrow shows ground tangent. Text label shows slope angle, surface type, and grounded state. Contact sphere is green when grounded, yellow when contact exists but not grounded. In Edit Mode no geometric gizmos are shown. |
| **Observed Result** | Pending manual validation |
| **Status** | `pending` |
| **Notes** | Label uses `UnityEditor.Handles.Label`. No runtime UI is displayed in Game View. |

---

## Automated Tests

### Edit Mode — Currency Module

The inherited `CurrencyWallet` class has 9 Edit Mode unit tests.

**How to run:**
1. Open `Window › General › Test Runner`.
2. Select **Edit Mode**.
3. Click **Run All**.

**Expected result:** 9/9 pass, 0 failures.

### Automated tests for slope logic

No separate automated test assembly for slope logic exists at this version. Slope angle calculation, tangent derivation, and walkability are validated through the manual test cases above and through the `GroundDetector2D` internal logic.

---

## Regression Checklist

Run after any change to physics scripts, prefab, or scene:

- [ ] Unity compiles without errors
- [ ] Console shows 0 errors and 0 new warnings
- [ ] Flat ground movement and deceleration unchanged
- [ ] Jump and landing unchanged
- [ ] Walkable slope traversal unchanged
- [ ] Steep slope behavior unchanged
- [ ] Ice behavior unchanged
- [ ] Currency collection unchanged
- [ ] HUD updates after each pickup
- [ ] No Missing Scripts in the Hierarchy
- [ ] No Missing References in the Inspector
- [ ] Player prefab serialized references intact
- [ ] Currency tests still pass (9/9)
- [ ] Re-entering Play Mode multiple times produces no new errors

---

## Console Validation

At every test session:

```
Target: 0 errors, 0 new avoidable warnings
```

Warnings produced by Unity packages that are unrelated to this project may be noted separately and are not counted against the project.

Debug logs must be removed or disabled before the final release. `SlopeSurfaceDebugGizmos2D` labels use `UnityEditor.Handles.Label` and are Editor-only; they do not appear in builds.

---

## Git Validation

Before any commit or tag:

- [ ] `git status` shows only intended modified or new files
- [ ] `git diff` shows no unintended changes to gameplay scripts, prefab, or scene
- [ ] Private context folders are excluded (not staged)
- [ ] `.meta` files for every changed asset are included
- [ ] No Unity Library, Temp, or user settings files are staged

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
| AC-24 | Currency modules reused without API change | T-01 |
| AC-26 | Pickups cannot be collected twice | Currency manual validation |
| AC-27 | Currency HUD updates through events | Currency manual validation |
| AC-28 | Automated slope-math tests pass | Currency Edit Mode tests (9/9) |
| AC-29 | No Missing Scripts | T-01 |
| AC-30 | No Missing References | T-01 |
| AC-31 | Console at 0 errors | T-02 |
