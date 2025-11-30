using System.Collections.Generic;
using WheelOfFortune.Data;
using Items.Data;
using UnityEngine;
using WheelOfFortune.UI;

namespace WheelOfFortune.Events
{
    public struct GameStartedEvent
    {
        public int StartingZone;
    }

    public struct GameEndedEvent
    {
        public int FinalZone;
    }

    public struct LevelsGeneratedEvent
    {
        public IReadOnlyList<ZoneLevelData> Levels;
        public ZoneConfig NormalZoneConfig;
        public ZoneConfig SafeZoneConfig;
        public ZoneConfig SuperZoneConfig;
        public int MaxLevel;
    }
    
    public struct WheelInitializedEvent
    {
        public RectTransform AnimationSpawnTransform; 
    }

    public struct ZoneChangedEvent
    {
        public int ZoneLevel;
        public ZoneType ZoneType;
        public ZoneConfig ZoneConfig;
    }

    public struct SafeZoneReachedEvent
    {
        public int NextSafeZone;
    }

    public struct SuperZoneReachedEvent
    {
        public int NextSuperZone;
        public ItemData NextSuperReward;
    }

    public struct SpinRequestedEvent { }

    public struct SpinStartedEvent
    {
        public int TargetWheelItemIndex;
        public WheelItem[] WheelItems;
        public int WheelItem;
        public ZoneConfig ZoneConfig;
    }

    public struct SpinCompletedEvent
    {
        public RectTransform ResultWheelItemTransform;
        public int ResultIndex;
    }

    public struct BombHitEvent
    {
        public int CurrentZone;
        public bool CanRevive;
        public int ReviveCost;
    }

    public struct GiveUpRequestedEvent { }
    public struct ReviveWithCurrencyRequestedEvent { }
    public struct ReviveWithAdRequestedEvent { }

    public struct ReviveSuccessEvent
    {
        public int ZoneLevel;
    }

    public struct ReviveFailedEvent
    {
        public string Reason;
    }

    public struct CollectRequestedEvent { }

    public struct ItemsCollectedEvent
    { }

    public struct StateChangedEvent
    {
        public GameState NewState;
        public GameState PreviousState;
    }
    
    public struct RewardAddedEvent
    {
        public ItemData Item;
        public int Amount;
        public int TotalAmount;
        public int ZoneLevel;
        public ZoneType ZoneType;
    }
    
    public struct RewardsResetEvent {}
    
    public struct RewardsFinalizedEvent
    {
        public Dictionary<ItemData, int> Rewards;
        public int TotalItems;
    }
    
    public struct RewardAnimationStartedEvent
    {
        public ItemData Item;
        public int Multiplier;
        public int Amount;
        public RectTransform TargetTransform;
    }
    
    public struct RewardAnimationCompletedEvent
    {
        public ItemData Item;
        public int Amount;
    }
}