using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Gungeon;
using Dungeonator;
using MonoMod;
using UnityEngine;
using SaveAPI;
using ItemAPI;

namespace Knives
{
    public class FlatLine : AdvancedGunBehaviour
    {
        public static void Add()
        {

            Gun gun = ETGMod.Databases.Items.NewGun("Flat Line", "Flatline");
            Game.Items.Rename("outdated_gun_mods:flat_line", "ski:flat_line");
            var behav = gun.gameObject.AddComponent<FlatLine>();
            gun.OverrideNormalFireAudioEvent = "Play_WPN_moonscraperLaser_shot_01";
            behav.preventNormalFireAudio = true;
            gun.SetShortDescription("Life's Final Note");
            gun.SetLongDescription("A medical instrument that doubles as a weapon. In the wars before the Guneva convention was radified, medical personel were seen as high value targets. Many common medical tools were multipurposed to aid with both the healing and the hurting." +
                "\n\n" +
                "Deals more damage at the top end of enemies health." +
                "\n\n\n - Knife_to_a_Gunfight");

            gun.SetupSprite(null, "Flatline_idle_005", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 9);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);

            //GUN STATS
            gun.doesScreenShake = false;
            gun.DefaultModule.ammoCost = 10;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Beam;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.carryPixelOffset = new IntVector2(6, 0);
            gun.reloadTime = 1.5f;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.DefaultModule.cooldownTime = 0.01f;
            gun.DefaultModule.numberOfShotsInClip = 50;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.gunClass = GunClass.BEAM;
            gun.DefaultModule.customAmmoType = "green_beam";
            gun.barrelOffset.transform.localPosition = new Vector3(1.6f, .3f, 0f);
            gun.SetBaseMaxAmmo(800);


            List<string> BeamAnimPaths = new List<string>()
            {
                "Knives/Resources/BeamSprites/Flatbeam_mid_001",
                "Knives/Resources/BeamSprites/Flatbeam_mid_002",
                "Knives/Resources/BeamSprites/Flatbeam_mid_003",
                "Knives/Resources/BeamSprites/Flatbeam_mid_004",
                "Knives/Resources/BeamSprites/Flatbeam_mid_005",
                "Knives/Resources/BeamSprites/Flatbeam_mid_006",
                "Knives/Resources/BeamSprites/Flatbeam_mid_007",
                "Knives/Resources/BeamSprites/Flatbeam_mid_008",
                "Knives/Resources/BeamSprites/Flatbeam_mid_009",
                "Knives/Resources/BeamSprites/Flatbeam_mid_010",
                "Knives/Resources/BeamSprites/Flatbeam_mid_011",
                "Knives/Resources/BeamSprites/Flatbeam_mid_012",

            };
            List<string> BeamEndPaths = new List<string>()
            {
               "Knives/Resources/BeamSprites/Flatbeam_end_001",

            };
            List<string> BeamStartPaths = new List<string>()
            {
                "Knives/Resources/BeamSprites/Flatbeam_start_001",
            };

            //BULLET STATS
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(15) as Gun).DefaultModule.projectiles[0]);

            BasicBeamController beamComp = projectile.GenerateBeamPrefab(
               "Knives/Resources/BeamSprites/Flatbeam_mid_001",
                new Vector2(12, 9),
                new Vector2(0, 1),
                BeamAnimPaths,
                9,
                //Impact
                null,
                -1,
                null,
                null,
                //End
                BeamEndPaths,
                1,
                new Vector2(8, 9),
                new Vector2(0, 1),
                //Beginning
                BeamStartPaths,
                1,
                new Vector2(8, 9),
                new Vector2(0, 1)
                );

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.baseData.damage = 100f;
            projectile.baseData.force *= 0.1f;
            projectile.baseData.range = 50f;
            projectile.baseData.speed *= 5f;
            projectile.gameObject.GetOrAddComponent<LifeRampModifier>();
            beamComp.penetration = 100;
          
            beamComp.boneType = BasicBeamController.BeamBoneType.Straight;
            beamComp.DamageModifier = 1f;
            beamComp.interpolateStretchedBones = true;
            gun.DefaultModule.projectiles[0] = projectile;

            gun.quality = PickupObject.ItemQuality.B;

            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.SetupUnlockOnCustomFlag(CustomDungeonFlags.FLATLINED, true);
        }
        bool limiter = false;

        public override void  OnPickup(GameActor owner)
        {
          
            base.OnPickup(owner);
        }
        public override void OnPostFired(PlayerController player, Gun gun)
        {

           
            base.OnPostFired(player, gun);
        }
        public override void OnFinishAttack(PlayerController player, Gun gun)
        {
          
            base.OnFinishAttack(player, gun);
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
           
            base.PostProcessProjectile(projectile);
           

        }


        private bool HasReloaded;
      
        public System.Random rng = new System.Random();
        public override void  Update()
        {
            if (this.gun.CurrentOwner != null)
            {

                PlayerController player = (PlayerController)this.gun.CurrentOwner;

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }

                if (this.gun.ClipShotsRemaining > 1 && gun.IsFiring && limiter == false && this.HasReloaded && Time.timeScale > 0f && !player.IsDodgeRolling)
                {


                   
                    AkSoundEngine.PostEvent("Play_WPN_moonscraperLaser_shot_01", base.gameObject);
                  
                    limiter = true;

                }

                

                if (!gun.IsFiring && limiter)
                {

                   
                    AkSoundEngine.PostEvent("Stop_WPN_moonscraperLaser_shot_01", base.gameObject);

                    limiter = false;
                }

                if (gun.IsFiring && player.IsDodgeRolling && limiter)
                {


                    AkSoundEngine.PostEvent("Stop_WPN_moonscraperLaser_shot_01", base.gameObject);

                    limiter = false;
                }

            }

        }


        

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                this.gun.Update();


                int sound = rng.Next(1, 3);
                if (sound == 1)
                {
                    AkSoundEngine.PostEvent("Play_Flatline_reload_001", base.gameObject); 
                }
                if (sound == 2)
                {
                    AkSoundEngine.PostEvent("Play_Flatline_reload_002", base.gameObject);
                }
                if (sound == 3)
                {
                    AkSoundEngine.PostEvent("Play_Flatline_reload_003", base.gameObject);
                }


                HasReloaded = false;

                base.OnReloadPressed(player, gun, bSOMETHING);


            }

        }

       


    }
}
