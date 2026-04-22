using _Project.CodeBase.Features.BusinessFeature;
using _Project.CodeBase.Services;
using Leopotam.EcsLite;
using UnityEngine;

namespace _Project.CodeBase.Features.IncomeFeature
{
    public class IncomeSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly IncomeService _incomeService;
        private EcsFilter _incomeBusinessesFilter;

        private EcsPool<Business> _businessPool;
        private EcsPool<IncomeProgress> _incomeProgressPool;
        private EcsPool<Income> _incomePool;

        public IncomeSystem(IncomeService incomeService)
        {
            _incomeService = incomeService;
        }

        public void Init(EcsSystems systems)
        {
            var world = systems.GetWorld();

            _incomeBusinessesFilter = world.Filter<Business>()
                           .Inc<IncomeProgress>()
                           .End();

            _businessPool = world.GetPool<Business>();
            _incomeProgressPool = world.GetPool<IncomeProgress>();
            _incomePool = world.GetPool<Income>();
        }

        public void Run(EcsSystems systems)
        {
            foreach (var entity in _incomeBusinessesFilter)
            {
                ref var business = ref _businessPool.Get(entity);
                ref var progress = ref _incomeProgressPool.Get(entity);

                progress.CurrentTime += Time.deltaTime;

                var delay = _incomeService.GetIncomeDelay(business.Id);

                if (progress.CurrentTime >= delay)
                {
                    progress.CurrentTime -= delay;

                    ref var income = ref _incomePool.Add(entity);
                    income.Value = _incomeService.CalculateIncome(business);
                }
            }
        }
    }
}
