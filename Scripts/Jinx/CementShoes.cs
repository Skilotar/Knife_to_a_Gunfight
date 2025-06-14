using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;
using HarmonyLib;

namespace Knives
{
    class CementShoes : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Cement Shoes";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Cement_shoes";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<CementShoes>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Fit Pretty Freaking Good";
            string longDesc =

                "Immunity to Goops and Knockback, Double Damage from Pits.\n\n" +
                "These were fished up from the gungeon's depths with a soggy note attached. Whoever Mugzy was he's not doing so hot." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;
            //Set the rarity of the item

            item.CanBeDropped = false;
            item.PersistsOnDeath = true;
            item.quality = PickupObject.ItemQuality.B;
            itemID = item.PickupObjectId;
            Remove_from_lootpool.RemovePickupFromLootTables(item);

            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Immunity to Goops and Knockback, Double Damage from Pits";
        }
        public static int itemID;
        public static PlayerController man;
        public override void Pickup(PlayerController player)
        {
            player.healthHaver.ModifyDamage += this.ModDamage;
            PassiveItem.IncrementFlag(player, typeof(HeavyBootsItem));
            man = player;
            base.Pickup(player);
        }

        [HarmonyPatch(typeof(GameActor), nameof(GameActor.HandleGoopChecks))]
        public class NoGoop
        {
            [HarmonyPrefix]
            public static bool Prefix()
            {
                if (man != null)
                {
                    if (man.HasPickupID(itemID)) // Skip GoopChecks;
                    {
                        return false;
                    }
                    return true;
                }
                return true;

            }
        }

        private void ModDamage(HealthHaver arg1, HealthHaver.ModifyDamageEventArgs arg2)
        {
            if (this.Owner)
            {
                if (this.Owner.IsFalling)
                {
                    arg2.ModifiedDamage *= 2;
                }
            }
        }

        
        public override DebrisObject Drop(PlayerController player)
        {
            player.healthHaver.ModifyDamage -= this.ModDamage;
            PassiveItem.DecrementFlag(player, typeof(HeavyBootsItem));
            man = null;
            return base.Drop(player);

        }
    }
}