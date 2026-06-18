# Guion de Vídeo — 02 Slope & Surface Physics

**Proyecto:** 02 — Slope & Surface Physics  
**Idioma:** Español  
**Duración estimada:** 5–8 minutos  
**Última actualización:** 2026-06-18

---

## Notas de producción

- Captura la pantalla del juego en Game View a 1920×1080 o 1280×720.
- Mantén la escena limpia: Consola a cero errores antes de grabar.
- Para las secciones de código y arquitectura, usa capturas del editor con el código visible.
- La voz en off puede grabarse por separado y sincronizarse con la grabación de pantalla.
- Mantén el ritmo activo: no más de 30 segundos sin movimiento visible en pantalla.

---

## Introducción

**[Pantalla: logo del proyecto o título en el editor de Unity]**

> Hola, soy Koeenji Dev.
>
> Este es el segundo proyecto de mi portfolio de sistemas Unity:
> **Slope and Surface Physics**.
>
> En este vídeo te voy a mostrar qué hace el sistema, cómo está construido por dentro, y por qué cada decisión técnica tiene sentido.

---

## Problema

**[Pantalla: imagen de un personaje intentando subir una rampa inclinada, deslizándose o rebotando]**

> Uno de los problemas clásicos en los juegos de plataformas 2D es el movimiento en rampas.
>
> Si simplemente aplicas velocidad horizontal, el jugador se desliza o se atasca en las esquinas de los colisionadores.
>
> Si el suelo tiene física nativa con fricción alta, el comportamiento cambia de forma impredecible dependiendo del ángulo y del material.
>
> Y si además quieres que una superficie de hielo se comporte de forma distinta a una de barro, las cosas se complican más.
>
> Este proyecto construye una solución clara para estos problemas, con código que puedo entender, explicar y reutilizar.

---

## Solución

**[Pantalla: escena en reposo con el jugador en pie sobre suelo plano]**

> La solución tiene tres partes principales:
>
> Primero, detección de contacto explícita. En lugar de depender de eventos de colisión de Unity, el sistema lanza un `CapsuleCast` hacia abajo en cada paso de física y recoge el punto de contacto, la normal de superficie y el ángulo.
>
> Segundo, movimiento proyectado. En vez de aplicar velocidad solo en el eje horizontal, calculamos la tangente de la superficie a partir de su normal y proyectamos el movimiento sobre ella. Así el jugador sube y baja rampas de forma natural.
>
> Tercero, modificadores de superficie. Cada colisionador del suelo puede tener un componente que ajusta la aceleración, la deceleración y la velocidad máxima del jugador. Sin cambiar la física del cuerpo rígido.

---

## Demo rápida

**[Pantalla: jugador moviéndose por la escena completa]**

> Aquí tienes la escena de demostración en marcha.
>
> En el suelo plano, el movimiento es estándar: aceleración, deceleración, salto limpio, animaciones de Idle, Run, Jump y Fall.
>
> Al llegar a una rampa transitable —en este caso de 35 grados— el jugador sube y baja siguiendo la superficie sin vibraciones.
>
> En esta zona hay hielo. Fíjate cómo la inercia se conserva más tiempo y la deceleración es mucho más lenta.
>
> Y aquí hay una pendiente pronunciada por encima de los 45 grados. El jugador no puede subirla con el movimiento normal: se desliza hacia abajo controlado por la gravedad.

---

## Arquitectura

**[Pantalla: diagrama del flujo de datos o código con el flujo comentado]**

> Vamos a ver cómo está organizado por dentro.
>
> El flujo principal es este:
>
> El componente `PlayerInput` de Unity recibe las teclas o el mando y llama a los métodos `OnMove` y `OnJump` en `PlayerSlopeController2D`.
>
> `PlayerSlopeController2D` es quien toma todas las decisiones de movimiento. Cada paso de física llama a `GroundDetector2D` para obtener el estado del suelo, calcula la velocidad final y la escribe en el `Rigidbody2D`.
>
> Después de aplicar la física, reenvía el estado de movimiento a `PlayerVisualController2D`, que es un módulo heredado de un proyecto anterior. Él se encarga de las animaciones sin necesidad de conocer nada de pendientes ni superficies.
>
> Este flujo unidireccional hace que cada componente tenga una responsabilidad clara y que no haya dependencias ocultas.

---

## Ground Detection

**[Pantalla: escena con gizmos visibles — flecha azul normal, flecha roja tangente, esfera de contacto]**

> Aquí puedes ver el `GroundDetector2D` en acción con los gizmos de depuración activos.
>
> La flecha azul es la normal de la superficie en el punto de contacto. La flecha roja es la tangente, que es la dirección en la que se proyecta el movimiento.
>
> La esfera verde indica que el jugador está en estado `IsGrounded`: hay contacto con una superficie transitable y el jugador no se está alejando de ella.
>
> El sistema distingue tres cosas: si hay contacto, si ese contacto es transitable según el ángulo configurado, y si el jugador está realmente apoyado. Son tres conceptos distintos que no se mezclan.
>
> El `CapsuleCast` usa la misma forma que el colisionador del jugador. Esto evita que una pared casi vertical cuente como suelo: su normal apunta hacia un lado, el ángulo calculado es cercano a 90 grados, y el sistema lo rechaza como suelo transitable.

