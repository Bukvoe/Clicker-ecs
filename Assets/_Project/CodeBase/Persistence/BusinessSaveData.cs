using System;
using System.Collections.Generic;

namespace _Project.CodeBase.Persistence
{
    [Serializable]
    public class BusinessSaveData
    {
        public int Id;
        public int Level;
        public float IncomeProgress;
        public List<int> PurchasedUpgradeIds;
    }
}
