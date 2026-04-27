using System;
using System.Numerics;

namespace _Project.CodeBase.Features.BalanceFeature.View
{
    public interface IBalanceViewListener
    {
        public event Action<BigInteger> BalanceChanged;
    }
}
