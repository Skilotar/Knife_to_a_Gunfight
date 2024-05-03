using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class clean_soul :PassiveItem
    {
        public static void Register()
        {
            string itemName = "Mournful Soul";

            string resourceName = "Knives/Resources/self_cleansing_soul";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<clean_soul>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Kaliber Forgive Me.";
            string longDesc = "Should we sin so that grace may abound? No. But if we must maybe Kaliber can forgive.\n\n" +
                "Costs 20 casings per floor and sets gained curse to 0." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            
            
            item.CanBeDropped = false;


            item.quality = PickupObject.ItemQuality.B;
            itemID = item.PickupObjectId;
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Cleanses curse, Charges casings per floor";

        }
        public static int itemID;
        public override void Pickup(PlayerController player)
        {
            player.OnNewFloorLoaded += this.OnLoadedFloor;
            base.Pickup(player);
        }

        bool half_toggle = false;
        public void OnLoadedFloor(PlayerController player)
        {

            if (half_toggle)
            {
                player.carriedConsumables.Currency = player.carriedConsumables.Currency - 15;
                half_toggle = false;
            }
            else
            {
                half_toggle = true;
            }


        }

        public override DebrisObject Drop(PlayerController player)
        {
            DecrementFlag(player, typeof(LiveAmmoItem));
            player.OnNewFloorLoaded -= this.OnLoadedFloor;
            return base.Drop(player);
        }

        public override void  Update()
        {
            base.Update();
            if (this.Owner != null)
            {
                AutoCleanse();
            }
            
        }
        public float token;
        public float lastknowncurse = 0;
        
        System.Random rng = new System.Random();
        public void AutoCleanse()
        {
            int mrclean = rng.Next(1, 300);
            if ( mrclean == 1)
            {
                if(lastknowncurse > 0)
                {
                    ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.Curse, -1f, StatModifier.ModifyMethod.ADDITIVE);
                    this.Owner.stats.RecalculateStats(Owner, true);
                    AkSoundEngine.PostEvent("Play_NPC_magic_blessing_01", base.gameObject);
                }
                
                if ( lastknowncurse < 0)
                {
                    token = 0;
                    ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.Curse, -lastknowncurse, StatModifier.ModifyMethod.ADDITIVE);
                    this.Owner.stats.RecalculateStats(Owner, true);
                }
                lastknowncurse = PlayerStats.GetTotalCurse();       
             }

        }
    }
}
