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
    public class Ammochemy_belt : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Ammochemy Belt";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Ammochemy_belt";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Ammochemy_belt>();

            //Adds a tk2dSprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Lead From Gold";
            string longDesc = "Every other casing collected is trasmuted into ammo for all guns.\n" +
                "A unique reversal of the anchient alchemical goal of turning lead into gold. Turns out reversing the process is much easier."
                 +
                "\n\n\n -Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            item.CanBeDropped = false;
            item.quality = PickupObject.ItemQuality.C;

            //Set the rarity of the item
            ID = item.PickupObjectId;
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.MoneyMultiplierFromEnemies, .25f);
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Every other casing collected is trasmuted into ammo.";


        }

        public static int ID;
        Hook CurrencyPickupHook = new Hook(
            typeof(CurrencyPickup).GetMethod("Pickup", BindingFlags.Instance | BindingFlags.Public),
            typeof(Ammochemy_belt).GetMethod("CurrencyHookMethod")
            );

        public static bool flipflopper = true;
        public static void CurrencyHookMethod(Action<PickupObject, PlayerController> orig, CurrencyPickup self, PlayerController player)
        {
            //ETGModConsole.Log("Hookwork");
            orig(self, player);
            if (player.HasPickupID(ID))
            {
                if (player.inventory.AllGuns.Count == 1 || GunsHaveMaxAmmo(player)) return;


                int iterationstodo;

                if (self.currencyValue == 1)
                {
                    iterationstodo = 1;
                }
                else
                {
                    float iterations = self.currencyValue / 2;
                    iterationstodo = (int)iterations.RoundToNearest(1);
                }

                

                for (int i = 0; i < iterationstodo; i++)
                {
                    if (player.gameObject.GetOrAddComponent<PlayerSpecialStates>().AmmochemyFlipper % 2 == 0)
                    {

                        foreach (Gun gun in player.inventory.AllGuns)
                        {
                            gun.GainAmmo(4);
                        }
                        player.carriedConsumables.Currency--;
                        player.gameObject.GetOrAddComponent<PlayerSpecialStates>().AmmochemyFlipper++;
                    }
                    else
                    {
                        player.gameObject.GetOrAddComponent<PlayerSpecialStates>().AmmochemyFlipper++;
                    }
                }

                

            }
        }

        private static bool GunsHaveMaxAmmo(PlayerController player)
        {
            bool maxed = true;
            foreach (Gun gun in player.inventory.AllGuns)
            {
                if (gun.CurrentAmmo < gun.GetBaseMaxAmmo())
                {
                    maxed = false;
                    //ETGModConsole.Log("fail");
                    return maxed;
                }

            }
            //ETGModConsole.Log("maxed");
            return maxed;
        }

        public override void Pickup(PlayerController player)
        {
            player.gameObject.GetOrAddComponent<PlayerSpecialStates>();
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