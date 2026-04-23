using System;
using System.Numerics;
using _Project.CodeBase.Shared.Interfaces;

namespace _Project.CodeBase.UI.Balance
{
    public class BalancePresenter : IDisposable
    {
        private readonly BalanceView _view;
        private readonly IListener<BigInteger> _dataProvider;

        public BalancePresenter(BalanceView view, IListener<BigInteger> dataProvider)
        {
            _view = view;
            _dataProvider = dataProvider;

            _dataProvider.Changed += OnDataChanged;
        }

        private void OnDataChanged(BigInteger newBalance)
        {
            _view.SetBalance(newBalance);
        }

        public void Dispose()
        {
            if (_dataProvider != null)
            {
                _dataProvider.Changed -= OnDataChanged;
            }
        }
    }
}
