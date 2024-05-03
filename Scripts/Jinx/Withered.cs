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
    public class WitheringRose : PassiveItem
    {
        public static void Register()
        {
            string itemName = "Withering Rose";

            string resourceName = "Knives/Resources/withering_rose";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<WitheringRose>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "By Any Other Name";
            string longDesc = "Lowers heath between floors, increases damage and adds thorns. This rose infests itself into the holder drain there life and giving the host its properties." +
                "\n\n\n -Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, .18f);

            item.quality = PickupObject.ItemQuality.C;
            item.PersistsOnDeath = true;
            item.CanBeDropped = false;
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            itemID = item.PickupObjectId;

            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Increases damage and gives you thorns, but lose health each floor.";
        }
        public static int itemID;
        public override void Pickup(PlayerController player)
        {
            player.healthHaver.OnDamaged += HealthHaver_OnDamaged;
            player.OnNewFloorLoaded += this.OnLoadedFloor;
            base.Pickup(player);

            owner = player;
        }

        public PlayerController owner;

        private void HealthHaver_OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            RoomHandler room = this.Owner.CurrentRoom;
            if (room.HasActiveEnemies(RoomHandler.ActiveEnemyType.All))
            {
                foreach (var enemy in room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                {
                    enemy.healthHaver.ApplyDamage(23, Vector2.zero, "rose", CoreDamageTypes.Magic, DamageCategory.Unstoppable, true, null, true);
                }
            }
            
        }


        bool half_toggle = false;
        public void OnLoadedFloor(PlayerController player)
        {

            if (half_toggle)
            {
                if(player.healthHaver.GetCurrentHealth() > .5f)
                {
                    if (player.healthHaver.Armor != 0)
                    {
                        GameManager.Instance.StartCoroutine(armorlost());

                    }
                    else
                    {
                        player.healthHaver.ForceSetCurrentHealth(player.healthHaver.GetCurrentHealth() - .5f);
                    }
                   
                }
                if(player.characterIdentity == PlayableCharacters.Robot)
                {
                    if(player.healthHaver.Armor > 1)
                    {
                        GameManager.Instance.StartCoroutine(armorlost());
                    }
                }
                half_toggle = false;
            }
            else
            {
                half_toggle = true;
            }


        }

        private IEnumerator armorlost()
        {
            yield return new WaitForSeconds(.01f);
            float armor = this.Owner.healthHaver.Armor;
            this.Owner.healthHaver.Armor *= 0;
            int adjarmor = (int)armor - 1;
            this.Owner.healthHaver.Armor = adjarmor;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.healthHaver.OnDamaged -= HealthHaver_OnDamaged;
            player.OnNewFloorLoaded -= this.OnLoadedFloor;
            return base.Drop(player);
        }

        public override void OnDestroy()
        {
            owner.healthHaver.OnDamaged -= HealthHaver_OnDamaged;
            owner.OnNewFloorLoaded -= this.OnLoadedFloor;
            base.OnDestroy();
        }

    }
}