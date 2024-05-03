using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using ItemAPI;

namespace Knives
{
    class BleakBubbles : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Bleaker's Bubbles";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/bleakers_bubbles";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<BleakBubbles>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Squeaky Clean";
            string longDesc =

                "Cleanliness is close to godliness. A perfected alchemical detergent that not only cleans the users filth but also removes imperfections in them. But be careful it is slippery and spilling it causes quite the mess! \n\n" +
                "Hold the item for statbuffs or roll to drop it and make a ring of bubbles." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;


            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, .50f, StatModifier.ModifyMethod.ADDITIVE);
            
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.MovementSpeed, 1f, StatModifier.ModifyMethod.ADDITIVE);
            //Set the rarity of the item


            item.quality = PickupObject.ItemQuality.A;
        }

        public override void Pickup(PlayerController player)
        {
            player.OnPreDodgeRoll += this.OnPreDodgeRoll;
            base.Pickup(player);
        }


       
        private void OnPreDodgeRoll(PlayerController player)
        {
            //drop when roll
            player.DropPassiveItem(this);
            int color = 1;
            //fire bubbles when roll
            for (int i = 0; i < 20; i++)
            {
                Projectile projectile2 = ((Gun)ETGMod.Databases.Items[599]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + (i * 18)), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag = component != null;
                if (flag)
                {
                    
                    if (color == 4) color = 1;
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    component.baseData.damage = 0f;
                    if(color == 1)
                    {
                        //poison
                        component.DefaultTintColor = UnityEngine.Color.green;
                        component.HasDefaultTint = true;
                        component.PoisonApplyChance = 100;
                        component.AppliesPoison = true;
                        component.baseData.speed = 25;
                    }
                    if (color == 2)
                    {
                        //fire
                        component.DefaultTintColor = UnityEngine.Color.red;
                        component.HasDefaultTint = true;
                        component.FireApplyChance = 100;
                        component.AppliesFire = true;
                        component.baseData.speed = 25;
                    }
                    if (color == 3)
                    {
                        //water?
                        component.DefaultTintColor = UnityEngine.Color.blue;
                        component.HasDefaultTint = true;
                        component.baseData.damage = 5;
                        component.baseData.speed = 25;
                    }
                    color++;
                }

            }
        }
        public override DebrisObject Drop(PlayerController player)
        {
            player.OnPreDodgeRoll -= this.OnPreDodgeRoll;
            return base.Drop(player);
        }

        public override void Update()
        {

            base.Update();
        }


    }
}
