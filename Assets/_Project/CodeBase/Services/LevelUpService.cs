using System.Numerics;
using _Project.CodeBase.Features.BusinessFeature;
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
            var businessDefinition = _configService.GetBusiness(business.Id);

            if (businessDefinition == null)
            {
                Debug.LogError($"{nameof(businessDefinition)} {business.Id} not found");
                return BigInteger.Zero;
            }

            return (business.Level + 1) * businessDefinition.BaseCost;
        }
    }
}
