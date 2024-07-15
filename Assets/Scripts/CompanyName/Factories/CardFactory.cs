using AYellowpaper.SerializedCollections;
using CompanyName.ReceiptData;
using UnityEngine;

namespace CompanyName.Factories
{
    public class CardFactory : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<ResourceType, Material> _resourceTypeToMaterialDictionary = new SerializedDictionary<ResourceType, Material>();

        public Material GetMaterial(ResourceType type)
        {
            return _resourceTypeToMaterialDictionary[type];
        }
    }
}