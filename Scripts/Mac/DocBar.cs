using System;
using System.Collections;

using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using Dungeonator;


namespace Knives
{
    public class DocBar : PassiveItem
    {
        public static void Register()
        {
            string itemName = "Doc's Candy Bar";

            string resourceName = "Knives/Resources/DocsBar";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<DocBar>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Post Round Aid";
            string longDesc = "Heals user between floors." +
                "\n\n\n -Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
           

            item.quality = PickupObject.ItemQuality.B;


        }
        public override void Pickup(PlayerController player)
        {
            IncrementFlag(player, typeof(LiveAmmoItem));
            player.OnNewFloorLoaded += this.OnLoadedFloor;
            base.Pickup(player);

           
        }

       
        
        bool half_toggle = false;
        public void OnLoadedFloor(PlayerController player)
        {

            if (half_toggle)
            {
                player.healthHaver.ApplyHealing(.5f);
                half_toggle = false;
            }
            else
            {
                half_toggle = true;
            }

           
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DecrementFlag(player, typeof(LiveAmmoItem));
            player.OnNewFloorLoaded -= this.OnLoadedFloor;
            return base.Drop(player);
        }

    }
}


