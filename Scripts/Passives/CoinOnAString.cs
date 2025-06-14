using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;

namespace Knives
{
    class CoinOnAString : PassiveItem
    {

        public static void Register()
        {

            string itemName = "Casing On A string";
            string resourceName = "Knives/Resources/CasingOnAString";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<CoinOnAString>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            //Ammonomicon entry variables
            string shortDesc = "Yoink!";
            string longDesc = "Chance to refund 50% of any purchase.\n\n" +
                "An ancient theivery technique handed down by crooks and scoundrels alike." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1);
            item.quality = PickupObject.ItemQuality.C;
        }

        private void OnItemPurchased(PlayerController arg1, ShopItemController arg2) // Post purchase
        {
            if (UnityEngine.Random.Range(0, 4f) <= 1) // 1/4
            {
                arg1.carriedConsumables.Currency += (arg2.ModifiedPrice / 2);
                arg1.BloopItemAboveHead(base.sprite, "Knives/Resources/CasingOnAString");
                AkSoundEngine.PostEvent("Play_ENM_wizardred_swing_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_OBJ_coin_large_01", base.gameObject);
            }
        }

        public override void Pickup(PlayerController player)
        {
            player.OnItemPurchased += this.OnItemPurchased;
            
            base.Pickup(player);
        }

       
        public override DebrisObject Drop(PlayerController player)
        {
            player.OnItemPurchased -= this.OnItemPurchased;

            return base.Drop(player);
        }

        
        public override void Update()
        {
            if (this.Owner)
            {
                
            }
            base.Update();
        }
    }
}

