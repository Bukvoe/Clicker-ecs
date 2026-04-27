using _Project.CodeBase.Features.BusinessFeature.Components;
using _Project.CodeBase.Features.IncomeFeature.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace _Project.CodeBase.Features.IncomeFeature.Systems
{
    public class IncomeTimerSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _filter;
        private EcsPool<IncomeTimer> _timerPool;

        public void Init(EcsSystems systems)
        {
            var world = systems.GetWorld();

            _filter = world.Filter<OwnedBusiness>().Inc<IncomeTimer>().End();
            _timerPool = world.GetPool<IncomeTimer>();
        }

        public void Run(EcsSystems systems)
        {
            var deltaTime = Time.deltaTime;

            foreach (var entity in _filter)
            {
                ref var timer = ref _timerPool.Get(entity);
                timer.Time += deltaTime;
            }
        }
    }
}
