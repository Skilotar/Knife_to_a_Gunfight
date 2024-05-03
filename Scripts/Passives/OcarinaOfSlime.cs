using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ItemAPI;
using UnityEngine;

namespace Knives
{
    public class OcarinaOfSlime : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "OcarinaOfSlime";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Slime_flute_001";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<OcarinaOfSlime>();

            //Adds a tk2dSprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "";
            string longDesc = ""
                 +
                "\n\n\n -Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            
          
        }
        
      
        public override void Pickup(PlayerController player)
        {

          
            base.Pickup(player);
        }
        public override DebrisObject Drop(PlayerController player)
        {
           
            return base.Drop(player);
        }
        public override void OnDestroy()
        {
           
            base.OnDestroy();
        }

       
    }
}
