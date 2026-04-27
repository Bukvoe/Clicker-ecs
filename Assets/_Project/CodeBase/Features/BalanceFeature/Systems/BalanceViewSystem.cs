using _Project.CodeBase.Features.BalanceFeature.Components;
using _Project.CodeBase.Features.BalanceFeature.View;
using _Project.CodeBase.Services.Player;
using Leopotam.EcsLite;

namespace _Project.CodeBase.Features.BalanceFeature.Systems
{
    public class BalanceViewSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly IPlayerService _playerService;
        private readonly IBalanceViewUpdater _balanceUpdater;

        private EcsPool<Balance> _balancePool;
        private EcsPool<BalanceDirty> _balanceDirtyPool;

        public BalanceViewSystem(IPlayerService playerService, IBalanceViewUpdater balanceUpdater)
        {
            _playerService = playerService;
            _balanceUpdater = balanceUpdater;
        }

        public void Init(EcsSystems systems)
        {
            var world = systems.GetWorld();

            _balancePool = world.GetPool<Balance>();
            _balanceDirtyPool = world.GetPool<BalanceDirty>();
        }

        public void Run(EcsSystems systems)
        {
            var playerEntity = _playerService.PlayerEntity;

            if (!_balanceDirtyPool.Has(playerEntity))
            {
                return;
            }

            _balanceDirtyPool.Del(playerEntity);

            ref var balance = ref _balancePool.Get(playerEntity);
            _balanceUpdater.UpdateBalance(balance.Value);
        }
    }
}
