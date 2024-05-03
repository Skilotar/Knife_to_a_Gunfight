using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brave.BulletScript;
using Dungeonator;
using Gungeon;
using ItemAPI;
using MultiplayerBasicExample;
using UnityEngine;
namespace Knives
{
    class Slide_reload : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Slide Tech Reload";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/slide_reload";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Slide_reload>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Just Long Enough";
            string longDesc = "A page neatly preserved in the cult of the slide's library. Masterfully reload a few bullets when you slide over a table.\n\n" +
                "" +
                "\n\n\n - Knife_to_a_Gunfight"
                ;

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            //Set the rarity of the item
            ID = item.PickupObjectId;
            item.quality = PickupObject.ItemQuality.B;
        }

        public static int ID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

        }
        bool doingreloadroutine = false;
        public override void  Update()
        {
            base.Update();
            if (this.Owner != null)
            {
                if (this.Owner.IsSlidingOverSurface)
                {
                    if (doingreloadroutine != true)
                    {
                        StartCoroutine(reloadroutine());
                    }
                }
                
            }
        }

        private IEnumerator reloadroutine()
        {
            doingreloadroutine = true;
            this.Owner.CurrentGun.MoveBulletsIntoClip(2);
            yield return new WaitForSeconds(.25f);
            doingreloadroutine = false;
        }
    }
}