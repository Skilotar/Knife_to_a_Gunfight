using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class WobblyCylinder : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Wobbly Cylinder";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/WobblyCylinder";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<WobblyCylinder>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Wibbly Wobbly";
            string longDesc = "Randomizes firerate every shot.\n "+
                "To the crosseyed gunsmith who made this cylinder it is a perfect specimen.\n\n" +
                "Beauty really is in the eye of the beholder, cause it rattles something fierce." +
                "\n\n\n - Knife_to_a_Gunfight";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");


            item.quality = PickupObject.ItemQuality.C;
        }
        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += this.PostProcessProjectile;
            base.Pickup(player);
        }
        private void PostProcessProjectile(Projectile source, float chance)
        {
            Buff = Mathf.Lerp(-.35f, .80f, UnityEngine.Random.Range(0f, 1f));
        }
        public float Buff = .25f;

        public override void Update()
        {
            if(this.Owner != null)
            {
                RemoveStat(PlayerStats.StatType.RateOfFire);
                AddStat(PlayerStats.StatType.RateOfFire, Buff);
                this.Owner.stats.RecalculateStats(Owner, true);
            }
            base.Update();
        }

        public override DebrisObject Drop(PlayerController player)
        {
            RemoveStat(PlayerStats.StatType.RateOfFire);
            AddStat(PlayerStats.StatType.RateOfFire, 0);
            this.Owner.stats.RecalculateStats(Owner, true);
            player.PostProcessProjectile -= this.PostProcessProjectile;
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
