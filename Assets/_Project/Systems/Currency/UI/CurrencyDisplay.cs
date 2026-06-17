using TMPro;
using UnityEngine;

namespace KoeenjiDev.CurrencySystem.UI
{
    public sealed class CurrencyDisplay : MonoBehaviour
    {
        [SerializeField]
        private CurrencyWallet wallet;

        [SerializeField]
        private TMP_Text balanceText;

        private void OnEnable()
        {
            if (wallet == null)
            {
                Debug.LogError(
                    $"{nameof(CurrencyDisplay)} requires a {nameof(CurrencyWallet)} reference. " +
                    "Assign it in the Inspector.",
                    this);
                return;
            }

            if (balanceText == null)
            {
                Debug.LogError(
                    $"{nameof(CurrencyDisplay)} requires a {nameof(TMP_Text)} reference. " +
                    "Assign it in the Inspector.",
                    this);
                return;
            }

            wallet.BalanceChanged += Refresh;
            Refresh(wallet.Balance);
        }

        private void OnDisable()
        {
            if (wallet == null)
            {
                return;
            }

            wallet.BalanceChanged -= Refresh;
        }

        private void Refresh(int balance)
        {
            balanceText.text = balance.ToString();
        }
    }
}
