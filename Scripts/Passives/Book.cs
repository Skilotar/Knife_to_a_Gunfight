using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;

namespace Knives
{ 
    class book : OnDamagedPassiveItem
    {

        public static void Register()
        {
            
            string itemName = "Book of book";

           
            string resourceName = "Knives/Resources/book";

            
            GameObject obj = new GameObject(itemName);

            
            var item = obj.AddComponent<book>();

            
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Book";
            string longDesc = "An colection of encyclopedias compressed with a powerjack into a singular book. Its heavy, thats all its good for..." +
                " its not like your gonna take the time to read it anyway. Maybe it can block some bullets. \n\n gives a ton of armor and lowers roll and speed stats." +
                " \n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

           
            item.ArmorToGainOnInitialPickup = 6;
            item.CanBeDropped = false;
            
            item.IgnoredByRat = true;
            

            
            item.quality = PickupObject.ItemQuality.B;


        }
        public int ArmorPoints = 6;
        public override void Pickup(PlayerController player)
        {
           
            player.OnReceivedDamage += this.OnReceivedDamage;
            base.Pickup(player);
        }
        public override DebrisObject Drop(PlayerController player)
        {
            player.OnReceivedDamage -= this.OnReceivedDamage;
            return base.Drop(player);
        }
        public void OnReceivedDamage(PlayerController player)
        {
            ArmorPoints--;
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
        public override void Update()
        {
            if (this.Owner)
            {
                if (ArmorPoints > 0)
                {
                    RemoveStat(PlayerStats.StatType.MovementSpeed);
                    AddStat(PlayerStats.StatType.MovementSpeed, -(0.1666666667f * ArmorPoints));
                    RemoveStat(PlayerStats.StatType.DodgeRollDistanceMultiplier);
                    AddStat(PlayerStats.StatType.DodgeRollDistanceMultiplier, -(0.016666666f * ArmorPoints));
                    RemoveStat(PlayerStats.StatType.DodgeRollSpeedMultiplier);
                    AddStat(PlayerStats.StatType.DodgeRollSpeedMultiplier, -(0.0333333333333333f * ArmorPoints));


                    this.Owner.stats.RecalculateStats(Owner, true);
                }
                if (ArmorPoints <= 0)
                {
                    RemoveStat(PlayerStats.StatType.MovementSpeed);
                    AddStat(PlayerStats.StatType.MovementSpeed, 0);
                    RemoveStat(PlayerStats.StatType.DodgeRollDistanceMultiplier);
                    AddStat(PlayerStats.StatType.DodgeRollDistanceMultiplier, 0);
                    RemoveStat(PlayerStats.StatType.DodgeRollSpeedMultiplier);
                    AddStat(PlayerStats.StatType.DodgeRollSpeedMultiplier, 0);


                    this.Owner.stats.RecalculateStats(Owner, true);
                }
            }
            base.Update();
        }
    }
}

