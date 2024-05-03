using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ItemAPI;
using UnityEngine;

namespace Knives
{
    public class SmokeAmmolet : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Cigarlet";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Cigarlet";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<SmokeAmmolet>();

            //Adds a tk2dSprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "What A Drag";
            string longDesc = "Gives a little coolness every time a blank is used."
                 +
                "\n\n\n -Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            item.CanBeDropped = true;
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalBlanksPerFloor, 1);
            //Set the rarity of the item
            item.quality = PickupObject.ItemQuality.B;
            item.AddToSubShop(ItemBuilder.ShopType.OldRed,.1f);
            ID = item.PickupObjectId;
        }

        public static int ID;
        int timesused = 0;
        private void OnUsedBlank(PlayerController arg1, int arg2)
        {
            timesused++;
            RemoveStat(PlayerStats.StatType.Coolness);
            AddStat(PlayerStats.StatType.Coolness, timesused * .25f);
            this.Owner.stats.RecalculateStats(Owner, true);
        }
        public override void Pickup(PlayerController player)
        {
           
            player.OnUsedBlank += this.OnUsedBlank;
            base.Pickup(player);
        }
        public override DebrisObject Drop(PlayerController player)
        {
            player.OnUsedBlank -= this.OnUsedBlank;
            return base.Drop(player);
        }
        public override void OnDestroy()
        {
            Owner.OnUsedBlank -= this.OnUsedBlank;
            base.OnDestroy();
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
