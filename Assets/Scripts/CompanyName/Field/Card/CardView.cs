using System;
using CompanyName.ReceiptData;
using UnityEngine;

namespace CompanyName.Field.Card
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private ResourceType _type;
        [SerializeField] private CardDrag _drag;
        [SerializeField] private Transform _nextCardContainer;

        public ResourceType Type => _type;
        public Vector3 Pos => transform.position;

        private Action<CardView> _onDragBeginCallback;
        private Action<CardView> _onDragEndCallback;

        private CardView _parentCard;
        private CardView _attachedCard;

        public void Init(Action<CardView> onDragBeginCallback, Action<CardView> onDragEndCallback)
        {
            _onDragBeginCallback = onDragBeginCallback;
            _onDragEndCallback = onDragEndCallback;
        }

        private void Start()
        {
            _drag.OnDragBegin += Drag_OnDragBegin;
            _drag.OnDragEnd += Drag_OnDragEnd;
        }

        public void Attach(CardView card)
        {
            if(_attachedCard != null)
                return;

            _attachedCard = card;
            card._parentCard = this;
            card.transform.SetParent(_nextCardContainer);
        }

        public void Detach()
        {
            _attachedCard._parentCard = null;
            _attachedCard = null;
        }
        
        private void Drag_OnDragBegin()
        {
            _onDragBeginCallback?.Invoke(this);
        }
        
        private void Drag_OnDragEnd()
        {
            _onDragEndCallback?.Invoke(this);
        }
    }
}