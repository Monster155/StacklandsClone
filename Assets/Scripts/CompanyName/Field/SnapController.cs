using CompanyName.Field.Card;
using UnityEngine;

namespace CompanyName.Field
{
    public class SnapController : MonoBehaviour
    {
        [SerializeField] private CardView[] _cards;

        private const float DistanceToSnap = 1f;

        private void Start()
        {
            foreach (CardView card in _cards)
            {
                card.OnDragBegin += Card_OnDragBegin;
                card.OnDragEnd += Card_OnDragEnd;
            }
        }

        private void Card_OnDragBegin(CardView card)
        {
            foreach (CardView c in _cards)
            {
                if (c.Equals(card) || c.HasChild)
                    continue;

                c.ChangeOverlapMarkerVisibility(true);
            }
        }

        private void Card_OnDragEnd(CardView card)
        {
            foreach (CardView c in _cards)
            {
                if (c.Equals(card) || c.HasChild)
                    continue;

                c.ChangeOverlapMarkerVisibility(false);
            }

            CardView cardToSnap = FindClosestCard(card);

            if (cardToSnap != null)
            {
                cardToSnap.AddChildCard(card);
                card.AddParentCard(cardToSnap);
            }
        }

        private CardView FindClosestCard(CardView card)
        {
            float minDistance = -1;
            CardView closestCard = null;

            foreach (CardView c in _cards)
            {
                if (c.Equals(card) || c.HasChild)
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