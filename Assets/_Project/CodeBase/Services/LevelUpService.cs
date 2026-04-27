using System.Numerics;
using _Project.CodeBase.Features.BusinessFeature.Components;
using UnityEngine;

namespace _Project.CodeBase.Services
{
    public class LevelUpService
    {
        private readonly ConfigService _configService;

        public LevelUpService(ConfigService configService)
        {
            _configService = configService;
        }

        public BigInteger CalculateLevelUpCost(Business business)
        {
            if (!_configService.TryGetBusiness(business.Id, out var businessDefinition))
            {
                Debug.LogError($"{nameof(businessDefinition)} {business.Id} not found");
                return BigInteger.Zero;
            }

            return (business.Level + 1) * businessDefinition.BaseCost;
        }
    }
}
