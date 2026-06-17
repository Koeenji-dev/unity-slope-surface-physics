using UnityEngine;

namespace KoeenjiDev.PlayerVisual
{
    public class PlayerVisualController2D : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private static readonly int SpeedHash =
            Animator.StringToHash("Speed");

        private static readonly int VerticalVelocityHash =
            Animator.StringToHash("VerticalVelocity");

        private static readonly int IsGroundedHash =
            Animator.StringToHash("IsGrounded");

        private void Awake()
        {
            if (animator == null)
            {
                Debug.LogError(
                    "[PlayerVisualController2D] Animator reference is missing.",
                    this
                );
            }

            if (spriteRenderer == null)
            {
                Debug.LogError(
                    "[PlayerVisualController2D] SpriteRenderer reference is missing.",
                    this
                );
            }
        }

        public void SetMotionState(
            float horizontalSpeed,
            float verticalVelocity,
            bool isGrounded
        )
        {
            if (animator == null)
            {
                return;
            }

            animator.SetFloat(SpeedHash, Mathf.Abs(horizontalSpeed));
            animator.SetFloat(VerticalVelocityHash, verticalVelocity);
            animator.SetBool(IsGroundedHash, isGrounded);
        }

        public void SetFacingDirection(float direction)
        {
            if (direction == 0f || spriteRenderer == null)
            {
                return;
            }

            spriteRenderer.flipX = direction < 0f;
        }
    }
}