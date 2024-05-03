using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ItemAPI;
using UnityEngine;

namespace Knives
{
    public class Chamber_pendant : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Chammolet";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/chammolet";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Chamber_pendant>();

            //Adds a tk2dSprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Just Add Lead";
            string longDesc = "Reloads for free when blank is used. The invention of a mad sciguntist that learned you can put bullets on the end of blanks."
                 +
                "\n\n\n -Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            item.CanBeDropped = true;
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalBlanksPerFloor, 1);
            //Set the rarity of the item
            item.quality = PickupObject.ItemQuality.B;
            item.AddToSubShop(ItemBuilder.ShopType.OldRed, .1f);

            ID = item.PickupObjectId;
        }

        public static int ID;
        
        private void OnUsedBlank(PlayerController arg1, int arg2)
        {
            arg1.CurrentGun.GainAmmo(arg1.CurrentGun.ClipCapacity);
            arg1.CurrentGun.MoveBulletsIntoClip(arg1.CurrentGun.ClipCapacity);
            LastKnownPlayer = arg1;
        }
        public override void Pickup(PlayerController player)
        {

            player.OnUsedBlank += this.OnUsedBlank;
            base.Pickup(player);
        }
        public override DebrisObject Drop(PlayerController player)
        {
            player.OnUsedBlank -= this.OnUsedBlank;
            return base.Drop(player);
        }
        public override void  OnDestroy()
        {
            if(LastKnownPlayer != null)
            {
                LastKnownPlayer.OnUsedBlank -= this.OnUsedBlank;
            }
            
            base.OnDestroy();
        }

        public static PlayerController LastKnownPlayer;
    }
}
