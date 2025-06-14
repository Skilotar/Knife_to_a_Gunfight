using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ItemAPI;
using UnityEngine;

namespace Knives
{
    public class BlankSlate : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Tabula Rasa";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Blank_Slate";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<BlankSlate>();

            //Adds a tk2dSprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Blank Slate";
            string longDesc = "Spent Blanks Echo, Extra blanks per floor\n\n" +
                "A worn stone tablet with an etching of a blank. The inscription is written in 3 different languages, all indecipherable."
                 +
                "\n\n\n -Knife_to_a_Gunfight" +
                "\n Sprite in collab with Dallan";

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
        public bool doing = false;
        private void OnUsedBlank(PlayerController arg1, int arg2)
        {
            if(doing == false)
            {
                StartCoroutine(DelayEchoBlank(arg1));
            }

            this.Owner.stats.RecalculateStats(Owner, true);
        }

        private IEnumerator DelayEchoBlank(PlayerController arg1)
        {
            doing = true;
            yield return new WaitForSeconds(.5f);
            arg1.Blanks++;
            arg1.ForceConsumableBlank();
            yield return new WaitForSeconds(.2f);
            doing = false;
        }

        public override void Pickup(PlayerController player)
        {

            player.OnUsedBlank += this.OnUsedBlank;
            base.Pickup(player);
        }
        public override DebrisObject Drop(PlayerController player)
        {
            doing = false;
            player.OnUsedBlank -= this.OnUsedBlank;
            return base.Drop(player);
        }
        public override void OnDestroy()
        {
            Owner.OnUsedBlank -= this.OnUsedBlank;
            base.OnDestroy();
        }

    }
}