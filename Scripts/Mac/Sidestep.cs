using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class SideStep : PlayerItem
    {
        // Only real use is for Monk
        public static void Register()
        {

            string itemName = "SideStep";

            string resourceName = "Knives/Resources/Sidestep";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<SideStep>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "";
            string longDesc = "" +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;




            // item.AddToSubShop(ItemBuilder.ShopType.Flynt, .01f);
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed,.3f);
            
            
            item.quality = PickupObject.ItemQuality.B;
        }
        public Projectile projectile1 = null;
        public override void Update()
        {
            if (this.LastOwner != null)
            {
                projectile1 = this.LastOwner.CurrentGun.projectile;
            }

            base.Update();
        }

        public override void  DoEffect(PlayerController user)
        {

            user.StartCoroutine(HandleDash(user, projectile1));
            AkSoundEngine.PostEvent("Play_BOSS_RatPunchout_Player_Dodge_01", base.gameObject);
            user.CurrentStoneGunTimer = .6f;
           
            
        }

        public IEnumerator HandleDash(PlayerController user, Projectile projectile)
        {

            float duration = .15f;
            float adjSpeed = 30;
            float elapsed = -BraveTime.DeltaTime;
            float angle = user.NonZeroLastCommandedDirection.ToAngle();
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                this.LastOwner.specRigidbody.Velocity = BraveMathCollege.DegreesToVector(angle).normalized * adjSpeed;
                yield return null;
            }

        }
    }




}