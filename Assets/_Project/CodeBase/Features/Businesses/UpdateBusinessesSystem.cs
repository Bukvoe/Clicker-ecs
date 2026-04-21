using System.Collections.Generic;
using _Project.CodeBase.Shared.Interfaces;
using Leopotam.EcsLite;

namespace _Project.CodeBase.Features.Businesses
{
    public class UpdateBusinessesSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly IUpdater<IReadOnlyList<BusinessViewData>> _businessListUpdater;
        private readonly List<BusinessViewData> _businessList = new();

        private EcsFilter _filter;
        private EcsPool<Business> _businessPool;

        public UpdateBusinessesSystem(IUpdater<IReadOnlyList<BusinessViewData>> businessListUpdater)
        {
            _businessListUpdater = businessListUpdater;
        }

        public void Init(EcsSystems systems)
        {
            var world = systems.GetWorld();

            _filter = world.Filter<Business>().End();
            _businessPool = world.GetPool<Business>();
        }

        public void Run(EcsSystems systems)
        {
            _businessList.Clear();

            foreach (var entity in _filter)
            {
                ref var business = ref _businessPool.Get(entity);
                _businessList.Add(new BusinessViewData
                {
                    Id = business.Id,
                    Name = business.Name,
                });
            }

            _businessListUpdater.Update(_businessList);
        }
    }
}
