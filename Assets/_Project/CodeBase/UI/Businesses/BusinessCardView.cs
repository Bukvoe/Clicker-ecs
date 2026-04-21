using _Project.CodeBase.Features.Businesses;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.CodeBase.UI.Businesses
{
    public class BusinessCardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameField;
        [SerializeField] private Slider _incomeSlider;
        [SerializeField] private TextMeshProUGUI _levelField;
        [SerializeField] private TextMeshProUGUI _incomeField;
        [SerializeField] private TextMeshProUGUI _levelUpCostField;

        public void Bind(BusinessViewData businessViewData)
        {
            _nameField.text = businessViewData.Name;
        }

        public void SetName(string name)
        {
            _nameField.text = name;
        }

        public void SetIncomeProgress(float incomeProgress)
        {
            _incomeSlider.value = incomeProgress;
        }

        public void SetLevel(int level)
        {
            _levelField.text = $"{level}";
        }

        public void SetIncome(string income)
        {
            _incomeField.text = income;
        }

        public void SetLevelUpCost(string levelUpCost)
        {
            _levelUpCostField.text = levelUpCost;
        }
    }
}
