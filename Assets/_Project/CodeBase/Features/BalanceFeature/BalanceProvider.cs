using System;
using System.Numerics;
using _Project.CodeBase.Shared.Interfaces;

namespace _Project.CodeBase.Features.BalanceFeature
{
    public class BalanceProvider : IListener<BigInteger>, IUpdater<BigInteger>
    {
        public event Action<BigInteger> Changed;

        public void Update(BigInteger businessList)
        {
            Changed?.Invoke(businessList);
        }
    }
}
