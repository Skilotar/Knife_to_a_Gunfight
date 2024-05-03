using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class DiogenesCoinpurse : PassiveItem
    {
        public static void Register()
        {

            string itemName = "Diogene's Coinpurse";


            string resourceName = "Knives/Resources/Diogene_coin_purse";


            GameObject obj = new GameObject(itemName);


            var item = obj.AddComponent<DiogenesCoinpurse>();


            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Need Not";
            string longDesc = "Less money more power, but more money less power. A coinpurse designed to be empty, rewarding those who know the curses of wealth. \n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item


            item.quality = PickupObject.ItemQuality.C;
            item.CanBeDropped = false;
            item.PersistsOnDeath = true;
            itemID = item.PickupObjectId;
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Less money more power";


        }
        public static int itemID;
        public bool hasBuff = false;
        public int casings = -1;
        public override void Pickup(PlayerController player)
        {
            
            base.Pickup(player);
        }

        public override void  Update()
        {
            base.Update();
            if (this.Owner != null)
            {
                if(casings != this.Owner.carriedConsumables.Currency)
                {
                    if (this.Owner.carriedConsumables.Currency >= 0 && this.Owner.carriedConsumables.Currency < 25)
                    {
                        RemoveStat(PlayerStats.StatType.Damage);
                        AddStat(PlayerStats.StatType.Damage, .30f);
                        this.Owner.stats.RecalculateStats(this.Owner, true);
                    }
                    if (this.Owner.carriedConsumables.Currency >= 25 && this.Owner.carriedConsumables.Currency < 50)
                    {
                        RemoveStat(PlayerStats.StatType.Damage);
                        AddStat(PlayerStats.StatType.Damage, .25f);
                        this.Owner.stats.RecalculateStats(this.Owner, true);
                    }
                    if (this.Owner.carriedConsumables.Currency >= 50 && this.Owner.carriedConsumables.Currency < 80)
                    {
                        RemoveStat(PlayerStats.StatType.Damage);
                        AddStat(PlayerStats.StatType.Damage, -.1f);
                        this.Owner.stats.RecalculateStats(this.Owner, true);
                    }
                    if (this.Owner.carriedConsumables.Currency >= 80)
                    {
                        RemoveStat(PlayerStats.StatType.Damage);
                        AddStat(PlayerStats.StatType.Damage, -.25f);
                        this.Owner.stats.RecalculateStats(this.Owner, true);
                    }
                    casings = this.Owner.carriedConsumables.Currency;
                }

                
            }

        }
        private void AddStat(PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
        {
            StatModifier modifier = new StatModifier();
            modifier.amount = amount;
            modifier.statToBoost = statType;
            modifier.modifyType = method;

            foreach (var m in passiveStatModifiers)
            {
                if (m.statToBoost == statType) return; //don't add duplicates
            }

            if (this.passiveStatModifiers == null)
                this.passiveStatModifiers = new StatModifier[] { modifier };
            else
                this.passiveStatModifiers = this.passiveStatModifiers.Concat(new StatModifier[] { modifier }).ToArray();
        }


        //Removes a stat
        private void RemoveStat(PlayerStats.StatType statType)
        {
            var newModifiers = new List<StatModifier>();
            for (int i = 0; i < passiveStatModifiers.Length; i++)
            {
                if (passiveStatModifiers[i].statToBoost != statType)
                    newModifiers.Add(passiveStatModifiers[i]);
            }
            this.passiveStatModifiers = newModifiers.ToArray();
        }
    }
}