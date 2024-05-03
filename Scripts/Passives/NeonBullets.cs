using System;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using UnityEngine;
using ItemAPI;
using HutongGames.PlayMaker.Actions;
using MultiplayerBasicExample;
using MonoMod.Utils;
using MonoMod;
using System.Runtime.CompilerServices;

namespace Knives
{
    public class Neon_bullets : PassiveItem
    {
        //Call this method from the Start() method of your ETGModule extension
        public static void Register()
        {
            //The name of the item
            string itemName = "Neon Bullets";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Neon_bullets";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Neon_bullets>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            
            string shortDesc = "Deadly When Wet!";
            string longDesc =
                "Bullets MiniCrit when hitting a wet enemy. These bullets do not hold much charge but can still pack a punch!" +
                "\n\n\n - Knife_to_a_Gunfight";

            

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

           

            item.quality = PickupObject.ItemQuality.B;
        }
        public System.Random rng = new System.Random();
        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += this.PostProcessProjectile;

            base.Pickup(player);
        }

        private void PostProcessProjectile(Projectile projectile, float chance)
        {

            projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(OnHitEnemy));
        }

        public void OnHitEnemy(Projectile proj, SpeculativeRigidbody arg2, bool yes)
        {

            if (arg2 != null)
            {
                if (arg2.aiActor != null)
                {
                    if (arg2.healthHaver.IsBoss != true || arg2.aiActor.EnemyGuid != "465da2bb086a4a88a803f79fe3a27677")
                    {
                        if (this.IsPointGoopy(arg2.aiActor.CenterPosition) || this.IsPointfirey(arg2.aiActor.CenterPosition))
                        {

                            Krzirrrrt(arg2);

                        }
                    }
                }

            }
           
        }
        public void Krzirrrrt(SpeculativeRigidbody body)
        {

            Projectile projectile = ((Gun)ETGMod.Databases.Items[370]).DefaultModule.chargeProjectiles[1].Projectile;
            projectile.baseData.damage = 7f;
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, body.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (this.Owner.CurrentGun == null) ? 0f : this.Owner.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            component.baseData.range = .01f;
            component.baseData.speed = 1f;
            PierceProjModifier stab = component.gameObject.GetOrAddComponent<PierceProjModifier>();
            stab.penetration = 0;
            component.Owner = this.Owner;
            if (body.healthHaver.IsVulnerable && body.healthHaver.GetCurrentHealth() <= 30 && !body.healthHaver.IsBoss)
            {
                AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
                AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
               
                
            }
        }
        private bool IsPointGoopy(Vector2 testPos)
        {
            IntVector2 Stupidfrickinggoop = (testPos / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2(VectorConversions.Floor);
            if (DeadlyDeadlyGoopManager.allGoopPositionMap.ContainsKey(Stupidfrickinggoop))
            {
                DeadlyDeadlyGoopManager deadlyDeadlyGoopManager = DeadlyDeadlyGoopManager.allGoopPositionMap[Stupidfrickinggoop];
                return deadlyDeadlyGoopManager.IsPositionInGoop(testPos);
            }
            return false;
        }
        private bool IsPointfirey(Vector2 testPos)
        {
            IntVector2 Stupidfrickinggoop = (testPos / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2(VectorConversions.Floor);
            if (DeadlyDeadlyGoopManager.allGoopPositionMap.ContainsKey(Stupidfrickinggoop))
            {
                DeadlyDeadlyGoopManager deadlyDeadlyGoopManager = DeadlyDeadlyGoopManager.allGoopPositionMap[Stupidfrickinggoop];
                return deadlyDeadlyGoopManager.IsPositionOnFire(testPos);
            }
            return false;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.PostProcessProjectile;
            return base.Drop(player);
        }

    }
}
