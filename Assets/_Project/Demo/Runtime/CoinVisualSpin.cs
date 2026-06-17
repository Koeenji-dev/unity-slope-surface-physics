using UnityEngine;

namespace KoeenjiDev.CurrencySystem.Demo
{
    public sealed class CoinVisualSpin : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        [SerializeField, Min(0f)]
        private float spinSpeed = 3f;

        [SerializeField, Range(0.01f, 1f)]
        private float minimumScaleX = 0.15f;

        private Vector3 initialScale;

        private void Awake()
        {
            if (target == null)
            {
                Debug.LogError(
                    $"{nameof(CoinVisualSpin)} requires a {nameof(Transform)} target reference. " +
                    "Assign it in the Inspector.",
                    this);
                return;
            }

            initialScale = target.localScale;
        }

        private void Update()
        {
            if (target == null)
            {
                return;
            }

            float t = Mathf.Abs(Mathf.Cos(Time.time * spinSpeed));
            float scaleX = minimumScaleX + (initialScale.x - minimumScaleX) * t;

            target.localScale = new Vector3(scaleX, initialScale.y, initialScale.z);
        }
    }
}
