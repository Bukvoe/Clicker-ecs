using System;
using System.Collections.Generic;
using System.Numerics;
using _Project.CodeBase.Configs;
using _Project.CodeBase.Services;
using _Project.CodeBase.Shared;
using _Project.CodeBase.UI.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.CodeBase.UI.Businesses
{
    public class BusinessCardView : MonoBehaviour
    {
        public event Action<int> LevelUpClicked;
        public event Action<UpgradeClickEvent> UpgradeClicked;

        private readonly Dictionary<int, UpgradeButton> _upgradeButtons = new();

        [SerializeField] private TextMeshProUGUI _nameField;
        [SerializeField] private Slider _incomeSlider;
        [SerializeField] private TextMeshProUGUI _levelField;
        [SerializeField] private TextMeshProUGUI _incomeField;
        [SerializeField] private Button _levelUpButton;
        [SerializeField] private TextMeshProUGUI _levelUpLabelField;
        [SerializeField] private TextMeshProUGUI _levelUpCostField;
        [SerializeField] private UpgradeButton _upgradeButtonPrefab;
        [SerializeField] private Transform _upgradesContainer;

        private int _businessId;

        private void Awake()
        {
            _levelUpButton.onClick.AddListener(OnLevelUpClicked);
        }

        public void SetId(int businessId)
        {
            _businessId = businessId;
        }

        public void SetName(string name)
        {
            _nameField.text = name;
        }

        public void SetData(int level, BigInteger income, BigInteger levelUpCost)
        {
            _levelField.text = $"{level}";
            _incomeField.text =  $"{income.ToCurrencyString()}";
            _levelUpLabelField.text = level > 0 ? "lvl up" : "buy";
            _levelUpCostField.text = $"{levelUpCost.ToCurrencyString()}";
        }

        public void InitializeUpgrades(List<UpgradeDefinition> upgradeDefinitions, ConfigService configService)
        {
            foreach (var upgradeDefinition in upgradeDefinitions)
            {
                var upgradeButton = Instantiate(_upgradeButtonPrefab, _upgradesContainer);

                upgradeButton.SetData(
                    upgradeDefinition: upgradeDefinition,
                    upgradeName: configService.GetUpgradeName(upgradeDefinition.Id),
                    isBought: false);

                upgradeButton.Clicked += OnUpgradeClicked;

                _upgradeButtons[upgradeDefinition.Id] = upgradeButton;
            }
        }

        public void UpdateUpgrade(UpgradeDefinition upgradeDefinition, string upgradeName, bool isBought)
        {
            if (_upgradeButtons.TryGetValue(upgradeDefinition.Id, out var button))
            {
                button.SetData(upgradeDefinition, upgradeName, isBought);
            }
        }

        public void SetIncomeProgress(float progress)
        {
            _incomeSlider.value = progress;
        }

        private void OnDestroy()
        {
            _levelUpButton.onClick.RemoveAllListeners();

            foreach (var upgradeButton in _upgradeButtons.Values)
            {
                if (upgradeButton != null)
                {
                    Destroy(upgradeButton.gameObject);
                }
            }
        }

        private void OnLevelUpClicked()
        {
            LevelUpClicked?.Invoke(_businessId);
        }

        private void OnUpgradeClicked(int upgradeId)
        {
            UpgradeClicked?.Invoke(new UpgradeClickEvent
            {
                BusinessId = _businessId,
                UpgradeId = upgradeId,
            });
        }
    }
}
