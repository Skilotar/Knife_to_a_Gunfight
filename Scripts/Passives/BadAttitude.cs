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
    public class bad_attitude : PassiveItem
    {
        
        public static void Register()
        {
            
            string itemName = "Bad Company";

            
            string resourceName = "Knives/Resources/arming_personality";

            
            GameObject obj = new GameObject(itemName);

           
            var item = obj.AddComponent<bad_attitude>();

            
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Increases Shop Prices...";
            string longDesc = "Your loud mouth and lack of tact hasn't made you many friends in the gungeon.\n\n" +
                "AH BUT WHO NEEDS FRIENDS ANYWAY! I've Got My GUNS to keep me company. That's all I need...       right?\n\n" +
                "right?" +
                "\n Increases shop prices and damage." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item


            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.GlobalPriceMultiplier, .3f, StatModifier.ModifyMethod.ADDITIVE);

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, .5f, StatModifier.ModifyMethod.ADDITIVE);


           
           // item.AddToSubShop(ItemBuilder.ShopType.Trorc, .01f);
            item.quality = PickupObject.ItemQuality.C;
            item.CanBeDropped = false;
            ID = item.PickupObjectId;
        }

        public static int ID;
    }
}