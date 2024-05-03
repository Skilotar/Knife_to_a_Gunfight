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
    class BrassKnuckles : PassiveItem
    {

        public static void Register()
        {
            //The name of the item
            string itemName = "Brass Knuckles";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/BrassKnuckles";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<BrassKnuckles>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Crass Brass";
            string longDesc = "Interact with nearby enemies to reach out and punch them in the face.\n\n" +
                "" +
                "A classic melee enhancer for unarmed combat styles. These were forged from spent bullet casings." +
                "\n\n\n - Knife_to_a_Gunfight";


            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");


            item.quality = PickupObject.ItemQuality.C;

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
                if (player.CurrentRoom != null)
                {
                    //ETGModConsole.Log("room good");
                    if (player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null)
                    {
                        foreach (AIActor actor in player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                        {
                            //ETGModConsole.Log("found enemeies");
                            if (actor != null)
                            {
                                if(actor.gameObject.GetComponent<PunchInteractor>() == null)
                                {
                                    actor.gameObject.GetOrAddComponent<PunchInteractor>();
                                }
                            }

                        }

                    }


                }


            }
            base.Update();
        }

       

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);

          
            return debrisObject;
        }


    }
}
