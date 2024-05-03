using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;

namespace Knives
{
    class Warlord_Ire : PassiveItem
    {
        public static void Register()
        {

            string itemName = "Warlord's Ire";


            string resourceName = "Knives/Resources/Warlords_Ire";


            GameObject obj = new GameObject(itemName);


            var item = obj.AddComponent<Warlord_Ire>();


            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "I'm Gonna Make This World Bleed";
            string longDesc = "Run faster with shotguns equipped and gain firerate when damaged. You know what they say, 'mess with the bull get the horns.' \n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item


            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1);


            item.quality = PickupObject.ItemQuality.C;
            ID = item.PickupObjectId;

        }
        public static int ID;

        public override void Pickup(PlayerController player)
        {
            player.inventory.OnGunChanged += Inventory_OnGunChanged;
            player.healthHaver.OnDamaged += HealthHaver_OnDamaged;
            base.Pickup(player);
        }

        private void HealthHaver_OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            if (resultValue > 0 && Doingit == false)
            {
                StartCoroutine(PissedOff());
            }
        }

        private IEnumerator PissedOff()
        {
            Doingit = true;

            
            RemoveStat(PlayerStats.StatType.RateOfFire);
            AddStat(PlayerStats.StatType.RateOfFire, 1f);
            this.Owner.stats.RecalculateStats(Owner, true);

            yield return new WaitForSeconds(5f);

            RemoveStat(PlayerStats.StatType.RateOfFire);
            AddStat(PlayerStats.StatType.RateOfFire, 0f);
            this.Owner.stats.RecalculateStats(Owner, true);

            Doingit = false;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.inventory.OnGunChanged -= Inventory_OnGunChanged;
           
            return base.Drop(player);
        }
        public AIActor Vengance = null;
      

        private void Inventory_OnGunChanged(Gun previous, Gun current, Gun previousSecondary, Gun currentSecondary, bool newGun)
        {
            
            if(current.gunClass == GunClass.SHOTGUN || current.PickupObjectId == 601 || ((PlayerController)this.Owner).HasActiveItem(Mares_Leg.ID))
            {
                if(this.Owner.PlayerHasActiveSynergy("Warlord's Best Friend"))
                {
                    RemoveStat(PlayerStats.StatType.MovementSpeed);
                    AddStat(PlayerStats.StatType.MovementSpeed, 2f);
                    this.Owner.stats.RecalculateStats(Owner, true);

                }
                else
                {
                    RemoveStat(PlayerStats.StatType.MovementSpeed);
                    AddStat(PlayerStats.StatType.MovementSpeed, 1.3f);
                    this.Owner.stats.RecalculateStats(Owner, true);

                }

            }
            else
            {
                RemoveStat(PlayerStats.StatType.MovementSpeed);
                AddStat(PlayerStats.StatType.MovementSpeed, 0f);
                this.Owner.stats.RecalculateStats(Owner, true);

            }

        }

        public bool hasBuff = false;

        public bool Doingit { get; private set; }

        public override void Update()
        {
            base.Update();
            if (this.Owner)
            {
              
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