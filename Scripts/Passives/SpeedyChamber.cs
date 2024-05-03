
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker.Actions;
using ItemAPI;
using UnityEngine;

namespace Knives
{
    class SpeedyChamber : PassiveItem
    {

        //Call this method from the Start() method of your ETGModule extension
        public static void Register()
        {
            //The name of the item
            string itemName = "Speedy Chamber";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/SpeedyChamber";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<SpeedyChamber>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Hot Hands";
            string longDesc = "Getting into a rhythm always helps to go quickly. Keep reloading and you can fire faster. This strategy may run you dry on ammo but at least its all getting down range.\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item


           


            //Set the rarity of the item

            item.quality = PickupObject.ItemQuality.A;

            ID = item.PickupObjectId;
        }

        public static int ID;
        public override void Pickup(PlayerController player)
        {
            
            player.OnReloadedGun = (Action<PlayerController,Gun>)Delegate.Combine(player.OnReloadedGun, new Action<PlayerController,Gun>(this.goFast));

            base.Pickup(player);
        }
        public int token;
        
        public int countdown;
        private void goFast(PlayerController user, Gun gun)
        {
            token++;
            countdown = 600;
        }

        public override void  Update()
        {
            if (this.Owner)
            {
                if (countdown > 0)
                {
                    countdown--;
                }
                else
                {
                    token = 0;

                }

                if (token >= 20)
                {
                    RemoveStat(PlayerStats.StatType.RateOfFire);
                    AddStat(PlayerStats.StatType.RateOfFire, 2);
                }
                else
                {
                    RemoveStat(PlayerStats.StatType.RateOfFire);
                    AddStat(PlayerStats.StatType.RateOfFire, token * .1f);
                }


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

