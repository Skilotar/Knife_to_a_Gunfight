using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gungeon;
using UnityEngine;
using ItemAPI;
using HutongGames.PlayMaker.Actions;
using MultiplayerBasicExample;
using MonoMod.Utils;
using MonoMod;
using System.Runtime.CompilerServices;

namespace Knives
{
    public class Luft_balloons : PlayerItem
    {
        //Call this method from the Start() method of your ETGModule extension
        public static void Register()
        {
            //The name of the item
            string itemName = "99 blobuloons";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/99_blob_balloons";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Luft_balloons>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Floating By";
            string longDesc = "Fire additional rockets.\n\n" +

                "The war machine springs to life \n " +
                "Opens up one eager eye \n" +
                "Focusing it on the sky\n" +
                "Where ninety-nine red balloons go by\n\n" +
                "This record can be heard faintly echoing through the gungeon.\n" +
                "It is rumored that in past lives the gundead held this song dear for its messages of war" +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 700f);


            //Set the rarity of the item
            
            item.consumable = false;
            item.quality = PickupObject.ItemQuality.A;
        }
        // gives balloon plays sound and subscribes the user to post fired projectile
        public override void  DoEffect(PlayerController user)
        {
            AkSoundEngine.PostEvent("m_WPN_balloon_shot_01", base.gameObject);
            user.GiveItem("balloon");
            float dura = 15f;


            user.PostProcessProjectile += this.PostProcessProjectile;
            StartCoroutine(ItemBuilder.HandleDuration(this, dura, user, EndEffect));


        }
        //this attempts to fire a rocket projectile currently broken
        private System.Random rng = new System.Random();
        private void PostProcessProjectile(Projectile SourceProjectile, float chance ) 
        {
            int rpg = rng.Next(1, 5);
            if (rpg == 1)
            {
                Projectile projectile = ((Gun)ETGMod.Databases.Items[129]).DefaultModule.projectiles[0];
                Vector3 vector = this.LastOwner.unadjustedAimPoint - this.LastOwner.LockedApproximateSpriteCenter;
                Vector3 vector2 = this.LastOwner.specRigidbody.UnitCenter;
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, this.LastOwner.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (this.LastOwner.CurrentGun == null) ? 0f : this.LastOwner.CurrentGun.CurrentAngle), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag = component != null;
                if (flag)
                {
                    component.Owner = this.LastOwner;
                    component.Shooter = this.LastOwner.specRigidbody;
                }
            }
            if (rpg == 2)
            {
                Projectile projectile = ((Gun)ETGMod.Databases.Items[16]).DefaultModule.projectiles[0];
                Vector3 vector = this.LastOwner.unadjustedAimPoint - this.LastOwner.LockedApproximateSpriteCenter;
                Vector3 vector2 = this.LastOwner.specRigidbody.UnitCenter;
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, this.LastOwner.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (this.LastOwner.CurrentGun == null) ? 0f : this.LastOwner.CurrentGun.CurrentAngle), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag = component != null;
                if (flag)
                {
                    component.Owner = this.LastOwner;
                    component.Shooter = this.LastOwner.specRigidbody;
                }
            }
        }


    
       //unsubs from projectiles
        protected void EndEffect(PlayerController user)
        {
           user.PostProcessProjectile -= this.PostProcessProjectile;
        }


        public override void  OnPreDrop(PlayerController user)
        {
            user.PostProcessProjectile -= this.PostProcessProjectile;
            base.OnPreDrop(user);
        }
    }
}