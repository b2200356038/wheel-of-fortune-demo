using System.Collections.Generic;
using UnityEngine;
using WheelOfFortune.Data;
using WheelOfFortune.Events;
using Events;
using Utilities;
namespace WheelOfFortune.UI
{

    public class ZoneProgressView : MonoBehaviour
    {
        [Header("Scroll Views")]
        [SerializeField] private List<HorizontalScrollView> scrollViews = new();

        private IReadOnlyList<ZoneLevelData> _levels;
        private int _currentLevel = 1;
        private ZoneConfig _normalConfig;
        private ZoneConfig _safeConfig;
        private ZoneConfig _superConfig;

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<LevelsGeneratedEvent>(OnLevelsGenerated, nameof(ZoneProgressView));
            EventBus.Instance.Subscribe<ZoneChangedEvent>(OnZoneChanged, nameof(ZoneProgressView));
        }

        private void OnDisable()
        {
            EventBus.Instance.Unsubscribe<LevelsGeneratedEvent>(OnLevelsGenerated);
            EventBus.Instance.Unsubscribe<ZoneChangedEvent>(OnZoneChanged);
        }

        private void OnLevelsGenerated(LevelsGeneratedEvent evt)
        {
            _currentLevel = 1;
            _levels = evt.Levels;
            _normalConfig = evt.NormalZoneConfig;
            _safeConfig = evt.SafeZoneConfig;
            _superConfig = evt.SuperZoneConfig;
            foreach (var scrollView in scrollViews)
            {
                scrollView.OnItemUpdate = UpdateItem;
                scrollView.Initialize(_currentLevel, _levels.Count);
            }
        }

        private void OnZoneChanged(ZoneChangedEvent evt)
        {
            _currentLevel = evt.ZoneLevel;
            foreach (var scrollView in scrollViews)
            {
                scrollView.ScrollTo(_currentLevel);
            }
        }
        private void UpdateItem(IScrollItem item, int index)
        { 
            if (item is not ZoneProgressItemView zoneItem)
                return;
            if (index < 1 || index > _levels.Count)
            {
                zoneItem.SetBackgroundVisible(false);
                zoneItem.SetTextVisible(false);
                return;
            }
            var levelData = _levels[index - 1];
            var config = GetConfigForType(levelData.ZoneType);
            zoneItem.SetLevel(levelData.Level);
            LevelState state = GetLevelState(levelData.Level);
            Color textColor = GetTextColor(state, config);
            Color? bgColor = GetBackgroundColor(state, config);
            zoneItem.SetColors(textColor, bgColor);
        }

        private LevelState GetLevelState(int level)
        {
            if (level < _currentLevel)
                return LevelState.Passed;
            else if (level == _currentLevel)
                return LevelState.Current;
            else
                return LevelState.Upcoming;
        }
        
        

        private Color GetTextColor(LevelState state, ZoneConfig config)
        {
            if (config == null)
            {
                return state switch
                {
                    LevelState.Passed => Color.gray,
                    LevelState.Current => Color.yellow,
                    _ => Color.white
                };
            }

            return state switch
            {
                LevelState.Passed => config.levelTextColorPassed,
                LevelState.Current => config.levelTextColorCurrent,
                _ => config.levelTextColor
            };
        }

        private Color? GetBackgroundColor(LevelState state, ZoneConfig config)
        {
            if (config == null)
                return null;
            if (state == LevelState.Current)
                return config.levelBackgroundColor;

            return null;
        }

        private ZoneConfig GetConfigForType(ZoneType type)
        {
            return type switch
            {
                ZoneType.Super => _superConfig,
                ZoneType.Safe => _safeConfig,
                _ => _normalConfig
            };
        }
    }
}