using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WheelOfFortune.Data;

namespace WheelOfFortune.UI
{
    public class WheelItemView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image ui_image_wheel_item_value;
        [SerializeField] private TMP_Text ui_text_wheel_item_amount_value;

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
        
        public RectTransform IconTransform() 
        {
            return ui_image_wheel_item_value != null 
                ? ui_image_wheel_item_value.rectTransform 
                : RectTransform;
        }


        public void UpdateWheelItems(WheelItem wheelItem, Sprite bombIcon = null)
        {
            if (wheelItem == null) return;

            if (ui_image_wheel_item_value != null)
            {
                Sprite icon = wheelItem.IsBomb ? bombIcon : wheelItem.GetIcon();
                ui_image_wheel_item_value.sprite = icon;
                ui_image_wheel_item_value.preserveAspect = true;
                ui_image_wheel_item_value.enabled = icon != null;
            }

            if (ui_text_wheel_item_amount_value != null)
                ui_text_wheel_item_amount_value.text = wheelItem.GetDisplayText();
        }

        public void SetPositionInCircle(int index, int totalWheelItems, float radius)
        {
            float angleStep = 360f / totalWheelItems;
            float angle = index * angleStep + 90f;
            float angleRad = angle * Mathf.Deg2Rad;

            RectTransform.anchoredPosition = new Vector2(
                Mathf.Cos(angleRad) * radius,
                Mathf.Sin(angleRad) * radius
            );
            RectTransform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_image_wheel_item_value == null)
            {
                var iconTransform = transform.Find("ui_image_wheel_item_value");
                if (iconTransform != null)
                    ui_image_wheel_item_value = iconTransform.GetComponent<Image>();
            }
    
            if (ui_text_wheel_item_amount_value == null)
            {
                var amountTransform = transform.Find("ui_text_wheel_item_amount_value");
                if (amountTransform != null)
                    ui_text_wheel_item_amount_value = amountTransform.GetComponent<TMP_Text>();
            }
        }
#endif
    }
}