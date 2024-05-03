using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;

namespace Knives
{
    class Looplets : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Looplets";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Looplets";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Looplets>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Ah Haha LookOUT";
            string longDesc =

                "Bullets loop when hitting a wall." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.RangeMultiplier, 1.5f);

            //Set the rarity of the item


            item.quality = PickupObject.ItemQuality.C;
        }
        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += this.PostProcessProjectile;
            base.Pickup(player);
        }
        private System.Random rng = new System.Random();
        
        private void PostProcessProjectile(Projectile source, float chance)
        {
            source.gameObject.GetOrAddComponent<LoopyDoopy>();
        }

       
        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.PostProcessProjectile;

            return base.Drop(player);

        }
    }
}