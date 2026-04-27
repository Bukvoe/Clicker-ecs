using System;
using System.Numerics;
using _Project.CodeBase.Features.BalanceFeature.View;

namespace _Project.CodeBase.UI.Balance
{
    public class BalancePresenter : IDisposable
    {
        private readonly BalanceView _view;
        private readonly IBalanceViewListener _balanceViewListener;

        public BalancePresenter(BalanceView view, IBalanceViewListener balanceViewListener)
        {
            _view = view;
            _balanceViewListener = balanceViewListener;

            _balanceViewListener.BalanceChanged += OnBalanceChanged;
        }

        private void OnBalanceChanged(BigInteger newBalance)
        {
            _view.SetBalance(newBalance);
        }

        public void Dispose()
        {
            if (_balanceViewListener != null)
            {
                _balanceViewListener.BalanceChanged -= OnBalanceChanged;
            }
        }
    }
}
