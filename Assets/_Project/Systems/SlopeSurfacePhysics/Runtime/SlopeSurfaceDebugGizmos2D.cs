using UnityEngine;

namespace KoeenjiDev.SlopeSurfacePhysics
{
    /// <summary>
    /// Visualizes ground contact, slope, and surface data in the Scene View
    /// for the selected Player object. Does not participate in gameplay.
    ///
    /// Attach alongside GroundDetector2D on the Player root.
    /// Data is only available at runtime (Play Mode). In Edit Mode the geometric
    /// gizmos are hidden because GroundDetector2D has not yet run a cast.
    /// </summary>
    [RequireComponent(typeof(GroundDetector2D))]
    public sealed class SlopeSurfaceDebugGizmos2D : MonoBehaviour
    {
        [Header("Gizmo Sizes")]
        [Tooltip("Length of the ground normal arrow drawn from the contact point.")]
        [SerializeField, Min(0.05f)] private float normalLength  = 0.6f;

        [Tooltip("Length of the ground tangent arrow drawn from the contact point.")]
        [SerializeField, Min(0.05f)] private float tangentLength = 0.6f;

        [Tooltip("Radius of the contact point sphere.")]
        [SerializeField, Min(0.01f)] private float contactRadius = 0.06f;

        private GroundDetector2D groundDetector;

        private void Awake()
        {
            groundDetector = GetComponent<GroundDetector2D>();
        }

        private void OnDrawGizmosSelected()
        {
            // Resolve in Edit Mode because Awake does not run outside Play Mode.
            if (groundDetector == null)
                groundDetector = GetComponent<GroundDetector2D>();

            if (groundDetector == null)
                return;

            GroundContactData2D data       = groundDetector.ContactData;
            bool                hasContact = groundDetector.HasGroundContact;
            bool                isGrounded = groundDetector.IsGrounded;
            bool                isWalkable = groundDetector.IsGroundWalkable;

#if UNITY_EDITOR
            if (Application.isPlaying)
                DrawLabel(data, hasContact, isGrounded, isWalkable);
#endif

            // Geometric gizmos are only meaningful at runtime with an active contact.
            if (!Application.isPlaying || !hasContact)
                return;

            // Contact sphere: green = grounded, yellow = contact but not grounded.
            Gizmos.color = isGrounded ? Color.green : Color.yellow;
            Gizmos.DrawSphere(data.Point, contactRadius);

            // Ground normal (blue arrow from contact point).
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(data.Point, data.Normal * normalLength);

            // Ground tangent (red arrow from contact point).
            Gizmos.color = Color.red;
            Gizmos.DrawRay(data.Point, data.Tangent * tangentLength);
        }

#if UNITY_EDITOR
        private void DrawLabel(
            GroundContactData2D data,
            bool hasContact,
            bool isGrounded,
            bool isWalkable)
        {
            if (!hasContact)
            {
                UnityEditor.Handles.Label(
                    transform.position + Vector3.up * 1.0f,
                    "[AIRBORNE]");
                return;
            }

            SurfaceType2D surfaceType = data.HasSurfaceModifier
                ? data.SurfaceModifier.SurfaceType
                : SurfaceType2D.Normal;

            string label =
                (isGrounded ? "[GROUNDED]" : "[NOT GROUNDED]") + "\n" +
                (isWalkable ? "Walkable" : "Non-walkable")      + "\n" +
                $"Slope: {data.Angle:F1}\u00b0\n"               +
                $"Surface: {surfaceType}";

            if (data.HasSurfaceModifier)
            {
                label +=
                    $"\nSpd \u00d7{data.SurfaceModifier.MaxSpeedMultiplier:F2}" +
                    $"  Acc \u00d7{data.SurfaceModifier.AccelerationMultiplier:F2}" +
                    $"  Dec \u00d7{data.SurfaceModifier.DecelerationMultiplier:F2}" +
                    $"  Jump \u00d7{data.SurfaceModifier.JumpForceMultiplier:F2}";
            }

            Vector3 anchor = (Vector3)data.Point + Vector3.up * 0.8f;
            UnityEditor.Handles.Label(anchor, label);
        }
#endif
    }
}
