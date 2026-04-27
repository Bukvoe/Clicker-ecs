using System.Collections.Generic;
using _Project.CodeBase.Features.UpgradesFeature.Components;
using Leopotam.EcsLite;

namespace _Project.CodeBase.Features.UpgradesFeature.Queries
{
    public class UpgradeQuery
    {
        private readonly EcsFilter _ownedUpgradesFilter;
        private readonly EcsPool<Upgrade> _upgradePool;

        public UpgradeQuery(EcsWorld world)
        {
            _ownedUpgradesFilter = world.Filter<Upgrade>()
                .Inc<OwnedUpgrade>()
                .End();

            _upgradePool = world.GetPool<Upgrade>();
        }

        public IReadOnlyList<int> GetOwnedUpgradeIds(int businessId)
        {
            var upgradeIds = new List<int>();

            foreach (var ownedUpgradeEntity in _ownedUpgradesFilter)
            {
                var upgrade = _upgradePool.Get(ownedUpgradeEntity);

                if (upgrade.BusinessId == businessId)
                {
                    upgradeIds.Add(upgrade.UpgradeId);
                }
            }

            return upgradeIds;
        }
    }
}
