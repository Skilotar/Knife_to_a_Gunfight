using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using Dungeonator;
using System.Collections.Generic;

namespace Knives
{
    public class EjectButton : PassiveItem
    {
        public static void Register()
        {
            try
            {
                string itemName = "Eject Button";

                string resourceName = "Knives/Resources/Eject";

                GameObject obj = new GameObject(itemName);

                var item = obj.AddComponent<EjectButton>();

                ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

                //Ammonomicon entry variables
                string shortDesc = "Run Away";
                string longDesc = "On every floor blocks 1 hit and sends user back one room instantly. \n\nAn instantanious transmission device used by one Alfred Von Peacock to avoid awkard social situations and escape the law." +
                    " \n\nJust one press and poof no more hard conversations" +

                    "\n\n\n -Knife_to_a_Gunfight";

                //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
                //Do this after ItemBuilder.AddSpriteToObject!
                ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");




                item.quality = PickupObject.ItemQuality.D;


               ID = item.PickupObjectId;
            }
            catch(Exception E)
            {
                ETGModConsole.Log(E.ToString());
            }



        }

        public static int ID;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnNewFloorLoaded += this.OnLoadedFloor;
            HealthHaver healthHaver = player.healthHaver;
            healthHaver.ModifyDamage = (Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>)Delegate.Combine(healthHaver.ModifyDamage, new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.ModifyIncomingDamage));

            player.OnEnteredCombat = (Action)Delegate.Combine(player.OnEnteredCombat, new Action(this.OnEnteredCombat));

            base.Pickup(player);
        }
        int synergyroomcounter = 3;
        private void OnEnteredCombat()
        {
            if (this.Owner.PlayerHasActiveSynergy("Second Impression"))
            {
                if (active == false && synergyroomcounter != 0)
                {
                    synergyroomcounter--;

                }
                if (active == false && synergyroomcounter == 0)
                {
                    active = true;
                    this.Owner.BloopItemAboveHead(base.sprite, "Knives/Resources/Eject");
                }
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnNewFloorLoaded -= this.OnLoadedFloor;
            HealthHaver healthHaver = player.healthHaver;
            healthHaver.ModifyDamage = (Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>)Delegate.Remove(healthHaver.ModifyDamage, new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.ModifyIncomingDamage));
            player.OnEnteredCombat = (Action)Delegate.Remove(player.OnEnteredCombat, new Action(this.OnEnteredCombat));
            return base.Drop(player);

        }

        private void ModifyIncomingDamage(HealthHaver arg1, HealthHaver.ModifyDamageEventArgs arg2)
        {
            if (active)
            {
                arg2.ModifiedDamage = 0;
                active = false;
                synergyroomcounter = 3;
                List<RoomHandler> list = new List<RoomHandler>();
                List<RoomHandler> list2 = new List<RoomHandler>();
                list.Add(this.Owner.CurrentRoom);
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
                    StartCoroutine(delayPopeffect());
                    this.Owner.RespawnInPreviousRoom(false, PlayerController.EscapeSealedRoomStyle.TELEPORTER, true, list[list.Count - 1]);
                   
                    for (int i = 1; i < list.Count - 1; i++)
                    {
                        list[i].ResetPredefinedRoomLikeDarkSouls();
                    }
                }

            }
        }
        

        private IEnumerator delayPopeffect()
        {
            yield return new WaitForSeconds(.5f);
            this.Owner.BloopItemAboveHead(base.sprite, "Knives/Resources/Eject");
        }

        bool active = true;
        bool half_toggle = false;
        public void OnLoadedFloor(PlayerController player)
        {

            if (half_toggle)
            {
                active = true;
                half_toggle = false;
            }
            else
            {
                half_toggle = true;
            }


        }

        
    }
}