using _Project.CodeBase.Features.BalanceFeature.Components;
using _Project.CodeBase.Features.BusinessFeature.Components;
using _Project.CodeBase.Features.UpgradesFeature.Components;
using _Project.CodeBase.Services;
using _Project.CodeBase.Services.Player;
using _Project.CodeBase.Shared;
using Leopotam.EcsLite;
using UnityEngine;

namespace _Project.CodeBase.Features.UpgradesFeature.Systems
{
    public class BuyUpgradeSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly ConfigService _configService;
        private readonly BusinessService _businessService;
        private readonly UpgradeService _upgradeService;
        private readonly IPlayerService _playerService;

        private EcsWorld _world;

        private EcsFilter _buyUpgradeRequestFilter;

        private EcsPool<Balance> _balancePool;
        private EcsPool<BalanceDirty> _balanceDirtyPool;
        private EcsPool<BusinessDirty> _businessDirtyPool;
        private EcsPool<BuyUpgradeRequest> _buyUpgradePool;
        private EcsPool<OwnedBusiness> _ownedBusinessPool;
        private EcsPool<OwnedUpgrade> _ownedUpgradePool;
        private EcsPool<UpgradeDirty> _upgradeDirtyPool;

        public BuyUpgradeSystem(
            ConfigService configService,
            BusinessService businessService,
            UpgradeService upgradeService,
            IPlayerService playerService)
        {
            _configService = configService;
            _businessService = businessService;
            _upgradeService = upgradeService;
            _playerService = playerService;
        }

        public void Init(EcsSystems systems)
        {
            _world = systems.GetWorld();

            _buyUpgradeRequestFilter = _world.Filter<BuyUpgradeRequest>().End();

            _balanceDirtyPool = _world.GetPool<BalanceDirty>();
            _balancePool = _world.GetPool<Balance>();
            _businessDirtyPool = _world.GetPool<BusinessDirty>();
            _buyUpgradePool = _world.GetPool<BuyUpgradeRequest>();
            _ownedBusinessPool = _world.GetPool<OwnedBusiness>();
            _ownedUpgradePool = _world.GetPool<OwnedUpgrade>();
            _upgradeDirtyPool = _world.GetPool<UpgradeDirty>();
        }

        public void Run(EcsSystems systems)
        {
            var playerEntity = _playerService.PlayerEntity;
            ref var playerBalance = ref _balancePool.Get(playerEntity);

            foreach (var entity in _buyUpgradeRequestFilter)
            {
                var request = _buyUpgradePool.Get(entity);
                _buyUpgradePool.Del(entity);

                if (!_businessService.TryGetEntity(request.BusinessId, out var businessEntity))
                {
                    continue;
                }

                if (!_ownedBusinessPool.Has(businessEntity))
                {
                    continue;
                }

                if (!_upgradeService.TryGetEntity(request.UpgradeId, out var upgradeEntity))
                {
                    Debug.LogError($"{nameof(request.UpgradeId)} not found");
                    continue;
                }

                if (_ownedUpgradePool.Has(upgradeEntity))
                {
                    continue;
                }

                _configService.TryGetUpgrade(request.UpgradeId, out var upgradeDefinition);

                if (playerBalance.Value < upgradeDefinition.Cost)
                {
                    continue;
                }

                playerBalance.Value -= upgradeDefinition.Cost;
                _ownedUpgradePool.Add(upgradeEntity);

                _upgradeDirtyPool.SetDirty(upgradeEntity);
                _balanceDirtyPool.SetDirty(playerEntity);
                _businessDirtyPool.SetDirty(businessEntity);
            }
        }
    }
}
