using System.Collections.Generic;
using UnityEngine;
using WheelOfFortune.Data;
using Items.Data;

namespace WheelOfFortune.Core
{
    public class ZoneManager
    {
        private readonly ZoneConfig _normalZone;
        private readonly ZoneConfig _safeZone;
        private readonly ZoneConfig _superZone;
        
        private int _currentZoneLevel;
        private ZoneConfig _currentZoneConfig;
        private List<ZoneLevelData> _levels;
        private int _maxLevel;

        public int CurrentZoneLevel => _currentZoneLevel;
        public ZoneConfig CurrentZoneConfig => _currentZoneConfig;
        public ZoneType CurrentZoneType => _currentZoneConfig?.zoneType ?? ZoneType.Normal;
        public SpinType CurrentSpinType => _currentZoneConfig?.spinType ?? SpinType.Bronze;
        
        public bool IsCurrentZoneSafe => CurrentZoneType == ZoneType.Safe || CurrentZoneType == ZoneType.Super;
        public bool IsCurrentZoneSuper => CurrentZoneType == ZoneType.Super;
        
        public IReadOnlyList<ZoneLevelData> Levels => _levels;
        public int MaxLevel => _maxLevel;

        public ZoneManager(ZoneConfig normalZone, ZoneConfig safeZone, ZoneConfig superZone)
        {
            _normalZone = normalZone;
            _safeZone = safeZone;
            _superZone = superZone;
        }

        public void Initialize(int startingZone = 1, int maxLevel = 60)
        {
            _maxLevel = maxLevel;
            _currentZoneLevel = startingZone;
            _currentZoneConfig = GetZoneConfigForLevel(_currentZoneLevel);
            GenerateLevelList();
        }

        private void GenerateLevelList()
        {
            _levels = new List<ZoneLevelData>(_maxLevel);
            
            for (int level = 1; level <= _maxLevel; level++)
            {
                ZoneType zoneType = CalculateZoneType(level);
                _levels.Add(new ZoneLevelData(level, zoneType));
            }
        }

        public ZoneLevelData GetLevelData(int level)
        {
            if (level < 1 || level > _levels.Count)
                return null;
            
            return _levels[level - 1];
        }

        public void AdvanceToNextZone()
        {
            _currentZoneLevel++;
            _currentZoneConfig = GetZoneConfigForLevel(_currentZoneLevel);
        }

        public ZoneConfig GetZoneConfigForLevel(int level)
        {
            ZoneType type = CalculateZoneType(level);
            
            return type switch
            {
                ZoneType.Super => _superZone,
                ZoneType.Safe => _safeZone,
                _ => _normalZone
            };
        }
        
        public static ZoneType CalculateZoneType(int level)
        {
            if (level % 30 == 0)
                return ZoneType.Super;
            
            if (level % 5 == 0)
                return ZoneType.Safe;
            
            return ZoneType.Normal;
        }
        

        public int GetNextSafeZoneLevel()
        {
            int next = _currentZoneLevel + 1;
            while (CalculateZoneType(next) == ZoneType.Normal)
            {
                next++;
            }
            return next;
        }
        
        public int GetNextSuperZoneLevel()
        {
            int next = ((_currentZoneLevel / 30) + 1) * 30;
            return next;
        }
        
        public void Reset(int startingZone = 1)
        {
            Initialize(startingZone, _maxLevel);
        }
        
        public bool CanExitCurrentZone()
        {
            return IsCurrentZoneSafe;
        }
    }
}