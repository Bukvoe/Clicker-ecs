using _Project.CodeBase.Features.BusinessFeature.Components;
using _Project.CodeBase.Features.BusinessFeature.View;
using _Project.CodeBase.Features.IncomeFeature.Components;
using _Project.CodeBase.Features.IncomeFeature.View;
using _Project.CodeBase.Features.UpgradesFeature.Components;
using _Project.CodeBase.Features.UpgradesFeature.Queries;
using _Project.CodeBase.Features.UpgradesFeature.View;
using _Project.CodeBase.Services;
using _Project.CodeBase.Shared;
using Leopotam.EcsLite;

namespace _Project.CodeBase.Features.BusinessFeature.Systems
{
    public class BusinessViewSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly IBusinessViewUpdater _businessUpdater;
        private readonly IncomeService _incomeService;
        private readonly ConfigService _configService;
        private readonly LevelUpService _levelUpService;

        private EcsFilter _allBusinessesFilter;
        private EcsFilter _dirtyUpgradesFilter;

        private UpgradeQuery _upgradeQuery;

        private EcsPool<Business> _businessPool;
        private EcsPool<BusinessDirty> _businessDirtyPool;
        private EcsPool<IncomeTimer> _incomeTimerPool;
        private EcsPool<OwnedBusiness> _ownedBusinessPool;
        private EcsPool<OwnedUpgrade> _ownedUpgradePool;
        private EcsPool<Upgrade> _upgradePool;
        private EcsPool<UpgradeDirty> _upgradeDirtyPool;

        public BusinessViewSystem(
            IBusinessViewUpdater businessUpdater,
            ConfigService configService,
            IncomeService incomeService,
            LevelUpService levelUpService)
        {
            _businessUpdater = businessUpdater;
            _incomeService = incomeService;
            _levelUpService = levelUpService;
            _configService = configService;
        }

        public void Init(EcsSystems systems)
        {
            var world = systems.GetWorld();

            _allBusinessesFilter = world.Filter<Business>().End();

            _dirtyUpgradesFilter = world.Filter<Upgrade>()
                .Inc<UpgradeDirty>()
                .End();

            _upgradeQuery = new UpgradeQuery(world);

            _businessDirtyPool = world.GetPool<BusinessDirty>();
            _businessPool = world.GetPool<Business>();
            _incomeTimerPool = world.GetPool<IncomeTimer>();
            _ownedUpgradePool = world.GetPool<OwnedUpgrade>();
            _upgradeDirtyPool = world.GetPool<UpgradeDirty>();
            _upgradePool = world.GetPool<Upgrade>();

            foreach (var entity in _allBusinessesFilter)
            {
                ref var business = ref _businessPool.Get(entity);
                _businessUpdater.CreateBusiness(business.Id);
            }
        }

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _allBusinessesFilter)
            {
                UpdateBusiness(entity);
                UpdateIncomeTimer(entity);
            }

            foreach (var dirtyUpgradeEntity in _dirtyUpgradesFilter)
            {
                UpdateUpgrade(dirtyUpgradeEntity);
            }
        }

        private void UpdateBusiness(int businessEntity)
        {
            ref var business = ref _businessPool.Get(businessEntity);

            if (!_businessDirtyPool.Has(businessEntity))
            {
                return;
            }

            _businessDirtyPool.Del(businessEntity);

            var upgradeIds = _upgradeQuery.GetOwnedUpgradeIds(business.Id);

            _businessUpdater.UpdateBusiness(new BusinessViewData
            {
                BusinessId = business.Id,
                Level = business.Level,
                Income = _incomeService.CalculateIncome(business, upgradeIds),
                Cost = _levelUpService.CalculateLevelUpCost(business),
            });
        }

        private void UpdateIncomeTimer(int businessEntity)
        {
            ref var business = ref _businessPool.Get(businessEntity);

            var incomeProgress = 0f;

            if (_configService.TryGetBusiness(business.Id, out var businessDefinition))
            {
                var time = _incomeTimerPool.Get(businessEntity).Time;
                incomeProgress = time.NormalizeProgress(businessDefinition.IncomeDelay);
            }

            _businessUpdater.UpdateIncomeProgress(new IncomeViewData
            {
                BusinessId = business.Id,
                Progress = incomeProgress,
            });
        }

        private void UpdateUpgrade(int upgradeEntity)
        {
            var upgrade = _upgradePool.Get(upgradeEntity);
            _upgradeDirtyPool.Del(upgradeEntity);

            _businessUpdater.UpdateUpgrade(new UpgradeViewData
            {
                BusinessId = upgrade.BusinessId,
                UpgradeId = upgrade.UpgradeId,
                IsBought = _ownedUpgradePool.Has(upgradeEntity),
            });

        }
    }
}
