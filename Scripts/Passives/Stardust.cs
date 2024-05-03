using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class stardust :PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Jar of Stardust";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Jar_of_stardust";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<stardust>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "The Universe Gasps";
            string longDesc = "This jar contains a sample of the dust from the first star's death. The universe itself is highly sentimental about it and will freeze in fear when its container is in danger.\n\n\n - Knife_to_a_Gunfight"
                ;

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            //Set the rarity of the item

            item.quality = PickupObject.ItemQuality.B;
            ID = item.PickupObjectId;
        }
        public static int ID;
        public override void Pickup(PlayerController player)
        {
            player.healthHaver.OnDamaged += HealthHaver_OnDamaged;
            base.Pickup(player);
        }

        private void HealthHaver_OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            Za_worldo();
           
        }

        public void Za_worldo()
        {
            RadialSlowInterface the_big_slow = new RadialSlowInterface();
            the_big_slow.RadialSlowHoldTime = 1.75f;
            the_big_slow.RadialSlowOutTime = .25f;
            the_big_slow.RadialSlowTimeModifier = 0f;
            the_big_slow.DoesSepia = true;
            the_big_slow.UpdatesForNewEnemies = true;
            the_big_slow.RadialSlowInTime = 0f;
            the_big_slow.DoRadialSlow(this.Owner.CenterPosition, this.Owner.CurrentRoom);

        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.healthHaver.OnDamaged -= HealthHaver_OnDamaged;
            return base.Drop(player);
        }

    }
}
