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
    public class ThePill : PassiveItem
    {

        public static void Register()
        {

            string itemName = "The Pill";


            string resourceName = "Knives/Resources/ThePill";


            GameObject obj = new GameObject(itemName);


            var item = obj.AddComponent<ThePill>();


            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Hard To Swallow";
            string longDesc = "Greatly improves invulnerability time when damaged.\n" +
                "A fast acting pain killer in a really big capsule. I hate the pill, but I can't deny it is good for me." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
           
            // item.AddToSubShop(ItemBuilder.ShopType.Trorc, .01f);
            item.quality = PickupObject.ItemQuality.D;

            ID = item.PickupObjectId;
        }
        
        public static int ID;

        public override void Pickup(PlayerController player)
        {
            player.OnReceivedDamage += Player_OnReceivedDamage;
            base.Pickup(player);
        }
        private void Player_OnReceivedDamage(PlayerController player)
        {
            player.healthHaver.TriggerInvulnerabilityPeriod(2f);
        }
        public override DebrisObject Drop(PlayerController player)
        {
            player.OnReceivedDamage -= Player_OnReceivedDamage;
            return base.Drop(player);
        }


    }
}
