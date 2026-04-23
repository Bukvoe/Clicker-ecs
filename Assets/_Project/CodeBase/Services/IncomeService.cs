using System.Numerics;
using _Project.CodeBase.Features.BusinessFeature;
using UnityEngine;

namespace _Project.CodeBase.Services
{
    public class IncomeService
    {
        private readonly ConfigService _configService;

        public IncomeService(ConfigService configService)
        {
            _configService = configService;
        }

        public float GetIncomeDelay(int businessId)
        {
            var businessDefinition = _configService.GetBusiness(businessId);
            return businessDefinition?.IncomeDelay ?? 0f;
        }

        public BigInteger CalculateIncome(Business business)
        {
            var businessDefinition = _configService.GetBusiness(business.Id);

            if (businessDefinition == null)
            {
                Debug.LogError($"{nameof(businessDefinition)} {business.Id} not found");
                return BigInteger.Zero;
            }

            return business.Level * businessDefinition.BaseIncome;
        }
    }
}
