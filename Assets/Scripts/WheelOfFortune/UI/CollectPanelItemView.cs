using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Items.Data;
using Utilities;

namespace WheelOfFortune.UI
{
    public class CollectPanelItemView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image ui_image_collected_frame;
        [SerializeField] private Image ui_image_collected_icon_value;
        [SerializeField] private TMP_Text ui_text_collected_amount_value;
        [SerializeField] private TMP_Text ui_text_collected_name_value;

        [Header("Rarity Colors")]
        [SerializeField] private Color commonColor = Color.white;
        [SerializeField] private Color rareColor = new Color(0.3f, 0.6f, 1f);
        [SerializeField] private Color epicColor = new Color(0.7f, 0.3f, 0.9f);
        [SerializeField] private Color legendaryColor = new Color(1f, 0.6f, 0.1f);

        private ItemData _itemData;
        private int _amount;

        public void Initialize(ItemData item, int initialAmount = 0)
        {
            _itemData = item;
            _amount = initialAmount;

            if (_itemData == null) return;
            if (ui_image_collected_icon_value && _itemData.itemIcon != null)
            {
                ui_image_collected_icon_value.sprite = _itemData.itemIcon;
                ui_image_collected_icon_value.preserveAspect = true;
            }
            if (ui_text_collected_amount_value)
                ui_text_collected_amount_value.text = NumberFormatter.Format(_amount);
            if (ui_text_collected_name_value)
                ui_text_collected_name_value.text = _itemData.itemName;
            ApplyRarityColor(_itemData.rarity);
        }

        private void ApplyRarityColor(ItemRarity rarity)
        {
            Color selectedColor;

            switch (rarity)
            {
                default:
                case ItemRarity.Common:
                    selectedColor = commonColor;
                    break;

                case ItemRarity.Rare:
                    selectedColor = rareColor;
                    break;

                case ItemRarity.Epic:
                    selectedColor = epicColor;
                    break;

                case ItemRarity.Legendary:
                    selectedColor = legendaryColor;
                    break;
            }

            if (ui_image_collected_frame)
                ui_image_collected_frame.color = selectedColor;
            if (ui_text_collected_name_value)
                ui_text_collected_name_value.color = selectedColor;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_image_collected_frame == null)
            {
                var frame = transform.Find("ui_image_collected_frame");
                if (frame != null)
                    ui_image_collected_frame = frame.GetComponent<Image>();
            }

            if (ui_image_collected_icon_value == null)
            {
                var icon = transform.Find("ui_image_collected_icon_value");
                if (icon != null)
                    ui_image_collected_icon_value = icon.GetComponent<Image>();
            }

            if (ui_text_collected_amount_value == null)
            {
                var amount = transform.Find("ui_text_collected_amount_value");
                if (amount != null)
                    ui_text_collected_amount_value = amount.GetComponent<TMP_Text>();
            }

            if (ui_text_collected_name_value == null)
            {
                var nameText = transform.Find("ui_text_collected_name_value");
                if (nameText != null)
                    ui_text_collected_name_value = nameText.GetComponent<TMP_Text>();
            }
        }
#endif
    }
}
