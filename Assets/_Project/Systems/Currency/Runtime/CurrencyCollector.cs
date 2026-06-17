using UnityEngine;

namespace KoeenjiDev.CurrencySystem
{
    public sealed class CurrencyCollector : MonoBehaviour
    {
        [SerializeField]
        private CurrencyWallet wallet;

        public CurrencyWallet Wallet => wallet;

        private void Awake()
        {
            if (wallet == null)
            {
                Debug.LogError(
                    $"{nameof(CurrencyCollector)} requires a {nameof(CurrencyWallet)} reference. " +
                    "Assign it in the Inspector.",
                    this);
            }
        }
    }
}
