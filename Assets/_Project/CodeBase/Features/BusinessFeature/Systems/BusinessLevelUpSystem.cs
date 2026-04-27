using _Project.CodeBase.Features.BalanceFeature.Components;
using _Project.CodeBase.Features.BusinessFeature.Components;
using _Project.CodeBase.Services;
using _Project.CodeBase.Services.Player;
using _Project.CodeBase.Shared;
using Leopotam.EcsLite;
using UnityEngine;

namespace _Project.CodeBase.Features.BusinessFeature.Systems
{
    public class BusinessLevelUpSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly IPlayerService _playerService;
        private readonly LevelUpService _levelUpService;
        private readonly BusinessService _businessService;

        private EcsFilter _requestFilter;
        private EcsPool<BusinessLevelUpRequest> _requestPool;
        private EcsPool<Business> _businessPool;
        private EcsPool<BusinessDirty> _businessDirtyPool;
        private EcsPool<OwnedBusiness> _ownedBusinessPool;
        private EcsPool<Balance> _balancePool;
        private EcsPool<BalanceDirty> _balanceDirtyPool;

        public BusinessLevelUpSystem(
            IPlayerService playerService,
            LevelUpService levelUpService,
            BusinessService businessService)
        {
            _playerService = playerService;
            _levelUpService = levelUpService;
            _businessService = businessService;
        }

        public void Init(EcsSystems systems)
        {
            var world = systems.GetWorld();

            _requestFilter = world.Filter<BusinessLevelUpRequest>().End();

            _balanceDirtyPool = world.GetPool<BalanceDirty>();
            _balancePool = world.GetPool<Balance>();
            _businessDirtyPool = world.GetPool<BusinessDirty>();
            _businessPool = world.GetPool<Business>();
            _ownedBusinessPool = world.GetPool<OwnedBusiness>();
            _requestPool = world.GetPool<BusinessLevelUpRequest>();
        }

        public void Run(EcsSystems systems)
        {
            var playerEntity = _playerService.PlayerEntity;
            ref var balance = ref _balancePool.Get(playerEntity);

            foreach (var requestEntity in _requestFilter)
            {
                var request = _requestPool.Get(requestEntity);
                _requestPool.Del(requestEntity);

                if (!_businessService.TryGetEntity(request.BusinessId, out var businessEntity))
                {
                    Debug.LogError($"{nameof(businessEntity)} not found");
                    continue;
                }

                ref var business = ref _businessPool.Get(businessEntity);
                var levelUpCost = _levelUpService.CalculateLevelUpCost(business);

                if (balance.Value < levelUpCost)
                {
                    continue;
                }

                balance.Value -= levelUpCost;

                if (!_ownedBusinessPool.Has(businessEntity))
                {
                    _ownedBusinessPool.Add(businessEntity);
                }

                business.Level++;

                _businessDirtyPool.SetDirty(businessEntity);
                _balanceDirtyPool.SetDirty(playerEntity);
            }
        }
    }
}
