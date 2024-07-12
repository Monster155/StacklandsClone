using System.Collections.Generic;
using CompanyName.Field.Card;
using UnityEngine;

namespace CompanyName.Field.Cards
{
    public class CardsController : MonoBehaviour
    {
        [SerializeField] private NewCardView[] _cards;
        [SerializeField] private Transform _cardsContainer;

        private HashSet<NewCardView> _headCards;

        private const float DistanceToSnap = 1f;

        private void Start()
        {
            _headCards = new HashSet<NewCardView>();
            foreach (NewCardView card in _cards)
            {
                card.Init();
                card.OnDragBegin += Card_OnDragBegin;
                card.OnDragEnd += Card_OnDragEnd;

                _headCards.Add(card);
            }
        }

        private void Card_OnDragBegin(NewCardView card)
        {
            if (card.PreviousCard != null)
            {
                card.PreviousCard.NextCard = null;
                card.PreviousCard = null;

                card.transform.SetParent(_cardsContainer);
                card.transform.position = new Vector3(
                    card.transform.position.x,
                    0,
                    card.transform.position.z);
            }

            _headCards.Add(card);
        }

        private void Card_OnDragEnd(NewCardView card)
        {
            _headCards.Remove(card);

            float minDistance = -1;
            NewCardView closestCard = null;

            // foreach (Collider c in card.PhysicsController.Colliders)
            foreach (NewCardView headCard in _headCards)
            {
                // TODO get idea to optimize it
                NewCardView currentLastCard = headCard;
                while (currentLastCard.NextCard != null)
                    currentLastCard = currentLastCard.NextCard;

                float distance = (currentLastCard.transform.position - card.transform.position).magnitude;

                if (closestCard == null || distance < minDistance)
                {
                    closestCard = currentLastCard;
                    minDistance = distance;
                }
            }

            NewCardView newParentCard = null;

            if (minDistance <= DistanceToSnap && closestCard != null)
                newParentCard = closestCard;

            Debug.LogError((minDistance <= DistanceToSnap) + "/" + (closestCard != null));

            if (newParentCard != null)
            {
                newParentCard.NextCard = card;
                card.PreviousCard = newParentCard;

                card.transform.SetParent(newParentCard.ChildCardContainer);
                card.transform.localPosition = Vector3.zero;
            }
            else
            {
                _headCards.Add(card);
            }
        }
    }
}