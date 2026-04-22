using _Project.CodeBase.Configs;
using _Project.CodeBase.Features.BusinessFeature;
using Leopotam.EcsLite;
using UnityEngine;

namespace _Project.CodeBase.Features.BootstrapFeature
{
    internal class BootstrapSystem : IEcsInitSystem
    {
        private readonly GameConfig _gameConfig;
        private EcsPool<Business> _businessPool;

        public BootstrapSystem(GameConfig gameConfig)
        {
            _gameConfig = gameConfig;

            if (_gameConfig == null)
            {
                Debug.LogError($"{nameof(_gameConfig)} is null");
                return;
            }

            if (_gameConfig.BusinessDefinitions == null)
            {
                Debug.LogError($"{nameof(_gameConfig.BusinessDefinitions)} is null");
                return;
            }
        }

        public void Init(EcsSystems systems)
        {
            var world = systems.GetWorld();
            _businessPool = world.GetPool<Business>();

            foreach (var businessDefinition in _gameConfig.BusinessDefinitions)
            {
                var entity = world.NewEntity();
                ref var business = ref _businessPool.Add(entity);

                business.Id = businessDefinition.Id;
                business.Name = businessDefinition.Name;
            }
        }
    }
}
