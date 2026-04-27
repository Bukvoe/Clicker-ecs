using System;
using System.Collections.Generic;
using System.Numerics;
using _Project.CodeBase.Configs;
using _Project.CodeBase.Features.BusinessFeature.Components;
using _Project.CodeBase.Features.BusinessFeature.View;
using _Project.CodeBase.Features.IncomeFeature.View;
using _Project.CodeBase.Features.UpgradesFeature.Components;
using _Project.CodeBase.Features.UpgradesFeature.View;
using _Project.CodeBase.Services;
using _Project.CodeBase.Shared.Interfaces;
using _Project.CodeBase.UI.Factories;
using _Project.CodeBase.UI.Upgrades;
using Object = UnityEngine.Object;

namespace _Project.CodeBase.UI.Businesses
{
    public class BusinessesPresenter : IDisposable
    {
        private readonly BusinessesView _businessesView;
        private readonly BusinessCardFactory _businessCardFactory;
        private readonly IBusinessViewListener _businessViewListener;
        private readonly IRequestWriter _requestWriter;
        private readonly Dictionary<int, BusinessCardView> _cardById = new();
        private readonly ConfigService _configService;

        public BusinessesPresenter(
            BusinessesView businessesView,
            BusinessCardFactory businessCardFactory,
            IRequestWriter requestWriter,
            IBusinessViewListener businessViewListener,
            ConfigService configService)
        {
            _businessesView = businessesView;
            _businessCardFactory = businessCardFactory;
            _requestWriter = requestWriter;
            _businessViewListener = businessViewListener;
            _configService = configService;

            _businessViewListener.BusinessCreated += OnBusinessCreated;
            _businessViewListener.BusinessUpdated += OnBusinessUpdated;
            _businessViewListener.IncomeProgressUpdated += OnIncomeProgressUpdated;
            _businessViewListener.UpgradeUpdated += OnUpgradeUpdated;
        }

        private void OnBusinessCreated(int businessId)
        {
            if (_cardById.ContainsKey(businessId))
            {
                return;
            }

            CreateCard(businessId);
        }

        private void CreateCard(int businessId)
        {
            var card = _businessCardFactory.Create(_businessesView.CardsContainer);
            _cardById[businessId] = card;
            card.SetId(businessId);

            _configService.TryGetBusiness(businessId, out var businessDefinition);
            card.SetName(businessDefinition.Name);

            var upgradeDefinitions = new List<UpgradeDefinition>();

            foreach (var upgradeId in businessDefinition.UpgradeIds)
            {
                if (_configService.TryGetUpgrade(upgradeId, out var upgradeDefinition))
                {
                    upgradeDefinitions.Add(upgradeDefinition);
                }
            }

            card.InitializeUpgrades(upgradeDefinitions);
            card.LevelUpClicked += OnLevelUpClicked;
            card.UpgradeClicked += OnUpgradeClicked;
        }

        private void OnBusinessUpdated(BusinessViewData businessViewData)
        {
            if (!_cardById.TryGetValue(businessViewData.BusinessId, out var card))
            {
                return;
            }

            var displayIncome = GetDisplayIncome(businessViewData);

            card.SetData(businessViewData.Level, displayIncome, businessViewData.Cost);
        }

        private BigInteger GetDisplayIncome(BusinessViewData businessViewData)
        {
            if (businessViewData.Level > 0)
            {
                return businessViewData.Income;
            }

            return _configService.TryGetBusiness(businessViewData.BusinessId, out var businessDefinition)
                ? businessDefinition.BaseIncome
                : BigInteger.Zero;
        }

        private void OnIncomeProgressUpdated(IncomeViewData incomeViewData)
        {
            if (!_cardById.TryGetValue(incomeViewData.BusinessId, out var card))
            {
                return;
            }

            card.SetIncomeProgress(incomeViewData.Progress);
        }

        private void OnUpgradeUpdated(UpgradeViewData upgradeViewData)
        {
            if (_cardById.TryGetValue(upgradeViewData.BusinessId, out var card)
                && _configService.TryGetUpgrade(upgradeViewData.UpgradeId, out var upgradeDefinition))
            {
                card.UpdateUpgrade(upgradeDefinition, upgradeViewData.IsBought);
            }
        }

        private void OnLevelUpClicked(int businessId)
        {
            _requestWriter.WriteRequest(new BusinessLevelUpRequest
            {
                BusinessId = businessId,
            });
        }

        private void OnUpgradeClicked(UpgradeClickEvent upgradeClickEvent)
        {
            _requestWriter.WriteRequest(new BuyUpgradeRequest
            {
                BusinessId = upgradeClickEvent.BusinessId,
                UpgradeId = upgradeClickEvent.UpgradeId,
            });
        }

        public void Dispose()
        {
            if (_businessViewListener != null)
            {
                _businessViewListener.BusinessCreated -= OnBusinessCreated;
                _businessViewListener.BusinessUpdated -= OnBusinessUpdated;
                _businessViewListener.IncomeProgressUpdated -= OnIncomeProgressUpdated;
                _businessViewListener.UpgradeUpdated -= OnUpgradeUpdated;
            }

            foreach (var (_, card) in _cardById)
            {
                if (card != null)
                {
                    card.LevelUpClicked -= OnLevelUpClicked;
                    card.UpgradeClicked -= OnUpgradeClicked;
                    Object.Destroy(card.gameObject);
                }
            }

            _cardById.Clear();
        }
    }
}
