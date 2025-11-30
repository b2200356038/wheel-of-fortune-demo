using System;
using System.Collections.Generic;
using UnityEngine;
using Items.Data;

namespace WheelOfFortune.Data
{
    [Serializable]
    public class RarityDropRate
    {
        [Range(0f, 100f)] public float commonRate = 60f;
        [Range(0f, 100f)] public float rareRate = 25f;
        [Range(0f, 100f)] public float epicRate = 10f;
        [Range(0f, 100f)] public float legendaryRate = 5f;

        public float GetRateForRarity(ItemRarity rarity)
        {
            return rarity switch
            {
                ItemRarity.Common => commonRate,
                ItemRarity.Rare => rareRate,
                ItemRarity.Epic => epicRate,
                ItemRarity.Legendary => legendaryRate,
                _ => 0f
            };
        }

        public ItemRarity SelectRandomRarity()
        {
            float total = commonRate + rareRate + epicRate + legendaryRate;
            float random = UnityEngine.Random.Range(0f, total);

            float cumulative = 0f;
            
            cumulative += commonRate;
            if (random <= cumulative) return ItemRarity.Common;
            
            cumulative += rareRate;
            if (random <= cumulative) return ItemRarity.Rare;
            
            cumulative += epicRate;
            if (random <= cumulative) return ItemRarity.Epic;
            
            return ItemRarity.Legendary;
        }
        
        public bool Validate()
        {
            float total = commonRate + rareRate + epicRate + legendaryRate;
            return Mathf.Approximately(total, 100f);
        }
    }
}