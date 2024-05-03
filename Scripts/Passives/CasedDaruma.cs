using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class cased_daruma :PlayerItem
    {
        // Only real use is for Monk
        public static void Register()
        {
         
            string itemName = "Stolen relic";

            string resourceName = "Knives/Resources/cased_daruma";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<cased_daruma>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "New beginnings";
            string longDesc = "This relic was stolen by a monk from his monestary as he ventured to the gungeon\n\n" +
            "The glass case has kept the daruma Fresh and Functional for years. If the daruma had not been in this case it would have ceased to function by now.";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;



            
           // item.AddToSubShop(ItemBuilder.ShopType.Flynt, .01f);
            item.consumable = true;
            item.quality = PickupObject.ItemQuality.SPECIAL;
        }
        //gives the user a little hope
        public override void  DoEffect(PlayerController user)
        {

            AkSoundEngine.PostEvent("Play_WPN_gun_empty_01", base.gameObject);

            user.GiveItem("daruma");

        }

    }
}
