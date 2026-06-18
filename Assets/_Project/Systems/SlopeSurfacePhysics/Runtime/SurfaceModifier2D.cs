using UnityEngine;

namespace KoeenjiDev.SlopeSurfacePhysics
{
    /// <summary>
    /// Surface types supported by the slope physics system.
    /// </summary>
    public enum SurfaceType2D
    {
        Normal,
        Ice,
        Mud,
        Sticky,
        JumpBoost
    }

    /// <summary>
    /// Placed on a ground GameObject to override the default movement response
    /// for any Player standing on that surface.
    ///
    /// Exposes read-only multipliers consumed by PlayerSlopeController2D.
    /// Does not move the Player, read input, play audio, or emit particles.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SurfaceModifier2D : MonoBehaviour
    {
        [SerializeField] private SurfaceType2D surfaceType = SurfaceType2D.Normal;

        [Header("Movement Multipliers")]
        [Tooltip("Multiplier applied to the Player's maximum movement speed.")]
        [SerializeField, Min(0f)] private float maxSpeedMultiplier     = 1.00f;

        [Tooltip("Multiplier applied to ground acceleration when input is active.")]
        [SerializeField, Min(0f)] private float accelerationMultiplier = 1.00f;

        [Tooltip("Multiplier applied to ground deceleration when input is released.")]
        [SerializeField, Min(0f)] private float decelerationMultiplier = 1.00f;

        [Tooltip("Multiplier applied to the Player's jump speed on this surface.")]
        [SerializeField, Min(0f)] private float jumpForceMultiplier    = 1.00f;

        /// <summary>Identifies which surface preset this modifier represents.</summary>
        public SurfaceType2D SurfaceType         => surfaceType;

        /// <summary>Fraction of the base maximum speed to allow on this surface.</summary>
        public float MaxSpeedMultiplier          => maxSpeedMultiplier;

        /// <summary>Fraction of base acceleration applied while input is active.</summary>
        public float AccelerationMultiplier      => accelerationMultiplier;

        /// <summary>Fraction of base deceleration applied when input is released.</summary>
        public float DecelerationMultiplier      => decelerationMultiplier;

        /// <summary>Fraction of base jump speed applied when jumping from this surface.</summary>
        public float JumpForceMultiplier         => jumpForceMultiplier;

        // ── Preset helper ─────────────────────────────────────────────────────

        /// <summary>
        /// Applies the recommended default multipliers for the current SurfaceType.
        /// Call this from the Inspector context menu after changing the surface type.
        ///
        /// Recommended values:
        ///   Normal    — maxSpeed 1.00 | accel 1.00 | decel 1.00 | jump 1.00
        ///   Ice       — maxSpeed 1.05 | accel 0.35 | decel 0.15 | jump 1.00
        ///   Mud       — maxSpeed 0.60 | accel 0.55 | decel 0.80 | jump 0.85
        ///   Sticky    — maxSpeed 0.75 | accel 0.70 | decel 1.80 | jump 0.70
        ///   JumpBoost — maxSpeed 1.00 | accel 1.00 | decel 1.00 | jump 2.00
        /// </summary>
        [ContextMenu("Apply Preset Values")]
        private void ApplyPresetValues()
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Apply Surface Preset");
#endif

            switch (surfaceType)
            {
                case SurfaceType2D.Normal:
                    maxSpeedMultiplier     = 1.00f;
                    accelerationMultiplier = 1.00f;
                    decelerationMultiplier = 1.00f;
                    jumpForceMultiplier    = 1.00f;
                    break;
                case SurfaceType2D.Ice:
                    maxSpeedMultiplier     = 1.05f;
                    accelerationMultiplier = 0.35f;
                    decelerationMultiplier = 0.15f;
                    jumpForceMultiplier    = 1.00f;
                    break;
                case SurfaceType2D.Mud:
                    maxSpeedMultiplier     = 0.60f;
                    accelerationMultiplier = 0.55f;
                    decelerationMultiplier = 0.80f;
                    jumpForceMultiplier    = 0.85f;
                    break;
                case SurfaceType2D.Sticky:
                    maxSpeedMultiplier     = 0.75f;
                    accelerationMultiplier = 0.70f;
                    decelerationMultiplier = 1.80f;
                    jumpForceMultiplier    = 0.70f;
                    break;
                case SurfaceType2D.JumpBoost:
                    maxSpeedMultiplier     = 1.00f;
                    accelerationMultiplier = 1.00f;
                    decelerationMultiplier = 1.00f;
                    jumpForceMultiplier    = 2.00f;
                    break;
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        private void OnValidate()
        {
            if (maxSpeedMultiplier     < 0f) maxSpeedMultiplier     = 0f;
            if (accelerationMultiplier < 0f) accelerationMultiplier = 0f;
            if (decelerationMultiplier < 0f) decelerationMultiplier = 0f;
            if (jumpForceMultiplier    < 0f) jumpForceMultiplier    = 0f;
        }
    }
}
