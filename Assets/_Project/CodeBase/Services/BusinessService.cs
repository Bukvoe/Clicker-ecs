using System.Collections.Generic;

namespace _Project.CodeBase.Services
{
    public class BusinessService
    {
        private readonly Dictionary<int, int> _entityByBusiness = new();

        public void Register(int businessId, int entityId)
        {
            _entityByBusiness.TryAdd(businessId, entityId);
        }

        public bool TryGetEntity(int businessId, out int entityId)
        {
            return _entityByBusiness.TryGetValue(businessId, out entityId);
        }
    }
}
