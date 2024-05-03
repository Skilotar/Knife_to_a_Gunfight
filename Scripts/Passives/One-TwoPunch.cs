using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class OneTwoPunch : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "One Two Punch";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/1-2-Punch";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<OneTwoPunch>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "The Ol One Two";
            string longDesc = "Consecutive hits to an enemy increase the damage they take to a max of +50%. This effect goes away if the combo stops.\n\n"+
                "Gloves with non euclidean padding. Rapid strikes will firm your attacks instead of wearing them out." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, -.1f);
            //Set the rarity of the item


            item.quality = PickupObject.ItemQuality.B;
        }
        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += this.PostProcessProjectile;
            
            base.Pickup(player);
        }
        private System.Random rng = new System.Random();
        private void PostProcessProjectile(Projectile source, float chance)
        {
            
            source.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(source.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy));
             
        }

        private void HandleHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2 && arg2.aiActor)
            {
                AIActor aiActor = arg2.aiActor;
                if (aiActor.IsNormalEnemy && !aiActor.IsHarmlessEnemy)
                {
                    
                    OneTwoComboComponent Punch = aiActor.gameObject.GetOrAddComponent<OneTwoComboComponent>();
                    float comboPerCalc = Punch.ComboPercent + .03f;
                    if (comboPerCalc > .5f) comboPerCalc = .5f;
                    Punch.ComboPercent = comboPerCalc;

                    float comboTimeCalc = Punch.ComboTimer + .25f;
                    if (comboTimeCalc >= .8f) comboTimeCalc = .8f;
                    Punch.ComboTimer = comboTimeCalc;
                    
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