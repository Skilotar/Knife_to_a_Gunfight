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
    class BeatDownUnder : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Beatdown Under", "boom");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:beatdown_under", "ski:beatdown_under");
            gun.gameObject.AddComponent<BeatDownUnder>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("You'd better take cover!");
            gun.SetLongDescription("In the time before this marvelous invention, boomerangs were thrown by hand in the great dusty frontier of gunymede's lower hemisphere. \n" +
                "As an adaptation to the slow speed of the boomerangs the prey of the fronteerman grew faster and faster. Eventually dodging them with ease.\n" +
                "The BeatDown Under, patent pending, is forged from the finest woods in the gundrominian region. " +
                "It increases your boomerang throwing prowes by over 150%! But dont forget to catch your boomerang or you might be in for quite a headache!" +
                
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "boom_idle_001", 8);
            gun.SetAnimationFPS(gun.chargeAnimation, 5);

            gun.SetAnimationFPS(gun.shootAnimation, 45);
            gun.SetAnimationFPS(gun.reloadAnimation, 9);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(59) as Gun, true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = 2f;
            gun.DefaultModule.numberOfShotsInClip = 6;
            gun.DefaultModule.cooldownTime = .5f;

            gun.SetBaseMaxAmmo(900);
            gun.quality = PickupObject.ItemQuality.B;
            gun.encounterTrackable.EncounterGuid = "I come from a Land down under";

            //swipe
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(3f, .5f, 0f);
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "sail";


            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 8f;
            projectile.baseData.speed = 3f;
            projectile.baseData.range = 3f;
            projectile.baseData.force = 5;
           


            

            
            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);

            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            gun.DefaultModule.projectiles[0] = projectile2;
            projectile2.baseData.damage = 15f;
            projectile2.baseData.speed = 10f;
            projectile2.baseData.range = 100f;
            projectile2.DefaultTintColor = UnityEngine.Color.gray;
           


           
            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 0f,
                AmmoCost = 0,


            };
            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile2,
                ChargeTime = 1f,


            };
           
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
                item2,
               
            };
            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }



        public override void OnPickedUpByPlayer(PlayerController player)
        {

            base.OnPickedUpByPlayer(player);
        }
        public override void OnPostFired(PlayerController player, Gun gun)
        {

            gun.PreventNormalFireAudio = true;
        }
        public override void PostProcessProjectile(Projectile projectile)
        {

            if (projectile.GetCachedBaseDamage == 8f)
            {
                //slash
                Gun swipeFlash = (Gun)PickupObjectDatabase.GetById(382);
                PlayerController player = (PlayerController)this.gun.CurrentOwner;
                Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                ProjectileSlashingBehaviour slasher = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashDimensions = 45;
                slasher.SlashRange = 3f;
                slasher.playerKnockback = 0;
                slasher.SlashVFX = swipeFlash.muzzleFlashEffects;
                player.knockbackDoer.ApplyKnockback(vector, 25, true);
                slasher.InteractMode = SlashDoer.ProjInteractMode.DESTROY;

            }
            if (projectile.GetCachedBaseDamage == 15f)
            {
                //boomer
               
            }
            

            base.PostProcessProjectile(projectile);
        }

     

        private bool HasReloaded;

        //This block of code allows us to change the reload sounds.
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
            }

           

        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_SAA_reload_01", base.gameObject);


            }
        }


    }
}


