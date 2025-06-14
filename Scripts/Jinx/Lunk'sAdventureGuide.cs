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
    class LunksAdventureGuide : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Lunk's Adventure Guide";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Lunk'sAdventureGuide";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<LunksAdventureGuide>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Always Exploring";
            string longDesc = "Increases damage for completed rooms, Chance to forget rooms you've completed. \n\n" +

                "Not all who wander are lost... but I sure am! \n- Lunk" +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            item.CanBeDropped = false;
            item.PersistsOnDeath = false;
            item.quality = PickupObject.ItemQuality.C;
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            itemID = item.PickupObjectId;
            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Increases damage for completed rooms, Chance to forget rooms you've completed.";

        }

        public static int itemID;
        public static PlayerController owner;

        public override void Pickup(PlayerController player)
        {
            player.OnEnteredCombat += ForgetfulExplorer;
           
            base.Pickup(player);
        }
        public override DebrisObject Drop(PlayerController player)
        {
            player.OnEnteredCombat -= ForgetfulExplorer;
            RemoveStat(PlayerStats.StatType.Damage);
            this.Owner.stats.RecalculateStats(player, true);
            return base.Drop(player);
        }


        private void ForgetfulExplorer()
        {
            if(this.Owner != null)
            {
                if ((UnityEngine.Random.Range(0, 8f - activeRooms.Count) <= 1) || activeRooms.Count > 8)
                {
                    for (int i = (int)UnityEngine.Random.Range(0, 3f); i > 0; i--)
                    {
                        RoomHandler room = activeRooms[UnityEngine.Random.Range(0, activeRooms.Count -1)];
                        if (room != null)
                        {
                            room.ResetPredefinedRoomLikeDarkSouls();
                            room.RevealedOnMap = false;
                            room.visibility = RoomHandler.VisibilityStatus.OBSCURED;
                            Minimap.Instance.DeflagPreviousRoom(room);
                        }
                    }
                   
                }

                UpdateVisibleRoomsList();
                RemoveStat(PlayerStats.StatType.Damage);
                AddStat(PlayerStats.StatType.Damage, .05f * activeRooms.Count);
                this.Owner.stats.RecalculateStats(Owner, true);

            }
        }
        public void UpdateVisibleRoomsList()
        {
            
            activeRooms.Clear();  //Clear and get new list of currently active rooms.
            DungeonData dungeonData = GameManager.Instance.Dungeon.data;
            for (int i = dungeonData.rooms.Count - 1; i >= 0; i--)
            {
                if (dungeonData.rooms[i].visibility == RoomHandler.VisibilityStatus.VISITED && dungeonData.rooms[i].visibility != RoomHandler.VisibilityStatus.OBSCURED)
                {
                    activeRooms.Add(dungeonData.rooms[i]);
                }
            }
            //ETGModConsole.Log(activeRooms.Count);
        }

        public static List<RoomHandler> activeRooms = new List<RoomHandler>();

       
        public override void Update()
        {

            if (this.Owner != null)
            {
                
            }
            base.Update();
        }
        private void AddStat(PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
        {
            StatModifier modifier = new StatModifier();
            modifier.amount = amount;
            modifier.statToBoost = statType;
            modifier.modifyType = method;

            foreach (var m in passiveStatModifiers)
            {
                if (m.statToBoost == statType) return; //don't add duplicates
            }

            if (this.passiveStatModifiers == null)
                this.passiveStatModifiers = new StatModifier[] { modifier };
            else
                this.passiveStatModifiers = this.passiveStatModifiers.Concat(new StatModifier[] { modifier }).ToArray();
        }


        //Removes a stat
        private void RemoveStat(PlayerStats.StatType statType)
        {
            var newModifiers = new List<StatModifier>();
            for (int i = 0; i < passiveStatModifiers.Length; i++)
            {
                if (passiveStatModifiers[i].statToBoost != statType)
                    newModifiers.Add(passiveStatModifiers[i]);
            }
            this.passiveStatModifiers = newModifiers.ToArray();
        }


    }
}
