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
    class menacing_aura : PassiveItem
    {

        public static void Register()
        {
            //The name of the item
            string itemName = "Menacing Aura";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/menacing_stare";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<menacing_aura>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Just Standing There. MENACINGLY!";
            string longDesc = "Channel your menacing aura to unnerve all who oppose you. Normally standing still on a battlefeild is a terrible strategy, but it works great if everyone is too afraid to shoot you." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item



            //Set the rarity of the item
            item.quality = PickupObject.ItemQuality.B;


        }


        public override void Pickup(PlayerController player)
        {
            menacing_aura.fleeData = new FleePlayerData();
            menacing_aura.fleeData.Player = player;
            menacing_aura.fleeData.StartDistance = 20f;

            
            base.Pickup(player);

          
        }

        private static IEnumerator RemoveFear(AIActor aiactor)
        {

            yield return new WaitForSeconds(1f);
            aiactor.behaviorSpeculator.FleePlayerData = null;
            yield break;
        }

        public bool vangaurd = false;

        public override void  Update()
        {
            if (this.m_pickedUp && !GameManager.Instance.IsLoadingLevel && this.m_owner != null && this.Owner.Velocity.magnitude == 0 && !this.m_owner.IsFalling && this.m_owner.healthHaver && !this.m_owner.healthHaver.IsDead && this.Owner.IsInCombat)
            {   


                RoomHandler currentRoom = this.Owner.CurrentRoom;
                foreach (AIActor aiactor in currentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                {
                    if (aiactor != null)
                    {
                        vangaurd = true;
                    }
                    
                    bool flag3 = aiactor.behaviorSpeculator != null;
                    if (flag3 && vangaurd)
                    {
                        aiactor.behaviorSpeculator.FleePlayerData = menacing_aura.fleeData;
                        FleePlayerData fleePlayerData = new FleePlayerData();
                        GameManager.Instance.StartCoroutine(menacing_aura.RemoveFear(aiactor));
                    }
                }
            }

        }
        private static FleePlayerData fleeData;
        
    }
}
