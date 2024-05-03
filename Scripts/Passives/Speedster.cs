using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class speedster :OnKillEnemyItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Speedster's helmet";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/speedster";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<speedster>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Show Me Your Moves!";
            string longDesc =

                "This helmet once belonged to a bounty hunting racer that could never move quick enough to satisfy his need for speed." +
                "The helmet itself seems to yearn to relive his past of high flying action.\n\n" +
                "Adds speed and gain 1 ammo for every kill." +
                "\n\n\n - Knife_to_a_Gunfight";
                

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;



            //Set the rarity of the item

            
            item.activationStyle = 0;
            item.chanceOfActivating = 1f;

            item.ammoToGain = 1;
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.MovementSpeed, 2.5f, StatModifier.ModifyMethod.ADDITIVE);
            
            item.ArmorToGainOnInitialPickup = 1;
            item.quality = PickupObject.ItemQuality.B;
           // item.AddToSubShop(ItemBuilder.ShopType.Trorc, .01f);
        }
       

    }
}
