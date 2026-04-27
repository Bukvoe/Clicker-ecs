using System.Collections.Generic;

namespace _Project.CodeBase.Services
{
    public class UpgradeService
    {
        private readonly Dictionary<int, int> _entityByUpgrade = new();

        public void Register(int upgradeId, int entityId)
        {
            _entityByUpgrade[upgradeId] = entityId;
        }

        public bool TryGetEntity(int upgradeId, out int entityId)
        {
            return _entityByUpgrade.TryGetValue(upgradeId, out entityId);
        }
    }
}
