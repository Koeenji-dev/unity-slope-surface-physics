using NUnit.Framework;
using UnityEngine;

namespace KoeenjiDev.CurrencySystem.Tests
{
    [TestFixture]
    public class CurrencyWalletTests
    {
        private GameObject walletGameObject;
        private CurrencyWallet wallet;

        [SetUp]
        public void SetUp()
        {
            walletGameObject = new GameObject();
            wallet = walletGameObject.AddComponent<CurrencyWallet>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(walletGameObject);
        }

        [Test]
        public void NewWallet_StartsWithZeroBalance()
        {
            Assert.AreEqual(0, wallet.Balance);
        }

        [Test]
        public void TryAdd_PositiveAmount_IncreasesBalance()
        {
            bool result = wallet.TryAdd(5);

            Assert.IsTrue(result);
            Assert.AreEqual(5, wallet.Balance);
        }

        [Test]
        public void TryAdd_MultipleValidAmounts_AccumulatesBalance()
        {
            bool firstResult = wallet.TryAdd(5);
            bool secondResult = wallet.TryAdd(10);

            Assert.IsTrue(firstResult);
            Assert.IsTrue(secondResult);
            Assert.AreEqual(15, wallet.Balance);
        }

        [Test]
        public void TryAdd_Zero_ReturnsFalse()
        {
            bool result = wallet.TryAdd(0);

            Assert.IsFalse(result);
        }

        [Test]
        public void TryAdd_NegativeAmount_ReturnsFalse()
        {
            bool result = wallet.TryAdd(-1);

            Assert.IsFalse(result);
        }

        [Test]
        public void TryAdd_ValidAmount_RaisesBalanceChanged()
        {
            int callCount = 0;
            wallet.BalanceChanged += _ => callCount++;

            wallet.TryAdd(5);

            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void TryAdd_InvalidAmount_DoesNotRaiseBalanceChanged()
        {
            int callCount = 0;
            wallet.BalanceChanged += _ => callCount++;

            wallet.TryAdd(0);
            wallet.TryAdd(-3);

            Assert.AreEqual(0, callCount);
        }

        [Test]
        public void TryAdd_MultipleAmounts_RaisesCurrentBalance()
        {
            int lastEmitted = -1;
            int callCount = 0;
            wallet.BalanceChanged += value =>
            {
                callCount++;
                lastEmitted = value;
            };

            wallet.TryAdd(5);
            Assert.AreEqual(1, callCount);
            Assert.AreEqual(5, lastEmitted);

            wallet.TryAdd(10);
            Assert.AreEqual(2, callCount);
            Assert.AreEqual(15, lastEmitted);
        }

        [Test]
        public void TryAdd_AmountCausingOverflow_ReturnsFalse()
        {
            wallet.TryAdd(int.MaxValue);

            int balanceBeforeOverflow = wallet.Balance;
            Assert.AreEqual(int.MaxValue, balanceBeforeOverflow);

            int callCount = 0;
            wallet.BalanceChanged += _ => callCount++;

            bool result = wallet.TryAdd(1);

            Assert.IsFalse(result);
            Assert.AreEqual(balanceBeforeOverflow, wallet.Balance);
            Assert.AreEqual(0, callCount);
        }
    }
}
