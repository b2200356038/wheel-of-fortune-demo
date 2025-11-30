using UnityEngine;
using TMPro;
using WheelOfFortune.Events;
using Events;

namespace WheelOfFortune.UI
{
    public class SafeZoneInfoView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text ui_text_safe_zone_level_value;

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<SafeZoneReachedEvent>(OnSafeZoneReached, nameof(SafeZoneInfoView));
        }

        private void OnDisable()
        {
            EventBus.Instance.Unsubscribe<SafeZoneReachedEvent>(OnSafeZoneReached);
        }

        private void OnSafeZoneReached(SafeZoneReachedEvent evt)
        {
            if (ui_text_safe_zone_level_value != null)
                ui_text_safe_zone_level_value.text = $"{evt.NextSafeZone}";
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_text_safe_zone_level_value == null)
            {
                var levelTransform = transform.Find("ui_text_safe_zone_level_value");
                if (levelTransform != null)
                    ui_text_safe_zone_level_value = levelTransform.GetComponent<TMP_Text>();
            }
        }
#endif
    }
    
}