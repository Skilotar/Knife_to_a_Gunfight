using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using ItemAPI;

namespace Knives
{
    public class Super_fly : PassiveItem
    {
        public static void Register()
        {
            string itemName = "Super Fly"; //The name of the item
            string resourceName = "Knives/Resources/super_fly"; //Refers to an embedded png in the project. Make sure to embed your resources!

            GameObject obj = new GameObject();
            var item = obj.AddComponent<Super_fly>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Feeling Good!";
            string longDesc = "When you're confident its like nothing can stop you! Your own coolness lets you handle weaponry more effectively. \n\n" +
                "Increase firerate when cool." +
                "\n\n\n - Knife_to_a_Gunfight";
                ;

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            
            item.quality = PickupObject.ItemQuality.SPECIAL;
           
        }
        
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }

        public override void  Update()
        {
            base.Update();
            if(this.Owner != null && Time.timeScale != 0)
            {
                Confidence();
            }
           
        }

        private float coolness = 0, lastCoolness = -1;
        private void Confidence()
        {
            if (!this.Owner || !this.Owner.stats) return;

            coolness = GetTrueTotalCoolness(this.Owner);
            if (coolness == lastCoolness) return;

            RemoveStat(PlayerStats.StatType.RateOfFire);
            AddStat(PlayerStats.StatType.RateOfFire, coolness * .10f);
            
            this.Owner.stats.RecalculateStats(Owner, true);

            lastCoolness = coolness;
        }

        public float GetTrueTotalCoolness(PlayerController player)
        {
            float coolness = player.stats.GetStatValue(PlayerStats.StatType.Coolness);
            if (PassiveItem.IsFlagSetForCharacter(player, typeof(ChamberOfEvilItem)))
            {
                float sixthChamberCoolness = player.stats.GetStatValue(PlayerStats.StatType.Curse);
                coolness += sixthChamberCoolness * 2f;
            }
            return coolness;
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