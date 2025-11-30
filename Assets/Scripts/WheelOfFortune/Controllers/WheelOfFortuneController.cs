using UnityEngine;
using WheelOfFortune.Data;
using WheelOfFortune.Events;
using WheelOfFortune.Core;
using Events;
using Items.Data;

namespace WheelOfFortune.Controllers
{
    public class WheelOfFortuneController : MonoBehaviour
    {
        [Header("Zone Configurations")] [SerializeField]
        private ZoneConfig normalZoneConfig;

        [SerializeField] private ZoneConfig safeZoneConfig;
        [SerializeField] private ZoneConfig superZoneConfig;

        [Header("Settings")] [SerializeField] private int wheelItemCount = 8;
        [SerializeField] private int startingZone = 1;
        [SerializeField] private int maxZoneLevel = 60;
        [SerializeField] private int maxRevives = 1;
        [SerializeField] private int baseReviveCost = 25;

        private ZoneManager _zoneManager;
        private RewardManager _rewardManager;
        private SessionManager _sessionManager;
        private WheelContentGenerator _contentGenerator;

        private GameState _currentState = GameState.Idle;
        private WheelItem[] _currentWheelItems;
        private int _pendingTargetIndex;

        public bool CanSpin => _currentState == GameState.WaitingForSpin;
        public bool CanCollect => _zoneManager?.CanExitCurrentZone() ?? false;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            _zoneManager = new ZoneManager(normalZoneConfig, safeZoneConfig, superZoneConfig);
            _rewardManager = new RewardManager();
            _sessionManager = new SessionManager(maxRevives) { BaseReviveCost = baseReviveCost };
            _contentGenerator = new WheelContentGenerator(wheelItemCount);
        }

