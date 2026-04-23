using _Project.CodeBase.Features.BusinessFeature;
using Leopotam.EcsLite;
using UnityEngine;

namespace _Project.CodeBase.Features.IncomeFeature
{
    public class IncomeTimerSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _incomeBusinessesFilter;
        private EcsPool<IncomeTimer> _incomeTimerPool;

        public void Init(EcsSystems systems)
        {
            var world = systems.GetWorld();

            _incomeBusinessesFilter = world.Filter<OwnedBusiness>()
                .Inc<IncomeTimer>()
                .End();

            _incomeTimerPool = world.GetPool<IncomeTimer>();
        }

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _incomeBusinessesFilter)
            {
                ref var timer = ref _incomeTimerPool.Get(entity);
                timer.Time += Time.deltaTime;
            }
        }
    }
}
