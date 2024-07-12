using System;
using UnityEngine;

namespace CompanyName.Field.Card
{
    public class CardDrag : MonoBehaviour
    {
        public event Action OnDragBegin;
        public event Action OnDragEnd;
        public event Action OnHoverBegin;
        public event Action OnHoverEnd;


        private Vector3 _screenPoint;
        private Vector3 _offset;

        private Camera _cam;

        private void Start()
        {
            _cam = Camera.main;
        }

        void OnMouseDown()
        {
            _screenPoint = _cam.WorldToScreenPoint(gameObject.transform.position);

            _offset = transform.position - _cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));

            OnDragBegin?.Invoke();
        }

        void OnMouseDrag()
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);

            Vector3 curPosition = _cam.ScreenToWorldPoint(curScreenPoint) + _offset;

            curPosition.y = transform.position.y;

            transform.position = curPosition;
        }

        private void OnMouseUp() => OnDragEnd?.Invoke();
        
        private void OnMouseEnter() => OnHoverBegin?.Invoke();
        private void OnMouseExit() => OnHoverEnd?.Invoke();
    }
}