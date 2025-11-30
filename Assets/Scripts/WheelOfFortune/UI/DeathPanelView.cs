using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using WheelOfFortune.Events;
using Events;

namespace WheelOfFortune.UI
{
    public class DeathPanelView : MonoBehaviour
    {
        [Header("Panel Container")]
        [SerializeField] private GameObject ui_container_death_panel;

        [Header("Animated Elements")]
        [SerializeField] private CanvasGroup ui_canvas_group_death_bg;
        [SerializeField] private CanvasGroup ui_canvas_group_death_buttons;
        [SerializeField] private CanvasGroup ui_canvas_group_death_bomb_card; 
        [SerializeField] private RectTransform ui_container_death_top;
        [SerializeField] private RectTransform ui_container_death_bomb_card;

        [Header("Buttons")]
        [SerializeField] private Button ui_button_death_give_up;
        [SerializeField] private Button ui_button_death_revive_currency;
        [SerializeField] private Button ui_button_death_revive_ad;

        [Header("Revive UI")]
        [SerializeField] private TMP_Text ui_text_death_revive_cost;
        [SerializeField] private GameObject ui_container_death_revive_buttons;

        [Header("Animation Settings")]
        [SerializeField] private float fadeDuration = 0.4f;
        [SerializeField] private float slideDuration = 0.5f;
        [SerializeField] private float slideDistance = 200f;
        
        [Header("Easing")]
        [SerializeField] private Ease slideEnterEase = Ease.OutExpo; 
        [SerializeField] private Ease slideExitEase = Ease.InBack;

        private Vector2 _topOriginalPos;
        private Vector2 _bombOriginalPos;
        private Sequence _anim;
        private int _reviveCost;

        private void Awake()
        {
            if (ui_container_death_top != null)
                _topOriginalPos = ui_container_death_top.anchoredPosition;

            if (ui_container_death_bomb_card != null)
                _bombOriginalPos = ui_container_death_bomb_card.anchoredPosition;

            ui_button_death_give_up?.onClick.AddListener(OnGiveUpClicked);
            ui_button_death_revive_currency?.onClick.AddListener(OnReviveCurrencyClicked);
            ui_button_death_revive_ad?.onClick.AddListener(OnReviveAdClicked);
        }

        private void Start()
        {
            if (ui_container_death_panel != null)
                ui_container_death_panel.SetActive(false);
        }

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<BombHitEvent>(OnBombHit, nameof(DeathPanelView));
            EventBus.Instance.Subscribe<ReviveSuccessEvent>(OnReviveSuccess, nameof(DeathPanelView));
            EventBus.Instance.Subscribe<ReviveFailedEvent>(OnReviveFailed, nameof(DeathPanelView));
            EventBus.Instance.Subscribe<GameStartedEvent>(OnGameStarted, nameof(DeathPanelView));
        }

        private void OnDisable()
        {
            EventBus.Instance.Unsubscribe<BombHitEvent>(OnBombHit);
            EventBus.Instance.Unsubscribe<ReviveSuccessEvent>(OnReviveSuccess);
            EventBus.Instance.Unsubscribe<ReviveFailedEvent>(OnReviveFailed);
            EventBus.Instance.Unsubscribe<GameStartedEvent>(OnGameStarted);
            _anim?.Kill();
        }

        private void OnBombHit(BombHitEvent evt)
        {
            _reviveCost = evt.ReviveCost;
            UpdateReviveUI();
            Show();
        }

        private void OnReviveSuccess(ReviveSuccessEvent evt)
        {
            Hide();
        }

        private void OnReviveFailed(ReviveFailedEvent evt)
        {
            SetButtonsInteractable(true);
        }

        private void OnGameStarted(GameStartedEvent evt)
        {
            HideImmediate();
        }

        private void UpdateReviveUI()
        {
            if (ui_text_death_revive_cost != null)
                ui_text_death_revive_cost.text = _reviveCost.ToString();

            if (ui_container_death_revive_buttons != null)
                ui_container_death_revive_buttons.SetActive(true);
        }

        private void OnGiveUpClicked()
        {
            SetButtonsInteractable(false);
            Hide();
            EventBus.Instance.Publish(new GiveUpRequestedEvent());
        }

        private void OnReviveCurrencyClicked()
        {
            SetButtonsInteractable(false);
            EventBus.Instance.Publish(new ReviveWithCurrencyRequestedEvent());
        }

