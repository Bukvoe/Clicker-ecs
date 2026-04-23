using _Project.CodeBase.Features.BusinessFeature;
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

        public void SetData(BusinessViewData data)
        {
            _nameField.text = data.Name;
            _levelField.text = $"{data.Level}";
            _incomeField.text = $"${data.Income}";
            _levelUpCostField.text = $"${data.Cost}";
            _incomeSlider.value = data.IncomeProgress;
        }
    }
}
