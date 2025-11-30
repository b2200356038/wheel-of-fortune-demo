using System.Collections.Generic;
using UnityEngine;
using Items.Data;
using WheelOfFortune.Data;
using WheelOfFortune.Events;
using Events;

namespace WheelOfFortune.Core
{
    public class RewardManager
    {
        private readonly Dictionary<ItemData, int> _rewardTotals = new Dictionary<ItemData, int>();
        
        public IReadOnlyDictionary<ItemData, int> RewardTotals => _rewardTotals;
        public int UniqueItemCount => _rewardTotals.Count;
        
        public void AddReward(ItemData item, int amount, int zoneLevel, ZoneType zoneType)
        {
            if (item == null || amount <= 0)
            {
                Debug.LogWarning($"[RewardManager] Invalid reward: item={item}, amount={amount}");
                return;
            }
            
            if (_rewardTotals.ContainsKey(item))
            {
                _rewardTotals[item] += amount;
            }
            else
            {
                _rewardTotals[item] = amount;
            }
            
            EventBus.Instance.Publish(new RewardAddedEvent
            {
                Item = item,
                Amount = amount,
                TotalAmount = _rewardTotals[item],
                ZoneLevel = zoneLevel,
                ZoneType = zoneType
            });
        }
        
        public int GetTotalAmount(ItemData item)
        {
            return _rewardTotals.TryGetValue(item, out int amount) ? amount : 0;
        }
        
        public void ClearAllRewards()
        {
            _rewardTotals.Clear();
            
            EventBus.Instance.Publish(new RewardsResetEvent());
        }
        
        public void FinalizeRewards()
        {
            EventBus.Instance.Publish(new RewardsFinalizedEvent
            {
                Rewards = new Dictionary<ItemData, int>(_rewardTotals),
                TotalItems = UniqueItemCount
            });
        }
        
    }
}