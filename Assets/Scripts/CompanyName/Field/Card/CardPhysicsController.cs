using System.Collections.Generic;
using UnityEngine;

namespace CompanyName.Field.Card
{
    public class CardPhysicsController : MonoBehaviour
    {
        public List<Collider> Colliders { get; } = new List<Collider>();

        private void OnTriggerEnter(Collider other)
        {
            Colliders.Add(other);
            Debug.LogError("Added collider");
        }

        private void OnTriggerExit(Collider other)
        {
            Colliders.Remove(other);
            Debug.LogError("Removed collider");
        }
    }
}