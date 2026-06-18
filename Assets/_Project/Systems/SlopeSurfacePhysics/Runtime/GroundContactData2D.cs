using UnityEngine;

namespace KoeenjiDev.SlopeSurfacePhysics
{
    /// <summary>
    /// Immutable snapshot of the ground contact detected below the Player
    /// during a single UpdateGroundState call.
    /// Use GroundContactData2D.None when no contact exists.
    /// </summary>
    public readonly struct GroundContactData2D
    {
        /// <summary>True when the capsule cast detected a ground collider.</summary>
        public bool HasHit { get; }

        /// <summary>World-space contact point on the detected surface.</summary>
        public Vector2 Point { get; }

        /// <summary>World-space surface normal at the contact point.</summary>
        public Vector2 Normal { get; }

        /// <summary>
        /// Rightward surface tangent derived from the normal.
        /// Points in the direction of positive horizontal travel along the surface.
        /// </summary>
        public Vector2 Tangent { get; }

        /// <summary>Slope angle in degrees, measured from Vector2.up.</summary>
        public float Angle { get; }

        /// <summary>Distance from the cast origin to the contact point.</summary>
        public float Distance { get; }

        /// <summary>Collider2D detected by the ground cast. Null when HasHit is false.</summary>
        public Collider2D Collider { get; }

        /// <summary>
        /// Rigidbody2D attached to the detected collider, if any.
        /// Null for static geometry.
        /// </summary>
        public Rigidbody2D Rigidbody { get; }

        /// <summary>
        /// Safe no-contact state.
        /// HasHit is false. Normal is Vector2.up. Tangent is Vector2.right.
        /// </summary>
        public static readonly GroundContactData2D None = new GroundContactData2D(
            hasHit:    false,
            point:     Vector2.zero,
            normal:    Vector2.up,
            angle:     0f,
            distance:  0f,
            collider:  null,
            rigidbody: null
        );

        /// <summary>
        /// Constructs a populated contact snapshot from a valid RaycastHit2D result.
        /// </summary>
        public GroundContactData2D(
            Vector2 point,
            Vector2 normal,
            float angle,
            float distance,
            Collider2D collider,
            Rigidbody2D rigidbody)
        {
            HasHit    = true;
            Point     = point;
            Normal    = normal;
            Tangent   = new Vector2(normal.y, -normal.x);
            Angle     = angle;
            Distance  = distance;
            Collider  = collider;
            Rigidbody = rigidbody;
        }

        private GroundContactData2D(
            bool hasHit,
            Vector2 point,
            Vector2 normal,
            float angle,
            float distance,
            Collider2D collider,
            Rigidbody2D rigidbody)
        {
            HasHit    = hasHit;
            Point     = point;
            Normal    = normal;
            Tangent   = new Vector2(normal.y, -normal.x);
            Angle     = angle;
            Distance  = distance;
            Collider  = collider;
            Rigidbody = rigidbody;
        }
    }
}
