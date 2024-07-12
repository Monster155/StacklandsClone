using System;
using CompanyName.ReceiptData;
using UnityEngine;

namespace CompanyName.Field.Card
{
    public class CardView : MonoBehaviour
    {
        public event Action<CardView> OnDragBegin;
        public event Action<CardView> OnDragEnd;

        [SerializeField] private ResourceType _type;
        [SerializeField] private CardDrag _drag;
        [SerializeField] private GameObject _overlapMarker;
        [SerializeField] private Transform _nextCardContainer;

        public ResourceType Type => _type;
        public Vector3 Pos => transform.position;

        private CardView _parentCard;
        private CardView _childCard;

        public bool HasChild => _childCard != null;
        public bool HasParent => _parentCard != null;


        private void Start()
        {
            _drag.OnDragBegin += Drag_OnDragBegin;
            _drag.OnDragEnd += Drag_OnDragEnd;
            
            _overlapMarker.SetActive(false);
        }

        public void AddParentCard(CardView card)
        {
            if (HasParent)
                return;

            _parentCard = card;
            transform.SetParent(_parentCard._nextCardContainer);
            transform.localPosition = Vector3.zero;
        }

        public void AddChildCard(CardView card)
        {
            if (HasChild)
                return;
            
            _childCard = card;
        }

        public void RemoveParentCard() => _parentCard = null;
        public void RemoveChildCard() => _childCard = null;

        public void ChangeOverlapMarkerVisibility(bool isVisible) => _overlapMarker.SetActive(isVisible);

        private void Drag_OnDragBegin()
        {
            _parentCard?.RemoveChildCard();
            RemoveParentCard();
            OnDragBegin?.Invoke(this);
        }
        
        private void Drag_OnDragEnd() => OnDragEnd?.Invoke(this);
    }
}