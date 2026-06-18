# 02 — Slope & Surface Physics

Un prototipo de física 2D enfocado, desarrollado en Unity 6, que demuestra detección de contacto con el suelo, lectura de normales de superficie, clasificación de ángulos de pendiente, movimiento en pendientes transitables, comportamiento en pendientes pronunciadas y modificadores de movimiento dependientes de la superficie.

> Este es un **prototipo de sistema técnico**, no un juego completo.
> El objetivo es construir, entender y demostrar un controlador de física de pendientes y superficies reutilizable para juegos 2D de plataformas.

---

## Objetivo del proyecto

Diseñar e implementar un controlador de física 2D reutilizable que resuelva los problemas de interacción con pendientes y superficies habituales en juegos de plataformas 2D.

El movimiento básico y el salto existen únicamente como la infraestructura mínima necesaria para validar el sistema de pendientes.

---

## Funcionalidades

- Detección explícita de contacto con el suelo mediante `CapsuleCast` descendente
- Normal de superficie y ángulo del suelo expuestos por fotograma
- Ángulo máximo de pendiente transitable configurable (por defecto: 45°)
- Movimiento proyectado sobre la tangente de la superficie para un recorrido estable en pendientes
- Movimiento estable cuesta arriba y cuesta abajo sin vibración visible
- Comportamiento en pendientes pronunciadas: la locomoción cuesta arriba es rechazada; el jugador se desliza hacia abajo bajo la gravedad
- Sistema de modificadores de superficie: componentes por colisionador que alteran la respuesta de movimiento sin modificar el cuerpo físico
- Tipos de superficie con valores predefinidos: Normal, Ice, Mud, Sticky, JumpBoost
- Superficie Ice: aceleración reducida, deceleración fuertemente reducida, inercia conservada
- Módulo visual del jugador reutilizable heredado (Idle, Run, Jump, Fall, orientación)
- Sistema de Currency heredado como demostración secundaria de integración de módulos
- Capa de gizmos de depuración: esfera de contacto, flecha de normal, flecha de tangente, etiquetas en la Vista de Escena
- Aislamiento de ensamblados: el código de física en tiempo de ejecución reside en `KoeenjiDev.SlopeSurfacePhysics`
- Solo el nuevo Input System de Unity; sin Input Manager legado

---

## Controles

| Acción | Teclado | Mando |
|--------|---------|-------|
| Moverse izquierda / derecha | A / D o teclas de flecha | Stick izquierdo |
| Saltar | Espacio | Botón Sur |

---

## Puntos técnicos destacados

- La consulta de suelo con `CapsuleCast` coincide con la huella física del jugador; evita falsos positivos contra paredes
- El estado de contacto distingue `HasGroundContact`, `IsGroundWalkable` e `IsGrounded` como conceptos separados
- `GroundContactData2D` es un struct de solo lectura inmutable actualizado una vez por `FixedUpdate` — sin asignación en heap por fotograma
- La tangente de pendiente se deriva analíticamente de la normal de superficie: `tangent = (normal.y, -normal.x)`; sin trigonometría en tiempo de ejecución
- La transitabilidad de una pendiente es una simple comparación de ángulo contra un umbral serializable
- Los modificadores de superficie usan un `MonoBehaviour` simple en el colisionador del suelo; si no hay modificador, se aplica el comportamiento Normal por defecto
- `PlayerSlopeController2D` lee los multiplicadores de `GroundContactData2D.SurfaceModifier` y los aplica a la aceleración, deceleración y velocidad máxima en cada paso fijo
- Ice se implementa mediante multiplicadores explícitos de aceleración y deceleración, no mediante fricción del Physics Material
- La gravedad es controlada por código (`Rigidbody2D.gravityScale = 0`) para un comportamiento aéreo predecible y una adhesión limpia a pendientes
- El material de física del jugador tiene fricción 0 y elasticidad 0; la respuesta de superficie es completamente controlada por código
- El estado visual (`motionSpeed`) usa la proyección escalar de la velocidad sobre la tangente de pendiente, de forma que Run permanece activo en inclinaciones
- `SlopeSurfaceDebugGizmos2D` es una capa de visualización de depuración pura; no participa en el gameplay

---

## Resumen de arquitectura

```
PlayerInput
→ PlayerSlopeController2D
→ GroundDetector2D
  └── GroundContactData2D  (punto de contacto, normal, tangente, ángulo, modificador de superficie)
→ SurfaceModifier2D        (multiplicadores de movimiento leídos por fotograma desde los datos de contacto)
→ Rigidbody2D              (recibe la velocidad final en cada FixedUpdate)
→ PlayerVisualController2D (estado de movimiento enviado tras el paso de física)
```

