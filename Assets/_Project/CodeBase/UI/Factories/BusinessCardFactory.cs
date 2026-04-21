using _Project.CodeBase.UI.Businesses;
using UnityEngine;

namespace _Project.CodeBase.UI.Factories
{
    public class BusinessCardFactory
    {
        private readonly BusinessCardView _cardPrefab;

        public BusinessCardFactory(BusinessCardView cardPrefab)
        {
            _cardPrefab = cardPrefab;
        }

        public BusinessCardView Create(Transform containerTransform)
        {
            return Object.Instantiate(_cardPrefab, containerTransform);
        }
    }
}
