using UnityEngine;

namespace KoeenjiDev.CurrencySystem
{
    public sealed class CurrencyPickup : MonoBehaviour
    {
        [SerializeField, Min(1)]
        private int value = 1;

        [SerializeField]
        private AudioClip collectionSound;

        public int Value => value;

        private bool isCollected;
        private Collider2D pickupCollider;

        private void Awake()
        {
            pickupCollider = GetComponent<Collider2D>();

            if (pickupCollider == null)
            {
                Debug.LogError(
                    $"{nameof(CurrencyPickup)} requires a {nameof(Collider2D)} component on the same GameObject.",
                    this);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isCollected)
            {
                return;
            }

            CurrencyCollector collector = other.GetComponentInParent<CurrencyCollector>();

            if (collector == null)
            {
                return;
            }

            if (collector.Wallet == null)
            {
                return;
            }

            bool added = collector.Wallet.TryAdd(value);

            if (!added)
            {
                return;
            }

            isCollected = true;

            if (pickupCollider != null)
            {
                pickupCollider.enabled = false;
            }

            if (collectionSound != null)
            {
                AudioSource.PlayClipAtPoint(collectionSound, transform.position);
            }

            Destroy(gameObject);
        }
    }
}
