using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;




namespace Knives
{
    class punk_helm :PassiveItem
    {

        public static void Register()
        {
            //The name of the item
            string itemName = "Punk";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Punk";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<punk_helm>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Harder, Better";
            string longDesc = "This helmet is a beautiful technicolor display of lights and sounds. Unfortunately it only has a single left headphone and half of the lyrics to its song are coming out." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Coolness, 1f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.KnockbackMultiplier, .5f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, .2f, StatModifier.ModifyMethod.ADDITIVE);




            //Set the rarity of the item

            item.quality = PickupObject.ItemQuality.C;

            ID = item.PickupObjectId;
        }


        public static int ID;














    }
}