        private void Start() => StartNewGame();

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<SpinRequestedEvent>(OnSpinRequested, nameof(WheelOfFortuneController));
            EventBus.Instance.Subscribe<SpinCompletedEvent>(OnSpinCompleted, nameof(WheelOfFortuneController));
            EventBus.Instance.Subscribe<RewardAnimationCompletedEvent>(OnShowingResultCompleted,
                nameof(WheelOfFortuneController));
            EventBus.Instance.Subscribe<ItemsCollectedEvent>(OnItemsCollected, nameof(WheelOfFortuneController));
            EventBus.Instance.Subscribe<CollectRequestedEvent>(OnCollectRequested, nameof(WheelOfFortuneController));
            EventBus.Instance.Subscribe<GiveUpRequestedEvent>(OnGiveUp, nameof(WheelOfFortuneController));
            EventBus.Instance.Subscribe<ReviveWithCurrencyRequestedEvent>(OnRevive, nameof(WheelOfFortuneController));
            EventBus.Instance.Subscribe<ReviveWithAdRequestedEvent>(OnRevive, nameof(WheelOfFortuneController));
        }

        private void OnDisable()
        {
            EventBus.Instance.Unsubscribe<SpinRequestedEvent>(OnSpinRequested);
            EventBus.Instance.Unsubscribe<SpinCompletedEvent>(OnSpinCompleted);
            EventBus.Instance.Unsubscribe<RewardAnimationCompletedEvent>(OnShowingResultCompleted);
            EventBus.Instance.Unsubscribe<ItemsCollectedEvent>(OnItemsCollected);
            EventBus.Instance.Unsubscribe<CollectRequestedEvent>(OnCollectRequested);
            EventBus.Instance.Unsubscribe<GiveUpRequestedEvent>(OnGiveUp);
            EventBus.Instance.Unsubscribe<ReviveWithCurrencyRequestedEvent>(OnRevive);
            EventBus.Instance.Unsubscribe<ReviveWithAdRequestedEvent>(OnRevive);
        }

        public void StartNewGame()
        {
            _zoneManager.Initialize(startingZone, maxZoneLevel);
            _rewardManager.ClearAllRewards();
            _sessionManager.Reset();
            EventBus.Instance.Publish(new LevelsGeneratedEvent
            {
                Levels = _zoneManager.Levels,
                MaxLevel = _zoneManager.MaxLevel,
                NormalZoneConfig = normalZoneConfig,
                SafeZoneConfig = safeZoneConfig,
                SuperZoneConfig = superZoneConfig
            });

            EventBus.Instance.Publish(new GameStartedEvent { StartingZone = startingZone });
            PublishNextSafeZoneInfo();
            PublishNextSuperZoneInfo();
            EnterZone();
        }

        private void EnterZone()
        {
            ChangeState(GameState.WaitingForSpin);

            EventBus.Instance.Publish(new ZoneChangedEvent
            {
                ZoneLevel = _zoneManager.CurrentZoneLevel,
                ZoneType = _zoneManager.CurrentZoneType,
                ZoneConfig = _zoneManager.CurrentZoneConfig
            });
            if (_zoneManager.CurrentZoneLevel >= maxZoneLevel)
            {
                return;
            }
            if (_zoneManager.CurrentZoneType == ZoneType.Safe)
            {
                PublishNextSafeZoneInfo();
            }
            else if (_zoneManager.CurrentZoneType == ZoneType.Super)
            { PublishNextSafeZoneInfo();
                PublishNextSuperZoneInfo();
            }
        }
        private void PublishNextSafeZoneInfo()
        {
            EventBus.Instance.Publish(new SafeZoneReachedEvent
            {
                NextSafeZone = _zoneManager.GetNextSafeZoneLevel(),
            });
        }
        private void PublishNextSuperZoneInfo()
        {
            int nextSuperLevel = _zoneManager.GetNextSuperZoneLevel();
            ZoneConfig superConfig = _zoneManager.GetZoneConfigForLevel(nextSuperLevel);
            ItemData exclusiveReward = _contentGenerator.GetRandomSuperZoneReward(superConfig);

            EventBus.Instance.Publish(new SuperZoneReachedEvent
            {
                NextSuperZone = nextSuperLevel,
                NextSuperReward = exclusiveReward
            });
        }

        private void OnSpinRequested(SpinRequestedEvent evt)
        {
            if (!CanSpin) return;

            ChangeState(GameState.Spinning);

            var config = _zoneManager.CurrentZoneConfig;
            _currentWheelItems = _contentGenerator.GenerateWheelItems(config, _zoneManager.CurrentZoneLevel);
            _pendingTargetIndex = _contentGenerator.SelectTargetIndex(_currentWheelItems, !config.hasBomb);

            EventBus.Instance.Publish(new SpinStartedEvent
            {
                TargetWheelItemIndex = _pendingTargetIndex,
                WheelItems = _currentWheelItems,
                WheelItem = wheelItemCount,
                ZoneConfig = config
            });
        }

        private void OnSpinCompleted(SpinCompletedEvent evt)
        {
            if (_currentState != GameState.Spinning) return;

            int index = (evt.ResultIndex >= 0 && evt.ResultIndex < _currentWheelItems.Length)
                ? evt.ResultIndex
                : _pendingTargetIndex;

            var result = _currentWheelItems[index];
            if (result.IsBomb)
                HandleBomb();
            else
                HandleReward(result);
        }
        
        private void OnShowingResultCompleted(RewardAnimationCompletedEvent obj)
        {
            _zoneManager.AdvanceToNextZone();
            EnterZone();
        }
        
        private void OnItemsCollected(ItemsCollectedEvent obj)
        {
            EndGame(true);
        }

        private void HandleBomb()
        {
            ChangeState(GameState.Death);
            _sessionManager.RecordDeath(_zoneManager.CurrentZoneLevel);

            EventBus.Instance.Publish(new BombHitEvent
            {
                CurrentZone = _zoneManager.CurrentZoneLevel,
                CanRevive = _sessionManager.CanRevive,
                ReviveCost = _sessionManager.CurrentReviveCost
            });
        }

        private void HandleReward(WheelItem result)
        {
            ChangeState(GameState.ShowingResult);
            int rewardAmount = result.RewardAmount;
            _rewardManager.AddReward(result.Item, rewardAmount, _zoneManager.CurrentZoneLevel,
                _zoneManager.CurrentZoneType);
            if (_zoneManager.CurrentZoneLevel >= maxZoneLevel)
            {
                ChangeState(GameState.Collecting);
                _rewardManager.FinalizeRewards();
            }
        }

        private void OnCollectRequested(CollectRequestedEvent evt)
        {
            if (!CanCollect) return;

            ChangeState(GameState.Collecting);
            _rewardManager.FinalizeRewards();
        }

        private void OnGiveUp(GiveUpRequestedEvent evt)
        {
            if (_currentState != GameState.Death) return;

            _rewardManager.ClearAllRewards();
            EndGame(false);
        }

        private void OnRevive(ReviveWithCurrencyRequestedEvent evt) => TryRevive();
        private void OnRevive(ReviveWithAdRequestedEvent evt) => TryRevive();

        private void TryRevive()
        {
            if (!_sessionManager.CanRevive)
            {
                EventBus.Instance.Publish(new ReviveFailedEvent { Reason = "Max revives reached" });
                return;
            }

            _sessionManager.RecordRevive();
            EventBus.Instance.Publish(new ReviveSuccessEvent { ZoneLevel = _zoneManager.CurrentZoneLevel });
            EnterZone();
        }

        private void EndGame(bool success)
        {
            ChangeState(GameState.GameOver);
            EventBus.Instance.Publish(new GameEndedEvent { FinalZone = _zoneManager.CurrentZoneLevel });
            StartNewGame();
        }

        private void ChangeState(GameState newState)
        {
            if (_currentState == newState) return;
            var prev = _currentState;
            _currentState = newState;
            EventBus.Instance.Publish(new StateChangedEvent { NewState = newState, PreviousState = prev });
        }
    }
}