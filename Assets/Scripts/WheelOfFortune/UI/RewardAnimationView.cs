using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using WheelOfFortune.Events;
using Events;
using Items.Data;
using Utilities;

namespace WheelOfFortune.UI
{
    public class RewardAnimationView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform ui_container_icons;
        [SerializeField] private ObjectPool iconPool;

        [Header("Count Limits")]
        [SerializeField] private int minIconCount = 3;
        [SerializeField] private int maxIconCount = 10;
        
        [Header("Expand Animation")]
        [SerializeField] private float expandDuration = 0.4f;
        [SerializeField] private Vector2 scaleRange = new Vector2(0.8f, 1.5f);
        [SerializeField] private float offsetRange = 80f;
        [SerializeField] private Ease expandEase = Ease.OutBack;

        [Header("Travel Animation")]
        [SerializeField] private float travelDuration = 0.6f;
        [SerializeField] private Ease travelEase = Ease.InOutQuad;
        
        [Header("Timing")]
        [SerializeField] private float delayBetweenPhases = 0.2f;

        private List<RewardAnimationItemView> _activeViews = new List<RewardAnimationItemView>();
        private Vector2 _animationSpawnTransform; 
        private bool _isInitialized;

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<RewardAnimationStartedEvent>(OnAnimationStarted, nameof(RewardAnimationView));
            EventBus.Instance.Subscribe<WheelInitializedEvent>(OnWheelInitialized, nameof(RewardAnimationView));
        }

        private void OnDisable()
        {
            EventBus.Instance.Unsubscribe<RewardAnimationStartedEvent>(OnAnimationStarted);
            EventBus.Instance.Unsubscribe<WheelInitializedEvent>(OnWheelInitialized);
            ClearAll();
        }

        private void OnWheelInitialized(WheelInitializedEvent evt)
        {
            if (evt.AnimationSpawnTransform != null)
            {
                _animationSpawnTransform = GetAnchoredPosition(evt.AnimationSpawnTransform);
                _isInitialized = true;
            }
        }

        private void OnAnimationStarted(RewardAnimationStartedEvent evt)
        {
            if (evt.Item == null)
                return;

            if (!_isInitialized)
            {
                return;
            }
            StartCoroutine(PlayAnimationSequence(evt));
        }

        private IEnumerator PlayAnimationSequence(RewardAnimationStartedEvent evt)
        {
            yield return PrepareLayout();
            
            Vector2 targetPos = GetAnchoredPosition(evt.TargetTransform);
            int iconCount = Mathf.Clamp(evt.Multiplier, minIconCount, maxIconCount);

            List<RewardAnimationItemView> views = SpawnViews(evt.Item, iconCount, _animationSpawnTransform);
            if (views.Count == 0)
            {
                PublishCompletionEvent(evt);
                yield break;
            }

            yield return PlayExpandPhase(views, _animationSpawnTransform);
            yield return new WaitForSeconds(delayBetweenPhases);
            yield return PlayTravelPhase(views, targetPos);

            ReturnViewsToPool(views);
            PublishCompletionEvent(evt);
        }

        private IEnumerator PrepareLayout()
        {
            yield return new WaitForEndOfFrame();
            Canvas.ForceUpdateCanvases();
        }

        private List<RewardAnimationItemView> SpawnViews(ItemData item, int count, Vector2 startPos)
        {
            List<RewardAnimationItemView> views = new List<RewardAnimationItemView>();
            
            for (int i = 0; i < count; i++)
            {
                GameObject iconObj = iconPool.Get();
                if (iconObj == null) continue;

                RewardAnimationItemView view = iconObj.GetComponent<RewardAnimationItemView>();
                if (view != null)
                {
                    view.Initialize(item);
                    view.SetPosition(startPos);
                    views.Add(view);
                    _activeViews.Add(view);
                }
            }
            
            return views;
        }

        private IEnumerator PlayExpandPhase(List<RewardAnimationItemView> views, Vector2 startPos)
        {
            Sequence expandSequence = DOTween.Sequence();

            foreach (var view in views)
            {
                if (view == null) continue;

                float randomScale = Random.Range(scaleRange.x, scaleRange.y);
                Vector2 expandPos = CalculateRandomExpandPosition(startPos);
                
                Sequence viewSequence = view.PlayExpandAnimation(expandPos, randomScale, expandDuration, expandEase);
                expandSequence.Join(viewSequence);
            }

            yield return expandSequence.WaitForCompletion();
        }

        private IEnumerator PlayTravelPhase(List<RewardAnimationItemView> views, Vector2 targetPos)
        {
            Sequence travelSequence = DOTween.Sequence();

            foreach (var view in views)
            {
                if (view == null) continue;

                Sequence viewSequence = view.PlayTravelAnimation(targetPos, travelDuration, travelEase);
                travelSequence.Join(viewSequence);
            }

            yield return travelSequence.WaitForCompletion();
        }

        private Vector2 CalculateRandomExpandPosition(Vector2 startPos)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(0, offsetRange);
            return startPos + (randomDir * randomDistance);
        }

        private void ReturnViewsToPool(List<RewardAnimationItemView> views)
        {
            foreach (var view in views)
            {
                if (view == null) continue;
                _activeViews.Remove(view);
                view.KillAnimation();
                iconPool.Return(view.gameObject);
            }
        }

        private void PublishCompletionEvent(RewardAnimationStartedEvent evt)
        {
            EventBus.Instance.Publish(new RewardAnimationCompletedEvent
            {
                Item = evt.Item,
                Amount = evt.Amount
            });
        }

        private Vector2 GetAnchoredPosition(RectTransform target)
        {
            if (target == null || ui_container_icons == null)
                return Vector2.zero;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                ui_container_icons,
                RectTransformUtility.WorldToScreenPoint(null, target.position),
                null,
                out var localPoint
            );

            return localPoint;
        }

        private void ClearAll()
        {
            foreach (var view in _activeViews)
            {
                if (view != null)
                {
                    view.KillAnimation();
                    iconPool.Return(view.gameObject);
                }
            }
            _activeViews.Clear();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_container_icons == null)
                ui_container_icons = GetComponent<RectTransform>();
                
            if (iconPool == null)
                iconPool = GetComponentInChildren<ObjectPool>();
        }
#endif
    }
}