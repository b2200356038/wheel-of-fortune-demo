using UnityEngine;

namespace Items.Data
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Items/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("Item Identification")]
        public string itemName;
        
        [Header("Item Type")]
        public ItemRarity rarity;
        
        [Header("Quantity")]
        public int minAmount = 1;
        public int maxAmount = 100;
        
        [Header("Visual")]
        public Sprite itemIcon;
    }
}