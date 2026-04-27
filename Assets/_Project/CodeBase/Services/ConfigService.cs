using System.Collections.Generic;
using _Project.CodeBase.Configs;
using UnityEngine;

namespace _Project.CodeBase.Services
{
    public class ConfigService
    {
        private readonly HashSet<int> _businessIdsOnStart;
        private readonly List<BusinessDefinition> _allBusinesses = new();
        private readonly Dictionary<int, BusinessDefinition> _businessById = new();
        private readonly Dictionary<int, UpgradeDefinition> _upgradeById  = new();
        private readonly Dictionary<int, string> _businessNamesById = new();
        private readonly Dictionary<int, string> _upgradeNamesById = new();

        public IReadOnlyList<BusinessDefinition> AllBusinesses => _allBusinesses;
        public IReadOnlyCollection<int> BusinessIdsOnStart => _businessIdsOnStart;

        public ConfigService(GameConfig gameConfig, NameConfig nameConfig)
        {
            _businessIdsOnStart = new HashSet<int>(gameConfig.BusinessIdsOnStart);

            InitializeBusinesses(gameConfig.BusinessDefinitions);
            InitializeUpgrades(gameConfig.UpgradeDefinitions);

            InitializeNames(nameConfig);
        }

        public bool TryGetBusiness(int id, out BusinessDefinition businessDefinition)
        {
            return _businessById.TryGetValue(id, out businessDefinition);
        }

        public bool TryGetUpgrade(int id, out UpgradeDefinition upgradeDefinition)
        {
            return _upgradeById.TryGetValue(id, out upgradeDefinition);
        }

        public string GetBusinessName(int id)
        {
            return _businessNamesById.TryGetValue(id, out var name)
                ? name
                : $"business_{id}";
        }

        public string GetUpgradeName(int id)
        {
            return _upgradeNamesById.TryGetValue(id, out var name)
                ? name
                : $"upgrade_{id}";
        }

        private void InitializeBusinesses(List<BusinessDefinition> businesses)
        {
            foreach (var businessDefinition in businesses)
            {
                if (!_businessById.TryAdd(businessDefinition.Id, businessDefinition))
                {
                    Debug.LogError($"Duplicate Business Id: {businessDefinition.Id}");
                    continue;
                }

                _allBusinesses.Add(businessDefinition);
            }
        }

        private void InitializeUpgrades(List<UpgradeDefinition> upgrades)
        {
            foreach (var upgradeDefinition in upgrades)
            {
                if (!_upgradeById.TryAdd(upgradeDefinition.Id, upgradeDefinition))
                {
                    Debug.LogError($"Duplicate Upgrade Id: {upgradeDefinition.Id}");
                }
            }
        }

        private void InitializeNames(NameConfig nameConfig)
        {
            foreach (var entry in nameConfig.BusinessNames)
            {
                if (!_businessNamesById.TryAdd(entry.Id, entry.Name))
                {
                    Debug.LogError($"Duplicate Business Name Id: {entry.Id}");
                }
            }

            foreach (var entry in nameConfig.UpgradeNames)
            {
                if (!_upgradeNamesById.TryAdd(entry.Id, entry.Name))
                {
                    Debug.LogError($"Duplicate Upgrade Name Id: {entry.Id}");
                }
            }
        }
    }
}
