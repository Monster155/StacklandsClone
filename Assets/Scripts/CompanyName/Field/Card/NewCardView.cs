using System;
using CompanyName.Factories;
using CompanyName.ReceiptData;
using UnityEngine;

namespace CompanyName.Field.Card
{
    public class NewCardView : MonoBehaviour
    {
        public event Action<NewCardView> OnDragBegin;
        public event Action<NewCardView> OnDragEnd;

        [SerializeField] private CardDrag _drag;
        [SerializeField] private Transform _progressBar;
        [SerializeField] private MeshRenderer _renderer;
        [field: SerializeField] public Transform ChildCardContainer { get; private set; }
        // [field: SerializeField] public CardPhysicsController PhysicsController { get; private set; }

        public NewCardView PreviousCard { get; set; }
        public NewCardView NextCard { get; set; }
        public ResourceType Type { get; private set; }

        public void Init(CardFactory cardFactory, ResourceType type)
        {
            PreviousCard = null;
            NextCard = null;

            Type = type;
            _renderer.material = cardFactory.GetMaterial(type);
        }

        private void Start()
        {
            _drag.OnDragBegin += Drag_OnDragBegin;
            _drag.OnDragEnd += Drag_OnDragEnd;
            UpdateTimer(0f);
        }

        public void UpdateTimer(float percent)
        {
            Vector3 scale = _progressBar.localScale;
            scale.x = percent;
            _progressBar.localScale = scale;
        }

        private void Drag_OnDragBegin() => OnDragBegin?.Invoke(this);
        private void Drag_OnDragEnd() => OnDragEnd?.Invoke(this);
    }
}