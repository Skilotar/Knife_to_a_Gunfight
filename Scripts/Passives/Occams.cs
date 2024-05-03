using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using ItemAPI;
using UnityEngine;


namespace Knives
{
    public class Occams : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Occam's Scalple";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Occam";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Occams>();

            //Adds a tk2dSprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "No More, No Less";
            string longDesc = "Gives firerate and reload speed for every enemy that is killed with exactly enough damage to kill. Resets every floor."
                 +
                "\n\n\n -Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            item.CanBeDropped = true;
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 2, StatModifier.ModifyMethod.ADDITIVE);
            //Set the rarity of the item
            item.quality = PickupObject.ItemQuality.B;
            item.AddToSubShop(ItemBuilder.ShopType.OldRed, .1f);
        }
        
       
        public override void Pickup(PlayerController player)
        {
            player.OnNewFloorLoaded += this.OnLoadedFloor;
            player.OnDealtDamageContext += this.Damagechecker;
            base.Pickup(player);
        }

        private void Damagechecker(PlayerController player, float damage, bool fatal, HealthHaver source)
        {
            if(damage == source.GetCurrentHealth())
            {
                ETGModConsole.Log("nice");
            }


        }

        public int Tokens = 0;
        bool half_toggle = false;

        public void OnLoadedFloor(PlayerController player)
        {

            if (half_toggle)
            {
                Tokens = 0;
                half_toggle = false;
            }
            else
            {
                half_toggle = true;
            }


        }

        public override DebrisObject Drop(PlayerController player)
        {
          
            player.OnNewFloorLoaded -= this.OnLoadedFloor;
            return base.Drop(player);
        }
        public override void  OnDestroy()
        {
          
            this.Owner.OnNewFloorLoaded -= this.OnLoadedFloor;
            base.OnDestroy();
        }

        public override void  Update()
        {
            if (this.Owner)
            {
               
                RemoveStat(PlayerStats.StatType.RateOfFire);
                AddStat(PlayerStats.StatType.RateOfFire, .05f * Tokens);
                RemoveStat(PlayerStats.StatType.ReloadSpeed);
                AddStat(PlayerStats.StatType.ReloadSpeed, -.05f * Tokens);
                this.Owner.stats.RecalculateStats(Owner, true);

               
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


