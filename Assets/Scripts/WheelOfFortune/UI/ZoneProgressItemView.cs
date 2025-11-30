using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WheelOfFortune.Data;
using Utilities;

namespace WheelOfFortune.UI
{
    public enum LevelState
    {
        Upcoming,
        Current,
        Passed
    }
    
    public class ZoneProgressItemView : MonoBehaviour, IScrollItem
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text ui_text_zone_level_value;
        [SerializeField] private Image ui_image_zone_bg_value;

        private RectTransform _rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        public GameObject GameObject => gameObject;
        public void SetLevel(int level)
        {
            if (ui_text_zone_level_value != null)
                ui_text_zone_level_value.text = level.ToString();
        }
        
        
        public void SetTextVisible(bool visible)
        {
            if (ui_text_zone_level_value != null)
                ui_text_zone_level_value.gameObject.SetActive(visible);
        }

        public void SetBackgroundVisible(bool visible)
        {
            if (ui_image_zone_bg_value != null)
                ui_image_zone_bg_value.gameObject.SetActive(visible);
        }
        
        public void SetColors(Color textColor, Color? backgroundColor = null)
        {
            if (ui_text_zone_level_value != null)
                ui_text_zone_level_value.color = textColor;

            if (ui_image_zone_bg_value != null && backgroundColor.HasValue)
            {
                ui_image_zone_bg_value.color = backgroundColor.Value;
                ui_image_zone_bg_value.enabled = true;
            }
            else if (ui_image_zone_bg_value != null)
            {
                ui_image_zone_bg_value.enabled = false;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_text_zone_level_value == null)
            {
                var levelTransform = transform.Find("ui_text_zone_level_value");
                if (levelTransform != null)
                    ui_text_zone_level_value = levelTransform.GetComponent<TMP_Text>();
            }
    
            if (ui_image_zone_bg_value == null)
            {
                var bgTransform = transform.Find("ui_image_zone_bg_value");
                if (bgTransform != null)
                    ui_image_zone_bg_value = bgTransform.GetComponent<Image>();
            }
        }
#endif
    }
}