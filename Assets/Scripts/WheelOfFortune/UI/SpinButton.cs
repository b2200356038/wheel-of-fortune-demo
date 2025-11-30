using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Events;
using WheelOfFortune.Data;
using Events;

namespace WheelOfFortune.UI
{
    [RequireComponent(typeof(Button))]
    public class SpinButton : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button ui_button_spin;

        private void Awake()
        {
            if (ui_button_spin == null)
                ui_button_spin = GetComponent<Button>();
        }

        private void OnEnable()
        {
            if (ui_button_spin != null)
                ui_button_spin.onClick.AddListener(OnSpinButtonClick);

            EventBus.Instance.Subscribe<SpinStartedEvent>(OnSpinStarted, nameof(SpinButton));
            EventBus.Instance.Subscribe<StateChangedEvent>(OnStateChanged, nameof(SpinButton));
        }

        private void OnDisable()
        {
            if (ui_button_spin != null)
                ui_button_spin.onClick.RemoveListener(OnSpinButtonClick);

            EventBus.Instance.Unsubscribe<SpinStartedEvent>(OnSpinStarted);
            EventBus.Instance.Unsubscribe<StateChangedEvent>(OnStateChanged);
        }

        private void OnSpinButtonClick()
        {
            EventBus.Instance.Publish(new SpinRequestedEvent());
        }

        private void OnSpinStarted(SpinStartedEvent evt)
        {
            SetInteractable(false);
        }

        private void OnStateChanged(StateChangedEvent evt)
        {
            if (evt.NewState == GameState.WaitingForSpin)
                SetInteractable(true);
        }

        private void SetInteractable(bool interactable)
        {
            if (ui_button_spin != null)
                ui_button_spin.interactable = interactable;
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_button_spin == null)
                ui_button_spin = GetComponent<Button>();
        }
#endif
    }
}