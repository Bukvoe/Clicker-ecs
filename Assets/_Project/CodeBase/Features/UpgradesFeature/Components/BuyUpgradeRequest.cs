using _Project.CodeBase.Shared.Interfaces;

namespace _Project.CodeBase.Features.UpgradesFeature.Components
{
    public struct BuyUpgradeRequest : IRequest
    {
        public int BusinessId;
        public int UpgradeId;
    }
}
