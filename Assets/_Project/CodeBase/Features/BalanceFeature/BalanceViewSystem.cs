using System.Numerics;
using _Project.CodeBase.Shared.Interfaces;
using Leopotam.EcsLite;

namespace _Project.CodeBase.Features.BalanceFeature
{
    public class BalanceViewSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly IUpdater<BigInteger> _balanceUpdater;

        private EcsWorld _world;
        private EcsPool<Balance> _balancePool;
        private EcsPool<BalanceDirty> _balanceDirtyPool;
        private int _playerEntity;

        public BalanceViewSystem(IUpdater<BigInteger> balanceUpdater)
        {
            _balanceUpdater = balanceUpdater;
        }

        public void Init(EcsSystems systems)
        {
            _world = systems.GetWorld();

            _playerEntity = _world.Filter<Player>()
                .Inc<Balance>()
                .End()
                .GetRawEntities()[0];

            _balancePool = _world.GetPool<Balance>();
            _balanceDirtyPool = _world.GetPool<BalanceDirty>();
        }

        public void Run(EcsSystems systems)
        {
            if (!_balanceDirtyPool.Has(_playerEntity))
            {
                return;
            }

            ref var balance = ref _balancePool.Get(_playerEntity);
            _balanceUpdater.Update(balance.Value);
            _balanceDirtyPool.Del(_playerEntity);
        }
    }
}
