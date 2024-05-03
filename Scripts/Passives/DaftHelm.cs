using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;



namespace Knives
{ 
    class daft_helm :PassiveItem
    {
        public static void Register()
        {
            string itemName = "Daft";

            string resourceName = "Knives/Resources/daft";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<daft_helm>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Faster, Stronger";
            string longDesc = "This helmet has a gorgeous silver shine. Unfortunatly it only has a single Right headphone and half of the lyrics to its song are coming out." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Coolness, 1f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.MovementSpeed, 1f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, .2f, StatModifier.ModifyMethod.ADDITIVE);





            item.quality = PickupObject.ItemQuality.C;


            ID = item.PickupObjectId;
        }

        public static int ID;




    }
}
