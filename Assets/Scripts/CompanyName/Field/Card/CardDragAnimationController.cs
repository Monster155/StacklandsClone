using System;
using UnityEngine;

namespace CompanyName.Field.Card
{
    public class CardDragAnimationController : MonoBehaviour
    {
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
                var pos = transform.position;
                pos.y = 0.2f;
                transform.position = pos;
            }
            else if (_isHovered)
            {
                var pos = transform.position;
                pos.y = 0.1f;
                transform.position = pos;
            }
            else
            {
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