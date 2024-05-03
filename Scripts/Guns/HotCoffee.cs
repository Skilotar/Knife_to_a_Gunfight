using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Gungeon;

namespace Knives
{
    class hot_coffee :AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Coffee Shotgun", "hot");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:coffee_shotgun", "ski:coffee_shotgun");
           
            gun.OverrideNormalFireAudioEvent = "Play_WPN_superdowser_shot_01";
            gun.gameObject.AddComponent<hot_coffee>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("A Hot Cup Of OW!");
            gun.SetLongDescription("Scalding hot and highly throwable.\n\n\n - Knife_to_a_gunfight");
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "hot_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 3);
            gun.SetAnimationFPS(gun.reloadAnimation, 10);
            
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
           
            // Here we just take the default projectile module and change its settings how we want it to be.
         

           
            for (int i = 0; i < 6; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(60) as Gun, true, false);
                gun.gunSwitchGroup = (PickupObjectDatabase.GetById(60) as Gun).gunSwitchGroup;

            }
            foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
            {
                projectileModule.ammoCost = 2;
                projectileModule.shootStyle = ProjectileModule.ShootStyle.Beam;
                projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                projectileModule.cooldownTime = .4f;
                projectileModule.angleVariance = 15f;
                projectileModule.numberOfShotsInClip = 3;

                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                projectileModule.projectiles[0] = projectile;
                projectile.baseData.damage *= .20f;
                projectile.baseData.range = 8f;
                projectile.AppliesFire = true;
                projectile.baseData.force = 0;
                projectile.AdditionalScaleMultiplier = .5f;
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
               
                gun.DefaultModule.projectiles[0] = projectile;
                bool flag = projectileModule != gun.DefaultModule;

                BasicBeamController beam = projectile.GetComponentInChildren<BasicBeamController>();
                if (!beam.IsReflectedBeam)
                {
                    beam.reflections = 0;
                }
                beam.usesTelegraph = false;
                
                beam.AdjustPlayerBeamTint(Color.black, 0);
                beam.usesChargeDelay = true;
                beam.chargeDelay = 0f;
                beam.penetration = 100;
                beam.PenetratesCover = true;

                
            }
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.AutoDetect;
            gun.reloadTime = 3f;
            gun.SetBaseMaxAmmo(1000);
            gun.quality = PickupObject.ItemQuality.B;
            //gun.encounterTrackable.EncounterGuid = "Capitalism Juice";
            

            ETGMod.Databases.Items.Add(gun, null, "ANY");



            ID = gun.PickupObjectId;
        }

        public static int ID;

        public override void  OnPickedUpByPlayer(PlayerController player)
        {
            base.OnPickedUpByPlayer(player);
            player.PostProcessBeam += this.PostProcessBeam;
            player.GunChanged += this.OnGunChanged;
        }

        private void PostProcessBeam(BeamController beam)
        {
            if (beam is BasicBeamController)
            {
                BasicBeamController basicBeamController = (beam as BasicBeamController);
                if (basicBeamController.Gun == this.gun)
                {
                    basicBeamController.ProjectileScale = .8f;
                }
              
            }
        }
        private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
        {

            if (this.gun && this.gun.CurrentOwner)
            {
                PlayerController player = this.gun.CurrentOwner as PlayerController;
                if (newGun == this.gun)
                {
                    player.PostProcessBeam += this.PostProcessBeam;
                }
                else
                {
                    player.PostProcessBeam -= this.PostProcessBeam;
                }
            }
        }
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            //This determines what sound you want to play when you fire a gun.
            //Sounds names are based on the Gungeon sound dump, which can be found at EnterTheGungeon/Etg_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/sfx.txt
            gun.PreventNormalFireAudio = true;

            


        }
        private bool HasReloaded;
        private bool limiter;

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
                PlayerController player = (PlayerController)this.gun.CurrentOwner;

                if (this.gun.ClipShotsRemaining > 1 && gun.IsFiring && limiter == false && this.HasReloaded && Time.timeScale > 0f && !player.IsDodgeRolling)
                {



                    AkSoundEngine.PostEvent("Play_WPN_superdowser_shot_01", base.gameObject);

                    limiter = true;

                }



                if (!gun.IsFiring && limiter)
                {


                    AkSoundEngine.PostEvent("Stop_WPN_superdowser_shot_01", base.gameObject);

                    limiter = false;
                }

                if (gun.IsFiring && player.IsDodgeRolling && limiter)
                {


                    AkSoundEngine.PostEvent("Stop_WPN_superdowser_shot_01", base.gameObject);

                    limiter = false;
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
                AkSoundEngine.PostEvent("Play_BOSS_blobulord_spew_01", base.gameObject);
                
            }
        }

        
    }
}
    