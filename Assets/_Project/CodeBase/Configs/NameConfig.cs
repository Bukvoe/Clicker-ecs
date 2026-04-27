using System.Collections.Generic;
using UnityEngine;

namespace _Project.CodeBase.Configs
{
    [CreateAssetMenu(fileName = "NameConfig", menuName = "Project/NameConfig")]
    public class NameConfig : ScriptableObject
    {
        public List<NamedEntry> BusinessNames;
        public List<NamedEntry> UpgradeNames;
    }
}
