using CompanyName.Field.Card;
using UnityEngine;

namespace CompanyName.Field
{
    public class SnapController : MonoBehaviour
    {
        [SerializeField] private CardView[] _cards;

        private const float DistanceToSnap = 0.1f;

        private void Start()
        {
            foreach (CardView card in _cards)
            {
                card.Init(OnDragBeginCallback, OnDragEndCallback);
            }
        }

        private void OnDragBeginCallback(CardView card)
        {
        }

        private void OnDragEndCallback(CardView card)
        {
            CardView cardToSnap = FindClosestCard(card);

            if (cardToSnap != null)
            {
                cardToSnap.Attach(card);
            }
        }

        private CardView FindClosestCard(CardView card)
        {
            float minDistance = -1;
            CardView closestCard = null;

            foreach (CardView c in _cards)
            {
                if (c.Equals(_cards))
                    continue;

                float distance = (c.Pos - card.Pos).magnitude;

                if (closestCard == null || distance < minDistance)
                {
                    closestCard = c;
                    minDistance = distance;
                }
            }

            Debug.LogError(minDistance + " / " + DistanceToSnap);
            if (minDistance <= DistanceToSnap)
                return closestCard;

            return null;
        }
    }
}