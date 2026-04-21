using System;
using System.Collections.Generic;
using _Project.CodeBase.Shared.Interfaces;

namespace _Project.CodeBase.Features.Businesses
{
    public class BusinessListProvider : IListener<IReadOnlyList<BusinessViewData>>, IUpdater<IReadOnlyList<BusinessViewData>>
    {
        public event Action<IReadOnlyList<BusinessViewData>> Changed;

        public void Update(IReadOnlyList<BusinessViewData> businessList)
        {
            Changed?.Invoke(businessList);
        }
    }
}
