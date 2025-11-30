using System.Collections.Generic;
using UnityEngine;
using WheelOfFortune.Events;
using Events;
using Items.Data;

namespace WheelOfFortune.UI
{
    public class RewardsView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform ui_container_reward_items;
        [SerializeField] private GameObject rewardItemPrefab;
        
        private Dictionary<ItemData, RewardsItemView> _rewardViews = new();

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<RewardAddedEvent>(OnRewardAdded, nameof(RewardsView));
            EventBus.Instance.Subscribe<RewardAnimationCompletedEvent>(OnAnimationCompleted, nameof(RewardsView));
            EventBus.Instance.Subscribe<RewardsResetEvent>(OnRewardsReset, nameof(RewardsView));
        }

        private void OnDisable()
        {
            EventBus.Instance.Unsubscribe<RewardAddedEvent>(OnRewardAdded);
            EventBus.Instance.Unsubscribe<RewardAnimationCompletedEvent>(OnAnimationCompleted);
            EventBus.Instance.Unsubscribe<RewardsResetEvent>(OnRewardsReset);
        }

        private void OnRewardAdded(RewardAddedEvent evt)
        {
            if (evt.Item == null || evt.Amount <= 0) 
                return;

            RewardsItemView targetView = GetOrCreateRewardView(evt.Item);
            if (targetView == null)
            {
                Debug.LogError($"[RewardsView] Failed to create view for {evt.Item.itemName}");
                return;
            }
            
            EventBus.Instance.Publish(new RewardAnimationStartedEvent
            {
                Item = evt.Item,
                Multiplier = evt.Amount / evt.Item.minAmount,
                Amount = evt.Amount,
                TargetTransform = targetView.IconTransform
            });
        }
        
        private void OnAnimationCompleted(RewardAnimationCompletedEvent evt)
        {
            if (!_rewardViews.TryGetValue(evt.Item, out var view))
                return;
            view.AddAmount(evt.Amount);
            
        }

        private RewardsItemView GetOrCreateRewardView(ItemData item)
        {
            if (_rewardViews.TryGetValue(item, out var existingView))
            {
                if (existingView != null)
                    return existingView;
                
                _rewardViews.Remove(item);
            }
            
            return CreateRewardView(item);
        }

        private RewardsItemView CreateRewardView(ItemData item)
        {
            if (rewardItemPrefab == null || ui_container_reward_items == null) 
                return null;

            var rewardObj = Instantiate(rewardItemPrefab, ui_container_reward_items);
            var rewardView = rewardObj.GetComponent<RewardsItemView>();

            if (rewardView != null)
            {
                rewardView.Initialize(item, 0);
                _rewardViews.Add(item, rewardView);
            }
            
            return rewardView;
        }

        private void OnRewardsReset(RewardsResetEvent evt)
        {
            ClearAllRewards();
        }
        
        public void ClearAllRewards()
        {
            foreach (var kvp in _rewardViews)
            {
                if (kvp.Value != null)
                    Destroy(kvp.Value.gameObject);
            }
            _rewardViews.Clear();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_container_reward_items == null)
                ui_container_reward_items = transform.Find("ui_container_reward_items");
        }
#endif
    }
}