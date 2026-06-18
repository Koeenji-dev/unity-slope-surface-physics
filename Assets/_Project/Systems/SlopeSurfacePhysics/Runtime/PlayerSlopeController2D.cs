using KoeenjiDev.PlayerVisual;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KoeenjiDev.SlopeSurfacePhysics
{
    /// <summary>
    /// Receives player input and applies movement, code-controlled gravity, and a
    /// simple vertical jump to the Player Rigidbody2D. When grounded on walkable
    /// ground, movement is projected onto the surface tangent. Forwards motion state
    /// to PlayerVisualController2D after each physics step.
    /// </summary>
    public sealed class PlayerSlopeController2D : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Rigidbody2D on the Player root.")]
        [SerializeField] private Rigidbody2D body;

        [Tooltip("GroundDetector2D on the Player root.")]
        [SerializeField] private GroundDetector2D groundDetector;

        [Tooltip("PlayerVisualController2D on the nested PlayerVisual prefab.")]
        [SerializeField] private PlayerVisualController2D playerVisual;

        [Header("Ground Movement")]
        [Tooltip("Maximum horizontal speed in world units per second.")]
        [SerializeField, Min(0f)]
        private float maximumSpeed = 7f;

        [Tooltip("Horizontal acceleration applied when input is active, in units/s².")]
        [SerializeField, Min(0f)]
        private float groundAcceleration = 45f;

        [Tooltip("Horizontal deceleration applied when input is released, in units/s².")]
        [SerializeField, Min(0f)]
        private float groundDeceleration = 55f;

        [Tooltip("Fraction of maximum speed applied when moving uphill on a walkable slope. 1 means no reduction.")]
        [SerializeField, Range(0.1f, 1f)]
        private float uphillSpeedMultiplier = 0.85f;

        [Header("Jump")]
        [Tooltip("Vertical speed assigned immediately on jump, in units/s.")]
        [SerializeField, Min(0f)]
        private float jumpSpeed = 12f;

        [Header("Gravity")]
        [Tooltip("Downward acceleration applied each fixed step, in units/s².")]
        [SerializeField, Min(0f)]
        private float gravityAcceleration = 35f;

        [Tooltip("Maximum downward speed (positive value), in units/s.")]
        [SerializeField, Min(0f)]
        private float maximumFallSpeed = 20f;

        private float horizontalInput;
        private bool jumpRequested;
        private float movementSpeed;

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
                horizontalInput = Mathf.Clamp(context.ReadValue<Vector2>().x, -1f, 1f);
            else if (context.canceled)
                horizontalInput = 0f;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
                jumpRequested = true;
        }

        private void FixedUpdate()
        {
            if (body == null || groundDetector == null)
                return;

            // 1. Update ground state first so IsGrounded is current.
            groundDetector.UpdateGroundState();

            // 2. Read current velocity.
            Vector2 velocity = body.linearVelocity;

            // 3. Jump — only from grounded state.
            bool jumped = jumpRequested && groundDetector.IsGrounded;

            // 4. Apply movement along the surface tangent or global X.
            if (groundDetector.IsGrounded && !jumped)
                velocity = ApplyGroundMovement(velocity);
            else
                velocity = ApplyAirMovement(velocity, jumped);

            // 5. Single velocity assignment.
            body.linearVelocity = velocity;

            // 6. Consume the jump request regardless of outcome.
            jumpRequested = false;

            // 7. Forward final velocity and ground state to the visual module.
            UpdateVisualState();
        }

        private Vector2 ApplyGroundMovement(Vector2 velocity)
        {
            Vector2 slopeDirection = GetGroundMovementDirection();

            // Read surface modifier from the current contact. Null means Normal (all 1f).
            SurfaceModifier2D modifier = groundDetector.ContactData.SurfaceModifier;
            float speedMult  = modifier != null ? modifier.MaxSpeedMultiplier      : 1f;
            float accelMult  = modifier != null ? modifier.AccelerationMultiplier  : 1f;
            float decelMult  = modifier != null ? modifier.DecelerationMultiplier  : 1f;

            float effectiveMaxSpeed    = maximumSpeed      * speedMult;
            float effectiveAcceleration = groundAcceleration * accelMult;
            float effectiveDeceleration = groundDeceleration * decelMult;

            float targetSpeed = horizontalInput * effectiveMaxSpeed;

            // Moving uphill means the actual travel direction has a positive Y component.
            Vector2 actualMoveDirection = slopeDirection * Mathf.Sign(targetSpeed);
            if (actualMoveDirection.y > 0.01f)
                targetSpeed *= uphillSpeedMultiplier;

            // Project current velocity onto the slope to get the scalar starting point.
            float currentSpeed = Vector2.Dot(velocity, slopeDirection);
            float rate = Mathf.Approximately(horizontalInput, 0f)
                ? effectiveDeceleration
                : effectiveAcceleration;

            movementSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * Time.fixedDeltaTime);

            // Fully define velocity along the surface; no gravity while grounded.
            return slopeDirection * movementSpeed;
        }

        private Vector2 ApplyAirMovement(Vector2 velocity, bool jumped)
        {
            float targetVelocityX = horizontalInput * maximumSpeed;
            float rate = Mathf.Approximately(horizontalInput, 0f)
                ? groundDeceleration
                : groundAcceleration;

            movementSpeed = Mathf.MoveTowards(velocity.x, targetVelocityX, rate * Time.fixedDeltaTime);
            velocity.x = movementSpeed;

            if (jumped)
            {
                SurfaceModifier2D modifier = groundDetector.ContactData.SurfaceModifier;
                float jumpMult = modifier != null ? modifier.JumpForceMultiplier : 1f;
                velocity.y = jumpSpeed * jumpMult;
            }
            else
            {
                velocity.y -= gravityAcceleration * Time.fixedDeltaTime;
            }

            velocity.y = Mathf.Max(velocity.y, -maximumFallSpeed);
            return velocity;
        }

        // Returns the slope tangent reoriented so its X component is always positive,
        // meaning positive movementSpeed always drives the player to the right.
        private Vector2 GetGroundMovementDirection()
        {
            Vector2 dir = groundDetector.GroundTangent;
            return dir.x < 0f ? -dir : dir;
        }

        private void UpdateVisualState()
        {
            if (playerVisual == null)
                return;

            playerVisual.SetMotionState(
                Mathf.Abs(movementSpeed),
                body.linearVelocity.y,
                groundDetector.IsGrounded
            );

            playerVisual.SetFacingDirection(horizontalInput);
        }

        private void OnValidate()
        {
            if (maximumSpeed < 0f)         maximumSpeed = 0f;
            if (groundAcceleration < 0f)   groundAcceleration = 0f;
            if (groundDeceleration < 0f)   groundDeceleration = 0f;
            if (jumpSpeed < 0f)            jumpSpeed = 0f;
            if (gravityAcceleration < 0f)  gravityAcceleration = 0f;
            if (maximumFallSpeed < 0f)     maximumFallSpeed = 0f;
        }
    }
}
