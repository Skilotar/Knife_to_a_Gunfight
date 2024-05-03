using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using JetBrains.Annotations;

namespace Knives
{
    class Im_blue :PassiveItem
    {
        public static void Register()
        {
            string itemName = "I'm blue";

            string resourceName = "Knives/Resources/Im_blue";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Im_blue>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "da ba de da ba DIE";
            string longDesc = "Everything you see is blue. This depressing outlook on life makes every second seem to drag on.\n\n\n - Knife_to_a_Gunfight" +
                "\n\n";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.MovementSpeed, -2f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.EnemyProjectileSpeedMultiplier, -.30f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.ProjectileSpeed, -.30f, StatModifier.ModifyMethod.ADDITIVE);




            item.quality = PickupObject.ItemQuality.C;
        }

       
    }
}