        private void OnReviveAdClicked()
        {
            SetButtonsInteractable(false);
            EventBus.Instance.Publish(new ReviveWithAdRequestedEvent());
        }

        private void SetButtonsInteractable(bool interactable)
        {
            if (ui_button_death_give_up != null) 
                ui_button_death_give_up.interactable = interactable;
            if (ui_button_death_revive_currency != null) 
                ui_button_death_revive_currency.interactable = interactable;
            if (ui_button_death_revive_ad != null) 
                ui_button_death_revive_ad.interactable = interactable;
        }

        private void Show()
        {
            _anim?.Kill();
            if (ui_container_death_panel != null)
                ui_container_death_panel.SetActive(true);
            
            SetButtonsInteractable(false);

            if (ui_canvas_group_death_bg != null)
                ui_canvas_group_death_bg.alpha = 0f;

            if (ui_canvas_group_death_buttons != null)
                ui_canvas_group_death_buttons.alpha = 0f;
            
            if (ui_canvas_group_death_bomb_card != null)
                ui_canvas_group_death_bomb_card.alpha = 0f;

            if (ui_container_death_top != null)
                ui_container_death_top.anchoredPosition = _topOriginalPos + Vector2.up * slideDistance;

            if (ui_container_death_bomb_card != null)
            {
                float bombStartDistance = slideDistance;
                RectTransform rootCanvasRect = transform.root.GetComponent<RectTransform>();
                if (rootCanvasRect != null)
                    bombStartDistance = (rootCanvasRect.rect.height / 2f) + ui_container_death_bomb_card.rect.height;

                ui_container_death_bomb_card.anchoredPosition = _bombOriginalPos + Vector2.down * bombStartDistance;
            }

            _anim = DOTween.Sequence();

            if (ui_canvas_group_death_bg != null)
                _anim.Append(ui_canvas_group_death_bg.DOFade(1f, fadeDuration));

            if (ui_canvas_group_death_buttons != null)
                _anim.Join(ui_canvas_group_death_buttons.DOFade(1f, fadeDuration));

            if (ui_container_death_bomb_card != null)
            {
                _anim.Join(ui_container_death_bomb_card.DOAnchorPos(_bombOriginalPos, slideDuration).SetEase(slideEnterEase));
                _anim.Join(ui_container_death_bomb_card.DORotate(Vector3.zero, slideDuration).SetEase(slideEnterEase));
                
                if (ui_canvas_group_death_bomb_card != null)
                    _anim.Join(ui_canvas_group_death_bomb_card.DOFade(1f, slideDuration));
            }

            if (ui_container_death_top != null)
                _anim.Join(ui_container_death_top.DOAnchorPos(_topOriginalPos, slideDuration).SetEase(slideEnterEase));

            _anim.OnComplete(() => SetButtonsInteractable(true));
        }

        private void Hide()
        {
            _anim?.Kill();
            SetButtonsInteractable(false);

            _anim = DOTween.Sequence();
            
            if (ui_container_death_top != null)
                _anim.Append(ui_container_death_top.DOAnchorPos(_topOriginalPos + Vector2.up * slideDistance, slideDuration).SetEase(slideExitEase));

            if (ui_container_death_bomb_card != null)
            {
                float bombExitDistance = slideDistance;
                RectTransform rootCanvasRect = transform.root.GetComponent<RectTransform>();
                if (rootCanvasRect != null)
                    bombExitDistance = (rootCanvasRect.rect.height / 2f) + ui_container_death_bomb_card.rect.height;

                _anim.Join(ui_container_death_bomb_card.DOAnchorPos(_bombOriginalPos + Vector2.down * bombExitDistance, slideDuration).SetEase(slideExitEase));
                
                if (ui_canvas_group_death_bomb_card != null)
                    _anim.Join(ui_canvas_group_death_bomb_card.DOFade(0f, slideDuration));
            }

            if (ui_canvas_group_death_bg != null)
                _anim.Join(ui_canvas_group_death_bg.DOFade(0f, fadeDuration));

            if (ui_canvas_group_death_buttons != null)
                _anim.Join(ui_canvas_group_death_buttons.DOFade(0f, fadeDuration));

            _anim.OnComplete(() =>
            {
                if (ui_container_death_panel != null)
                    ui_container_death_panel.SetActive(false);
                    
                ResetPositions();
            });
        }

