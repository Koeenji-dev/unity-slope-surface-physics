# Media Guide — 02 Slope & Surface Physics

**Project:** 02 — Slope & Surface Physics  
**Last updated:** 2026-06-18

This guide defines the required and recommended media captures for the public portfolio release of this project.

---

## Required Screenshots

These screenshots must be captured before the v1.0.0 release and added to the repository.

| ID | Subject | Description |
|----|---------|-------------|
| SS-01 | Overview — flat ground | Player standing on flat Normal ground. Idle animation visible. Scene View shows gizmos with normal and tangent arrows. |
| SS-02 | Walkable slope | Player standing or walking on a 30–45° slope. Debug gizmo label visible showing angle, surface type, and grounded state. |
| SS-03 | Steep slope | Player sliding on a slope above 45°. Debug label shows angle and non-walkable state. |
| SS-04 | Ice surface | Player on an ice surface. Debug label shows `Surface: Ice` with multipliers visible. |
| SS-05 | Surface modifier Inspector | Inspector screenshot of a `SurfaceModifier2D` component configured as Ice, showing multiplier fields. |
| SS-06 | Player prefab Inspector | Inspector view of the Player root with all components listed. |
| SS-07 | Scene hierarchy | Hierarchy view showing the Player with its child objects (GroundProbe, Visual, PlayerVisual). |

---

## Recommended GIFs

These animated captures best demonstrate the system behavior in the portfolio.

| ID | Subject | Duration | Priority |
|----|---------|----------|----------|
| GIF-01 | Walkable slope — uphill and downhill | 4–6 s | High |
| GIF-02 | Flat to slope transition | 3–4 s | High |
| GIF-03 | Ice vs Normal comparison | 5–8 s | High |
| GIF-04 | Steep slope sliding | 3–5 s | High |
| GIF-05 | Debug gizmos during slope walk (Scene View) | 4–6 s | Medium |
| GIF-06 | JumpBoost surface jump | 2–3 s | Medium |
| GIF-07 | Visual state — Idle, Run, Jump, Fall | 5–7 s | Medium |

**Capture settings for GIFs:** 30–60 fps playback. Capture from Game View at 1920×1080 or 1280×720. Keep duration short and focused on one behavior per clip.

---

## Optional Video Shots

Useful for a longer portfolio showcase or supplementary YouTube description video:

- Side-by-side comparison of movement on Normal vs Ice flat ground
- Player stopping mid-slope on 35° and 45° angles
- Slope transition at high speed (slope to flat, no bounce)
- Pressing Space on a JumpBoost surface vs a Normal surface from the same position
- Full demo route walkthrough with HUD visible (Currency collection)
- Debug gizmo overlay during the full route

---

## Capture Rules

1. **Disable Vsync issues:** Run captures at a stable 60 fps. Check `Edit › Project Settings › Quality` if the frame rate is unstable during capture.

2. **Select the Player before capturing gizmos:** `SlopeSurfaceDebugGizmos2D` only draws in `OnDrawGizmosSelected`. The Player object must be selected in the Hierarchy for gizmos to appear.

3. **Clean Console before recording:** Clear the Console window and verify 0 errors before starting a capture session.

4. **Scene View vs Game View:**
   - Use **Game View** for player-facing GIFs and screenshots.
   - Use **Scene View** for gizmo-focused screenshots and GIFs.

5. **No editor overlays or development artifacts:** Close floating windows, hide the Hierarchy if it overlaps the Game View, and ensure no Unity crash dialogs are visible.

6. **Consistent starting position:** Place the Player at a known starting position for reproducible captures. Do not commit scene changes made only for capture purposes.

---

## File Naming

Store media files in `Documentation/Media/` (create the folder locally; do not commit large binary files to the repository unless using Git LFS).

| Type | Naming pattern | Example |
|------|---------------|---------|
| Screenshots | `ss-NN-description.png` | `ss-01-flat-ground.png` |
| GIFs | `gif-NN-description.gif` | `gif-01-walkable-slope.gif` |
| Video clips | `clip-NN-description.mp4` | `clip-01-ice-vs-normal.mp4` |

Use lowercase, hyphens only, no spaces, no special characters.

---

## Portfolio Integration

When embedding media in `README.md` or `README.es.md`, use relative paths:

```markdown
![Walkable slope movement](Documentation/Media/gif-01-walkable-slope.gif)
```

Replace the placeholder line in `README.md` under the **Media** section once the captures are ready.
