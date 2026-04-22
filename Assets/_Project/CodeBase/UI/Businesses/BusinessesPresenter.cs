using System;
using System.Collections.Generic;
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
        private readonly Dictionary<int, BusinessCardView> _cardById = new();

        private readonly HashSet<int> _existingIds = new();
        private readonly List<int> _idsToRemove = new();

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

            _dataProvider.Changed += OnDataChanged;
        }

        private void OnDataChanged(IReadOnlyList<BusinessViewData> businessViewDataList)
        {
            _existingIds.Clear();
            _idsToRemove.Clear();

            foreach (var businessViewData in businessViewDataList)
            {
                _existingIds.Add(businessViewData.BusinessId);

                if (!_cardById.TryGetValue(businessViewData.BusinessId, out var card))
                {
                    card = _businessCardFactory.Create(_businessesView.CardsContainer);
                    _cardById.Add(businessViewData.BusinessId, card);
                }

                card.SetData(businessViewData);
            }

            foreach (var (id, _) in _cardById)
            {
                if (!_existingIds.Contains(id))
                {
                    _idsToRemove.Add(id);
                }
            }

            foreach (var cardId in _idsToRemove)
            {
                Object.Destroy(_cardById[cardId].gameObject);
                _cardById.Remove(cardId);
            }
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
