using System;

namespace _Project.CodeBase.Configs
{
    [Serializable]
    public class UpgradeDefinition
    {
        public int Id;
        public string Name;
        public long Cost;
        public float IncomeMultiplier;
    }
}
