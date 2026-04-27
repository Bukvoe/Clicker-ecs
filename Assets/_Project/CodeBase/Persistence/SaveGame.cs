using System;
using System.Collections.Generic;
using System.Numerics;

namespace _Project.CodeBase.Persistence
{
    [Serializable]
    public class GameSaveData
    {
        public BigInteger Balance;
        public List<BusinessSaveData> Businesses;
    }
}
