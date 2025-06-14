using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using System.Reflection;
using Dungeonator;
using HarmonyLib;

namespace Knives
{
    public class CompoundLense : PassiveItem
    {

        public static void Register()
        {

            string itemName = "Compound Lens";


            string resourceName = "Knives/Resources/CompoundLense";


            GameObject obj = new GameObject(itemName);


            var item = obj.AddComponent<CompoundLense>();


            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Unflinching Aim";
            string longDesc = "Accuracy Increase\nIgnore any accuracy stat downs.\n\n" +
                "A binocular set of scopes whos lenses are filled with hundreds of microscopic scopes.\n\n" +
                "Crafted from the eyes of the elusive Dragunfly." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Accuracy, .8f,StatModifier.ModifyMethod.MULTIPLICATIVE);
            item.quality = PickupObject.ItemQuality.B;


            ID = item.PickupObjectId;
        }

        public static int ID;

        public override void Pickup(PlayerController player)
        {
            CompoundLense.owner = player;
            base.Pickup(player);
        }

        
        public override void Update()
        {
            if (this.Owner)
            {
                
            }

            base.Update();
        }
        public static PlayerController owner = null;
       

        [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.ApplyStatModifier))]
        public class CapMinAccuracy 
        {
            [HarmonyPrefix]
            public static bool Prefix(StatModifier modifier)
            {
                if(CompoundLense.owner != null)
                {
                    if (CompoundLense.owner.HasPassiveItem(CompoundLense.ID))
                    {
                        if (modifier.statToBoost == PlayerStats.StatType.Accuracy)
                        {
                            if (modifier.amount > 1) // Spread go up   >:(
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return true;
                    }

                }
                else
                {
                    return true;
                }

            }
            
        }

       


        public override DebrisObject Drop(PlayerController player)
        {
            CompoundLense.owner = null;
            return base.Drop(player);
        }

     
    }
}
