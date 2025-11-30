using UnityEngine;

namespace WheelOfFortune.Core
{
    public class SessionManager
    {
        public int DeathCount { get; private set; }
        public int ReviveCount { get; private set; }
        public int MaxZoneReached { get; private set; }
        public int DeathZoneLevel { get; private set; }
        public int TotalSpins { get; private set; }
        public bool CanRevive => ReviveCount < MaxRelivesAllowed;
        public int MaxRelivesAllowed { get; private set; } = 1;
        public int CurrentReviveCost => CalculateReviveCost();
        
        public int BaseReviveCost { get; set; } = 25;
        
        public float ReviveCostMultiplier { get; set; } = 2f;
        
        public SessionManager(int maxRevives = 1)
        {
            MaxRelivesAllowed = maxRevives;
            Reset();
        }
        
        public void Reset()
        {
            DeathCount = 0;
            ReviveCount = 0;
            MaxZoneReached = 0;
            DeathZoneLevel = 0;
            TotalSpins = 0;
        }
        public void RecordSpin()
        {
            TotalSpins++;
        }
        
        public void RecordZoneProgress(int zoneLevel)
        {
            if (zoneLevel > MaxZoneReached)
            {
                MaxZoneReached = zoneLevel;
            }
        }
        
        public void RecordDeath(int zoneLevel)
        {
            DeathCount++;
            DeathZoneLevel = zoneLevel;
        }
        
        public void RecordRevive()
        {
            ReviveCount++;
        }
        private int CalculateReviveCost()
        {
            if (ReviveCount == 0)
                return BaseReviveCost;
            float multiplier = Mathf.Pow(ReviveCostMultiplier, ReviveCount);
            return Mathf.RoundToInt(BaseReviveCost * multiplier);
        }
        public SessionSummary GetSummary()
        {
            return new SessionSummary
            {
                MaxZoneReached = MaxZoneReached,
                TotalSpins = TotalSpins,
                DeathCount = DeathCount,
                ReviveCount = ReviveCount,
                DeathZoneLevel = DeathZoneLevel
            };
        }
    }
    public struct SessionSummary
    {
        public int MaxZoneReached;
        public int TotalSpins;
        public int DeathCount;
        public int ReviveCount;
        public int DeathZoneLevel;
    }
}