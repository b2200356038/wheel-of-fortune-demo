using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Items.Data;

namespace WheelOfFortune.UI
{
    public class RewardAnimationItemView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image ui_image_icon;

        private RectTransform _rectTransform;
        private Sequence _currentSequence;

        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Initialize(ItemData item)
        {
            if (item == null) return;

            if (ui_image_icon != null && item.itemIcon != null)
            {
                ui_image_icon.sprite = item.itemIcon;
                ui_image_icon.preserveAspect = true;
            }

            ResetState();
        }

        public void SetPosition(Vector2 anchoredPosition)
        {
            RectTransform.anchoredPosition = anchoredPosition;
        }

        public void ResetState()
        {
            KillAnimation();
            RectTransform.localScale = Vector3.zero;
        }

        public Sequence PlayExpandAnimation(
            Vector2 targetPosition,
            float targetScale,
            float duration,
            Ease ease)
        {
            KillAnimation();

            _currentSequence = DOTween.Sequence();
            _currentSequence.Append(RectTransform.DOScale(targetScale, duration).SetEase(ease));
            _currentSequence.Join(RectTransform.DOAnchorPos(targetPosition, duration).SetEase(ease));

            return _currentSequence;
        }

        public Sequence PlayTravelAnimation(
            Vector2 targetPosition,
            float duration,
            Ease ease)
        {
            KillAnimation();

            _currentSequence = DOTween.Sequence();
            _currentSequence.Append(RectTransform.DOAnchorPos(targetPosition, duration).SetEase(ease));
            _currentSequence.Join(RectTransform.DOScale(1f, duration).SetEase(ease));

            return _currentSequence;
        }

        public void KillAnimation()
        {
            _currentSequence?.Kill();
            DOTween.Kill(RectTransform);
        }

        private void OnDestroy()
        {
            KillAnimation();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_image_icon == null)
            {
                var iconTransform = transform.Find("ui_image_icon");
                if (iconTransform != null)
                    ui_image_icon = iconTransform.GetComponent<Image>();
            }
        }
#endif
    }
}