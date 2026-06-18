# Known Issues — 02 Slope & Surface Physics

**Version:** v0.5.0-review-debug-polish  
**Last updated:** 2026-06-18

---

## Current Known Issues

No critical bugs are known at the current release state.

Minor observations:

- **Collider seam micro-bounce:** When crossing the boundary between two adjacent flat colliders placed end-to-end, a brief single-frame grounded state fluctuation may occur depending on the exact collider alignment. This is a standard Unity physics limitation. It can be minimized with smooth geometry.

- **Steep-slope contact at extreme angles (> 70°):** At very steep angles approaching vertical, the player may slide faster than visually expected due to gravity acceleration being unrestricted during air movement on non-walkable contacts. The slide speed is bounded by `maximumFallSpeed` and is not a safety issue.

- **No steep-slope slide deceleration against walkable ground edge:** When the player slides off a steep surface and immediately contacts walkable flat ground, there is a single fixed step where both contacts may exist simultaneously. The transition resolves correctly on the next step.

---

## Accepted Limitations

These behaviors are known and accepted as part of the current scope:

- **Jump is purely vertical.** There is no slope-normal jump, angled launch, or directional jump. This keeps the system focused on slope physics rather than advanced jump mechanics.

- **Air control is identical whether rising or falling.** Reducing air control during ascent versus descent was not included in this prototype's scope.

- **No ground snap.** If the player exits a slope edge at speed, they become airborne rather than snapping to the ground geometry. A ground snap pass was not part of this version.

- **`CapsuleCast` single query.** Ground detection uses one capsule cast per fixed step. Multiple simultaneous contacts (e.g. standing exactly on the apex of two adjacent slopes) resolve to the closest hit. Complex multi-contact scenarios are outside this prototype's scope.

- **Debug gizmos are Editor-only.** `SlopeSurfaceDebugGizmos2D` labels use `UnityEditor.Handles.Label` and do not appear in runtime builds. A runtime HUD overlay for in-game use is not included.

- **No camera follow script is included.** The scene uses a static camera. A large demonstration route would require either a custom follow script or Cinemachine, neither of which was added in this version.

---

## Not Implemented by Design

The following features are explicitly excluded from this prototype:

| Feature | Reason for exclusion |
|---------|---------------------|
| Coyote time | Out of scope for this prototype; would be added to a future advanced jump system |
| Jump buffer | Out of scope for this prototype |
| Variable jump height (hold to jump higher) | Out of scope for this prototype |
| Double jump | Out of scope for this prototype |
| Dash | Planned for a separate future portfolio project (03 — Stamina & Dash) |
| Wall slide and wall jump | Planned for a separate future portfolio project (04 — Advanced Jump & Wall Interaction) |
| Climbing and ledge interaction | Planned for a separate future portfolio project |
| Moving or disappearing platforms | Out of scope; would require a separate platform controller system |
| Health, damage, enemies, combat | Out of scope for a physics system prototype |
| Checkpoints and save/load | Out of scope |
| ScriptableObject-based surface database | Intentionally avoided; `SurfaceModifier2D` as a plain MonoBehaviour is sufficient for this scope |
| Complex Tilemap environments | Unnecessary for demonstrating the physics system |
| Cinemachine | Not needed; static camera is sufficient for the current scene |
| Multiplayer or networking | Out of scope |
| Procedural terrain | Out of scope |

---

## Future Improvements

These are potential improvements for a future version or follow-up project:

- **Automated Edit Mode tests for slope math:** Angle calculation, tangent derivation, and walkability boundary checks could be isolated into a dedicated test assembly for regression coverage.

- **Ground snap on slope exits:** A short snap pass that keeps the player on the geometry when descending a slope at speed would improve the feel at slope exits.

- **Adjustable air gravity multiplier:** Separate gravity scales for the rising and falling phases of a jump would allow more responsive feel without changing the core slope behavior.

- **Runtime debug UI panel:** A lightweight in-game HUD showing ground state, angle, and surface type would be useful for demonstration builds.

- **Package extraction:** The runtime assembly `KoeenjiDev.SlopeSurfacePhysics` could be extracted as a reusable Unity package for other projects once the system is stable.
