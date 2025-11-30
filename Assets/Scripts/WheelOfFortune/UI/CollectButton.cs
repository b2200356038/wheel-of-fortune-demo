using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Events;
using WheelOfFortune.Data;
using Events;

namespace WheelOfFortune.UI
{
    [RequireComponent(typeof(Button))]
    public class CollectButton : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button ui_button_collect;

        private bool _canCollect;
        private bool _isSpinning;

        private void OnEnable()
        {
            if (ui_button_collect != null)
                ui_button_collect.onClick.AddListener(OnCollectClicked);

            EventBus.Instance.Subscribe<SpinStartedEvent>(OnSpinStarted, nameof(CollectButton));
            EventBus.Instance.Subscribe<ZoneChangedEvent>(OnZoneChanged, nameof(CollectButton));
            EventBus.Instance.Subscribe<StateChangedEvent>(OnStateChanged, nameof(CollectButton));
        }

        private void OnDisable()
        {
            if (ui_button_collect != null)
                ui_button_collect.onClick.RemoveListener(OnCollectClicked);

            EventBus.Instance.Unsubscribe<SpinStartedEvent>(OnSpinStarted);
            EventBus.Instance.Unsubscribe<ZoneChangedEvent>(OnZoneChanged);
            EventBus.Instance.Unsubscribe<StateChangedEvent>(OnStateChanged);
        }

        private void OnZoneChanged(ZoneChangedEvent evt)
        {
            bool isSafeOrSuper = evt.ZoneType == ZoneType.Safe || evt.ZoneType == ZoneType.Super;
            _canCollect = isSafeOrSuper;
            _isSpinning = false;
            UpdateButtonState();
        }

        private void OnSpinStarted(SpinStartedEvent evt)
        {
            _isSpinning = true;
            UpdateButtonState();
        }

        private void OnStateChanged(StateChangedEvent evt)
        {
            if (evt.NewState == GameState.WaitingForSpin)
            {
                _isSpinning = false;
                UpdateButtonState();
            }
        }

        private void OnCollectClicked()
        {
            EventBus.Instance.Publish(new CollectRequestedEvent());
        }

        private void UpdateButtonState()
        {
            if (ui_button_collect != null)
                ui_button_collect.interactable = _canCollect && !_isSpinning;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_button_collect == null)
                ui_button_collect = GetComponent<Button>();
        }
#endif
    }
}