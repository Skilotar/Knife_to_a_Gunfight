using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;

using Dungeonator;

namespace Knives
{ 
    class BloodyNapkin :PlayerItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Bloody Napkin";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Bloody_napkin";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<BloodyNapkin>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Ecnalubma";
            string longDesc = "Resets you back 1 room as if nothing had ever happned." +
                "\n\n" +
                "Well its a good thing that I brought this napkin. I knew that this would happen. Why does this always happen?!\n\n" +
                "Help me out I can't seem to get this window open. Nevermind now its open. I think my hand is broken!\n\n" +
                "Look at that now you're all patched up there. Almost like it never happened. Almost like what never happened??\n\n" +
                "Exactly.\n\n" +
                "" +
                "" +
                "\n\n\n - Knife_to_a_Gunfight";

           
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item


            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 800f);
           
            //Set the rarity of the item
            
            item.quality = PickupObject.ItemQuality.C;

        }
        public override void Pickup(PlayerController player)
        {
            player.OnEnteredCombat = (Action)Delegate.Combine(player.OnEnteredCombat, new Action(this.detectHealth));
            thisRoom = player.healthHaver.GetCurrentHealth();

            base.Pickup(player);
        }
        
        public float lastRoom = 0;
        public float thisRoom = 0;
        public void detectHealth()
        {
            lastRoom = thisRoom;
            thisRoom = this.LastOwner.healthHaver.GetCurrentHealth();
        }
        public void Healuser(PlayerController player)
        {

            thisRoom = this.LastOwner.healthHaver.GetCurrentHealth();

            if (lastRoom > thisRoom)
            {
                float reset = lastRoom - thisRoom;
                player.healthHaver.ApplyHealing(reset);
            }
           
           
        }
        public List<RoomHandler> list = new List<RoomHandler>();
        public override void DoEffect(PlayerController user)
        {

            Healuser(user);
            List<RoomHandler> list2 = new List<RoomHandler>();
            list.Add(user.CurrentRoom);
            while (list.Count - 1 < 1)
            {
                RoomHandler roomHandler = list[list.Count - 1];
                list2.Clear();
                foreach (RoomHandler roomHandler2 in roomHandler.connectedRooms)
                {
                    if (roomHandler2.hasEverBeenVisited && roomHandler2.distanceFromEntrance < roomHandler.distanceFromEntrance && !list.Contains(roomHandler2))
                    {
                        if (!roomHandler2.area.IsProceduralRoom || roomHandler2.area.proceduralCells == null)
                        {
                            list2.Add(roomHandler2);
                        }
                    }
                }
                if (list2.Count == 0)
                {
                    break;
                }
                list.Add(BraveUtility.RandomElement<RoomHandler>(list2));
            }
            if (list.Count > 1)
            {
                
                user.RespawnInPreviousRoom(false, PlayerController.EscapeSealedRoomStyle.TELEPORTER, true, list[list.Count - 1]);

                for (int i = 1; i < list.Count - 1; i++)
                {
                    list[i].ResetPredefinedRoomLikeDarkSouls();
                }
            }
           

        }

        public override void Update()
        {
           
            if(this.LastOwner != null)
            {
                CanBeUsed(LastOwner);
            }
            base.Update();
        }

        public override bool CanBeUsed(PlayerController user)
        {
            if (user.IsInCombat)
            {
                return true;
            }
            else
            {
                return false;
            }

            
        }

        protected void EndEffect(PlayerController user)
        {
       
        }

        
        public override void OnPreDrop(PlayerController player)
        {
            player.OnEnteredCombat = (Action)Delegate.Combine(player.OnEnteredCombat, new Action(this.detectHealth));
            
            base.OnPreDrop(player);
        }
    }
}
