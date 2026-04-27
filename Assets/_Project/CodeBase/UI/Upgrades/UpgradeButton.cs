using System;
using System.Numerics;
using _Project.CodeBase.Configs;
using _Project.CodeBase.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.CodeBase.UI.Upgrades
{
    public class UpgradeButton : MonoBehaviour
    {
        public event Action<int> Clicked;

        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _nameField;
        [SerializeField] private TextMeshProUGUI _descriptionField;
        [SerializeField] private TextMeshProUGUI _costField;

        private int _upgradeId;

        private void Awake()
        {
            _button.onClick.AddListener(OnClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void SetData(UpgradeDefinition upgradeDefinition, bool bought)
        {
            _upgradeId = upgradeDefinition.Id;

            _nameField.text = upgradeDefinition.Name;
            _descriptionField.text = $"+{upgradeDefinition.IncomeMultiplier * 100}%";
            _costField.text = bought ? "Bought" : $"{((BigInteger)upgradeDefinition.Cost).ToCurrencyString()}";

            _button.interactable = !bought;
        }

        private void OnClicked()
        {
            Clicked?.Invoke(_upgradeId);
        }
    }
}
