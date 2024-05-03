using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;


namespace Knives
{
    class Sus_rounds :PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Superstitious bullets";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/superstitious_bullets";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Sus_rounds>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Tetraphobia";
            string longDesc = "These bullets were forged by a gunslinger that believed that it was bad luck to shoot directally at your target. Use them to fire around pillars and strike from unpredictable locations.\n\n" +
            "These bullets are also deeply afraid of the number four.\n\n Adds targeting wall bounces but every fourth bullet is deleted." +
            "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.RangeMultiplier, 2f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, .12f, StatModifier.ModifyMethod.ADDITIVE);
            //Set the rarity of the item
            item.quality = PickupObject.ItemQuality.B;

        }
        int fours = 3;
        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += Player_PostProcessProjectile;
            
            base.Pickup(player);
        }

        private void Player_PostProcessProjectile(Projectile arg1, float arg2)
        {
            if(fours == 0)
            {
                arg1.DieInAir(true, false, false, true);
                fours = 3;
            }
            else
            {
                BounceProjModifier bnc = arg1.gameObject.GetOrAddComponent<BounceProjModifier>();
                bnc.bouncesTrackEnemies = true;
                bnc.bounceTrackRadius = 15;
                bnc.numberOfBounces = 2;
                fours--;
                
            }
           
           
        }


        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= Player_PostProcessProjectile;
            return base.Drop(player);
        }
    }
}
