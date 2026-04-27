using System.Numerics;
using _Project.CodeBase.Features.BalanceFeature.Components;
using _Project.CodeBase.Features.BusinessFeature.Components;
using _Project.CodeBase.Features.IncomeFeature.Components;
using _Project.CodeBase.Features.UpgradesFeature.Queries;
using _Project.CodeBase.Services;
using _Project.CodeBase.Services.Player;
using _Project.CodeBase.Shared;
using Leopotam.EcsLite;

namespace _Project.CodeBase.Features.IncomeFeature.Systems
{
    public class IncomeSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly IncomeService _incomeService;
        private readonly IPlayerService _playerService;

        private EcsFilter _incomeBusinessesFilter;
        private EcsFilter _ownedUpgradesFilter;

        private UpgradeQuery _upgradeQuery;

        private EcsPool<Balance> _balancePool;
        private EcsPool<BalanceDirty> _balanceDirtyPool;
        private EcsPool<Business> _businessPool;
        private EcsPool<IncomeTimer> _timerPool;


        public IncomeSystem(IncomeService incomeService, IPlayerService playerService)
        {
            _incomeService = incomeService;
            _playerService = playerService;
        }

        public void Init(EcsSystems systems)
        {
            var world = systems.GetWorld();

            _incomeBusinessesFilter = world.Filter<Business>()
                .Inc<OwnedBusiness>()
                .Inc<IncomeTimer>()
                .End();

            _upgradeQuery = new UpgradeQuery(world);

            _balanceDirtyPool = world.GetPool<BalanceDirty>();
            _balancePool = world.GetPool<Balance>();
            _businessPool = world.GetPool<Business>();
            _timerPool = world.GetPool<IncomeTimer>();
        }

        public void Run(EcsSystems systems)
        {
            var totalIncome = CalculateTotalIncome();
            ApplyIncome(totalIncome);
        }

        private BigInteger CalculateTotalIncome()
        {
            BigInteger totalIncome = 0;

            foreach (var entity in _incomeBusinessesFilter)
            {
                ref var business = ref _businessPool.Get(entity);
                ref var timer = ref _timerPool.Get(entity);

                var interval = _incomeService.GetIncomeInterval(business.Id);

                if (timer.Time < interval)
                {
                    continue;
                }

                var incomeCyclesCompleted = (int)(timer.Time / interval);
                timer.Time -= incomeCyclesCompleted * interval;

                var ownedUpgrades = _upgradeQuery.GetOwnedUpgradeIds(business.Id);
                totalIncome += _incomeService.CalculateIncome(business, ownedUpgrades) * incomeCyclesCompleted;
            }

            return totalIncome;
        }

        private void ApplyIncome(BigInteger income)
        {
            if (income < 1)
            {
                return;
            }

            var playerEntity = _playerService.PlayerEntity;
            ref var playerBalance = ref _balancePool.Get(playerEntity);

            playerBalance.Value += income;
            _balanceDirtyPool.SetDirty(playerEntity);
        }
    }
}
