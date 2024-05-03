using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;


namespace Knives
{
    class Space_hammer : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Space Hammer";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Space_Hammer";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Space_hammer>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Modded Gungeon's Best Friend";
            string longDesc =

                "A hammer created by a gungoneer from a planet of the second dimension. This hammer boasts the power to tear a pocket halfway into the next dimension in order to put excess items inside." +
                "\n\n\n - Knife_to_a_Gunfight ";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 8f, StatModifier.ModifyMethod.ADDITIVE);


            //Set the rarity of the item

            //item.AddToSubShop(ItemBuilder.ShopType.Goopton, .01f);
            item.quality = PickupObject.ItemQuality.B;
        }

    }
}
