using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Collections.Generic;
using MultiplayerBasicExample;
using Alexandria.BreakableAPI;

namespace Knives
{
    class BigMag : AdvancedGunBehaviour
    {

        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Compact 50", "BigMag");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:compact_50", "ski:compact_50");
            gun.gameObject.AddComponent<BigMag>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("SV_Cheats BigMag 1");
            gun.SetLongDescription("//Devtool\n" +
                "//LogNote: This was a bug but we're keeping it in because its funny." +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "BigMag_idle_001", 8);


            gun.SetAnimationFPS(gun.shootAnimation, 12);
            gun.SetAnimationFPS(gun.reloadAnimation, 4);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 5;

            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 5f;
            gun.DefaultModule.numberOfShotsInClip = 50;
            gun.DefaultModule.cooldownTime = .15f;
            
            gun.SetBaseMaxAmmo(500);
            gun.quality = PickupObject.ItemQuality.B;
           
            Gun shellingFlash = (Gun)PickupObjectDatabase.GetById(96);
            gun.muzzleFlashEffects = shellingFlash.muzzleFlashEffects;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(26 / 16f, 37 / 16f, 0f);
            gun.carryPixelOffset = new IntVector2(13, 0);

            gun.shellsToLaunchOnFire = 1;
            Gun gun2 = PickupObjectDatabase.GetById(84) as Gun;
            gun.shellCasing = gun2.shellCasing;
            Casing = gun2.shellCasing;
            
            gun.shellsToLaunchOnReload = 1;
            gun.reloadShellLaunchFrame = 7;
            Mag = BreakableAPIToolbox.GenerateDebrisObject("Knives/Resources/Casings/BigMag_Mag.png", true, 2, 4, 200, 150, null, 3f, null, null, 1).gameObject;
            
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 4f;
            projectile.baseData.speed *= 2f;
            projectile.baseData.range *= 1f;
            projectile.baseData.force = 5f;
            
            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;

            MiscToolMethods.TrimAllGunSprites(gun);
        }
        public static GameObject Casing;
        public static GameObject Mag;

        public static int ID;

        public override void OnPickedUpByPlayer(PlayerController player)
        {

            base.OnPickedUpByPlayer(player);
        }
        public override void OnGunsChanged(Gun previous, Gun current, bool newGun)
        {
           
            base.OnGunsChanged(previous, current, newGun);
        }
        public override void OnPostFired(PlayerController player, Gun gun)
        {


            gun.PreventNormalFireAudio = true;
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
           
            base.PostProcessProjectile(projectile);
        }
        public bool HasReloaded = true;
        public bool doing = false;
        public override void Update()
        {
            if (this.gun.CurrentOwner)
            {

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;

                }
                if (!gun.IsReloading && !HasReloaded)
                {

                    this.HasReloaded = true;
                }

                if (this.gun.IsReloading)
                {
                    gun.shellCasing = Mag;
                }
                else
                {
                    gun.shellCasing = Casing;
                }
                PlayerController player = this.gun.CurrentOwner as PlayerController;
                if (player.PlayerHasActiveSynergy("Basically Cheating"))
                {
                    if(HasSyn == false)
                    {
                        HasSyn = true;
                        gun.InfiniteAmmo = true;
                    }
                }
                else
                {
                    if (HasSyn == true)
                    {
                        HasSyn = false;
                        gun.InfiniteAmmo = false;
                    }
                }
            }

            base.Update();
        }
        public bool HasSyn = false;

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_big_mag_reload", base.gameObject);

            }

        }

    }
}


