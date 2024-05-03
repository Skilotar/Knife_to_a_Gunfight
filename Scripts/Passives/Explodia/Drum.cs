using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;



namespace Knives
{
    class Drum_otfg : PassiveItem
    {
        public static void Register()
        {
            string itemName = "Drum of the Forbidden Gun";

            string resourceName = "Knives/Resources/Explodia/Drum_otfg_2";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Drum_otfg>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Emptied and Discarded";
            string longDesc = "The drum of a forbidden gun. An ornate drum magazine that once fed a mighty beast, its parts now Scattered. \n\n" +
                "The gun yearns to be whole. Free Him..." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalClipCapacityMultiplier, .25f, StatModifier.ModifyMethod.ADDITIVE);
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
