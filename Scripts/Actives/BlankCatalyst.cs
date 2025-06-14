using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using Dungeonator;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class BlankCatalyst : PlayerItem
    {

        public static void Register()
        {
            //The name of the item
            string itemName = "Omega Catalyst";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Catalyst";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<BlankCatalyst>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Spectacular Star";
            string longDesc = "Uses itself automatically when off cooldown. Produces Blanks.\n\n" +
                "An exceedingly rare blankite heartstone harvested from a fallen meteorite.\n" +
                "It draws in debris and slowly crushes it into an energy rich substance perfect for blanks." +
                "" +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 700f);
            item.consumable = false;
            item.quality = PickupObject.ItemQuality.B;

            ID = item.PickupObjectId;
        }
        public static int ID;

        public override void DoEffect(PlayerController user)
        {
            user.Blanks++;
        }

        public override void Update()
        {
            if(this.LastOwner != null)
            {
                if (this.IsOnCooldown != true)
                {
                    float destroytime = -1;
                    this.Use(this.LastOwner, out destroytime);
                }
            }
            base.Update();
        }

        
    }
}