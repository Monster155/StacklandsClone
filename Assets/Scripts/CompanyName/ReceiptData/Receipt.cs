namespace CompanyName.ReceiptData
{
    public class Receipt
    {
        public ResourceType[] Resources { get; private set; }
        public float CraftTime { get; private set; }
        public float DropChance { get; private set; }
        public ResourceType Result { get; private set; }

        public Receipt(ResourceType[] resources, float craftTime, float dropChance, ResourceType result)
        {
            Resources = resources;
            CraftTime = craftTime;
            DropChance = dropChance;
            Result = result;
        }
    }
}