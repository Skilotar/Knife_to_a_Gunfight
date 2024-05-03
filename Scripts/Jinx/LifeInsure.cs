using System;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using Dungeonator;


namespace Knives
{
    public class LifeInsure : PassiveItem
    {
        public static void Register()
        {
            string itemName = "Life Insurance";

            string resourceName = "Knives/Resources/lifeInsureance";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<LifeInsure>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Do You Need It?";
            string longDesc = "Protects from death once. Costs per floor. " +
                "\n\n\n -Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            item.quality = PickupObject.ItemQuality.A;
            item.PersistsOnDeath = true;
            item.CanBeDropped = false;
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            itemID = item.PickupObjectId;
            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Saves you from death once, But charges you every floor";
        }

        public static int itemID;
        public override void Pickup(PlayerController player1)
        {
            player = player1;
            player1.healthHaver.OnPreDeath += HealthHaver_OnPreDeath;
            player1.OnNewFloorLoaded += this.OnLoadedFloor;
            base.Pickup(player);


        }

        private void HealthHaver_OnPreDeath(Vector2 obj)
        {
            SaveLife(Vector2.zero);
        }

        public static bool CanBlockDeath = true;
        bool half_toggle = false;
        public void OnLoadedFloor(PlayerController player)
        {

            if (half_toggle)
            {
                if (CanBlockDeath == true)
                {
                    player.carriedConsumables.Currency = player.carriedConsumables.Currency - 30;
                }
                half_toggle = false;
            }
            else
            {
                half_toggle = true;
            }


        }

        public override DebrisObject Drop(PlayerController player)
        {

            player.OnNewFloorLoaded -= this.OnLoadedFloor;
            return base.Drop(player);
        }

        private void SaveLife(Vector2 finalDamageDirection)
        {
            if (CanBlockDeath == true)
            {
                CanBlockDeath = false;
                if (player.ForceZeroHealthState == true)
                { player.healthHaver.Armor = 2; }
                else { player.healthHaver.ApplyHealing(2); }

                GameManager.Instance.MainCameraController.SetManualControl(false, false);
                player.ToggleGunRenderers(true, "non-death");
                player.ToggleHandRenderers(true, "non-death");
                player.CurrentInputState = PlayerInputState.AllInput;

                player.StartCoroutine(HandleShield(player));
                this.Owner.BloopItemAboveHead(base.sprite, "Knives/Resources/lifeInsureance");
            }
        }

        private IEnumerator HandleShield(PlayerController user)
        {
            
            
            user.healthHaver.IsVulnerable = false;
            float elapsed = 0f;
            while (elapsed < 5)
            {
                
                user.healthHaver.IsVulnerable = false;
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            
            user.healthHaver.IsVulnerable = true;
            yield break;
        }



        
        public PlayerController player;
       
    }

}

