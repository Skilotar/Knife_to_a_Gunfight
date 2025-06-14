using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Brave;
using Dungeonator;
using Brave.BulletScript;
using Gungeon;
using HutongGames.PlayMaker.Actions;
using ItemAPI;
using UnityEngine;

namespace Knives
{
    class Volt : AdvancedGunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Volt Haymaker", "volt");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:volt_haymaker", "ski:volt_haymaker");
            gun.gameObject.AddComponent<Volt>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("You've Been...");
            gun.SetLongDescription("THUNDERSTRUCK!\n\n" +
                "A Charge-able gauntlet that is Positive to make them Hurtz!\n" +
                "reloading places pilon nodes that can be used to either power up your gauntlet or herd your enemies." +
                "\n\n\n - Knife_to_a_GunJam"); ;


            gun.SetupSprite(null, "volt_charge_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(59) as Gun, true, false);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 5;



            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = 1.2f;

            gun.DefaultModule.cooldownTime = .55f;
            gun.CanReloadNoMatterAmmo = true;



            gun.DefaultModule.numberOfShotsInClip = 600;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.B;


            //swipe
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(.5f, .5f, 0f);

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 12f;
            projectile.BossDamageMultiplier = 2;
            projectile.baseData.speed = 3f;
            projectile.baseData.range = 3f;
            projectile.baseData.force = 5;
            ProjectileSlashingBehaviour slasher = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();

            slasher.SlashDimensions = 30;
            slasher.SlashRange = 3f;
            slasher.playerKnockback = 0;
            Gun swipeFlash = (Gun)PickupObjectDatabase.GetById(335);
            slasher.soundToPlay = "Play_gln_swing_miss_001";
            slasher.SlashVFX = swipeFlash.muzzleFlashEffects;
            slasher.InteractMode = SlashDoer.ProjInteractMode.IGNORE;
         

            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = .5f,
                AmmoCost = 0,


            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
               
            };
            projectile.transform.parent = gun.barrelOffset;

            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }

        public override void  OnPickedUpByPlayer(PlayerController player)
        {
            
            base.OnPickedUpByPlayer(player);
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            PlayerController player = this.gun.CurrentOwner as PlayerController;
            projectile.Owner.StartCoroutine(HandleDash(projectile.Owner as PlayerController,projectile));
            Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
          
            player.StartCoroutine(this.HandleSwing(player, vector, (.5f * player.stats.GetStatValue(PlayerStats.StatType.Damage)), 2.5f));
            base.PostProcessProjectile(projectile);
        }

        private IEnumerator HandleSwing(PlayerController user, Vector2 aimVec, float rayDamage, float rayLength)
        {
            float elapsed = 0f;
            while (elapsed < .3)
            {
                elapsed += BraveTime.DeltaTime;
                SpeculativeRigidbody hitRigidbody = this.IterativeRaycast(user.CenterPosition, aimVec, rayLength, int.MaxValue, user.specRigidbody);
                if (hitRigidbody && hitRigidbody.aiActor && hitRigidbody.aiActor.IsNormalEnemy)
                {

                    if (user.IsPrimaryPlayer)
                    {

                        hitRigidbody.aiActor.healthHaver.ApplyDamage(rayDamage, aimVec, "primaryplayer", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                        hitRigidbody.aiActor.knockbackDoer.ApplyKnockback(aimVec, 20);


                    }
                    else
                    {

                        hitRigidbody.aiActor.healthHaver.ApplyDamage(rayDamage, aimVec, "secondaryplayer", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                        hitRigidbody.aiActor.knockbackDoer.ApplyKnockback(aimVec, 20);

                    }
                }
               

                yield return null;
            }
            yield break;
        }


        protected SpeculativeRigidbody IterativeRaycast(Vector2 rayOrigin, Vector2 rayDirection, float rayDistance, int collisionMask, SpeculativeRigidbody ignoreRigidbody)
        {
            int num = 0;
            RaycastResult raycastResult;
            while (PhysicsEngine.Instance.Raycast(rayOrigin, rayDirection, rayDistance, out raycastResult, true, true, collisionMask, new CollisionLayer?(CollisionLayer.BulletBlocker), false, null, ignoreRigidbody))
            {
                num++;
                SpeculativeRigidbody speculativeRigidbody = raycastResult.SpeculativeRigidbody;
                if (num < 3 && speculativeRigidbody != null)
                {
                    MinorBreakable component = speculativeRigidbody.GetComponent<MinorBreakable>();
                    if (component != null)
                    {
                        component.Break(rayDirection.normalized * 3f);
                        RaycastResult.Pool.Free(ref raycastResult);
                        continue;
                    }
                }
                RaycastResult.Pool.Free(ref raycastResult);
                return speculativeRigidbody;
            }
            return null;
        }

        public IEnumerator HandleDash(PlayerController user, Projectile projectile)
        {
            float duration = .30f;
            float adjSpeed = 35;
            float elapsed = -BraveTime.DeltaTime;
            float angle = user.CurrentGun.CurrentAngle;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                this.Owner.specRigidbody.Velocity = BraveMathCollege.DegreesToVector(angle).normalized * adjSpeed;
                yield return null;
            }

        }

        private bool HasReloaded;

        public override void  Update()
        {
            if (gun.CurrentOwner)
            {

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
                if (gun.ClipShotsRemaining == gun.ClipCapacity)
                {
                    gun.ClipShotsRemaining = gun.ClipCapacity - 1;
                }

                PassiveItem.IncrementFlag((PlayerController)this.gun.CurrentOwner, typeof(LiveAmmoItem));
            }


        }


        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);

                base.OnReloadPressed(player, gun, bSOMETHING);

            }

        }


    }
}