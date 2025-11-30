using System;
using UnityEngine;
using Items.Data;

namespace WheelOfFortune.Data
{
    [Serializable]
    public class RewardEntry
    {
        [Header("Reward Item")]
        public ItemData item;
        
        [Header("Spawn Settings")]
        public bool isGuaranteed = false;
        public bool isSuperZoneExclusive = false;
    }
}