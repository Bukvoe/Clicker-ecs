using System.Linq;
using _Project.CodeBase.Features.BusinessFeature;
using _Project.CodeBase.Features.IncomeFeature;
using _Project.CodeBase.Services;
using Leopotam.EcsLite;

namespace _Project.CodeBase.Features.BootstrapFeature
{
    internal class BootstrapSystem : IEcsInitSystem
    {
        private readonly ConfigService _configService;

        public BootstrapSystem(ConfigService configService)
        {
            _configService = configService;
        }

        public void Init(EcsSystems systems)
        {
            var world = systems.GetWorld();
            var businessPool = world.GetPool<Business>();
            var incomeProgressPool = world.GetPool<IncomeProgress>();

            foreach (var businessDefinition in _configService.AllBusinesses)
            {
                var entity = world.NewEntity();

                ref var business = ref businessPool.Add(entity);
                business.Id = businessDefinition.Id;
                business.Level = _configService.BusinessIdsOnStart.Contains(businessDefinition.Id) ? 1 : 0;

                ref var incomeProgress = ref incomeProgressPool.Add(entity);
                incomeProgress.CurrentTime = 0f;
            }
        }
    }
}
