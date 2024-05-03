using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class Greedy : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Greedy Clips";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Greedy_clip";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Greedy>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Bottomless?";
            string longDesc =

                "Clips fill at the bottom. Bullets that are fired when the clip is empty cost double ammo. Does not work with infinite ammo guns \n\n" +
                "An abyssal clip offered as a gift from an entity that knows only hunger." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;



            //Set the rarity of the item

            item.CanBeDropped = false;
            item.quality = PickupObject.ItemQuality.B;
            ID = item.PickupObjectId;
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Clips Fill at the bottom, Overdrawn shots take double";
        }

        public static int ID;
        public override void Pickup(PlayerController player)
        {
            player.CurrentGun.OnPostFired += this.Onpostfired;
            player.PostProcessProjectile += Player_PostProcessProjectile;
            player.inventory.OnGunChanged += Inventory_OnGunChanged;
            base.Pickup(player);
        }

        private void Inventory_OnGunChanged(Gun previous, Gun current, Gun previousSecondary, Gun currentSecondary, bool newGun)
        {
            this.Owner.CurrentGun.OnPostFired -= this.Onpostfired;
            this.Owner.CurrentGun.OnPostFired += this.Onpostfired;
        }

        private void Player_PostProcessProjectile(Projectile arg1, float arg2)
        {
            Gun thisone = this.Owner.CurrentGun;
            if (!thisone.InfiniteAmmo)
            {
                if(thisone.DefaultModule.shootStyle != ProjectileModule.ShootStyle.SemiAutomatic)
                {
                    if (thisone.ClipShotsRemaining == 1)
                    {
                        thisone.MoveBulletsIntoClip(1);
                        thisone.LoseAmmo(1);
                    }
                }
            }
        }

        private void Onpostfired(PlayerController arg1, Gun arg2)
        {
            Gun thisone = this.Owner.CurrentGun;
            if (!thisone.InfiniteAmmo)
            {
                if(thisone.DefaultModule.shootStyle == ProjectileModule.ShootStyle.SemiAutomatic)
                { 
                    if(thisone.ClipShotsRemaining == 0)
                    {
                        thisone.MoveBulletsIntoClip(1);
                        thisone.LoseAmmo(1);

                    }
                }
            }
           
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.CurrentGun.OnPostFired -= this.Onpostfired;
            player.inventory.OnGunChanged -= Inventory_OnGunChanged;
            player.PostProcessProjectile -= Player_PostProcessProjectile;
            return base.Drop(player);
        }
       
        public override void  Update()
        {
            
            base.Update();
        }

        private void PostProcessProjectile(Projectile projectile, float chance)
        {
           
        }
       
          

        

      
    }
}

