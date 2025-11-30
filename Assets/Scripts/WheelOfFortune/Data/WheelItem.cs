using UnityEngine;
using Items.Data;

namespace WheelOfFortune.Data
{
    public class WheelItem
    {
        public ItemData Item { get; }
        public int Multiplier { get; }
        public bool IsBomb { get; }
        
        public int RewardAmount { get; }
        
        public WheelItem(ItemData item, int multiplier ,int rewardAmount)
        {
            Item = item;
            Multiplier = multiplier;
            RewardAmount = rewardAmount;
            IsBomb = false;
        }

        public WheelItem(bool isBomb)
        {
            Item = null;
            Multiplier = 0;
            IsBomb = isBomb;
            RewardAmount= 0;
        }

        public Sprite GetIcon() => Item?.itemIcon;

        public string GetDisplayText() => IsBomb ? "BOMB!" : $"x{RewardAmount}";
    }
}