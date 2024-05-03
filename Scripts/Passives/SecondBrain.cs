using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;

namespace Knives
{
    class Second_Brain : PassiveItem
    {
        public static void Register()
        {

            string itemName = "Second Brain";


            string resourceName = "Knives/Resources/second_brain";


            GameObject obj = new GameObject(itemName);


            var item = obj.AddComponent<Second_Brain>();


            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Great Minds..";
            string longDesc = "Increases gun stats based on the classes of carried weapons.\n" +
                "" +
                "A secondary brain in a jar. It subtly calculates how to best use the weaponry you have and hightens your abilities accordingly.\n\n" +
                "" +
                "None, Rifle, Beam, and Charge weapons increase damage. \n\n" +
                "Pistol and Poison weapons increase Accuracy. \n\n" +
                "Shotgun and Bad weapons increase Knockback. \n\n" +
                "Full Auto, Fire, and Silly weapons increase Rate of Fire. \n\n" +
                "Ice weapons increase coolness. \n\n" +
                "Charm weapons increase projectile speed. \n\n" +
                "Explosive weapons increase reload speed. \n\n" +
                "" +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            item.quality = PickupObject.ItemQuality.B;
            ID = item.PickupObjectId;

        }
        public static int ID;

        public override void Pickup(PlayerController player)
        {

            base.Pickup(player);
        }



        public override DebrisObject Drop(PlayerController player)
        {
            ResetDict();
            DoBuffs();
            KnownGunsCount = 0;
            return base.Drop(player);
        }

        public int KnownGunsCount = 0;
        public override void Update()
        {
           
            if (this.Owner)
            {
                if(this.Owner.inventory.AllGuns.Count != KnownGunsCount)
                {
                    ResetDict();
                    foreach (Gun gun in this.Owner.inventory.AllGuns)
                    {   // ugly code :(
                        // GunClass enum numbers are weird 
                        if (gun.gunClass == GunClass.NONE) BuffDict["None"]++;
                        if (gun.gunClass == GunClass.PISTOL) BuffDict["Pistol"]++;
                        if (gun.gunClass == GunClass.SHOTGUN) BuffDict["Shotgun"]++;
                        if (gun.gunClass == GunClass.FULLAUTO) BuffDict["FullAuto"]++;
                        if (gun.gunClass == GunClass.RIFLE) BuffDict["Rifle"]++;
                        if (gun.gunClass == GunClass.RIFLE) BuffDict["Beam"]++;
                        if (gun.gunClass == GunClass.POISON) BuffDict["Poison"]++;
                        if (gun.gunClass == GunClass.FIRE) BuffDict["Fire"]++;
                        if (gun.gunClass == GunClass.ICE) BuffDict["Ice"]++;
                        if (gun.gunClass == GunClass.CHARM) BuffDict["Charm"]++;
                        if (gun.gunClass == GunClass.EXPLOSIVE) BuffDict["Explosive"]++;
                        if (gun.gunClass == GunClass.SILLY) BuffDict["Silly"]++;
                        if (gun.gunClass == GunClass.SHITTY) BuffDict["Shitty"]++;
                        if (gun.gunClass == GunClass.CHARGE) BuffDict["Charge"]++;
                    }
                    DoBuffs();
                    KnownGunsCount = this.Owner.inventory.AllGuns.Count;
                }
                
            }
            base.Update();
        }
        public float buffpercent = .075f;
        private void DoBuffs()
        {

            float damage = BuffDict["None"] + BuffDict["Rifle"] + BuffDict["Charge"] + BuffDict["Beam"];
            float accuracy = BuffDict["Pistol"] + BuffDict["Poison"];
            float knockback = BuffDict["Shotgun"] + BuffDict["Shitty"];
            float ROF = BuffDict["FullAuto"] + BuffDict["Fire"] + BuffDict["Silly"];

            RemoveStat(PlayerStats.StatType.Damage);  
            AddStat(PlayerStats.StatType.Damage, buffpercent * damage);

            RemoveStat(PlayerStats.StatType.Accuracy);  
            AddStat(PlayerStats.StatType.Accuracy, -buffpercent * accuracy);

            RemoveStat(PlayerStats.StatType.KnockbackMultiplier); 
            AddStat(PlayerStats.StatType.KnockbackMultiplier, buffpercent * knockback);

            RemoveStat(PlayerStats.StatType.RateOfFire);  
            AddStat(PlayerStats.StatType.RateOfFire, buffpercent * ROF);

            RemoveStat(PlayerStats.StatType.Coolness); 
            AddStat(PlayerStats.StatType.Coolness, 1 * BuffDict["Ice"]);

            RemoveStat(PlayerStats.StatType.ProjectileSpeed); 
            AddStat(PlayerStats.StatType.ProjectileSpeed, buffpercent * BuffDict["Charm"]);

            RemoveStat(PlayerStats.StatType.ReloadSpeed); 
            AddStat(PlayerStats.StatType.ReloadSpeed, -buffpercent * BuffDict["Explosive"]);

            this.Owner.stats.RecalculateStats(Owner, true);
        }

        private void ResetDict()
        {
            BuffDict["None"] = 0;
            BuffDict["Pistol"] = 0;
            BuffDict["Shotgun"] = 0;
            BuffDict["FullAuto"] = 0;
            BuffDict["Rifle"] = 0;
            BuffDict["Poison"] = 0;
            BuffDict["Fire"] = 0;
            BuffDict["Ice"] = 0;
            BuffDict["Charm"] = 0;
            BuffDict["Explosive"] = 0;
            BuffDict["Silly"] = 0;
            BuffDict["Shitty"] = 0;
            BuffDict["Charge"] = 0;
            BuffDict["Beam"] = 0;
        }

        public static Dictionary<string, float> BuffDict = new Dictionary<string, float>
        {
            {"None",0},
            {"Pistol",0},
            {"Shotgun",0},
            {"FullAuto",0},
            {"Rifle",0},
            {"Beam",0},
            {"Poison",0},
            {"Fire",0},
            {"Ice",0},
            {"Charm",0},
            {"Explosive",0},
            {"Silly",0},
            {"Shitty",0},
            {"Charge",0},
        };

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