Flujo secundario independiente:

```
CurrencyPickup
→ CurrencyCollector
→ CurrencyWallet
→ evento BalanceChanged
→ CurrencyDisplay
```

---

## Estructura del proyecto

```
Assets/
├── _Project/
│   ├── Demo/
│   │   └── Runtime/           CoinVisualSpin (ayuda de animación de giro)
│   ├── Input/
│   │   └── PlayerControls.inputactions
│   ├── Player/
│   │   ├── Prefabs/           Player.prefab
│   │   └── Visual/            Módulo visual del jugador heredado
│   ├── Scenes/
│   │   └── SlopeSurfacePhysics.unity
│   └── Systems/
│       ├── Currency/          Módulo de moneda heredado
│       │   ├── Prefabs/
│       │   ├── Runtime/
│       │   ├── Tests/
│       │   └── UI/
│       └── SlopeSurfacePhysics/
│           ├── Physics/       Material de física PlayerNoFriction
│           └── Runtime/
│               ├── GroundContactData2D.cs
│               ├── GroundDetector2D.cs
│               ├── PlayerSlopeController2D.cs
│               ├── SlopeSurfaceDebugGizmos2D.cs
│               └── SurfaceModifier2D.cs
└── ThirdParty/
    ├── Kenney/
    │   └── PlatformerArtDeluxe/   Tilesets CC0 (hierba y tundra/hielo)
    └── OzzbitGames/
        └── FantasyCharacter/      Hojas de sprites del jugador (con licencia)
```

---

## Cómo ejecutarlo

**Requisitos:** Unity 6000.3.17f1, plantilla Universal 2D, paquete New Input System activo.

1. Clona o descarga el repositorio.
2. Abre el proyecto en Unity Hub con la versión `6000.3.17f1`.
3. Abre la escena: `Assets/_Project/Scenes/SlopeSurfacePhysics.unity`.
4. Pulsa **Play**.
5. Usa WASD o las teclas de flecha para moverte y Espacio para saltar.

---

## Pruebas

### Tests automatizados

El módulo de Currency heredado incluye 9 pruebas unitarias en modo Editor. Abre la ventana **Test Runner** (`Window › General › Test Runner`) y ejecuta todas las pruebas en Edit Mode.

### Tests manuales

Consulta `Documentation/TEST_PLAN.md` para la matriz de pruebas manuales completa, que cubre suelo plano, pendientes transitables, superficies de hielo, modificadores de superficie, visualización de depuración e integración de Currency.

---

## Errores conocidos

Consulta `Documentation/KNOWN_ISSUES.md` para la lista actual de problemas conocidos, limitaciones aceptadas y funcionalidades excluidas intencionalmente de este prototipo.

---

## Mejoras futuras

Las siguientes son posibles mejoras para una versión futura, no planificadas para la entrega actual:

- Tests automatizados en Edit Mode para los cálculos de ángulo y tangente de pendiente
- Coyote time y jump buffer (excluidos intencionalmente de este prototipo)
- Altura de salto variable
- Soporte para plataformas móviles
- Script de seguimiento de cámara para niveles más grandes
- Ruta de demostración adicional con marcadores de ángulo etiquetados
- Extracción como paquete para reutilización en otros proyectos

---

## Historial de versiones

| Etiqueta | Descripción |
|----------|-------------|
| `v0.5.0-review-debug-polish` | Capa de gizmos de depuración, revisión de código y paso de pulido |
| `v0.4.0-surface-modifiers` | Sistema de modificadores de superficie: Normal, Ice, Mud, Sticky, JumpBoost |
| `v0.3.0-slope-movement` | Movimiento en pendientes transitables, proyección sobre tangente, comportamiento en pendientes pronunciadas |
| `v0.2.0-ground-contact-data` | Struct `GroundContactData2D`, información extendida de contacto con el suelo |
| `v0.1.0-basic-player-movement` | Movimiento horizontal, salto, gravedad, integración visual |
| `v0.0.1-foundation` | Configuración del proyecto, ensamblados, asset de entrada, módulos heredados |

Detalles completos en `Documentation/CHANGELOG.md`.

---

## Material visual

Las capturas de pantalla y GIFs se añadirán antes de la versión v1.0.0.

Consulta `Documentation/MEDIA_GUIDE.md` para instrucciones de captura y convenciones de nomenclatura.

---

## Autor

**Koeenji Dev** — Portfolio de sistemas de gameplay en Unity  
Repositorio: [unity-slope-surface-physics](https://github.com/Koeenji-dev/unity-slope-surface-physics)
