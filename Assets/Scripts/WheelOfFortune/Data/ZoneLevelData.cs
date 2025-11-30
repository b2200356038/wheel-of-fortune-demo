namespace WheelOfFortune.Data
{
    public class ZoneLevelData
    {
        public int Level { get; }
        public ZoneType ZoneType { get; }

        public ZoneLevelData(int level, ZoneType zoneType)
        {
            Level = level;
            ZoneType = zoneType;
        }

        public bool IsSafe => ZoneType == ZoneType.Safe || ZoneType == ZoneType.Super;
        public bool IsSuper => ZoneType == ZoneType.Super;
    }
}