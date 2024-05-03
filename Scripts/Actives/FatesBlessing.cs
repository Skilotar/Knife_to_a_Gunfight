using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;


namespace Knives
{
    class Fates_blessing : PassiveItem
    {
        public static void Register()
        {
            string itemName = "Fate's Blessing";

            string resourceName = "Knives/Resources/Fates_Blessing";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Fates_blessing>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Chosen.";
            string longDesc = "For some reason you are still alive, a guiding force is pulling you towards your destiny whether you like it or not.\n\n" +
                "Onward gungeoneer! Stand Proud! \n\n" +
                "Increases coolness, boss damage, health, and blanks. Also gives chance to fire a genie." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Coolness, 4f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.DamageToBosses, 2f, StatModifier.ModifyMethod.ADDITIVE);
            
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalBlanksPerFloor, 1f, StatModifier.ModifyMethod.ADDITIVE);



            item.PersistsOnDeath = true;
            item.CanBeDropped = false;
            item.quality = PickupObject.ItemQuality.SPECIAL;
            ID = item.PickupObjectId;
        }
        public static int ID;
        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += this.PostProcessProjectile;
            base.Pickup(player);
        }
        private System.Random rng = new System.Random();
        private void PostProcessProjectile(Projectile SourceProjectile, float chance)
        {
            int rpg = rng.Next(1, 15);
            if (rpg == 1)
            {
                Projectile projectile = ((Gun)ETGMod.Databases.Items[0]).DefaultModule.projectiles[0];
                Vector3 vector = this.Owner.unadjustedAimPoint - this.Owner.LockedApproximateSpriteCenter;
                Vector3 vector2 = this.Owner.specRigidbody.UnitCenter;
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, this.Owner.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (this.Owner.CurrentGun == null) ? 0f : this.Owner.CurrentGun.CurrentAngle), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag = component != null;
                if (flag)
                {
                    component.allowSelfShooting = false;
                    component.Owner = this.Owner;
                    component.Shooter = this.Owner.specRigidbody;
                }
            
           
                Projectile projectile2 = ((Gun)ETGMod.Databases.Items[0]).DefaultModule.projectiles[0];
                Vector3 vector3 = this.Owner.unadjustedAimPoint - this.Owner.LockedApproximateSpriteCenter;
                Vector3 vector4 = this.Owner.specRigidbody.UnitCenter;
                GameObject gameObject2 = SpawnManager.SpawnProjectile(projectile.gameObject, this.Owner.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (this.Owner.CurrentGun == null) ? 0f : this.Owner.CurrentGun.CurrentAngle), true);
                Projectile component2 = gameObject2.GetComponent<Projectile>();
                bool flag2 = component2 != null;
                if (flag2)
                {
                    component2.allowSelfShooting = false;
                    component2.Owner = this.Owner;
                    component2.Shooter = this.Owner.specRigidbody;
                }
                Projectile projectile3 = ((Gun)ETGMod.Databases.Items[0]).DefaultModule.projectiles[0];
                Vector3 vector5 = this.Owner.unadjustedAimPoint - this.Owner.LockedApproximateSpriteCenter;
                Vector3 vector6 = this.Owner.specRigidbody.UnitCenter;
                GameObject gameObject3 = SpawnManager.SpawnProjectile(projectile.gameObject, this.Owner.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (this.Owner.CurrentGun == null) ? 0f : this.Owner.CurrentGun.CurrentAngle), true);
                Projectile component3 = gameObject3.GetComponent<Projectile>();
                bool flag3 = component3 != null;
                if (flag3)
                {
                    component3.allowSelfShooting = false;
                    component3.Owner = this.Owner;
                    component3.Shooter = this.Owner.specRigidbody;
                }
            }
        }

    }
}
   

