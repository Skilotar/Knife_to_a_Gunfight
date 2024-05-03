using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ItemAPI;
using UnityEngine;

namespace Knives
{
    public class Stop_pendant : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Halting Ammolet";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Stopper";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Stop_pendant>();

            //Adds a tk2dSprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Freeze Don't Move";
            string longDesc = "On blank stops enemy bullets\n" +
                "A ammolet with a finely tuned resonance freqency. The waves it generates disrupt enemy bullet movement."
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
            item.AddToSubShop(ItemBuilder.ShopType.OldRed, .1f);

            ID = item.PickupObjectId;
        }

        public static int ID;

        private void OnUsedBlank(PlayerController arg1, int arg2)
        {
            Stop();
        }

        public void Stop()
        {

            foreach (var projectile in GetBullets())
            {
                PassiveReflectItem.ReflectBullet(projectile, true, this.Owner, 2f, 1f, 4f, 0f);
                projectile.RemoveBulletScriptControl();
                projectile.BulletScriptSettings.preventPooling = true;
                projectile.baseData.speed *= 0f;
                
                projectile.baseData.damage *= 0.5f;
                projectile.UpdateSpeed();
                
               
                ChildProjCleanup cleanup = projectile.gameObject.GetOrAddComponent<ChildProjCleanup>();
                cleanup.delay = 7f;
                cleanup.parentProjectile = null;
                cleanup.doColor = false;
            }
        }

        private static List<Projectile> GetBullets()
        {
            List<Projectile> list = new List<Projectile>();
            var allProjectiles = StaticReferenceManager.AllProjectiles;
            for (int i = 0; i < allProjectiles.Count; i++)
            {
                Projectile projectile = allProjectiles[i];
                if (projectile && projectile.sprite && !projectile.ImmuneToBlanks && !projectile.ImmuneToSustainedBlanks)
                {
                    if (projectile.Owner != null)
                    {
                        if (projectile.isFakeBullet || projectile.Owner is AIActor || (projectile.Shooter != null && projectile.Shooter.aiActor != null) || projectile.ForcePlayerBlankable)
                        {
                            list.Add(projectile);
                        }
                    }
                }
            }
            return list;
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

    }
}