using System;
using UnityEngine;

namespace KoeenjiDev.CurrencySystem
{
    public sealed class CurrencyWallet : MonoBehaviour
    {
        private int balance;

        public int Balance => balance;

        public event Action<int> BalanceChanged;

        public bool TryAdd(int amount)
        {
            if (amount <= 0)
            {
                return false;
            }

            if (balance > int.MaxValue - amount)
            {
                return false;
            }

            balance += amount;
            BalanceChanged?.Invoke(balance);
            return true;
        }
    }
}
