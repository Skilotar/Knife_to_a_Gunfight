using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using ItemAPI;
using UnityEngine;
using System.Reflection;
using GungeonAPI;
using MonoMod.RuntimeDetour;


namespace Knives
{
    public class MasterKey : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Master Key";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Master_Key";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<MasterKey>();

            //Adds a tk2dSprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Stay Winning";
            string longDesc = "Gain two keys when picking up a new master round. \n\n" +
                "A key gifted to locksmiths who perfectly pick a lock on the first try."
                 +
                "\n\n\n -Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            item.CanBeDropped = true;
            item.quality = PickupObject.ItemQuality.C;
            //Set the rarity of the item
            ID = item.PickupObjectId;


        }

            public static int ID;
            Hook MasterRoundPickupHook = new Hook(
                typeof(BasicStatPickup).GetMethod("Pickup", BindingFlags.Instance | BindingFlags.Public),
                typeof(MasterKey).GetMethod("MasterRoundHookMethod")
          );
        public static void MasterRoundHookMethod(Action<PickupObject, PlayerController> orig, PickupObject self, PlayerController player)
        {
            //ETGModConsole.Log("Hookworks yoooo");
            orig(self, player);
            if (player.HasPickupID(ID))
            {
                if (self.PickupObjectId == 469 || self.PickupObjectId == 471 || self.PickupObjectId == 468 || self.PickupObjectId == 470 || self.PickupObjectId == 467)
                {
                    if (self.gameObject.GetOrAddComponent<ItemSpecialStates>().MarkedMasterRound != true)
                    {
                        self.gameObject.GetOrAddComponent<ItemSpecialStates>().MarkedMasterRound = true;
                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(67).gameObject, player.specRigidbody.UnitCenter, Vector2.zero, 0f, false, false, false);
                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(67).gameObject, player.specRigidbody.UnitCenter, Vector2.zero, 0f, false, false, false);
                    }
                }
               
            }
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