        private void HideImmediate()
        {
            _anim?.Kill();
            if (ui_container_death_panel != null)
                ui_container_death_panel.SetActive(false);
                
            ResetPositions();
        }

        private void ResetPositions()
        {
            if (ui_canvas_group_death_bg != null)
                ui_canvas_group_death_bg.alpha = 0f;

            if (ui_canvas_group_death_buttons != null)
                ui_canvas_group_death_buttons.alpha = 0f;

            if (ui_canvas_group_death_bomb_card != null)
                ui_canvas_group_death_bomb_card.alpha = 0f;

            if (ui_container_death_top != null)
                ui_container_death_top.anchoredPosition = _topOriginalPos;

            if (ui_container_death_bomb_card != null)
            {
                ui_container_death_bomb_card.anchoredPosition = _bombOriginalPos;
                ui_container_death_bomb_card.rotation = Quaternion.identity;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_container_death_panel == null)
            {
                var contentTransform = transform.Find("ui_container_death_panel");
                if (contentTransform != null)
                    ui_container_death_panel = contentTransform.gameObject;
            }

            if (ui_canvas_group_death_bg == null && ui_container_death_panel != null)
            {
                var bgTransform = ui_container_death_panel.transform.Find("ui_image_death_bg");
                if (bgTransform != null)
                    ui_canvas_group_death_bg = bgTransform.GetComponent<CanvasGroup>();
            }

            if (ui_canvas_group_death_buttons == null && ui_container_death_panel != null)
            {
                var buttonsTransform = ui_container_death_panel.transform.Find("ui_container_death_buttons");
                if (buttonsTransform != null)
                    ui_canvas_group_death_buttons = buttonsTransform.GetComponent<CanvasGroup>();
            }

            if (ui_container_death_top == null && ui_container_death_panel != null)
            {
                var topTransform = ui_container_death_panel.transform.Find("ui_container_death_top");
                if (topTransform != null)
                    ui_container_death_top = topTransform.GetComponent<RectTransform>();
            }

            if (ui_container_death_bomb_card == null && ui_container_death_panel != null)
            {
                var bombTransform = ui_container_death_panel.transform.Find("ui_container_death_bomb_card");
                if (bombTransform != null)
                    ui_container_death_bomb_card = bombTransform.GetComponent<RectTransform>();
            }

            if (ui_canvas_group_death_bomb_card == null && ui_container_death_bomb_card != null)
            {
                ui_canvas_group_death_bomb_card = ui_container_death_bomb_card.GetComponent<CanvasGroup>();
            }

            if (ui_button_death_give_up == null && ui_container_death_panel != null)
            {
                var giveUpTransform = ui_container_death_panel.transform.Find("ui_button_death_give_up");
                if (giveUpTransform != null)
                    ui_button_death_give_up = giveUpTransform.GetComponent<Button>();
            }

            if (ui_button_death_revive_currency == null && ui_container_death_panel != null)
            {
                var reviveCurrencyTransform = ui_container_death_panel.transform.Find("ui_button_death_revive_currency");
                if (reviveCurrencyTransform != null)
                    ui_button_death_revive_currency = reviveCurrencyTransform.GetComponent<Button>();
            }

            if (ui_button_death_revive_ad == null && ui_container_death_panel != null)
            {
                var reviveAdTransform = ui_container_death_panel.transform.Find("ui_button_death_revive_ad");
                if (reviveAdTransform != null)
                    ui_button_death_revive_ad = reviveAdTransform.GetComponent<Button>();
            }

            if (ui_text_death_revive_cost == null && ui_container_death_panel != null)
            {
                var reviveCostTransform = ui_container_death_panel.transform.Find("ui_text_death_revive_cost");
                if (reviveCostTransform != null)
                    ui_text_death_revive_cost = reviveCostTransform.GetComponent<TMP_Text>();
            }

            if (ui_container_death_revive_buttons == null && ui_container_death_panel != null)
            {
                var reviveButtonsTransform = ui_container_death_panel.transform.Find("ui_container_death_revive_buttons");
                if (reviveButtonsTransform != null)
                    ui_container_death_revive_buttons = reviveButtonsTransform.gameObject;
            }
        }
#endif
    }
}