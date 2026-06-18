using UnityEngine;

namespace KoeenjiDev.SlopeSurfacePhysics
{
    /// <summary>
    /// Queries and reports the current surface contact below the Player capsule.
    /// Call UpdateGroundState once per FixedUpdate before reading any property.
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

        // ── Public state ──────────────────────────────────────────────────────

        /// <summary>True when the capsule cast detected a supporting collider below the Player.</summary>
        public bool HasGroundContact { get; private set; }

        /// <summary>
        /// True when HasGroundContact is true and vertical velocity is within
        /// the grounded tolerance (player is not jumping away from the surface).
        /// </summary>
        public bool IsGrounded { get; private set; }

        /// <summary>Full contact snapshot from the most recent UpdateGroundState call.</summary>
        public GroundContactData2D ContactData { get; private set; }

        // ── Convenience pass-through properties ───────────────────────────────

        public Vector2    ContactPoint    => ContactData.Point;
        public Vector2    ContactNormal   => ContactData.Normal;
        public Vector2    GroundTangent   => ContactData.Tangent;
        public float      GroundAngle     => ContactData.Angle;
        public float      GroundDistance  => ContactData.Distance;
        public Collider2D GroundCollider  => ContactData.Collider;
        public Rigidbody2D GroundRigidbody => ContactData.Rigidbody;

        // ── Core update ───────────────────────────────────────────────────────

        /// <summary>
        /// Performs the capsule cast and updates all contact properties.
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
            IsGrounded       = HasGroundContact && body.linearVelocity.y <= groundedVerticalTolerance;

            if (HasGroundContact)
            {
                float angle = Vector2.Angle(hit.normal, Vector2.up);

                ContactData = new GroundContactData2D(
                    hit.point,
                    hit.normal,
                    angle,
                    hit.distance,
                    hit.collider,
                    hit.rigidbody
                );
            }
            else
            {
                ContactData = GroundContactData2D.None;
            }
        }

        // ── Gizmos ────────────────────────────────────────────────────────────

        private void OnDrawGizmosSelected()
        {
            // Ground check direction — visible in Edit and Play mode.
            if (playerCollider != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(playerCollider.bounds.center, Vector2.down * groundCheckDistance);
            }

            // Contact data — only meaningful at runtime when ground is detected.
            if (!Application.isPlaying || !HasGroundContact)
                return;

            // Contact point.
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(ContactData.Point, 0.05f);

            // Surface normal (blue).
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(ContactData.Point, ContactData.Normal * 0.5f);

            // Surface tangent (red).
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ContactData.Point, ContactData.Tangent * 0.5f);
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
