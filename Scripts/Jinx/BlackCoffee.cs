using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Collections.Generic;
using HarmonyLib;

namespace Knives
{
    class Black_Coffee : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Black Coffee";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/BlackCoffee";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Black_Coffee>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Morning Magic";
            string longDesc ="Triples damage to jammed enemies, Increased jammed enemy speed. \n\n" +

                "A pungent cup of coffee that is both crimson and pitch black. The Jammed like their coffee, just like themselves... Dark and Bitter." +
                "\n\n\n - Knife_to_a_Gunfight" +
                "\n Sprite in collab with Dallan";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1);
            

            //item.CanBeDropped = false;
            item.PersistsOnDeath = false;
            item.quality = PickupObject.ItemQuality.C;
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            itemID = item.PickupObjectId;
            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Triples damage to jammed enemies, Increased jammed enemy speed.";

        }

        public static int itemID;
        public static PlayerController owner;

        public override void Pickup(PlayerController player)
        {
            Black_Coffee.owner = player;
            base.Pickup(player);
        }


        [HarmonyPatch(typeof(AIActor), "BecomeBlackPhantom")]
        public class FASTPHANTOM
        {
            [HarmonyPrefix]
            public static bool PostFix(AIActor __instance)
            {
                if(owner != null)
                {
                    if (owner.HasPassiveItem(itemID))
                    {
                        __instance.LocalTimeScale = 1.5f;
                        __instance.healthHaver.AllDamageMultiplier += 2;
                    }
                }

                return true;
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            Black_Coffee.owner = null;
            
            return base.Drop(player);
        }
       
        public override void Update()
        {

            if (this.Owner != null)
            {
                
            }
            base.Update();
        }
        


    }
}
