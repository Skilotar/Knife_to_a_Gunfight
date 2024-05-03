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
    class Fly_Friend : PassiveItem
    {
        public static void Register()
        {
            string itemName = "Fly Friend";

            string resourceName = "Knives/Resources/Fly_Friend";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Fly_Friend>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Hope";
            string longDesc = "Dispite her small size this little fly can provide great hope to her friends. \n\n" +
                    "____________________________________________________________\n\n" +
                    "Minor increase to many stats" +
                    "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Coolness, 1f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.GlobalPriceMultiplier, -.05f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.RateOfFire, .2f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.ReloadSpeed, -.2f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.PlayerBulletScale, -.2f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.KnockbackMultiplier, -.1f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.MovementSpeed, .5f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, .05f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Accuracy, -.02f, StatModifier.ModifyMethod.ADDITIVE);
            


            item.quality = PickupObject.ItemQuality.B;

            ID = item.PickupObjectId;
        }

        public static int ID;
    }
}
