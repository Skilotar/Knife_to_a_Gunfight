using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Brave;
using Brave.BulletScript;
using Gungeon;
using HutongGames.PlayMaker.Actions;
using ItemAPI;
using UnityEngine;

namespace Knives
{
    class Righty : AdvancedGunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Righty", "righty");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:righty", "ski:righty");
            gun.gameObject.AddComponent<Righty>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Killer Offence");
            gun.SetLongDescription("" +
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "righty_idle_001", 8);

            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(59) as Gun, true, false);



            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.HiddenOneHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = 1f;

            gun.DefaultModule.cooldownTime = .5f;
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
            projectile.baseData.speed = 3f;
            projectile.baseData.range = 3f;
            projectile.baseData.force = 5;
            projectile.BossDamageMultiplier = 2;
            ProjectileSlashingBehaviour slasher = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
            slasher.SlashDimensions = 30;
            
            slasher.SlashRange = 3f;
            slasher.playerKnockback = 0;
            Gun swipeFlash = (Gun)PickupObjectDatabase.GetById(335);
            slasher.soundToPlay = "Play_gln_swing_miss_001";
            slasher.SlashVFX = swipeFlash.muzzleFlashEffects;
            slasher.InteractMode = SlashDoer.ProjInteractMode.IGNORE;
           


            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }

        public bool DingDing = false;

        public override void  OnPickedUpByPlayer(PlayerController player)
        {

            base.OnPickedUpByPlayer(player);
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            
            
           
            base.PostProcessProjectile(projectile);
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

               
            }


        }
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);

                base.OnReloadPressed(player, gun, bSOMETHING);

            }

        }


    }
}