using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;
using ItemAPI;
using System.Collections;

namespace Knives
{
    class blobsplit : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "split tester";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/book";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<blobsplit>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Great now there are two of them";
            string longDesc =

                "You're not supposed to see this.. How did you get here?";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;



            //Set the rarity of the item


            item.quality = PickupObject.ItemQuality.EXCLUDED;
        }
        public override void Pickup(PlayerController player)
        {
           
            base.Pickup(player);
        }
        private System.Random rng = new System.Random();
        public bool Toggle = false;
        public int Toggle2 = 0;
        public override void Update()
        {
            try
            {
                RoomHandler currentRoom = this.Owner.CurrentRoom;
                foreach (AIActor aiactor in currentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                {
                    if (aiactor.EnemyGuid == "1b5810fafbec445d89921a4efb4e42b7" && aiactor.healthHaver.GetCurrentHealth() <= (aiactor.healthHaver.GetMaxHealth() / 3) && Toggle == false)
                    {
                        Toggle = true;
                        string guid;
                        if (this.Owner)
                        {
                            guid = "1b5810fafbec445d89921a4efb4e42b7";
                        }
                        else
                        {
                            guid = "1b5810fafbec445d89921a4efb4e42b7";
                        }
                        PlayerController owner = base.Owner;
                        AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(guid);
                        IntVector2? intVector = new IntVector2?(base.Owner.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
                        AIActor blob2 = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Default, true);
                        blob2.CanTargetEnemies = false;
                        blob2.CanTargetPlayers = true;
                        PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(blob2.specRigidbody, null, false);
                        blob2.gameObject.AddComponent<KillOnRoomClear>();
                        blob2.IsHarmlessEnemy = false;
                        blob2.IgnoreForRoomClear = false;
                        blob2.healthHaver.SetHealthMaximum(blob2.healthHaver.GetMaxHealth() / 5);
                        blob2.reinforceType = AIActor.ReinforceType.Instant;
                        blob2.HandleReinforcementFallIntoRoom(0f);
                        ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.RateOfFire, 3f, StatModifier.ModifyMethod.ADDITIVE);


                    }
                    if (aiactor.EnemyGuid == "1b5810fafbec445d89921a4efb4e42b7" && aiactor.healthHaver.GetCurrentHealth() <= (aiactor.healthHaver.GetMaxHealth() / 5) && Toggle2 < 2)
                    {
                        Toggle2 ++;
                        string guid;
                        if (this.Owner)
                        {
                            guid = "1b5810fafbec445d89921a4efb4e42b7";
                        }
                        else
                        {
                            guid = "1b5810fafbec445d89921a4efb4e42b7";
                        }
                        PlayerController owner = base.Owner;
                        AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(guid);
                        IntVector2? intVector = new IntVector2?(base.Owner.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
                        AIActor blob2 = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Default, true);
                        blob2.CanTargetEnemies = false;
                        blob2.CanTargetPlayers = true;
                        PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(blob2.specRigidbody, null, false);
                        blob2.gameObject.AddComponent<KillOnRoomClear>();
                        blob2.IsHarmlessEnemy = false;
                        blob2.IgnoreForRoomClear = false;
                        blob2.healthHaver.SetHealthMaximum(blob2.healthHaver.GetMaxHealth() / 8);
                        blob2.reinforceType = AIActor.ReinforceType.Instant;
                        blob2.HandleReinforcementFallIntoRoom(0f);
                        ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.RateOfFire, 3f, StatModifier.ModifyMethod.ADDITIVE);


                    }


                }

            }
            catch
            {

            }
            base.Update();
        }
        
    }
}
