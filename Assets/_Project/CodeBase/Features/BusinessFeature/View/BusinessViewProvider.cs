using System;
using _Project.CodeBase.Features.IncomeFeature.View;
using _Project.CodeBase.Features.UpgradesFeature.View;

namespace _Project.CodeBase.Features.BusinessFeature.View
{
    public class BusinessViewProvider : IBusinessViewListener, IBusinessViewUpdater
    {
        public event Action<int> BusinessCreated;
        public event Action<BusinessViewData> BusinessUpdated;
        public event Action<IncomeViewData> IncomeProgressUpdated;
        public event Action<UpgradeViewData> UpgradeUpdated;

        public void CreateBusiness(int businessId)
        {
            BusinessCreated?.Invoke(businessId);
        }

        public void UpdateBusiness(BusinessViewData businessViewData)
        {
            BusinessUpdated?.Invoke(businessViewData);
        }

        public void UpdateIncomeProgress(IncomeViewData incomeViewData)
        {
            IncomeProgressUpdated?.Invoke(incomeViewData);
        }

        public void UpdateUpgrade(UpgradeViewData upgradeViewData)
        {
            UpgradeUpdated?.Invoke(upgradeViewData);
        }
    }
}
