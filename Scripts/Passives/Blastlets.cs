using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class blastlets : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Blastlets";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Blastlets";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<blastlets>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Seems Safe Enough";
            string longDesc =

                "A Bullet type capped with a small gunpowder barrel that miraculously does not explode unpon firing. Chance to afflict enemies with [BlastBlight]. Afflicted enemies will explode after 3 instances of damage or on death. " +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;



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
            chance = rng.Next(1, 21);
            if (chance == 1)
            {
                source.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(source.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy));
                source.HasDefaultTint = true;
                source.DefaultTintColor = new Color(.70f, .40f, .24f);
                source.CurseSparks = true;
            }
        }

        private void HandleHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2 && arg2.aiActor)
            {
                AIActor aiActor = arg2.aiActor;
                if (aiActor.IsNormalEnemy && !aiActor.IsHarmlessEnemy)
                {
                   
                    BlastBlightedStatusController boom = aiActor.gameObject.GetOrAddComponent<BlastBlightedStatusController>();
                    boom.statused = true;
                   
                }
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.PostProcessProjectile;
            return base.Drop(player);

        }
    }
}
