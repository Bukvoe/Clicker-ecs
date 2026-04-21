using System.Collections.Generic;
using UnityEngine;

namespace _Project.CodeBase.Configs
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Project/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public List<BusinessDefinition> BusinessDefinitions;
    }
}
