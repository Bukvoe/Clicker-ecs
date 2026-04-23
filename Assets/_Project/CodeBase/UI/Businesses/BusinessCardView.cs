using System;
using _Project.CodeBase.Features.BusinessFeature;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.CodeBase.UI.Businesses
{
    public class BusinessCardView : MonoBehaviour
    {
        public event Action<int> LevelUpClicked;

        [SerializeField] private TextMeshProUGUI _nameField;
        [SerializeField] private Slider _incomeSlider;
        [SerializeField] private TextMeshProUGUI _levelField;
        [SerializeField] private TextMeshProUGUI _incomeField;
        [SerializeField] private Button _levelUpButton;
        [SerializeField] private TextMeshProUGUI _levelUpLabelField;
        [SerializeField] private TextMeshProUGUI _levelUpCostField;

        private int _businessId;

        private void Awake()
        {
            _levelUpButton.onClick.AddListener(OnLevelUpClicked);
        }

        public void SetData(BusinessViewData data)
        {
            _businessId = data.BusinessId;

            _nameField.text = data.Name;
            _levelField.text = $"{data.Level}";
            _incomeField.text = $"${data.Income}";
            _levelUpLabelField.text = data.Level > 0 ? "lvl up" : "buy";
            _levelUpCostField.text = $"${data.Cost}";
            _incomeSlider.value = data.IncomeProgress;
        }

        private void OnDestroy()
        {
            _levelUpButton.onClick.RemoveAllListeners();
        }

        private void OnLevelUpClicked()
        {
            LevelUpClicked?.Invoke(_businessId);
        }
    }
}
