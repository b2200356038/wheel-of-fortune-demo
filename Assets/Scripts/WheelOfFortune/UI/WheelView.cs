using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using WheelOfFortune.Data;
using WheelOfFortune.Events;
using Events;

namespace WheelOfFortune.UI
{
    public class WheelView : MonoBehaviour
    {
        [Header("UI References")] [SerializeField]
        private Transform ui_transform_wheel_rotator;

        [SerializeField] private Transform ui_container_wheel_items;
        [SerializeField] private Image ui_image_wheel_base_value;
        [SerializeField] private Image ui_image_wheel_pointer_value;
        [SerializeField] private TMP_Text ui_text_wheel_title_value;
        [SerializeField] private TMP_Text ui_text_wheel_multiplier_value;

        [Header("Prefab")] [SerializeField] private GameObject wheelItemViewPrefab;

        [Header("Configuration")] [SerializeField]
        private float wheelItemRadius = 150f;

        [Header("Spin Animation")] [SerializeField]
        private float spinDuration = 3f;

        [SerializeField] private int minRotations = 3;
        [SerializeField] private int maxRotations = 5;
        [SerializeField] private Ease spinEase = Ease.OutQuart;
        
        [SerializeField] private float revealDelay = 1.2f;

        [SerializeField] private float wheelItemRevealDuration = 0.3f;
        [SerializeField] private Ease wheelItemRevealEase = Ease.OutBack;

        private List<WheelItemView> _wheelItemViews = new List<WheelItemView>();
        private int _wheelItemCount;
        private Tweener _spinTween;
        private Sequence _revealSequence;
        private Sprite _bombIcon;

        private void Awake()
        {
            EventBus.Instance.Subscribe<ZoneChangedEvent>(OnZoneChanged, nameof(WheelView));
            EventBus.Instance.Subscribe<SpinStartedEvent>(OnSpinStarted, nameof(WheelView));
            EventBus.Instance.Subscribe<GameStartedEvent>(OnGameStarted, nameof(WheelView));
        }

        private void OnDestroy()
        {
            EventBus.Instance.Unsubscribe<ZoneChangedEvent>(OnZoneChanged);
            EventBus.Instance.Unsubscribe<SpinStartedEvent>(OnSpinStarted);
            EventBus.Instance.Unsubscribe<GameStartedEvent>(OnGameStarted);
            _spinTween?.Kill();
            _revealSequence?.Kill();
        }

        private void OnGameStarted(GameStartedEvent evt)
        {
            _revealSequence?.Kill();
            _spinTween?.Kill();
            ClearWheelItemViews();
        }

        private void OnZoneChanged(ZoneChangedEvent evt)
        {
            var config = evt.ZoneConfig;
            if (config == null) return;

            _bombIcon = config.bombIcon;

            if (ui_text_wheel_title_value != null)
            {
                ui_text_wheel_title_value.text = config.spinName;
                ui_text_wheel_title_value.color = config.textColor;
            }

            if (ui_text_wheel_multiplier_value != null)
            {
                ui_text_wheel_multiplier_value.text = config.GetMultiplierText();
                ui_text_wheel_multiplier_value.color = config.textColor;
            }

            if (ui_image_wheel_pointer_value != null)
                ui_image_wheel_pointer_value.sprite = config.spinIndicatorSprite;

            if (ui_image_wheel_base_value != null)
                ui_image_wheel_base_value.sprite = config.spinBaseSprite;
        }

        private void OnSpinStarted(SpinStartedEvent evt)
        {
            _wheelItemCount = evt.WheelItem;

            _spinTween?.Kill();

            if (ui_transform_wheel_rotator != null)
            {
                float currentAngle = ui_transform_wheel_rotator.eulerAngles.z;
                if (ui_container_wheel_items != null)
                    ui_container_wheel_items.localRotation = Quaternion.Euler(0, 0, -currentAngle);
            }

            if (_wheelItemViews.Count != _wheelItemCount)
                InitializeItemViews(_wheelItemCount);

            UpdateWheelItems(evt.WheelItems);
            AnimateReveal(() => AnimateSpin(evt.TargetWheelItemIndex));
        }

