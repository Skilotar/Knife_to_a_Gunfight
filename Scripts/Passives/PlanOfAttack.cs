using System;
using System.Collections;

using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using Dungeonator;
using System.Collections.Generic;

namespace Knives
{
    public class PlanOfAttack : PassiveItem
    {
        public static void Register()
        {
            string itemName = "Plan of Attack";

            string resourceName = "Knives/Resources/PlanOfAttack";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<PlanOfAttack>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Decisive Action";
            string longDesc = "Plots a course to the boss chamber, provides a speed buff if the plan is followed.\n\n" +
                "Thourough planning can turn a speculative venture into a guaranteed result. We've planned this out down to the dust floating in the air afterwards.\n\n" +
                "Stick to the plan and this will be an easy job." +
                "" +
                "\n\n\n -Knife_to_a_Gunfight" +
                "\n Sprite in collab with Dallan";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item


            item.quality = PickupObject.ItemQuality.B;
            ID = item.PickupObjectId;

        }
        public static int ID;
        public bool pickedUpThisRun = false;
        public override void Pickup(PlayerController user)
        {
            needsNewMap = true;
            GameManager.Instance.OnNewLevelFullyLoaded += Instance_OnNewLevelFullyLoaded;
            user.OnEnteredCombat += this.CheckRoom;
            base.Pickup(user);
            if (pickedUpThisRun == false)
            {
                GeneratePathList(user);
                pickedUpThisRun = true;
            }

        }


        

        public bool IsOnPath = true;
        
        private void CheckRoom()
        {

            if(IsOnPath == true && Pathlist.Count > 0)
            {
                bool notbroke = false;
                for(int i = Pathlist.Count -1; i >= 0; i--)
                {
                    RoomHandler room = Pathlist[i];
                    if (room == this.Owner.CurrentRoom)
                    {
                        notbroke = true;
                        
                        i = -1;
                    }
                }
                if(notbroke == false) AkSoundEngine.PostEvent("Play_Stat_Down", base.gameObject);
                IsOnPath = notbroke;
            }
 
        }
        public bool hasBuff = false;
        public bool hasSyn = false;
        public override void Update()
        {
            if (this.Owner != null && !GameManager.Instance.IsLoadingLevel)
            {

                if (IsOnPath == true && hasBuff == false)
                {
                    if (this.Owner.PlayerHasActiveSynergy("Backup Plan") ) // remove when back on path
                    {
                        RemoveStat(PlayerStats.StatType.Damage);
                        AddStat(PlayerStats.StatType.Damage, 0f);
                        this.Owner.stats.RecalculateStats(Owner, true);
                    }

                    RemoveStat(PlayerStats.StatType.MovementSpeed);
                    AddStat(PlayerStats.StatType.MovementSpeed, 1f);
                    this.Owner.stats.RecalculateStats(Owner, true);
                    hasBuff = true;
                }
                if (IsOnPath == false && hasBuff)
                {
                   
                    RemoveStat(PlayerStats.StatType.MovementSpeed);
                    AddStat(PlayerStats.StatType.MovementSpeed, 0f);

                    this.Owner.stats.RecalculateStats(Owner, true);
                    hasBuff = false;
                }

                if (this.Owner.PlayerHasActiveSynergy("Backup Plan"))
                {
                    if(hasSyn == false && IsOnPath == false)
                    {
                        RemoveStat(PlayerStats.StatType.Damage);
                        AddStat(PlayerStats.StatType.Damage, .25f);
                        this.Owner.stats.RecalculateStats(Owner, true);
                        hasSyn = true;
                    }
                }
                else
                {
                    if (hasSyn == true && IsOnPath == false)
                    {
                        RemoveStat(PlayerStats.StatType.Damage);
                        AddStat(PlayerStats.StatType.Damage, 0f);
                        this.Owner.stats.RecalculateStats(Owner, true);
                        hasSyn = false;
                    }
                }

                if (needsNewMap == true && GameManager.Instance.Dungeon.data.rooms.Count > 0)
                {
                    GeneratePathList(this.Owner);
                }
                
            }
            base.Update();
        }

        public void GeneratePathList(PlayerController user)
        {
            if(user.CurrentRoom != null && !GameManager.Instance.IsLoadingLevel)
            {
                RoomHandler roomHandler = null;
                for (int i = 0; i < GameManager.Instance.Dungeon.data.rooms.Count; i++)
                {
                    RoomHandler roomHandler2 = GameManager.Instance.Dungeon.data.rooms[i];
                    if (roomHandler2.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.EXIT)
                    {
                        roomHandler = roomHandler2;
                        break;
                    }
                }
                if (roomHandler == null)
                {
                    List<AIActor> allEnemies = StaticReferenceManager.AllEnemies;
                    for (int j = 0; j < allEnemies.Count; j++)
                    {
                        if (allEnemies[j])
                        {
                            if (allEnemies[j].GetComponent<LichDeathController>())
                            {
                                roomHandler = allEnemies[j].ParentRoom;
                                break;
                            }
                        }
                    }
                }
                if (roomHandler == null)
                {
                    Debug.LogError("Using SenseOfDirection in Dungeon with no EXIT?");
                    return;
                }
                RoomHandler currentRoom = user.CurrentRoom;
                if (roomHandler == currentRoom)
                {
                    return;
                }
                Pathlist = SenseOfDirectionItem.FindPathBetweenNodes(currentRoom, roomHandler, GameManager.Instance.Dungeon.data.rooms);
                IsOnPath = true;
                needsNewMap = false;
                foreach (RoomHandler room in Pathlist)
                {
                    Minimap.Instance.RevealMinimapRoom(room, true, true, room == GameManager.Instance.PrimaryPlayer.CurrentRoom);
                }
            }
            else
            {
                needsNewMap = true;
            }
            
        }

        List<RoomHandler> Pathlist;
        bool half_toggle = false;
        bool needsNewMap = false;

        private void Instance_OnNewLevelFullyLoaded()
        {
            if(this.Owner != null)
            {
                needsNewMap = true;
            }
            
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
        public override DebrisObject Drop(PlayerController player)
        {
            player.OnEnteredCombat -= this.CheckRoom;
            GameManager.Instance.OnNewLevelFullyLoaded -= Instance_OnNewLevelFullyLoaded;
            
            RemoveStat(PlayerStats.StatType.Damage);
            AddStat(PlayerStats.StatType.Damage, 0f);
            RemoveStat(PlayerStats.StatType.MovementSpeed);
            AddStat(PlayerStats.StatType.MovementSpeed, 0f);

            this.Owner.stats.RecalculateStats(Owner, true);
            return base.Drop(player);
        }

    }
}


