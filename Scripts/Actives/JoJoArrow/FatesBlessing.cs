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

            string resourceName = "Knives/Resources/Fated_Blessing";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Fates_blessing>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Chosen.";
            string longDesc = "For some reason you are still alive, a guiding force is pulling you towards your destiny whether you like it or not.\n\n" +
                "Onward gungeoneer! Stand Proud! \n\n" +
                "Increases coolness, damage, health. Also gives chance to fire a genie." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Coolness, 4f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.DamageToBosses, 1f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, .5f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Health, 3);
           
            item.PersistsOnDeath = true;
            item.CanBeDropped = false;
            item.quality = PickupObject.ItemQuality.SPECIAL;
            ID = item.PickupObjectId;
        }
        public static int ID;
        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += this.PostProcessProjectile;
            KnownOwner = player;
            base.Pickup(player);
        }
        public PlayerController KnownOwner;
        private System.Random rng = new System.Random();
        private void PostProcessProjectile(Projectile SourceProjectile, float chance)
        {
            int rpg = rng.Next(1, 10);
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
                    component.baseData.damage = 0;
                    component.OnHitEnemy += DoGenieHit;
                }
            
            }
        }

        private void DoGenieHit(Projectile arg1, SpeculativeRigidbody Body, bool arg3)
        {
            AIActor enemy = Body.aiActor;
            if(enemy != null)
            {
                bool flag = enemy && enemy.behaviorSpeculator; // THE ENEMY!
                if (flag)
                {
                    enemy.behaviorSpeculator.Stun(.5f, false);
                    Projectile projectile = ((Gun)ETGMod.Databases.Items[0]).DefaultModule.projectiles[0];
                    Projectile component = MiscToolMethods.SpawnProjAtPosi(projectile, enemy.sprite.WorldCenter, KnownOwner);
                    Projectile component2 = MiscToolMethods.SpawnProjAtPosi(projectile, enemy.sprite.WorldCenter, KnownOwner);
                    component.IgnoreTileCollisionsFor(10f);
                    component.Owner = KnownOwner;
                    component.baseData.damage = 0f;
                    component.baseData.force = 0f;
                    component.baseData.range = 0f;
                    component.pierceMinorBreakables = true;
                    component.collidesWithPlayer = false;
                    component2.IgnoreTileCollisionsFor(10f);
                    component2.Owner = KnownOwner;
                    component2.baseData.damage = 0f;
                    component2.baseData.force = 0f;
                    component2.baseData.range = 0f;
                    component2.pierceMinorBreakables = true;
                    component2.collidesWithPlayer = false;


                }
            }
           
        }
    }
}
   

