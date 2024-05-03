using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Gungeon;
using SaveAPI;

namespace Knives
{
    class HeavyImpact : PassiveItem
    {
        //Call this method from the Start() method of your ETGModule extension
        public static void Register()
        {
            //The name of the item
            string itemName = "Slap Bass";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/HeavyImpact";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<HeavyImpact>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Heavy Impact";
            string longDesc = "Rolling into enemies deals more damage and launches killed enemies as projectiles.\n\n" +
                "" +
                "A powerful guitar capable of withstanding being smashed into the floor. While used primarily for intense rock concerts the amps attached boost the kinetic energy of collisions." +
            "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.KnockbackMultiplier, .2f);

            //Set the rarity of the item
            item.quality = PickupObject.ItemQuality.B;


            ID = item.PickupObjectId;
            item.SetupUnlockOnCustomFlag(CustomDungeonFlags.SKATER_SLAP, true);



        }
        public static int ID;

        public override void Pickup(PlayerController player)
        {
            PassiveItem.IncrementFlag(player, typeof(LiveAmmoItem));
            player.OnRolledIntoEnemy += Player_OnRolledIntoEnemy;
            base.Pickup(player);
        }

        private void Player_OnRolledIntoEnemy(PlayerController arg1, AIActor arg2)
        {
            if(arg1 != null)
            {
                if(arg2.aiActor != null)
                {
                    
                    arg2.knockbackDoer.ApplySourcedKnockback(arg1.CurrentGun.CurrentAngle.DegreeToVector2(), 30, arg1.gameObject, true);
                   
                    arg1.knockbackDoer.ApplySourcedKnockback(arg1.CurrentGun.CurrentAngle.DegreeToVector2() * -1, 5, arg1.gameObject, true);
                    
                    if(arg2.healthHaver.GetCurrentHealth() <= 15 +  (arg1.stats.rollDamage * 3))
                    {
                        arg2.specRigidbody.RegisterGhostCollisionException(arg1.specRigidbody);
                        Projectile proj = ((Gun)PickupObjectDatabase.GetById(15)).DefaultModule.projectiles[0].projectile;
                        Projectile comp = MiscToolMethods.SpawnProjAtPosi(proj, arg2.CenterPosition, arg1, arg1.CurrentGun, 0, 1, false);
                        comp.AdditionalScaleMultiplier *= 1.5f;
                        comp.baseData.damage = 20f;
                        comp.sprite.renderer.enabled = false;
                        comp.gameObject.GetOrAddComponent<KilledEnemiesBecomeProjectileModifier>();
                        arg1.healthHaver.TriggerInvulnerabilityPeriod(1.5f);

                        if (UnityEngine.Random.Range(0, 2) < 1)
                        {
                            AkSoundEngine.PostEvent("Play_HeavyImpact_001", base.gameObject);
                        }
                        else
                        {
                            AkSoundEngine.PostEvent("Play_HeavyImpact_002", base.gameObject);
                        }
                        Exploder.DoDistortionWave(arg1.CenterPosition, .5f, 0.04f, 1.5f, .4f);
                    }
                    else
                    {
                        arg2.healthHaver.ApplyDamage(10, Vector2.zero, "impact");
                    }

                }
            }
        }


        public override DebrisObject Drop(PlayerController player)
        {
           
            player.OnRolledIntoEnemy -= Player_OnRolledIntoEnemy;
            PassiveItem.DecrementFlag(player, typeof(LiveAmmoItem));
            return base.Drop(player);
        }
        public override void Update()
        {
            //extra damage while standing on goop
            base.Update();
        }
    }
}
