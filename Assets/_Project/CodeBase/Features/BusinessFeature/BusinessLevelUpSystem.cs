using _Project.CodeBase.Features.BalanceFeature;
using _Project.CodeBase.Services;
using Leopotam.EcsLite;
using UnityEngine;

namespace _Project.CodeBase.Features.BusinessFeature
{
    public class BusinessLevelUpSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly LevelUpService _levelUpService;

        private EcsWorld _world;
        private EcsFilter _levelUpCommandFilter;
        private EcsFilter _businessFilter;
        private EcsPool<BusinessLevelUpCommand> _levelUpCommandPool;
        private EcsPool<Business> _businessPool;
        private EcsPool<OwnedBusiness> _ownedBusinessPool;
        private EcsPool<Balance> _balancePool;
        private EcsPool<BalanceDirty> _balanceDirtyPool;
        private int _playerEntity;

        public BusinessLevelUpSystem(LevelUpService levelUpService)
        {
            _levelUpService = levelUpService;
        }

        public void Init(EcsSystems systems)
        {
            _world = systems.GetWorld();

            _playerEntity = _world.Filter<Player>()
                .Inc<Balance>()
                .End()
                .GetRawEntities()[0];

            _businessFilter = _world.Filter<Business>().End();
            _levelUpCommandFilter = _world.Filter<BusinessLevelUpCommand>().End();

            _levelUpCommandPool = _world.GetPool<BusinessLevelUpCommand>();
            _businessPool = _world.GetPool<Business>();
            _ownedBusinessPool = _world.GetPool<OwnedBusiness>();
            _balancePool = _world.GetPool<Balance>();
            _balanceDirtyPool = _world.GetPool<BalanceDirty>();
        }

        public void Run(EcsSystems systems)
        {
            ref var playerBalance = ref _balancePool.Get(_playerEntity);

            foreach (var levelUpEntity in _levelUpCommandFilter)
            {
                ref var levelUpCommand = ref _levelUpCommandPool.Get(levelUpEntity);
                var businessEntity = GetBusinessEntityById(levelUpCommand.BusinessId);

                if (businessEntity == -1)
                {
                    Debug.LogError($"{nameof(businessEntity)} not found");
                    _levelUpCommandPool.Del(levelUpEntity);
                    continue;
                }

                ref var business = ref _businessPool.Get(businessEntity);
                var levelUpCost = _levelUpService.CalculateLevelUpCost(business);

                if (playerBalance.Value >= levelUpCost)
                {
                    playerBalance.Value -= levelUpCost;

                    if (business.Level == 0)
                    {
                        _ownedBusinessPool.Add(businessEntity);
                    }

                    business.Level++;

                    if (!_balanceDirtyPool.Has(_playerEntity))
                    {
                        _balanceDirtyPool.Add(_playerEntity);
                    }
                }

                _levelUpCommandPool.Del(levelUpEntity);
            }
        }

        private int GetBusinessEntityById(int businessId)
        {
            foreach (var entity in _businessFilter)
            {
                var business = _businessPool.Get(entity);

                if (business.Id == businessId)
                {
                    return entity;
                }
            }

            return -1;
        }
    }
}
