using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;
using ItemAPI;
using MonoMod;
using Gungeon;

namespace Knives
{
    class ActiveCharger : PlayerItem
    {
        public static void Register()
        {
            string itemName = "Devtool_InstaCharger";

            string resourceName = "Knives/Resources/ActiveCharger";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<ActiveCharger>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Its electric!";
            string longDesc ="Instantly refills all active item charges for testing purposes only. ";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item


            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 20f, StatModifier.ModifyMethod.ADDITIVE);



            item.consumable = false;
            item.UsesNumberOfUsesBeforeCooldown = true;
            item.numberOfUses = 99;
            item.quality = PickupObject.ItemQuality.EXCLUDED;
        }
        public override void DoEffect(PlayerController user)
        {
            FieldInfo remainingDamageCooldown = typeof(PlayerItem).GetField("remainingDamageCooldown", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo remainingTimeCooldown = typeof(PlayerItem).GetField("remainingTimeCooldown", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo remainingRoomCooldown = typeof(PlayerItem).GetField("remainingRoomCooldown", BindingFlags.NonPublic | BindingFlags.Instance);
           
            foreach (PlayerItem item in user.activeItems)
            {
                remainingTimeCooldown.SetValue(item, 0);
                remainingDamageCooldown.SetValue(item, 0);
                remainingRoomCooldown.SetValue(item, 0);
            }
        }
      
    }
}

