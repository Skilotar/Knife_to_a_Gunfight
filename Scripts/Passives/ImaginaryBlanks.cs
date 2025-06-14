using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;
using System.Reflection;
using Dungeonator;

namespace Knives
{
    class ImaginaryBlanks : PassiveItem
    {

        public static void Register()
        {
            //The name of the item
            string itemName = "Imaginary Blanks";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/imaginaryBlanks";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<ImaginaryBlanks>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "I Think I Can";
            string longDesc = "Chance to blank any time the button is pressed.\n" +
                "These blanks may or may not actually exist." +
                "\n\n\n - Knife_to_a_Gunfight" +
                "\n Sprite by Dallan";


            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            item.quality = PickupObject.ItemQuality.D;

        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
        }

        
        public override void Update()
        {
            if (this.Owner != null)
            {
                PlayerController player = this.Owner;
                BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(player.PlayerIDX);
                if (instanceForPlayer.ActiveActions.BlankAction.WasPressed && cooldown <= 0)
                {
                    if(UnityEngine.Random.Range(0,10f) < 1)
                    {
                        player.Blanks++;
                        player.ForceConsumableBlank();
                        cooldown = 30;
                    }
                    
                }
                if(cooldown > 0)
                {
                    cooldown -= BraveTime.DeltaTime;
                }
            }
            base.Update();
        }

        public float cooldown = 0;
        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
           
            return debrisObject;
        }



    }
}
