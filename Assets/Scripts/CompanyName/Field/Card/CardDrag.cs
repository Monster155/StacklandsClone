using System;
using UnityEngine;

namespace CompanyName.Field.Card
{
    public class CardDrag : MonoBehaviour
    {
        public event Action OnDragBegin;
        public event Action OnDragEnd;
        
        private Vector3 _screenPoint;
        private Vector3 _offset;
        
        private Camera _cam;
        
        private bool _isDragging;
        private bool _isHovered;

        private void Start()
        {
            _cam = Camera.main;
        }

        private void Update()
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

        void OnMouseDown()
        {
            _screenPoint = _cam.WorldToScreenPoint(gameObject.transform.position);

            _offset = transform.position - _cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));

            _isDragging = true;
            OnDragBegin?.Invoke();
        }

        void OnMouseDrag()
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);

            Vector3 curPosition = _cam.ScreenToWorldPoint(curScreenPoint) + _offset;

            curPosition.y = transform.position.y;

            transform.position = curPosition;
        }

        private void OnMouseUp()
        {
            _isDragging = false;
            OnDragEnd?.Invoke();
        }
        private void OnMouseEnter() => _isHovered = true;
        private void OnMouseExit() => _isHovered = false;
    }
}