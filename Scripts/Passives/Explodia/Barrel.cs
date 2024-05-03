using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;



namespace Knives
{
    class Barrel_otfg : PassiveItem
    {
        public static void Register()
        {
            string itemName = "Barrel of the Forbidden Gun";

            string resourceName = "Knives/Resources/Explodia/Barrel_otfg_2";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Barrel_otfg>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Broken and Bound";
            string longDesc = "The barrel of a forbidden gun. The gun was feared by kaliber. She shattered it and had its pieces gaurded by her trusted kin. \n\n" +
                "The gun yearns to be whole. Free Him..." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.RateOfFire, .15f, StatModifier.ModifyMethod.ADDITIVE);
            item.quality = PickupObject.ItemQuality.B;
            item.RespawnsIfPitfall = true;

            ID = item.PickupObjectId;
        }

        public static int ID;

        public override void Pickup(PlayerController player)
        {
            player.gameObject.GetOrAddComponent<Explodia_part_holder_Controller>();

            base.Pickup(player);
        }


    }
}
