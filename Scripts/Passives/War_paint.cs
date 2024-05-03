using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using Dungeonator;
using UnityEngine;
using ItemAPI;
using System.Reflection;
using MultiplayerBasicExample;


namespace Knives
{
    class War_paint : PassiveItem
    {

        public static void Register()
        {
            //The name of the item
            string itemName = "War Paint";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/WarPaint_icon";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<War_paint>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Sending A Message!";
            string longDesc = "Getting kills quickly applies fear to enemies\n\n" +
                "Your body is covered in various red paints. Your victims will get the picture soon enough...";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item



            //Set the rarity of the item
            item.quality = PickupObject.ItemQuality.C;


        }
        public int Counter = 0;
        public int recent_kills = 0;
        public override void Pickup(PlayerController player)
        {
             War_paint.fleeData = new FleePlayerData();
            War_paint.fleeData.Player = player;
            War_paint.fleeData.StartDistance = 20f;

            player.OnKilledEnemy += this.OnKilledEnemy;

            base.Pickup(player);


        }

        public void OnKilledEnemy(PlayerController user)
        {
            
            recent_kills++;
            StartCoroutine(this.Killtimer());
        }

        private IEnumerator Killtimer()
        {

            yield return new WaitForSeconds(5f);
            Counter = 0;
            recent_kills = 0;
            yield break;
        }
        private static IEnumerator RemoveFear(AIActor aiactor)
        {

            yield return new WaitForSeconds(5f);
            aiactor.behaviorSpeculator.FleePlayerData = null;
            yield break;
        }

        public override void  Update()
        {
           
            //fear effect on getting 3 quick kills
            if(this.Owner != null)
            {
                if (recent_kills >= 3)
                {
                    RoomHandler currentRoom = this.Owner.CurrentRoom;
                    if(currentRoom != null)
                    {
                        foreach (AIActor aiactor in currentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                        {
                            aiactor.behaviorSpeculator.FleePlayerData = War_paint.fleeData;
                            FleePlayerData fleePlayerData = new FleePlayerData();
                            GameManager.Instance.StartCoroutine(War_paint.RemoveFear(aiactor));

                        }
                    }
                   
                }
            }
           

        }
        private static FleePlayerData fleeData;

    }
}
