using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class disco_inferno :PassiveItem
    {
        public static void Register()
        {

            string itemName = "Disco Inferno";


            string resourceName = "Knives/Resources/disco_inferno";


            GameObject obj = new GameObject(itemName);


            var item = obj.AddComponent<disco_inferno>();


            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Burn Baby Burn!";
            string longDesc = "Being on fire gives stat buffs, dance and get Hot Hot Hot, but don't get burned!\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item


           


            
            item.quality = PickupObject.ItemQuality.B;


            ID = item.PickupObjectId;
        }
        public static int ID;
        public bool hasBuff = false;
        public override void  Update()
        {
            base.Update();
            if (this.Owner)
            {
                if (this.Owner.IsOnFire && hasBuff == false)
                {

                    RemoveStat(PlayerStats.StatType.RateOfFire);
                    AddStat(PlayerStats.StatType.RateOfFire, 1.2f);
                    RemoveStat(PlayerStats.StatType.Damage);
                    AddStat(PlayerStats.StatType.Damage, .5f);
                    RemoveStat(PlayerStats.StatType.MovementSpeed);
                    AddStat(PlayerStats.StatType.MovementSpeed, .5f);
                    RemoveStat(PlayerStats.StatType.ReloadSpeed);
                    AddStat(PlayerStats.StatType.ReloadSpeed, -1f);

                    this.Owner.stats.RecalculateStats(Owner, true);
                    hasBuff = true;
                }
                if (!this.Owner.IsOnFire && hasBuff)
                {

                    RemoveStat(PlayerStats.StatType.RateOfFire);
                    AddStat(PlayerStats.StatType.RateOfFire, 0f);
                    RemoveStat(PlayerStats.StatType.Damage);
                    AddStat(PlayerStats.StatType.Damage, 0f);
                    RemoveStat(PlayerStats.StatType.MovementSpeed);
                    AddStat(PlayerStats.StatType.MovementSpeed, 0f);
                    RemoveStat(PlayerStats.StatType.ReloadSpeed);
                    AddStat(PlayerStats.StatType.ReloadSpeed, 0f);

                    this.Owner.stats.RecalculateStats(Owner, true);
                    hasBuff = false;
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
