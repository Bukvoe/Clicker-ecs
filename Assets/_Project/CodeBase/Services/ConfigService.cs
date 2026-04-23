using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Configs;
using UnityEngine;

namespace _Project.CodeBase.Services
{
    public class ConfigService
    {
        private readonly Dictionary<int, BusinessDefinition> _businessById;
        private readonly List<BusinessDefinition> _allBusinesses;
        private readonly HashSet<int> _businessIdsOnStart;

        public IReadOnlyList<BusinessDefinition> AllBusinesses => _allBusinesses;
        public IReadOnlyCollection<int> BusinessIdsOnStart => _businessIdsOnStart;

        public ConfigService(GameConfig gameConfig)
        {
            _businessById = new Dictionary<int, BusinessDefinition>();
            _allBusinesses = new List<BusinessDefinition>();

            _businessIdsOnStart = gameConfig.BusinessIdsOnStart.ToHashSet();

            foreach (var businessDefinition in gameConfig.BusinessDefinitions)
            {
                if (!_businessById.TryAdd(businessDefinition.Id, businessDefinition))
                {
                    Debug.LogError( $"Duplicate Id: {businessDefinition.Id} | Name: {businessDefinition.Name}");
                    continue;
                }

                _allBusinesses.Add(businessDefinition);
            }
        }

        public BusinessDefinition GetBusiness(int id)
        {
            return _businessById[id];
        }
    }
}
