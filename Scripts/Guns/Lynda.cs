using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;

namespace Knives
{

    public class Lynda : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Lynda 9mm", "Lynda");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:lynda_9mm", "ski:lynda_9mm");
            gun.gameObject.AddComponent<Lynda>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Hold On!");
            gun.SetLongDescription("Accurate unless fired Rapidly.\n\n" +
                "The Herman Arms Lynda is a sturdy carbine style pistol that untilizes an internal bolt. " +
                "If you can keep the gun from wobbling around it remains quite accurate. " +

                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "Lynda_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 13);
            gun.SetAnimationFPS(gun.reloadAnimation, 7);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 2f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;

            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1f;
            gun.DefaultModule.cooldownTime = .08f;
            gun.DefaultModule.numberOfShotsInClip = 20;
            gun.SetBaseMaxAmmo(600);
            gun.quality = PickupObject.ItemQuality.C;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(15) as Gun).muzzleFlashEffects;
            gun.shellCasing = (PickupObjectDatabase.GetById(15) as Gun).shellCasing;
            
            
            gun.shellCasingOnFireFrameDelay = 3;
            gun.shellsToLaunchOnFire = 1;
            gun.shellsToLaunchOnReload = 0;
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.carryPixelOffset = new IntVector2(13, 0);
            gun.barrelOffset.transform.position = new Vector3(32 / 16f, 11 / 16f, 0 / 16f);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 4f;
            projectile.baseData.force = 0;
            projectile.baseData.speed *= 1.2f;
            projectile.BossDamageMultiplier *= .1f;
            projectile.AppliesKnockbackToPlayer = true;
            projectile.PlayerKnockbackForce = 3;
            projectile.DefaultTintColor = ExtendedColours.silvedBulletsSilver;
            projectile.HasDefaultTint = true;


            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.FULLAUTO;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            ID = gun.PickupObjectId;
        }

        public static int ID;


        public override void OnPostFired(PlayerController player, Gun gun)
        {

        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            AkSoundEngine.PostEvent("Play_CHR_female_voice_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_WPN_earthwormgun_shot_01", base.gameObject);
            ExtraSpread++;
            ExtraSpread++;
            if (ExtraSpread >= 35) ExtraSpread = 35;
            base.PostProcessProjectile(projectile);
        }

        private bool HasReloaded;
        public int ExtraSpread = 0;
        public float SpreadResetTimer = .25f;
        public override void Update()
        {
            if (gun.CurrentOwner)
            {
                PlayerController player = (PlayerController)gun.CurrentOwner;
                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;

                }

                if (gun.IsFiring)
                {
                    gun.DefaultModule.angleVariance = 2f + ExtraSpread;
                    SpreadResetTimer = .25f;
                }
                else
                {
                    if(SpreadResetTimer > 0)
                    {
                        SpreadResetTimer -= Time.deltaTime;
                    }
                    else
                    {
                        ExtraSpread = 0;
                        gun.DefaultModule.angleVariance = 2f;
                    }
                }
                if(gun.ClipShotsRemaining == 0)
                {
                    gun.reloadTime = 1.5f;
                    special_reload = true;
                }
                else
                {
                    gun.reloadTime = 1f;
                    special_reload = false;
                }
                
            }

        }

        bool special_reload = false;
        
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_rpg_reload_01", base.gameObject);
                if (special_reload)
                {
                    StartCoroutine(RackNewRound());
                }
            }

        }

        private IEnumerator RackNewRound()
        {
            yield return new WaitForSeconds(1);
            gun.spriteAnimator.Play("Lynda_intro");

        }
    }
}
