using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class roundabout :PlayerItem
    {
        
        public static void Register()
        {
            //The name of the item
            string itemName = "Round After Roundabout";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/round after round";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<roundabout>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "To Be Continued...";
            string longDesc = "This particular song took several years to write as the first verse had the unexplained ability to stop time. \n\n\n - Knife_to_a_Gunfight" +
                "";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 500f);
            
            //Set the rarity of the item
            item.quality = PickupObject.ItemQuality.B;

        }
        
        public override void  DoEffect(PlayerController user)
        {

            RadialSlowInterface test = new RadialSlowInterface();
            test.RadialSlowHoldTime = 12f;
            test.RadialSlowOutTime = 5f;
            test.RadialSlowTimeModifier = 0f;
            test.DoesSepia = true;
            test.UpdatesForNewEnemies = true;
            test.RadialSlowInTime = 2.5f;
            test.DoRadialSlow(user.CenterPosition, user.CurrentRoom);
        }
        
    }
}

