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
using HarmonyLib;

namespace Knives
{
    public class PlasmaBuckler : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Plasma Buckler";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/PlasmaBuckler";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<PlasmaBuckler>();

            //Adds a tk2dSprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Tactical Reload";
            string longDesc = "Reloads block nearby bullets, Armor no longer blanks and Blanks do not regnerate." +
                "A much more active approach to defensive. This ultra-light buckler draws power away from your shields to expel incoming fire." +
                "\n\n\n -Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            item.CanBeDropped = true;
            item.quality = PickupObject.ItemQuality.B;
            //Set the rarity of the item
            ID = item.PickupObjectId;
            
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalBlanksPerFloor, -20);
            text.jinxItemDisplayText = "Reloads block nearby bullets, Armor and Blanks are Weakend";


        }
        public static int ID;
        public static PlayerController man;
        

        [HarmonyPatch(typeof(PlayerController), "OnLostArmor")]
        public class ArmorLoss
        {
            [HarmonyPrefix]
            public static bool Prefix(PlayerController __instance)
            {

                if (__instance.HasPickupID(ID))
                {

                    if (__instance.lostAllArmorVFX != null && __instance.healthHaver.Armor == 0f)
                    {
                        GameObject gameObject = SpawnManager.SpawnDebris(__instance.lostAllArmorVFX, __instance.specRigidbody.UnitTopCenter, Quaternion.identity);
                        gameObject.GetComponent<DebrisObject>().Trigger(Vector3.zero, 0.5f, 1f);
                    }
                    if (__instance.LostArmor != null)
                    {
                        __instance.LostArmor();
                    }
                    return false;
                }
                else
                {
                   return true;
                }

            }
        }

        [HarmonyPatch(typeof(SilencerInstance), nameof(SilencerInstance.TriggerSilencer))]
        public class WeakBlanks
        {
            [HarmonyPrefix]
            public static void Prefix(ref float maxRadius, ref float distRadius, PlayerController user)
            {
                if(user != null)
                {
                    if (user.HasPickupID(ID))
                    {
                        if (user == man)
                        {
                            maxRadius *= .20f;

                            distRadius *= .20f;
                        }
                    }
                }
               
            }
        }


        public override void Pickup(PlayerController player)
        {
            man = player;
            /*
            Hook WeakArmor = new Hook(
            typeof(PlayerController).GetMethod("OnLostArmor", BindingFlags.Instance | BindingFlags.Public),
            typeof(PlasmaBuckler).GetMethod("WeakArmor")
            );
            */
            player.OnReloadedGun += this.onreload;
            
            base.Pickup(player);
        }

        

        private void onreload(PlayerController player, Gun gun)
        {
            
           
            float remain = player.CurrentGun.ClipShotsRemaining;
            float max = player.CurrentGun.ClipCapacity;
            float multiplier = remain / max;
            multiplier = 1.0f - multiplier;
            float radius = 4f * multiplier;
            radius += 3f;
            StartCoroutine(this.DestroyEnemyBulletsInCircleForDuration(player.specRigidbody.UnitCenter, radius, .15f));
            
            Exploder.DoDistortionWave(player.CenterPosition, .5f, 0.04f, radius, .4f);
            AkSoundEngine.PostEvent("Play_ITM_Macho_Brace_Trigger_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_ITM_Macho_Brace_Trigger_01", base.gameObject);
            
           
        }
        public IEnumerator DestroyEnemyBulletsInCircleForDuration(Vector2 center, float radius, float duration)
        {
            float ela = 0f;
            while (ela < duration)
            {
                ela += BraveTime.DeltaTime;
                SilencerInstance.DestroyBulletsInRange(center, radius, true, false, null, false, null, false, null);
                yield return null;
            }
            yield break;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            this.Owner.OnReloadedGun -= this.onreload;
            
            return base.Drop(player);
        }
        public override void OnDestroy()
        {
            man.OnReloadedGun -= this.onreload;
            
            base.OnDestroy();
        }

       
       
    }
}
