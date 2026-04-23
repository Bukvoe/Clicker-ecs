using System.Numerics;
using _Project.CodeBase.Features.BalanceFeature;
using _Project.CodeBase.Features.BusinessFeature;
using _Project.CodeBase.Services;
using Leopotam.EcsLite;

namespace _Project.CodeBase.Features.IncomeFeature
{
    public class IncomeSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly IncomeService _incomeService;

        private EcsWorld _world;
        private EcsFilter _incomeBusinessesFilter;
        private EcsFilter _playerBalanceFilter;
        private EcsPool<Business> _businessPool;
        private EcsPool<IncomeTimer> _timerPool;
        private EcsPool<Balance> _balancePool;
        private EcsPool<BalanceDirty> _balanceDirtyPool;
        private int _playerEntity;

        public IncomeSystem(IncomeService incomeService)
        {
            _incomeService = incomeService;
        }

        public void Init(EcsSystems systems)
        {
            _world = systems.GetWorld();

            _incomeBusinessesFilter = _world.Filter<Business>()
                .Inc<OwnedBusiness>()
                .Inc<IncomeTimer>()
                .End();

            _playerEntity = _world.Filter<Player>()
                .Inc<Balance>()
                .End()
                .GetRawEntities()[0];

            _businessPool = _world.GetPool<Business>();
            _timerPool = _world.GetPool<IncomeTimer>();
            _balancePool = _world.GetPool<Balance>();
            _balanceDirtyPool = _world.GetPool<BalanceDirty>();
        }

        public void Run(EcsSystems systems)
        {
            ref var playerBalance = ref _balancePool.Get(_playerEntity);

            BigInteger totalIncome = 0;

            foreach (var entity in _incomeBusinessesFilter)
            {
                ref var business = ref _businessPool.Get(entity);
                ref var timer = ref _timerPool.Get(entity);

                var delay = _incomeService.GetIncomeDelay(business.Id);

                if (timer.Time < delay)
                {
                    continue;
                }

                var incomeCyclesCompleted = (int)(timer.Time / delay);
                timer.Time -= incomeCyclesCompleted * delay;

                totalIncome += _incomeService.CalculateIncome(business) * incomeCyclesCompleted;
            }

            if (totalIncome > 0)
            {
                playerBalance.Value += totalIncome;

                if (!_balanceDirtyPool.Has(_playerEntity))
                {
                    _balanceDirtyPool.Add(_playerEntity);
                }
            }
        }
    }
}
