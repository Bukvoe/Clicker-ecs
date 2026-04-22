using System;
using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Features.BusinessFeature;
using _Project.CodeBase.Services;
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
        private readonly ConfigService _configService;
        private readonly ICommandWriter _commandWriter;
        private readonly Dictionary<int, BusinessCardView> _cards = new();

        public BusinessesPresenter(
            BusinessesView businessesView,
            BusinessCardFactory businessCardFactory,
            ICommandWriter commandWriter,
            IListener<IReadOnlyList<BusinessViewData>> dataProvider,
            ConfigService configService)
        {
            _businessesView = businessesView;
            _businessCardFactory = businessCardFactory;
            _commandWriter = commandWriter;
            _dataProvider = dataProvider;
            _configService = configService;

            _dataProvider.Changed += DataUpdated;
        }

        private void DataUpdated(IReadOnlyList<BusinessViewData> viewModels)
        {
            var existingIds = new HashSet<int>();

            foreach (var businessViewData in viewModels)
            {
                existingIds.Add(businessViewData.Id);

                if (!_cards.TryGetValue(businessViewData.Id, out var card))
                {
                    card = _businessCardFactory.Create(_businessesView.CardsContainer);
                    _cards.Add(businessViewData.Id, card);

                    var businessDefinition = _configService.GetBusiness(businessViewData.Id);

                    if (businessDefinition != null)
                    {
                        card.SetName(businessDefinition.Name);
                    }
                }

                card.SetLevel(businessViewData.Level);
                card.SetIncome(businessViewData.Income);
            }

            foreach (var (id, businessCardView) in _cards.ToList())
            {
                if (!existingIds.Contains(id))
                {
                    Object.Destroy(businessCardView.gameObject);
                    _cards.Remove(id);
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
