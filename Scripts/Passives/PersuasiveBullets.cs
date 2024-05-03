using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class persuasive_bullets : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Persuasive bullets";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Persuasive_bullets";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<persuasive_bullets>();
            
            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Just Following Orders";
            string longDesc =

                "These bullets are adorned with a beautiful captain's hat. The enemies of the gungeon are all well aquainted with military heirarchy and are happy to do what they are told. " +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;



            //Set the rarity of the item

           
            item.quality = PickupObject.ItemQuality.A;
        }
        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += this.PostProcessProjectile;
            base.Pickup(player);
        }
        private System.Random rng = new System.Random();
        private void PostProcessProjectile(Projectile source, float chance)
        {
            chance = rng.Next(1, 10);
            if(chance == 1)
            {    
                    source.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(source.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy));

            }
        }

        private void HandleHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2 && arg2.aiActor)
            {
                AIActor aiActor = arg2.aiActor;
                if (aiActor.IsNormalEnemy && !aiActor.healthHaver.IsBoss && !aiActor.IsHarmlessEnemy && !aiActor.gameObject.GetComponent<MindControlEffect>())
                {
                    MindControlEffect orAddComponent = aiActor.gameObject.GetOrAddComponent<MindControlEffect>();
                    orAddComponent.owner = (arg1.Owner as PlayerController);
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

