using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using CompanyName.Field.Card;
using CompanyName.ReceiptData;
using UnityEngine;

namespace CompanyName.Field.Cards
{
    public class CardsController : MonoBehaviour
    {
        [SerializeField] private ReceiptsController _receiptsController;
        [SerializeField] private NewCardView[] _cards;
        [SerializeField] private Transform _cardsContainer;
        [Space]
        [SerializeField] private NewCardView _houseCard;
        [SerializeField] private NewCardView _brickCard;
        [SerializeField] private NewCardView _lumberCampCard;
        [SerializeField] private NewCardView _ironMineCard;

        private HashSet<NewCardView> _headCards;
        private Dictionary<NewCardView, Coroutine> _cardsToTimerDictionary;

        private const float DistanceToSnap = 1f;

        private void Start()
        {
            _headCards = new HashSet<NewCardView>();
            _cardsToTimerDictionary = new Dictionary<NewCardView, Coroutine>();
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
                NewCardView headCard = card;
                while (headCard.PreviousCard != null)
                    headCard = headCard.PreviousCard;
                if (_cardsToTimerDictionary.ContainsKey(headCard))
                {
                    StopCoroutine(_cardsToTimerDictionary[headCard]);
                    headCard.UpdateTimer(0f);
                    _cardsToTimerDictionary.Remove(headCard);
                }

                //

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
            NewCardView newParentCard = FindClosestCard(card, out NewCardView headCard);

            if (newParentCard != null)
            {
                newParentCard.NextCard = card;
                card.PreviousCard = newParentCard;

                card.transform.SetParent(newParentCard.ChildCardContainer);
                card.transform.localPosition = Vector3.zero;
                _headCards.Remove(card);

                CheckForReceipts(headCard);
            }
        }

        private NewCardView FindClosestCard(NewCardView card, out NewCardView head)
        {
            float minDistance = -1;
            NewCardView closestCard = null;
            head = null;

            // foreach (Collider c in card.PhysicsController.Colliders)
            foreach (NewCardView headCard in _headCards)
            {
                if (headCard.Equals(card))
                    continue;

                // TODO get idea to optimize it
                NewCardView currentLastCard = headCard;
                while (currentLastCard.NextCard != null)
                    currentLastCard = currentLastCard.NextCard;

                float distance = (currentLastCard.transform.position - card.transform.position).magnitude;

                if (closestCard == null || distance < minDistance)
                {
                    head = headCard;
                    closestCard = currentLastCard;
                    minDistance = distance;
                }
            }

            if (minDistance <= DistanceToSnap && closestCard != null)
                return closestCard;

            return null;
        }

        private void CheckForReceipts(NewCardView card)
        {
            List<ResourceType> resources = new List<ResourceType>();
            NewCardView nextCard = card;
            while (nextCard != null)
            {
                resources.Add(nextCard.Type);
                nextCard = nextCard.NextCard;
            }

            Receipt receipt = _receiptsController.FindReceipt(resources);

            if (receipt != null)
                StartMakeTimer(card, receipt);
        }

        private void StartMakeTimer(NewCardView card, Receipt receipt)
        {
            Coroutine timer = StartCoroutine(
                TimerCoroutine(receipt.CraftTime,
                    f => card.UpdateTimer(f),
                    () =>
                    {
                        _cardsToTimerDictionary.Remove(card);

                        card.UpdateTimer(0f);

                        // TODO make it normal - create cards factory and change head card to new (result) card

                        // destroy all cards
                        _headCards.Remove(card);
                        NewCardView nextCard = card;
                        while (nextCard != null)
                        {
                            Destroy(nextCard.gameObject);
                            nextCard = nextCard.NextCard;
                        }
                        // create result card
                        NewCardView newCard = receipt.Result switch
                        {
                            ResourceType.House => Instantiate(_houseCard, _cardsContainer),
                            ResourceType.Brick => Instantiate(_brickCard, _cardsContainer),
                            ResourceType.LumberCamp => Instantiate(_lumberCampCard, _cardsContainer),
                            ResourceType.IronMine => Instantiate(_ironMineCard, _cardsContainer),
                            _ => null
                        };
                        _headCards.Add(newCard);
                    }));
            _cardsToTimerDictionary.Add(card, timer);
        }

        // TODO rework to normal timer (or timer system better) - coroutine can be stopped on windows changing 
        private IEnumerator TimerCoroutine(float maxTime, Action<float> updateCallback, Action finalCallback)
        {
            updateCallback?.Invoke(1f);
            float time = maxTime;
            while (time > 0)
            {
                time -= Time.deltaTime;
                updateCallback?.Invoke(time / maxTime);
                yield return null;
            }
            finalCallback?.Invoke();
        }
    }
}