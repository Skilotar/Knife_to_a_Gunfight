using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class Manuel : PassiveItem
    {

        public static void Register()
        {
            //The name of the item
            string itemName = "Manuel's Manual";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Manuel'sManual";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Manuel>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Rolling 101";
            string longDesc = "Roll into enemies to reload some bullets instantly. \n\n"+
                "A heavily noted version of a rolling instructional book written by Sir Manuel. \n\n" +
                "With some tweaks to his theory you can manage to free your hands during a roll by using an enemy to soften your fall. This is just barely long enough to reload a few bullets." +
               
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

           

            item.quality = PickupObject.ItemQuality.SPECIAL;
        }

        public override void Pickup(PlayerController player)
        {
            PassiveItem.IncrementFlag(player, typeof(LiveAmmoItem));
            player.OnRolledIntoEnemy += Player_OnRolledIntoEnemy;
            player.OnIsRolling += Player_OnIsRolling;
            base.Pickup(player);
        }

        private void Player_OnIsRolling(PlayerController obj)
        {
            
        }

        private void Player_OnRolledIntoEnemy(PlayerController arg1, AIActor arg2)
        {
            if(arg1.CurrentGun != null)
            {
                arg1.CurrentGun.MoveBulletsIntoClip(3);
            }
        }
        public override DebrisObject Drop(PlayerController player)
        {
            player.OnIsRolling -= Player_OnIsRolling;
            player.OnRolledIntoEnemy -= Player_OnRolledIntoEnemy;
            PassiveItem.DecrementFlag(player, typeof(LiveAmmoItem));
            return base.Drop(player);
        }



    }
}
