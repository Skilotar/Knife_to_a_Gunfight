using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;
using System.Reflection;

namespace Knives
{
    class HotStreak : PassiveItem
    {
        
        public static void Register()
        {
            //The name of the item
            string itemName = "Hot Streak";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/HotStreak";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<HotStreak>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Spicy Shots";
            string longDesc = "Hitting enemies slightly reduces active item cooldowns. Bullets Have Hot Auras\n\n" +
                "" +
                "Extra spicy extract has been injected directly into your bullets causing your shots to have a some heat on em." +
                "\n\n\n - Knife_to_a_Gunfight";


            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            
            item.quality = PickupObject.ItemQuality.B;

        }
        
        public override void Pickup(PlayerController player)
        {
           
            base.Pickup(player);
            player.OnDealtDamage += Player_OnDealtDamage;
            player.OnReceivedDamage += Player_OnReceivedDamage;
            player.PostProcessProjectile += Player_PostProcessProjectile;
        }

        private void Player_PostProcessProjectile(Projectile arg1, float arg2)
        {
            if (arg1 && UnityEngine.Random.Range(0,5) < 1)
            {
                ProjectileHeatRingModifier hot = arg1.gameObject.GetOrAddComponent<ProjectileHeatRingModifier>();
                float Radius = 1.8f * (combo / 100);
                hot.rad = Radius;
                hot.dur = 20;
            }
        }

        private void Player_OnReceivedDamage(PlayerController obj)
        {
            if(obj != null)
            {
                combo = 1;
            }
        }

        private void Player_OnDealtDamage(PlayerController arg1, float arg2)
        {
            
            foreach (PlayerItem item in this.Owner.activeItems)
            {

                float alttime = item.CurrentTimeCooldown - (item.timeCooldown * .015f);
                float altDam = item.CurrentDamageCooldown - (item.damageCooldown * .01f);
                int altRoom = item.CurrentRoomCooldown - 1;

                if (alttime <= 0) alttime = 0;
                if (altDam <= 0) altDam = 0;
                if (altRoom <= 0) altRoom = 0;


                item.CurrentTimeCooldown = alttime;
                item.CurrentDamageCooldown = altDam;

                item.DidDamage(this.Owner, 1);

                if (this.hits > 200)
                {
                    item.CurrentRoomCooldown = altRoom;
                    hits = 0;
                }

            }

            if(combo < 200)
            {
                combo++;
            }
        }


        public override void Update()
        {
            if (this.Owner != null)
            {
               
            }
            base.Update();
        }

        public int hits = 0;
        public int combo = 1;

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);

            player.OnDealtDamage -= Player_OnDealtDamage;
            player.OnReceivedDamage -= Player_OnReceivedDamage;
            player.PostProcessProjectile -= Player_PostProcessProjectile;
            return debrisObject;
        }

       
    }
}

