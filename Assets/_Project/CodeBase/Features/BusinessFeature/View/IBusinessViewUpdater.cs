using _Project.CodeBase.Features.IncomeFeature.View;
using _Project.CodeBase.Features.UpgradesFeature.View;

namespace _Project.CodeBase.Features.BusinessFeature.View
{
    public interface IBusinessViewUpdater
    {
        public void CreateBusiness(int businessId);
        public void UpdateBusiness(BusinessViewData businessViewData);
        public void UpdateIncomeProgress(IncomeViewData incomeViewData);
        public void UpdateUpgrade(UpgradeViewData upgradeViewData);
    }
}
