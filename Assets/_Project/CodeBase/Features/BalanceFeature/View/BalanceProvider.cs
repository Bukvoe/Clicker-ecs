using System;
using System.Numerics;

namespace _Project.CodeBase.Features.BalanceFeature.View
{
    public class BalanceViewProvider : IBalanceViewListener, IBalanceViewUpdater
    {
        public event Action<BigInteger> BalanceChanged;

        public void UpdateBalance(BigInteger balance)
        {
            BalanceChanged?.Invoke(balance);
        }
    }
}
