using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;
using NpcApi;


namespace Knives
{
    class loan : PassiveItem
    {

        public static void Register()
        {
            string itemName = "Devilish Loan Note";

            string resourceName = "Knives/Resources/loan";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<loan>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Already Paid Off";
            string longDesc = "A sinister looking bank note signed with your own blood. The deal was simple, money now, soul later.\n\nWithout a soul the gundead no longer see you as human and you will not gain from dropped casings.\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item


            
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.MoneyMultiplierFromEnemies, -667f, StatModifier.ModifyMethod.ADDITIVE);
           // item.CanBeDropped = false;

            item.IgnoredByRat = true;


            item.quality = PickupObject.ItemQuality.A;
            item.PersistsOnDeath = true;
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            itemID = item.PickupObjectId;
            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Gives casings upfront, but can't gain from dropped casings";



        }

        

        public static int itemID;
        public override void Pickup(PlayerController player)
        {
            CalcMaxCase(player);
            player.carriedConsumables.Currency = max_case;


            base.Pickup(player);
            
        }
        bool hit;
        private void CalcMaxCase(PlayerController player)
        {
            hit = false;
            // depth
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CASTLEGEON)//0
            {
                max_case = 700;
                hit = true;
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.SEWERGEON || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.JUNGLEGEON)//.5
            {
                max_case = 640;
                hit = true;
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.GUNGEON)//1
            {
                max_case = 580;
                hit = true;
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CATHEDRALGEON || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.BELLYGEON)//1.5
            {
                max_case = 520;
                hit = true;
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON)//2
            {
                max_case = 460;
                hit = true;
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.RATGEON)//2.5
            {
                max_case = 420;
                hit = true;
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CATACOMBGEON)//3
            {
                max_case = 400;
                hit = true;
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.OFFICEGEON || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.WESTGEON)//3.5
            {
                max_case = 350;
                hit = true;
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON)//4
            {
                max_case = 300;
                hit = true;
            }
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FINALGEON || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.HELLGEON)//past
            {
                max_case = 300;
                hit = true;
            }
            if(hit == false)// floor not found default 300;
            {
                max_case = 300;
            }



        }

        public int max_case = 600;
        public bool doinglable;
        
        public override void  Update()
        {   //currency limiter
            base.Update();
            if (this.Owner != null)
            {
                if (this.Owner.carriedConsumables.Currency > max_case)
                {
                    this.Owner.carriedConsumables.Currency = max_case;
                }

                if (this.Owner.carriedConsumables.Currency < max_case)
                {
                    max_case = this.Owner.carriedConsumables.Currency;
                }
    


               
            }
            
            
        }



  
    }
}
