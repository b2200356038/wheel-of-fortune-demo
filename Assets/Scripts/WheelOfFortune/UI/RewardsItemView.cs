using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Items.Data;
using Utilities;

namespace WheelOfFortune.UI
{
    public class RewardsItemView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image ui_image_reward_icon_value;
        [SerializeField] private TMP_Text ui_text_reward_amount_value;

        [Header("Animation Settings")]
        [SerializeField] private float punchScale = 0.2f;
        [SerializeField] private float punchDuration = 0.3f;
        [SerializeField] private float counterDuration = 0.5f;
        [SerializeField] private Ease counterEase = Ease.OutQuad;

        private ItemData _itemData;
        private int _currentAmount;
        private int _previousAmount;

        private Tweener _counterTween;
        private Sequence _punchSequence;

        public ItemData ItemData => _itemData;
        public int CurrentAmount => _currentAmount;
        public RectTransform IconTransform => ui_image_reward_icon_value.rectTransform;
        

        public void Initialize(ItemData item, int initialAmount = 0)
        {
            _itemData = item;
            _currentAmount = initialAmount;
            _previousAmount = initialAmount;

            if (_itemData == null) return;

            if (ui_image_reward_icon_value != null && _itemData.itemIcon != null)
            {
                ui_image_reward_icon_value.sprite = _itemData.itemIcon;
                ui_image_reward_icon_value.preserveAspect = true;
            }

            if (ui_text_reward_amount_value != null)
                ui_text_reward_amount_value.text = NumberFormatter.Format(initialAmount);
        }

        public void AddAmount(int amount)
        {
            if (amount <= 0) return;
            _previousAmount = _currentAmount;
            _currentAmount += amount;
            UpdateAmount();
        }

        public void SetAmount(int newAmount)
        {
            _previousAmount = _currentAmount;
            _currentAmount = newAmount;
        }
        
        public void UpdateAmount()
        {
            AnimateCounter(_previousAmount, _currentAmount);
            AnimateIconPunch();
        }

        private void AnimateCounter(int from, int to)
        {
            _counterTween?.Kill(true);

            if (ui_text_reward_amount_value == null) return;

            _counterTween = DOTween.To(
                () => from,
                x => ui_text_reward_amount_value.text = NumberFormatter.Format(x),
                to,
                counterDuration
            ).SetEase(counterEase);
        }

        private void AnimateIconPunch()
        {
            if (ui_image_reward_icon_value == null) return;

            _punchSequence?.Kill();

            _punchSequence = DOTween.Sequence();
            _punchSequence.Append(
                ui_image_reward_icon_value.transform.DOPunchScale(
                    Vector3.one * punchScale,
                    punchDuration,
                    vibrato: 1,
                    elasticity: 0.5f
                )
            );
        }

        private void OnDestroy()
        {
            _counterTween?.Kill();
            _punchSequence?.Kill();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_image_reward_icon_value == null)
            {
                var iconTransform = transform.Find("ui_image_reward_icon_value");
                if (iconTransform != null)
                    ui_image_reward_icon_value = iconTransform.GetComponent<Image>();
            }

            if (ui_text_reward_amount_value == null)
            {
                var amountTransform = transform.Find("ui_text_reward_amount_value");
                if (amountTransform != null)
                    ui_text_reward_amount_value = amountTransform.GetComponent<TMP_Text>();
            }
        }
#endif
    }
}
