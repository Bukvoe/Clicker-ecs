using System.Numerics;
using _Project.CodeBase.Shared;
using TMPro;
using UnityEngine;

namespace _Project.CodeBase.UI.Balance
{
    public class BalanceView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _balanceField;

        public void SetBalance(BigInteger balance)
        {
            _balanceField.text = $"{balance.ToCurrencyString()}";
        }
    }
}
