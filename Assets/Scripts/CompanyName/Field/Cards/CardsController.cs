using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using CompanyName.Factories;
using CompanyName.Field.Card;
using CompanyName.ReceiptData;
using UnityEngine;

namespace CompanyName.Field.Cards
{
    public class CardsController : MonoBehaviour
    {
        [SerializeField] private ReceiptsController _receiptsController;
        [SerializeField] private NewCardView _cardPrefab;
        [SerializeField] private Transform _cardsContainer;
        [Space]
        [SerializeField] private CardFactory _cardFactory;
        [SerializeField] private List<Vector3ToResType> _cardsSpawnPoint = new List<Vector3ToResType>();


        private HashSet<NewCardView> _headCards;
        private Dictionary<NewCardView, Coroutine> _cardsToTimerDictionary;

        private const float DistanceToSnap = 1f;

        private void Start()
        {
            _headCards = new HashSet<NewCardView>();
            _cardsToTimerDictionary = new Dictionary<NewCardView, Coroutine>();
            foreach (Vector3ToResType value in _cardsSpawnPoint)
            {
                NewCardView card = CreateCard(value.Pos, value.Type);
                _headCards.Add(card);
            }
        }

        private NewCardView CreateCard(Vector3 pos, ResourceType type)
        {
            NewCardView card = Instantiate(_cardPrefab, _cardsContainer);
            card.transform.position = pos;
            card.Init(_cardFactory, type);
            card.OnDragBegin += Card_OnDragBegin;
            card.OnDragEnd += Card_OnDragEnd;

            return card;
        }

        private void DestroyCard(NewCardView card)
        {
            card.OnDragBegin -= Card_OnDragBegin;
            card.OnDragEnd -= Card_OnDragEnd;
            Destroy(card.gameObject);
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
                        Vector3 pos = card.transform.position;

                        // destroy all cards
                        _headCards.Remove(card);
                        NewCardView nextCard = card;
                        int villagersCount = 0;
                        while (nextCard != null)
                        {
                            if (nextCard.Type == ResourceType.Villager)
                                villagersCount++;

                            DestroyCard(nextCard);
                            nextCard = nextCard.NextCard;
                        }

                        // create result card
                        NewCardView newCard = CreateCard(pos, receipt.Result);
                        _headCards.Add(newCard);
                        for (int i = 0; i < villagersCount; i++)
                        {
                            newCard = CreateCard(pos + new Vector3(0.1f * (i + 1), 0, -0.1f * (i + 1)),
                                ResourceType.Villager);
                            _headCards.Add(newCard);
                        }
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

        [Serializable]
        private class Vector3ToResType
        {
            public Vector3 Pos;
            public ResourceType Type;
        }
    }
}