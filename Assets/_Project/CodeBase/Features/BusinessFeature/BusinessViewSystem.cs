using System.Collections.Generic;
using _Project.CodeBase.Features.IncomeFeature;
using _Project.CodeBase.Services;
using _Project.CodeBase.Shared;
using _Project.CodeBase.Shared.Interfaces;
using Leopotam.EcsLite;
using UnityEngine;

namespace _Project.CodeBase.Features.BusinessFeature
{
    public class BusinessViewSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly IUpdater<IReadOnlyList<BusinessViewData>> _businessListUpdater;
        private readonly List<BusinessViewData> _businesses = new();
        private readonly IncomeService _incomeService;
        private readonly ConfigService _configService;

        private EcsFilter _allBusinessesFilter;
        private EcsPool<Business> _businessPool;
        private EcsPool<IncomeTimer> _incomeProgressPool;

        public BusinessViewSystem(
            IUpdater<IReadOnlyList<BusinessViewData>> businessListUpdater,
            ConfigService configService,
            IncomeService incomeService)
        {
            _businessListUpdater = businessListUpdater;
            _incomeService = incomeService;
            _configService = configService;
        }

        public void Init(EcsSystems systems)
        {
            var world = systems.GetWorld();

            _allBusinessesFilter = world
                .Filter<Business>()
                .Inc<IncomeTimer>()
                .End();

            _businessPool = world.GetPool<Business>();
            _incomeProgressPool = world.GetPool<IncomeTimer>();
        }

        public void Run(EcsSystems systems)
        {
            _businesses.Clear();

            foreach (var entity in _allBusinessesFilter)
            {
                ref var business = ref _businessPool.Get(entity);
                ref var incomeProgress = ref _incomeProgressPool.Get(entity);
                var businessDefinition = _configService.GetBusiness(business.Id);

                if (businessDefinition == null)
                {
                    Debug.LogError($"{nameof(businessDefinition)} is null | {business.Id}");
                    continue;
                }

                _businesses.Add(new BusinessViewData
                {
                    BusinessId = business.Id,
                    Name = businessDefinition.Name,
                    Level = business.Level,
                    Income = business.Level > 0 ? _incomeService.CalculateIncome(business) : businessDefinition.BaseIncome,
                    IncomeProgress = incomeProgress.Time.NormalizeProgress(businessDefinition.IncomeDelay),
                    Cost = businessDefinition.BaseCost,
                });
            }

            _businessListUpdater.Update(_businesses);
        }

    }
}