        private void InitializeItemViews(int count)
        {
            ClearWheelItemViews();
            _wheelItemCount = count;

            for (int i = 0; i < count; i++)
            {
                var obj = Instantiate(wheelItemViewPrefab, ui_container_wheel_items);
                var view = obj.GetComponent<WheelItemView>();
                view.SetPositionInCircle(i, count, wheelItemRadius);
                _wheelItemViews.Add(view);
            }
            WheelItemView topWheelItem = _wheelItemViews[0];
            RectTransform iconTransform = topWheelItem.IconTransform(); 
    
            EventBus.Instance.Publish(new WheelInitializedEvent
            {
                AnimationSpawnTransform = iconTransform
            });
        }

        private void ClearWheelItemViews()
        {
            foreach (var view in _wheelItemViews)
                if (view != null)
                    Destroy(view.gameObject);
            _wheelItemViews.Clear();
        }

        private void UpdateWheelItems(WheelItem[] wheelItems)
        {
            for (int i = 0; i < wheelItems.Length && i < _wheelItemViews.Count; i++)
            {
                _wheelItemViews[i].UpdateWheelItems(wheelItems[i], _bombIcon);
                _wheelItemViews[i].SetPositionInCircle(i, _wheelItemCount, wheelItemRadius);
                _wheelItemViews[i].transform.localScale = Vector3.zero;
            }
        }

        private void AnimateReveal(System.Action onComplete)
        {
            _revealSequence?.Kill();
            _revealSequence = DOTween.Sequence();

            float delay = revealDelay;

            for (int i = 0; i < _wheelItemViews.Count; i++)
            {
                _revealSequence.Insert(i * delay,
                    _wheelItemViews[i].transform.DOScale(Vector3.one, wheelItemRevealDuration).SetEase(wheelItemRevealEase));
            }

            _revealSequence.OnComplete(() => onComplete?.Invoke());
        }

        private void AnimateSpin(int targetIndex)
        {
            float currentAngle = ui_transform_wheel_rotator.eulerAngles.z;
            float itemAngle = 360f / _wheelItemCount;
            int rotations = Random.Range(minRotations, maxRotations + 1);
            float targetAngle = currentAngle + (360f * rotations) - (targetIndex * itemAngle);
            _spinTween = ui_transform_wheel_rotator
                .DORotate(new Vector3(0, 0, targetAngle), spinDuration, RotateMode.FastBeyond360)
                .SetEase(spinEase)
                .OnComplete(() => EventBus.Instance.Publish(new SpinCompletedEvent
                {
                    ResultWheelItemTransform = _wheelItemViews[targetIndex].RectTransform, ResultIndex = targetIndex
                }));
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_transform_wheel_rotator == null)
                ui_transform_wheel_rotator = transform.Find("ui_transform_wheel_rotator");

            if (ui_container_wheel_items == null)
                ui_container_wheel_items = transform.Find("ui_container_wheel_items");

            if (ui_image_wheel_base_value == null)
            {
                var baseTransform = transform.Find("ui_image_wheel_base_value");
                if (baseTransform != null)
                    ui_image_wheel_base_value = baseTransform.GetComponent<Image>();
            }

            if (ui_image_wheel_pointer_value == null)
            {
                var pointerTransform = transform.Find("ui_image_wheel_pointer_value");
                if (pointerTransform != null)
                    ui_image_wheel_pointer_value = pointerTransform.GetComponent<Image>();
            }

            if (ui_text_wheel_title_value == null)
            {
                var titleTransform = transform.Find("ui_text_wheel_title_value");
                if (titleTransform != null)
                    ui_text_wheel_title_value = titleTransform.GetComponent<TMP_Text>();
            }

            if (ui_text_wheel_multiplier_value == null)
            {
                var multiplierTransform = transform.Find("ui_text_wheel_multiplier_value");
                if (multiplierTransform != null)
                    ui_text_wheel_multiplier_value = multiplierTransform.GetComponent<TMP_Text>();
            }
        }
#endif
    }
}