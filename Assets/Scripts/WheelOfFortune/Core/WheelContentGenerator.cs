using System.Collections.Generic;
using UnityEngine;
using WheelOfFortune.Data;
using Items.Data;

namespace WheelOfFortune.Core
{
    public class WheelContentGenerator
    {
        private readonly int _wheelItemCount;
        public WheelContentGenerator(int wheelItemCount)
        {
            _wheelItemCount = wheelItemCount;
        } public ItemData GetRandomSuperZoneReward(ZoneConfig zone)
        {
            if (zone == null || zone.rewardPool == null || zone.rewardPool.Length == 0)
                return null;

            var exclusiveEntries = new List<RewardEntry>();
            foreach (var entry in zone.rewardPool)
            { if (entry != null && entry.item != null && entry.isSuperZoneExclusive)
                {
                    exclusiveEntries.Add(entry);
                }
            }
            if (exclusiveEntries.Count > 0)
            {
                return exclusiveEntries[Random.Range(0, exclusiveEntries.Count)].item;
            }
            return null;
        }

        public WheelItem[] GenerateWheelItems(ZoneConfig zone, int zoneLevel, ItemData guaranteedSuperReward = null)
        {
            if (zone == null || zone.rewardPool == null || zone.rewardPool.Length == 0)
                return null;

            bool isSuperZone = zone.zoneType == ZoneType.Super;
            var wheelItems = new List<WheelItem>(_wheelItemCount);

            int bombCount = zone.hasBomb ? zone.bombCount : 0;
            int rewardCount = _wheelItemCount - bombCount;

            if (isSuperZone && guaranteedSuperReward != null)
            {
                int multiplier = GenerateMultiplier(zone.maxMultiplier, zoneLevel);
                int amount = Random.Range(guaranteedSuperReward.minAmount, guaranteedSuperReward.maxAmount) * multiplier;
                wheelItems.Add(new WheelItem(guaranteedSuperReward, multiplier, amount));
                rewardCount--;
            }

            var guaranteedEntries = GetGuaranteedRewards(zone.rewardPool, isSuperZone);
            int guaranteedCount = Mathf.Min(guaranteedEntries.Count, rewardCount);

            for (int i =0; i < guaranteedCount; i++)
            {
                var entry = guaranteedEntries[i];
                int multiplier = GenerateMultiplier(zone.maxMultiplier, zoneLevel);
                int amount = Random.Range(entry.item.minAmount, entry.item.maxAmount) * multiplier;
                wheelItems.Add(new WheelItem(entry.item, multiplier, amount));
            } int remainingSlots = rewardCount - guaranteedCount;
            var availableEntries = GetAvailableRewards(zone.rewardPool, isSuperZone);
            for (int i = 0; i < remainingSlots; i++)
            {
                var entry = SelectRewardEntry(availableEntries, zone.rarityDropRate);
                if (entry != null && entry.item != null)
                {
                    int multiplier = GenerateMultiplier(zone.maxMultiplier, zoneLevel);
                    int amount = Random.Range(entry.item.minAmount, entry.item.maxAmount) * multiplier;
                    wheelItems.Add(new WheelItem(entry.item, multiplier,amount));
                }
            }

            for (int i =0; i < bombCount; i++)
                wheelItems.Add(new WheelItem(true));

            Shuffle(wheelItems);
            return wheelItems.ToArray();
        }

        private List<RewardEntry> GetGuaranteedRewards(RewardEntry[] pool, bool isSuperZone)
        {
            var guaranteed = new List<RewardEntry>();

            for (int i = 0; i < pool.Length; i++)
            {
                var entry = pool[i];
                if (entry == null || entry.item == null)
                    continue;

                if (entry.isSuperZoneExclusive && !isSuperZone)
                    continue;
                if (entry.isGuaranteed)
                    guaranteed.Add(entry);
            }
            return guaranteed;
        }

        private List<RewardEntry> GetAvailableRewards(RewardEntry[] pool, bool isSuperZone)
        {
            var available = new List<RewardEntry>();

            for (int i = 0; i < pool.Length; i++)
            {
                var entry = pool[i];
                if (entry == null || entry.item == null)
                    continue;

                if (entry.isGuaranteed)
                    continue;

                if (entry.isSuperZoneExclusive && !isSuperZone)
                    continue;

                available.Add(entry);
            }

            return available;
        }

        private RewardEntry SelectRewardEntry(List<RewardEntry> availableEntries, RarityDropRate rarityDropRate)
        {
            if (availableEntries == null || availableEntries.Count == 0)
                return null;

            if (rarityDropRate == null)
                return availableEntries[Random.Range(0, availableEntries.Count)];

            ItemRarity targetRarity = rarityDropRate.SelectRandomRarity();
            var matchingEntries = new List<RewardEntry>();

            foreach (var entry in availableEntries)
            {
                if (entry.item != null && entry.item.rarity == targetRarity)
                    matchingEntries.Add(entry);
            }

            if (matchingEntries.Count > 0)
                return matchingEntries[Random.Range(0, matchingEntries.Count)];
            return availableEntries[Random.Range(0, availableEntries.Count)];
        }
        private int GenerateMultiplier(int maxMultiplier, int zoneLevel)
        {
            float highChance = Mathf.Clamp01((zoneLevel - 1) / 20f);

            if (Random.value < highChance)
                return Random.Range(maxMultiplier / 2 + 1, maxMultiplier + 1);
            else
                return Random.Range(1, Mathf.Max(2, maxMultiplier / 2 + 1));
        }

        private void Shuffle(List<WheelItem> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
        public int SelectTargetIndex(WheelItem[] wheelItems, bool avoidBomb)
        {
            if (!avoidBomb || wheelItems == null)
                return Random.Range(0, _wheelItemCount);

            var validIndices = new List<int>();

            for (int i = 0; i < wheelItems.Length; i++)
                if (!wheelItems[i].IsBomb)
                    validIndices.Add(i);

            return validIndices.Count > 0
                ? validIndices[Random.Range(0, validIndices.Count)]
                : Random.Range(0, _wheelItemCount);
        }
    }
}