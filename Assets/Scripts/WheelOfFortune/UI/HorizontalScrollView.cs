using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace WheelOfFortune.UI
{
    public class HorizontalScrollView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform viewport;
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject itemPrefab;

        [Header("Item Settings")]
        [SerializeField] private float itemWidth = 100f;
        [SerializeField] private float spacing = 20f;

        [Header("Animation")]
        [SerializeField] private float scrollDuration = 0.3f;
        [SerializeField] private Ease scrollEase = Ease.OutCubic;

        private readonly List<IScrollItem> _items = new();
        private int _currentIndex;
        private int _totalCount;
        private float _itemStride;
        private Tweener _scrollTween;
        
        public System.Action<IScrollItem, int> OnItemUpdate;
        
        public void Initialize(int startIndex, int totalCount)
        {
            _scrollTween?.Kill();
            _scrollTween = null;
            ClearItems();
            content.anchoredPosition = Vector2.zero;
            Canvas.ForceUpdateCanvases();
            _currentIndex = startIndex;
            _totalCount = totalCount;
            _itemStride = itemWidth + spacing;
            CreateItems();
        }
        
        public void ScrollTo(int targetIndex, bool animate = true)
        {
            int delta = targetIndex - _currentIndex;
            if (delta == 0) return;

            _currentIndex = targetIndex;
            _scrollTween?.Kill();

            float targetX = content.anchoredPosition.x - (delta * _itemStride);

            if (animate)
            {
                _scrollTween = content.DOAnchorPosX(targetX, scrollDuration)
                    .SetEase(scrollEase)
                    .OnUpdate(UpdateItems)
                    .OnComplete(UpdateItems);
            }
            else
            {
                content.anchoredPosition = new Vector2(targetX, 0);
                UpdateItems();
            }
        }
        
        public void RefreshAllItems()
        {
            UpdateAllItemData();
        }

        private void CreateItems()
        {
            int itemCount = Mathf.CeilToInt(viewport.rect.width / _itemStride) + 2;

            for (int i = 0; i < itemCount; i++)
            {
                int index = _currentIndex + i;
                if (!IsValidIndex(index)) continue;

                GameObject obj = Instantiate(itemPrefab, content);
                IScrollItem item = obj.GetComponent<IScrollItem>();

                if (item == null)
                {
                    Destroy(obj);
                    continue;
                }

                item.RectTransform.anchoredPosition = new Vector2(i * _itemStride, 0);
                OnItemUpdate?.Invoke(item, index);
                _items.Add(item);
            }
        }

        private void UpdateItems()
        {
            RecycleOffscreenItems();
            UpdateAllItemData();
        }

        private void RecycleOffscreenItems()
        {
            float viewportLeftEdge = -viewport.rect.width / 2f;
            float contentX = content.anchoredPosition.x;

            foreach (IScrollItem item in _items)
            {
                float itemWorldX = item.RectTransform.anchoredPosition.x + contentX;
                if (itemWorldX < viewportLeftEdge - _itemStride)
                {
                    float rightmostX = GetRightmostItemX();
                    item.RectTransform.anchoredPosition = new Vector2(rightmostX + _itemStride, 0);
                }
            }
        }

        private void UpdateAllItemData()
        {
            foreach (IScrollItem item in _items)
            {
                int index = CalculateItemIndex(item);
                if (IsValidIndex(index))
                {
                    OnItemUpdate?.Invoke(item, index);
                }
            }
        }

        private float GetRightmostItemX()
        {
            float maxX = float.MinValue;
            foreach (IScrollItem item in _items)
            {
                if (item.RectTransform.anchoredPosition.x > maxX)
                {
                    maxX = item.RectTransform.anchoredPosition.x;
                }
            }
            return maxX;
        }

        private int CalculateItemIndex(IScrollItem item)
        {
            float itemX = item.RectTransform.anchoredPosition.x;
            return Mathf.RoundToInt(itemX / _itemStride) + 1;
        }

        private bool IsValidIndex(int index)
        {
            return index >= 1;
        }

        private void ClearItems()
        {
            foreach (IScrollItem item in _items)
            {
                if (item?.GameObject != null)
                {
                    Destroy(item.GameObject);
                }
            }
            _items.Clear();
        }

        private void OnDestroy()
        {
            _scrollTween?.Kill();
        }
    }
}