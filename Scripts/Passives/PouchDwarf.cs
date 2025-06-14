using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using HarmonyLib;
using System.Reflection;
using System.Collections;

namespace Knives
{
    public class Pouch_Dwarf : PassiveItem
    {

        public static void Register()
        {

            string itemName = "Pouch Dwarf";


            string resourceName = "Knives/Resources/Pouch_Dwarf";


            GameObject obj = new GameObject(itemName);


            var item = obj.AddComponent<Pouch_Dwarf>();


            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "I WAS BORN READY!";
            string longDesc = "Doubles the speed that guns are reloaded when stowed.\n\n" +
                "A very small man who hides in your gun pouch. He will reload all your guns with great speed!" +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            item.quality = PickupObject.ItemQuality.C;

            ID = item.PickupObjectId;
        }
        
        public static int ID;
        public static PlayerController user;
        public static System.Random rng = new System.Random();

        public override void Pickup(PlayerController player)
        {
            user = player;
            base.Pickup(player);
        }

        [HarmonyPatch(typeof(GunInventory), nameof(GunInventory.FrameUpdate))]
        public class FasterStowedReload
        {
            [HarmonyPrefix]
            public static void Prefix()
            {
                if (user != null)
                {
                    if (user.HasPickupID(ID))
                    {
                        // Copies the time add section of the stowed gun code to double the speed at which guns are stowed reloaded.
                        var guntimeDict = GetPrivateType<GunInventory, Dictionary<Gun, float>>(user.inventory, "m_gunStowedTime");
                        for (int i = 0; i < user.inventory.AllGuns.Count; i++)
                        {
                            if (user.inventory.AllGuns[i]== user.CurrentGun)
                            {
                                guntimeDict[user.inventory.AllGuns[i]] = 0f;
                                continue;
                            }
                            guntimeDict[user.inventory.AllGuns[i]] += BraveTime.DeltaTime;
                           
                        }
                        SetPrivateType<GunInventory>(user.inventory, "m_gunStowedTime", guntimeDict);
                        
                    }
                }
            }

        }
        private static void SetPrivateType<T>(T obj, string field, Dictionary<Gun, float> value)  // Privated variables are such a pain in my -
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            f.SetValue(obj, value);
        }
        private static T2 GetPrivateType<T, T2>(T obj, string field)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T2)f.GetValue(obj);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            user = null;
            return base.Drop(player);
        }


    }
}
