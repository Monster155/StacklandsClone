using CompanyName.Field.Card;
using UnityEngine;

namespace CompanyName.Field.Cards
{
    public class CardsStock : MonoBehaviour
    {
        [SerializeField] private Transform _container;

        private NewCardView _firstCard;
        private NewCardView _lastCard;

        public void Init(NewCardView card)
        {
            _firstCard = card;
            _lastCard = card;

            card.transform.SetParent(_container);
            card.transform.localPosition = Vector3.zero;
        }

        public void ApplyStock(CardsStock cardsStock)
        {
            // _lastCard.NextCard = card;
            // card.PreviousCard = _lastCard;
            //
            // _lastCard = card;
            //
            // card.transform.SetParent(_container);
            // card.transform.localPosition = Vector3.zero;
        }

        public CardsStock ReleaseStock(NewCardView card)
        {
            if (_firstCard.Equals(card))
                return this;

            return null; // disable error
        }
    }
}