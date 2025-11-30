using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WheelOfFortune.Events;
using Events;

namespace WheelOfFortune.UI
{
    public class SuperZoneInfoView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image ui_image_super_zone_reward_value;
        [SerializeField] private TMP_Text ui_text_super_zone_level_value;

        private void Awake()
        {
            if(ui_image_super_zone_reward_value != null)
                ui_image_super_zone_reward_value.preserveAspect = true;
        }

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<SuperZoneReachedEvent>(OnSuperZoneReached, nameof(SuperZoneInfoView));
        }

        private void OnDisable()
        {
            EventBus.Instance.Unsubscribe<SuperZoneReachedEvent>(OnSuperZoneReached);
        }

        private void OnSuperZoneReached(SuperZoneReachedEvent evt)
        {
            if (ui_image_super_zone_reward_value != null && 
                evt.NextSuperReward != null && 
                evt.NextSuperReward.itemIcon != null)
            {
                ui_image_super_zone_reward_value.sprite = evt.NextSuperReward.itemIcon;
            }

            if (ui_text_super_zone_level_value != null)
                ui_text_super_zone_level_value.text = $"{evt.NextSuperZone}";
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_image_super_zone_reward_value == null)
            {
                var rewardTransform = transform.Find("ui_image_super_zone_reward_value");
                if (rewardTransform != null)
                    ui_image_super_zone_reward_value = rewardTransform.GetComponent<Image>();
            }
    
            if (ui_text_super_zone_level_value == null)
            {
                var levelTransform = transform.Find("ui_text_super_zone_level_value");
                if (levelTransform != null)
                    ui_text_super_zone_level_value = levelTransform.GetComponent<TMP_Text>();
            }
        }
#endif
    }
}