using System.Collections.Generic;
using System.Numerics;
using _Project.CodeBase.Features.BusinessFeature.Components;
using UnityEngine;

namespace _Project.CodeBase.Services
{
    public class IncomeService
    {
        private const int Precision = 10_000;

        private readonly ConfigService _configService;

        public IncomeService(ConfigService configService)
        {
            _configService = configService;
        }

        public float GetIncomeInterval(int businessId)
        {
            if (!_configService.TryGetBusiness(businessId, out var businessDefinition))
            {
                Debug.LogError($"Business {businessId} not found");
                return 0f;
            }

            return businessDefinition.IncomeDelay;
        }

        public BigInteger CalculateIncome(Business business, IReadOnlyList<int> upgrades)
        {
            if (!_configService.TryGetBusiness(business.Id, out var businessDefinition))
            {
                Debug.LogError($"Business {business.Id} not found");
                return BigInteger.Zero;
            }

            var baseIncome = CalculateBaseIncome(business.Level, businessDefinition.BaseIncome);
            var multiplier = CalculateMultiplier(upgrades);

            return baseIncome * multiplier / Precision;
        }

        private BigInteger CalculateBaseIncome(int level, BigInteger baseIncome)
        {
            return level * baseIncome;
        }

        private BigInteger CalculateMultiplier(IReadOnlyList<int> upgrades)
        {
            BigInteger multiplier = Precision;

            for (var i = 0; i < upgrades.Count; i++)
            {
                if (_configService.TryGetUpgrade(upgrades[i], out var upgradeDefinition))
                {
                    multiplier += (BigInteger)(upgradeDefinition.IncomeMultiplier * Precision);
                }
            }

            return multiplier;
        }
    }
}
