using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using WheelOfFortune.Events;
using Events;
using Items.Data;

namespace WheelOfFortune.UI
{
    public class CollectPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject ui_container_collect_panel;
        [SerializeField] private Transform ui_container_collected_items;
        [SerializeField] private GameObject collectedItemPrefab;
        [SerializeField] private Button ui_button_collect_ok;
        [SerializeField] private CanvasGroup ui_canvas_group_panel;

        [SerializeField] private float fadeDuration = 0.35f;

        private readonly List<GameObject> _spawnedItems = new();
        private Sequence _anim;

        private void Awake()
        {
            ui_button_collect_ok?.onClick.AddListener(OnOkClicked);
        }

        private void Start()
        {
            if (ui_container_collect_panel != null)
                ui_container_collect_panel.SetActive(false);

            if (ui_canvas_group_panel != null)
                ui_canvas_group_panel.alpha = 0f;
        }

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<RewardsFinalizedEvent>(OnRewardsFinalized, nameof(CollectPanelView));
        }

        private void OnDisable()
        {
            EventBus.Instance.Unsubscribe<RewardsFinalizedEvent>(OnRewardsFinalized);
        }

        private void OnRewardsFinalized(RewardsFinalizedEvent evt)
        {
            ClearCollectedItems();
            SpawnCollectedItems(evt.Rewards);
            Show();
        }

        private void SpawnCollectedItems(Dictionary<ItemData, int> rewards)
        {
            foreach (var kvp in rewards)
            {
                GameObject obj = Instantiate(collectedItemPrefab, ui_container_collected_items);

                var view = obj.GetComponent<CollectPanelItemView>();
                if (view != null)
                    view.Initialize(kvp.Key, kvp.Value);

                _spawnedItems.Add(obj);
            }
        }

        private void ClearCollectedItems()
        {
            foreach (var item in _spawnedItems)
            {
                if (item != null)
                    Destroy(item);
            }
            _spawnedItems.Clear();
        }

        private void OnOkClicked()
        {
            Hide();
            EventBus.Instance.Publish(new ItemsCollectedEvent());
        }

        private void Show()
        {
            _anim?.Kill();

            ui_container_collect_panel.SetActive(true);
            ui_canvas_group_panel.alpha = 0f;

            _anim = DOTween.Sequence();
            _anim.Append(ui_canvas_group_panel.DOFade(1f, fadeDuration));
        }

        private void Hide()
        {
            _anim?.Kill();

            _anim = DOTween.Sequence();
            _anim.Append(ui_canvas_group_panel.DOFade(0f, fadeDuration));

            _anim.OnComplete(() =>
            {
                ui_container_collect_panel.SetActive(false);
                ClearCollectedItems();
            });
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_container_collect_panel == null)
            {
                var p = transform.Find("ui_container_collect_panel");
                if (p != null) ui_container_collect_panel = p.gameObject;
            }

            if (ui_canvas_group_panel == null && ui_container_collect_panel != null)
            {
                ui_canvas_group_panel = ui_container_collect_panel.GetComponent<CanvasGroup>();
            }

            if (ui_container_collected_items == null && ui_container_collect_panel != null)
            {
                var r = ui_container_collect_panel.transform.Find("ui_container_collected_items");
                if (r != null) ui_container_collected_items = r;
            }

            if (ui_button_collect_ok == null && ui_container_collect_panel != null)
            {
                var ok = ui_container_collect_panel.transform.Find("ui_button_collect_ok");
                if (ok != null) ui_button_collect_ok = ok.GetComponent<Button>();
            }
        }
#endif
    }
}
