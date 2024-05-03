using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;

namespace Knives
{

    public class GoYo : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Go Yo", "Goyo");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:go_yo", "ski:go_yo");
            gun.gameObject.AddComponent<GoYo>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Flair and Function");
            gun.SetLongDescription("Fire and hold to throw it out into a sleeper and release to draw in the goyo and fire all chambers.\n\n" +
                "" +
                "A revolver cylinder expertly crafted together with the components of a high tech yo-yo system." +
                "" +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "Goyo_idle_001", 6);
            
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).loopStart = 1;

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);

            
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.4f;
            gun.DefaultModule.cooldownTime = .2f;
            gun.DefaultModule.numberOfShotsInClip = 6;
            gun.SetBaseMaxAmmo(250);
            gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1, StatModifier.ModifyMethod.ADDITIVE);
            gun.AddPassiveStatModifier(PlayerStats.StatType.Coolness, 1, StatModifier.ModifyMethod.ADDITIVE);
            
            gun.quality = PickupObject.ItemQuality.B;
            gun.barrelOffset.transform.localPosition = new Vector3(1.5f, 1.5f, 0f);
            
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0].projectile); // yoyo 
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            
            projectile.baseData.damage = 30f;
            projectile.baseData.speed = 17f;
            projectile.SetProjectileSpriteRight("Goyo", 10, 10, false, tk2dBaseSprite.Anchor.MiddleCenter, 11, 11);
            projectile.AdditionalScaleMultiplier *= 1.5f;
            ProjectileDoFancyBoomerang yoyo = projectile.gameObject.GetOrAddComponent<ProjectileDoFancyBoomerang>();
            yoyo.isfancy = true;
            yoyo.max_Distance = .5f;
            yoyo.isyoyo = true;

            ImprovedAfterImage trail = projectile.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            trail.shadowLifetime = .2f;
            trail.shadowTimeDelay = .0001f;
            trail.dashColor = new Color(.47f, .30f, .37f);
            trail.spawnShadows = true;

            Gun gun2 = PickupObjectDatabase.GetById(80) as Gun;
            gun.shellCasing = gun2.shellCasing;
            gun.shellsToLaunchOnReload = 6;
            gun.reloadClipLaunchFrame = 4;
            gun.shellsToLaunchOnFire = 0;
            

            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.SILLY;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            
            ID = gun.PickupObjectId;

        }

        public static int ID;
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_ENM_bulletking_throw_01", base.gameObject);
        }
        public override void PostProcessProjectile(Projectile projectile)
        {

            base.PostProcessProjectile(projectile);
        }

        private bool HasReloaded;
        public static bool RestartIdle;
        public bool toggle = false;
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

                if (GoYo.RestartIdle)
                {
                    tk2dSpriteAnimationClip clip = gun.spriteAnimator.GetClipByName(gun.idleAnimation);
                    gun.spriteAnimator.Play(clip, 0, 6, true);
                    GoYo.RestartIdle = false;
                }

            }

        }




        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_rustysidearm_reload_01", base.gameObject);
                
            }

        }

    }
}


