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
    public class Banana_Mag : PassiveItem
    {

        public static void Register()
        {

            string itemName = "Banana Mag";


            string resourceName = "Knives/Resources/banana_Mag";


            GameObject obj = new GameObject(itemName);


            var item = obj.AddComponent<Banana_Mag>();


            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Bunches of Bullets";
            string longDesc = "A magazine used originally by the guerilla gorilla rebelion forces for both extra capacity and MREs.\n\n" +
                "Extra clip size, Throws a banana when you reload at empty." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalClipCapacityMultiplier, .2f, StatModifier.ModifyMethod.ADDITIVE);



            // item.AddToSubShop(ItemBuilder.ShopType.Trorc, .01f);
            item.quality = PickupObject.ItemQuality.C;

            Gun gun = (Gun)ETGMod.Databases.Items[81];
            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0].projectile);
            
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);

            projectile2.baseData.damage = 20f;
            
            projectile2.baseData.range = 15f;
            
            projectile2.SetProjectileSpriteRight("nanner", 5, 11, false, tk2dBaseSprite.Anchor.MiddleCenter, 5, 11);

            naner = projectile2; 
            

            ID = item.PickupObjectId;
        }
        public static Projectile naner;
        public static int ID;

        public override void Pickup(PlayerController player)
        {
            player.OnReloadedGun = (Action<PlayerController, Gun>)Delegate.Combine(player.OnReloadedGun, new Action<PlayerController, Gun>(this.DoEffect));
            base.Pickup(player);
        }

        private void DoEffect(PlayerController player, Gun gun)
        {
            if(gun.ClipShotsRemaining == 0)
            {
                GameObject gameObject = SpawnManager.SpawnProjectile(naner.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag2 = component != null;
                if (flag2)
                {
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                }

            }

        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnReloadedGun = (Action<PlayerController, Gun>)Delegate.Remove(player.OnReloadedGun, new Action<PlayerController, Gun>(this.DoEffect));
            return base.Drop(player);
        }


    }
}
