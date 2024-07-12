using UnityEngine;

namespace CompanyName.Field.Card
{
    public class NewCardDragAnimController : MonoBehaviour
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private CardDrag _drag;

        private bool _isDragging;
        private bool _isHovered;

        private void OnEnable()
        {
            _drag.OnDragBegin += Drag_OnDragBegin;
            _drag.OnDragEnd += Drag_OnDragEnd;
            _drag.OnHoverBegin += Drag_OnHoverBegin;
            _drag.OnHoverEnd += Drag_OnHoverEnd;
        }

        private void OnDisable()
        {
            _drag.OnDragBegin -= Drag_OnDragBegin;
            _drag.OnDragEnd -= Drag_OnDragEnd;
            _drag.OnHoverBegin -= Drag_OnHoverBegin;
            _drag.OnHoverEnd -= Drag_OnHoverEnd;
        }

        private void SetIsDragging(bool isDragging)
        {
            _isDragging = isDragging;
            UpdateContent();
        }

        private void SetIsHovered(bool isHovered)
        {
            _isHovered = isHovered;
            UpdateContent();
        }

        private void UpdateContent()
        {
            if (_isDragging)
            {
                _collider.isTrigger = true;
                _rigidbody.useGravity = false;

                var pos = transform.position;
                pos.y = 0.2f;
                transform.position = pos;
            }
            else if (_isHovered)
            {
                _collider.isTrigger = false;
                _rigidbody.useGravity = false;
                
                var pos = transform.position;
                pos.y = 0.1f;
                transform.position = pos;
            }
            else
            {
                _collider.isTrigger = false;
                _rigidbody.useGravity = true;
                
                var pos = transform.position;
                pos.y = 0f;
                transform.position = pos;
            }
        }

        private void Drag_OnDragBegin() => SetIsDragging(true);
        private void Drag_OnDragEnd() => SetIsDragging(false);
        private void Drag_OnHoverBegin() => SetIsHovered(true);
        private void Drag_OnHoverEnd() => SetIsHovered(false);
    }
}