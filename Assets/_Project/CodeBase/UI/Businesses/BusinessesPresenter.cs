using System;
using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Features.BusinessFeature;
using _Project.CodeBase.Shared.Interfaces;
using _Project.CodeBase.UI.Factories;
using Object = UnityEngine.Object;

namespace _Project.CodeBase.UI.Businesses
{
    public class BusinessesPresenter : IDisposable
    {
        private readonly BusinessesView _businessesView;
        private readonly BusinessCardFactory _businessCardFactory;
        private readonly IListener<IReadOnlyList<BusinessViewData>> _dataProvider;
        private readonly ICommandWriter _commandWriter;
        private readonly Dictionary<int, BusinessCardView> _cards = new();

        public BusinessesPresenter(
            BusinessesView businessesView,
            BusinessCardFactory businessCardFactory,
            ICommandWriter commandWriter,
            IListener<IReadOnlyList<BusinessViewData>> dataProvider)
        {
            _businessesView = businessesView;
            _businessCardFactory = businessCardFactory;
            _commandWriter = commandWriter;
            _dataProvider = dataProvider;

            _dataProvider.Changed += DataUpdated;
        }

        private void DataUpdated(IReadOnlyList<BusinessViewData> viewModels)
        {
            var existingIds = new HashSet<int>();

            foreach (var vm in viewModels)
            {
                existingIds.Add(vm.Id);

                if (!_cards.TryGetValue(vm.Id, out var card))
                {
                    card = _businessCardFactory.Create(_businessesView.CardsContainer);
                    _cards.Add(vm.Id, card);
                }

                card.Bind(vm);
            }

            foreach (var kv in _cards.ToList())
            {
                if (!existingIds.Contains(kv.Key))
                {
                    Object.Destroy(kv.Value.gameObject);
                    _cards.Remove(kv.Key);
                }
            }
        }

        public void Dispose()
        {
            if (_dataProvider != null)
            {
                _dataProvider.Changed -= DataUpdated;
            }
        }
    }
}
