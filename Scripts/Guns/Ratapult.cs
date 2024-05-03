using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MonoMod.RuntimeDetour;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


using MultiplayerBasicExample;

namespace Knives
{

    class Ratapult : AdvancedGunBehaviour
    {
       
        public static void Add()
        {
            
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Ratapult", "rat_launcher");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:ratapult", "ski:ratapult");
            gun.gameObject.AddComponent<Ratapult>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Bio Warfare Times 2");
            gun.SetLongDescription("This gun is loaded with Clones of Henry the rat. Say hi to Henry! Henry has been infected with 14 diseases. Say bye to Henry and launch him at your enemies!\n\n" +
                "Projectiles apply illness. Illness ramps up in damage over time." +
                "\n\n\n - Knife_to_a_Gunfight");
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "rat_launcher_idle_001", 3);

            
            gun.SetAnimationFPS(gun.shootAnimation, 12);
            gun.SetAnimationFPS(gun.reloadAnimation, 4);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(197) as Gun, true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 1;
            gun.gunClass = GunClass.POISON;
            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.muzzleFlashEffects = null;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = 1f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.DefaultModule.cooldownTime = .5f;
            //gun.encounterTrackable.EncounterGuid = "Ski's Ratapult";


            gun.SetBaseMaxAmmo(300);
            gun.quality = PickupObject.ItemQuality.D;
            gun.barrelOffset.transform.localPosition = new Vector3(1.7f, .7f, 0f);

            
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            
          

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 7f;
            projectile.baseData.speed = 15f;
            projectile.baseData.range = 20f;
            projectile.baseData.force = 5;
            projectile.angularVelocity = 60;
            
            
            projectile.SetProjectileSpriteRight("Henry", 13, 13, false, tk2dBaseSprite.Anchor.MiddleCenter, 13, 13);
            projectile.AdditionalScaleMultiplier = 1.4f;
            projectile.HasDefaultTint = false;
          

            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            ID = gun.PickupObjectId;
        }

        

        public static int ID;

        private void onHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2.aiActor)
            {
                arg2.aiActor.ApplyEffect(new GameActorillnessEffect());
            }
        }

        public System.Random rng = new System.Random();

        public override void  OnPickedUpByPlayer(PlayerController player)
        {

            base.OnPickedUpByPlayer(player);
        }
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_WPN_stickycrossbow_shot_01", base.gameObject);
            gun.PreventNormalFireAudio = true;
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            projectile.OnHitEnemy += this.onHitEnemy;
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
                AkSoundEngine.PostEvent("Play_WPN_crossbow_reload_01", base.gameObject);


            }
        }
    }
}



