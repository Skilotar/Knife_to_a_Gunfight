using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using ItemAPI;

using Dungeonator;


namespace Knives
{
    class Pilots_Helmet : PassiveItem
    {
      

        public static void Register()
        {
            //The name of the item
            string itemName = "Pilots Helmet";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Cooper_helmet";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Pilots_Helmet>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Speed Is Life";
            string longDesc = "A battered jumpkit used by infantry of the frontier wars. This paticular helmet has the name -Cooper- etched into the back.\n" +
                "Allows for 5 additional rolls while beside a wall. " +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            item.ArmorToGainOnInitialPickup = 1; 
            

            item.quality = PickupObject.ItemQuality.B;
           
            ID = item.PickupObjectId;


        }

        public static int ID;

        public override void Pickup(PlayerController player)
        {
            
            player.OnIsRolling += Player_OnIsRolling;
            base.Pickup(player);
        }

        public bool CurrentlyRolling = false;
        public int rollcancels = 5;
        private void Player_OnIsRolling(PlayerController obj)
        {
            
            CurrentlyRolling = true;
            
        }

        public override DebrisObject Drop(PlayerController player)
        {
            
            player.OnIsRolling -= Player_OnIsRolling;
            return base.Drop(player);
        }
        
        public override void Update()
        {
            if (this.Owner != null)
            {
                if(this.Owner.CurrentRoom != null)
                {
                    PlayerController user = this.Owner;
                    RoomHandler room;
                    room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(Vector2Extensions.ToIntVector2(user.CenterPosition, VectorConversions.Round));
                    CellData cellaim = room.GetNearestCellToPosition(user.CenterPosition);
                    CellData cellaimmunis = room.GetNearestCellToPosition(user.CenterPosition - new Vector2(0, 1f));

                    //wallride state
                    if (cellaim.HasWallNeighbor(true, true) != false || cellaimmunis.HasWallNeighbor(true, true) != false)
                    {
                        BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(this.Owner.PlayerIDX);
                        if (instanceForPlayer.ActiveActions.DodgeRollAction.WasPressed && Time.timeScale != 0 && rollcancels > 0)
                        {
                            rollcancels--;
                            Owner.FallingProhibited = true;
                            Owner.ForceStopDodgeRoll();
                            Owner.ForceStartDodgeRoll();

                        }

                    }


                    if (CurrentlyRolling == true)// roll was initiated
                    {
                        if (this.Owner.IsDodgeRolling != true) //roll ended
                        {

                            rollcancels = 5;
                            CurrentlyRolling = false;
                            Owner.FallingProhibited = false;

                        }
                    }

                }

                base.Update();
            }

        }
    }
}