using System;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using Dungeonator;
using System.Collections.Generic;

namespace Knives
{
    public class Sundail : PassiveItem
    {
        public static void Register()
        {
            string itemName = "Sundial Stopwatch";

            string resourceName = "Knives/Resources/sundial_stopwatch";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Sundail>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Sun's Setting";
            string longDesc = "Boosts stats at the start of a floor and gradually lowers over time into a deficit. \n\n" +
                "Your high noon will only last so long. Take your shots and celebrate before dusk comes to end your story." +
                "\n\n\n -Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item


            item.quality = PickupObject.ItemQuality.A;
            
            item.PersistsOnDeath = true;
            item.CanBeDropped = false;
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            itemID = item.PickupObjectId;

            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Stat Boost at start of floor that decays over time into negatives.";
        }

        public static int itemID;
        public override void Pickup(PlayerController player)
        {
            
            player.OnNewFloorLoaded += this.OnLoadedFloor;
            base.Pickup(player);


        }
        float dusktimer = 180;
        bool half_toggle = false;
        public void OnLoadedFloor(PlayerController player)
        {

            if (half_toggle)
            {
                dusktimer = 180;
                PlayedSSS = false;
                PlayedHowl = false;
                half_toggle = false;
            }
            else
            {
                half_toggle = true;
            }


        }
        bool PlayedSSS = false;
        bool PlayedHowl = false; 
        public override void  Update()
        {
            if (this.Owner)
            {
                if(dusktimer >= -180)
                {
                    dusktimer -= Time.deltaTime; // 3 mins until debuffing
                }
                RemoveStat(PlayerStats.StatType.RateOfFire);
                AddStat(PlayerStats.StatType.RateOfFire, dusktimer * .00333333333f); // 60% initial boost and - 60% total negative
                RemoveStat(PlayerStats.StatType.Damage);
                AddStat(PlayerStats.StatType.Damage, dusktimer * .00333333333f);
                this.Owner.stats.RecalculateStats(Owner, true);
            }
            base.Update();
        }

        public override DebrisObject Drop(PlayerController player)
        {

            player.OnNewFloorLoaded -= this.OnLoadedFloor;
            return base.Drop(player);
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