---

## Slope Movement

**[Pantalla: jugador subiendo y bajando varias rampas, etiquetas de ángulo visibles si las hay]**

> El movimiento en pendientes funciona así.
>
> La tangente de la superficie siempre apunta en la dirección de avance positiva. `PlayerSlopeController2D` asegura que su componente horizontal sea siempre positivo, de forma que un valor de velocidad positivo siempre mueve al jugador hacia la derecha, independientemente del ángulo de la rampa.
>
> La velocidad que se envía al módulo visual es la proyección escalar del movimiento sobre la tangente, no solo la velocidad horizontal. Esto hace que la animación de correr permanezca activa mientras el jugador se mueve por una rampa, aunque su velocidad horizontal sea relativamente baja.
>
> En pendientes pronunciadas, el estado `IsGrounded` es falso. El sistema entra en modo aéreo, la gravedad hace efecto y el material de física sin fricción permite el deslizamiento natural sobre la superficie.

---

## Surface Modifiers

**[Pantalla: Inspector del componente SurfaceModifier2D — superficie Ice; luego jugador en superficie de hielo]**

> El sistema de modificadores de superficie es la parte más extensible.
>
> Cada colisionador del suelo puede tener un componente `SurfaceModifier2D`. Cuando el `GroundDetector2D` detecta ese colisionador, recupera el componente y lo almacena en el snapshot de contacto `GroundContactData2D`.
>
> En el siguiente paso fijo, `PlayerSlopeController2D` lee los multiplicadores: velocidad máxima, aceleración, deceleración y fuerza de salto. Si no hay modificador, todos los valores son 1.0, es decir, comportamiento Normal.
>
> El hielo usa aceleración reducida a un 35% y deceleración reducida a un 15% de los valores base. El resultado es ese comportamiento resbaladizo donde el jugador acumula velocidad lentamente pero tarda mucho en parar o cambiar de dirección.
>
> También hay Mud, que reduce la velocidad máxima al 60%; Sticky, que tiene una deceleración extremadamente rápida; y JumpBoost, que duplica la fuerza del salto cuando se pulsa el botón.
>
> Nada de esto toca el `Rigidbody2D`. Es todo código explícito en cada paso de física.

---

## Pruebas y validaciones

**[Pantalla: Test Runner de Unity con los tests en verde]**

> El módulo de Currency heredado tiene 9 tests automáticos en modo editor que siguen funcionando sin cambios.
>
> Para la lógica de pendientes, las pruebas son manuales por ahora: cada tipo de superficie, cada ángulo de rampa, las transiciones entre superficies, el salto, las animaciones. Todo está documentado en el Test Plan del repositorio.
>
> La consola se mantiene en cero errores. Los gizmos de depuración son solo para el editor: no aparecen en la build y no afectan al gameplay.

---

## Limitaciones

**[Pantalla: escena mostrando un salto desde pendiente — salto vertical]**

> Este sistema es un prototipo técnico. Hay cosas que no están incluidas por diseño.
>
> El salto es vertical global. No hay coyote time, ni jump buffer, ni doble salto. Eso mantiene el foco en la física de pendientes y no en un sistema avanzado de saltos.
>
> No hay snap al suelo en la salida de rampas. El jugador se queda aéreo brevemente si sale al final de una rampa con velocidad.
>
> No hay cámara de seguimiento. La escena usa una cámara estática, suficiente para el alcance actual.
>
> Estas características son candidatas para proyectos futuros de la serie, no pendientes de este prototipo.

---

## Cierre

**[Pantalla: repositorio de GitHub o pantalla final con el nombre del proyecto]**

> El repositorio está disponible en GitHub con documentación completa en inglés y español.
>
> El código es legible, la arquitectura es proporcional al problema y no hay capas innecesarias.
>
> El siguiente proyecto de la serie añadirá un sistema de stamina y dash. Pero eso es otro vídeo.
>
> Si tienes preguntas sobre algún detalle técnico, déjalas en los comentarios.
>
> Gracias por ver.

---

## Notas de edición

- **Intro:** mostrar el nombre del proyecto sobre un fondo oscuro con el gameplay en segundo plano o en un recuadro.
- **Secciones de código:** usar zoom o highlight para las partes más relevantes. No mostrar código sin contexto.
- **Gizmos:** asegurarse de que la cámara de la Scene View esté bien orientada y los gizmos sean claramente visibles.
- **Cortes:** usar cortes directos o fundidos breves. Evitar transiciones llamativas que distraigan del contenido técnico.
- **Subtítulos opcionales:** si se publican con subtítulos, revisar términos técnicos en inglés que no deben traducirse (como `CapsuleCast`, `Rigidbody2D`, `FixedUpdate`).
