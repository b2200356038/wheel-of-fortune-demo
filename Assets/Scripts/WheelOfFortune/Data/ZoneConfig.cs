using UnityEngine;
using Items.Data;

namespace WheelOfFortune.Data
{
    [CreateAssetMenu(fileName = "ZoneConfig", menuName = "WheelOfFortune/Zone Config")]
    public class ZoneConfig : ScriptableObject
    {
        [Header("Zone Identity")]
        public ZoneType zoneType = ZoneType.Normal;
        public SpinType spinType = SpinType.Bronze;
        public string spinName = "BRONZE SPIN";

        [Header("Multiplier")]
        public int maxMultiplier = 3;

        [Header("Rewards")]
        public RewardEntry[] rewardPool;
        public RarityDropRate rarityDropRate;
        

        [Header("Bomb")]
        public bool hasBomb = true;
        public int bombCount = 1;
        public Sprite bombIcon;

        [Header("Spin Visuals")]
        public Sprite spinBaseSprite;
        public Sprite spinIndicatorSprite;
        public Color textColor = Color.white;

        [Header("Level Bar Colors")]
        public Color levelTextColor = Color.white;        
        public Color levelTextColorCurrent = Color.black; 
        public Color levelTextColorPassed = Color.gray; 

        [Header("Level Bar Background Color")]
        public Color levelBackgroundColor = Color.white;

        public string GetMultiplierText() => $"Up To x{maxMultiplier} Rewards";
    }
}