using KoeenjiDev.PlayerVisual;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KoeenjiDev.SlopeSurfacePhysics
{
    /// <summary>
    /// Receives player input and applies horizontal movement, code-controlled
    /// gravity, and a simple vertical jump to the Player Rigidbody2D.
    /// Forwards motion state to PlayerVisualController2D after each physics step.
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
            // 1. Update ground state first so IsGrounded is current.
            groundDetector.UpdateGroundState();

            // 2. Read current velocity.
            Vector2 velocity = body.linearVelocity;

            // 3. Horizontal movement.
            float targetVelocityX = horizontalInput * maximumSpeed;
            float rate = Mathf.Approximately(horizontalInput, 0f)
                ? groundDeceleration
                : groundAcceleration;
            velocity.x = Mathf.MoveTowards(
                velocity.x,
                targetVelocityX,
                rate * Time.fixedDeltaTime
            );

            // 4. Jump — only from grounded state.
            bool jumped = false;
            if (jumpRequested && groundDetector.IsGrounded)
            {
                velocity.y = jumpSpeed;
                jumped = true;
            }

            // 5. Apply gravity only when no jump occurred this step.
            if (!jumped)
                velocity.y -= gravityAcceleration * Time.fixedDeltaTime;

            // 6. Clamp fall speed.
            velocity.y = Mathf.Max(velocity.y, -maximumFallSpeed);

            // 7. Single velocity assignment.
            body.linearVelocity = velocity;

            // 8. Consume the jump request regardless of outcome.
            jumpRequested = false;

            // 9. Forward final velocity and ground state to the visual module.
            UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            if (playerVisual == null)
                return;

            Vector2 velocity = body.linearVelocity;

            playerVisual.SetMotionState(
                Mathf.Abs(velocity.x),
                velocity.y,
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
