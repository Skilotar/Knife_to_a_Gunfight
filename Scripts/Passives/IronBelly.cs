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
    public class IronStomach : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Iron Stomach";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/IronStomach";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<IronStomach>();

            //Adds a tk2dSprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Bring Me Another!";
            string longDesc = "Hearts stored for later are converted to armor.\n\n" +
                "A Stomach extension module used for lengthy scouting missions. " +
                "The Hegemony of man only installed these units into a few groups. " +
                "The unit was discontinued as adverse affects of the excess calorical intake lead to health issues."
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
        Hook BellyHook = new Hook(
            typeof(HealthPickup).GetMethod("Interact", BindingFlags.Instance | BindingFlags.Public),
            typeof(IronStomach).GetMethod("BellyHookMethod")
      );

        public static void BellyHookMethod(Action<PickupObject, PlayerController> orig, PickupObject self, PlayerController player)
        {
            int knownStoredHearts = HeartDispenser.CurrentHalfHeartsStored;


            orig(self, player);
            if (player.HasPickupID(ID))
            {
                
                if (player.healthHaver.GetCurrentHealthPercentage() >= 1f)
                {
                    int difference = HeartDispenser.CurrentHalfHeartsStored - knownStoredHearts;
                    HeartDispenser.CurrentHalfHeartsStored = knownStoredHearts;

                    player.healthHaver.Armor = player.healthHaver.Armor + difference;

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