using System;
using System.Collections.Generic;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;

namespace Knives
{

    public class Magic10 : AdvancedGunBehaviour
    {

        public static int clip = 7;
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("MAgiC-10", "Mac");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:magic-10", "ski:magic-10");
            gun.gameObject.AddComponent<Magic10>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Survival of the Quickest");
            gun.SetLongDescription("" +
                "\n\n\n - Knife_to_a_Gunfight");
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "Mac_idle_001", 8);
           
            gun.SetAnimationFPS(gun.shootAnimation, 20);
            gun.SetAnimationFPS(gun.reloadAnimation, 4);
            Gun gun2 = (Gun)PickupObjectDatabase.GetById(94);

            
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(94) as Gun, true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;

            gun.gunHandedness = GunHandedness.OneHanded;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = .7f;
            gun.DefaultModule.numberOfShotsInClip = 25;
            gun.DefaultModule.cooldownTime = .15f;
            gun.SetBaseMaxAmmo(500);
            gun.quality = PickupObject.ItemQuality.C;

            gun.muzzleFlashEffects = gun2.muzzleFlashEffects;
            
           
            gun.PreventNormalFireAudio = true;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(0f, .6f, 0f);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 3f;
            projectile.baseData.speed *= 1f;
            projectile.baseData.range *= 1f;
            projectile.HasDefaultTint = true;
            projectile.PoisonApplyChance = .05f;
            projectile.AppliesPoison = true;
            projectile.DefaultTintColor = UnityEngine.Color.green;
            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");




            
        }
       
        public override void PostProcessProjectile(Projectile projectile)
        {
           
            PlayerController player = (PlayerController)Owner;
            BeamToolboxOMITB.FreeFireBeamFromAnywhere(PickupObjectDatabase.GetById(87).projectile, player, null, Owner.CurrentGun.sprite.WorldTopCenter, true, Owner.CurrentGun.CurrentAngle, .5f);

            base.PostProcessProjectile(projectile);
        }

        public override void  OnPickedUpByPlayer(PlayerController player)
        {
           
            base.OnPickedUpByPlayer(player);
        }

        
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            this.HasReloaded = false;
            
            AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
            base.OnReloadPressed(player, gun, bSOMETHING);



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
            }
        }
    }
}