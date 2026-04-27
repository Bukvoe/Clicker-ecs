using System;
using _Project.CodeBase.Features.IncomeFeature.View;
using _Project.CodeBase.Features.UpgradesFeature.View;

namespace _Project.CodeBase.Features.BusinessFeature.View
{
    public interface IBusinessViewListener
    {
        public event Action<int> BusinessCreated;
        public event Action<BusinessViewData> BusinessUpdated;
        public event Action<IncomeViewData> IncomeProgressUpdated;
        public event Action<UpgradeViewData> UpgradeUpdated;
    }
}
