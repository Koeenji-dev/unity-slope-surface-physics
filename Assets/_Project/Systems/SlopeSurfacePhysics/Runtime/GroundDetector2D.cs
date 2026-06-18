using UnityEngine;

namespace KoeenjiDev.SlopeSurfacePhysics
{
    /// <summary>
    /// Queries and reports whether the Player currently has valid supporting
    /// ground contact below its capsule.
    /// </summary>
    public sealed class GroundDetector2D : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Rigidbody2D on the Player root.")]
        [SerializeField] private Rigidbody2D body;

        [Tooltip("CapsuleCollider2D on the Player root.")]
        [SerializeField] private CapsuleCollider2D playerCollider;

        [Tooltip("Child transform positioned at the base of the capsule.")]
        [SerializeField] private Transform groundProbe;

        [Tooltip("Layers that count as ground. Must include the Ground layer (7).")]
        [SerializeField] private LayerMask groundLayers;

        [Header("Configuration")]
        [Tooltip("Downward cast distance beyond the capsule base, in world units.")]
        [SerializeField, Min(0f)]
        private float groundCheckDistance = 0.08f;

        [Tooltip("Maximum upward velocity at which the player is still considered grounded.")]
        [SerializeField, Min(0f)]
        private float groundedVerticalTolerance = 0.1f;

        public bool HasGroundContact { get; private set; }
        public bool IsGrounded { get; private set; }

        /// <summary>
        /// Performs the capsule cast and updates HasGroundContact and IsGrounded.
        /// Call this at the start of PlayerSlopeController2D.FixedUpdate.
        /// </summary>
        public void UpdateGroundState()
        {
            Vector3 lossyScale = playerCollider.transform.lossyScale;
            Vector2 worldSize = new Vector2(
                playerCollider.size.x * Mathf.Abs(lossyScale.x),
                playerCollider.size.y * Mathf.Abs(lossyScale.y)
            );

            RaycastHit2D hit = Physics2D.CapsuleCast(
                playerCollider.bounds.center,
                worldSize,
                playerCollider.direction,
                0f,
                Vector2.down,
                groundCheckDistance,
                groundLayers
            );

            HasGroundContact = hit.collider != null;
            IsGrounded = HasGroundContact && body.linearVelocity.y <= groundedVerticalTolerance;
        }

        private void OnValidate()
        {
            if (groundCheckDistance < 0f)
                groundCheckDistance = 0f;

            if (groundedVerticalTolerance < 0f)
                groundedVerticalTolerance = 0f;
        }
    }
}
