using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class CylindSpurs : PassiveItem
    {
        public static void Register()
        {

            string itemName = "Cylindspurs";


            string resourceName = "Knives/Resources/Cylindspurs";


            GameObject obj = new GameObject(itemName);


            var item = obj.AddComponent<CylindSpurs>();


            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "He YAH!";
            string longDesc = "A little extra incentive to get a move on when you're most vulnerable. \n" +
                "Increases speed when reloading based on ammo left in clip" +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            item.quality = PickupObject.ItemQuality.C;


        }
        public float magFillPercent = 1;
        public float lastknownclipshotsremaining;
        public bool hasBuff = false;
        public override void  Update()
        {
            
            if (this.Owner)
            {
                
                if (this.Owner.CurrentGun.IsReloading && hasBuff == false)
                {
                    magFillPercent = lastknownclipshotsremaining / this.Owner.CurrentGun.ClipCapacity;
                    float SpeedBoost = 1.6f - magFillPercent;
                    RemoveStat(PlayerStats.StatType.MovementSpeed);
                    AddStat(PlayerStats.StatType.MovementSpeed, SpeedBoost );
                   
                    this.Owner.stats.RecalculateStats(this.Owner, true);
                    hasBuff = true;
                }
                if (!this.Owner.CurrentGun.IsReloading && hasBuff)
                {

                    RemoveStat(PlayerStats.StatType.MovementSpeed);
                    AddStat(PlayerStats.StatType.MovementSpeed, 0f);
                    hasBuff = false;
                    this.Owner.stats.RecalculateStats(Owner, true);
                }
                lastknownclipshotsremaining = this.Owner.CurrentGun.ClipShotsRemaining;
            }
            base.Update();
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
