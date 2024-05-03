using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using Alexandria.ItemAPI;
using MultiplayerBasicExample;
using System.Collections.Generic;
using Dungeonator;

namespace Knives
{

    public class SMRT : AdvancedGunBehavior
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("SMRT", "SMRT");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:smrt", "ski:smrt");
            gun.gameObject.AddComponent<SMRT>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Target Rich Environment");
            gun.SetLongDescription("A specialty high tech pistol used by pilots in emergency situations." +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "SMRT_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(79) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1f;
            gun.DefaultModule.cooldownTime = .1f;
            gun.DefaultModule.numberOfShotsInClip = 28;
            gun.SetBaseMaxAmmo(28);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.EXCLUDED;

            AlexandriaTags.SetTag(gun.PickupObjectId, "modular_special_override");

            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            
            projectile.baseData.damage = 7f;
            projectile.baseData.speed *= 1f;
            HomingModifier home = projectile.gameObject.GetOrAddComponent<HomingModifier>();
            home.enabled = true;
            home.HomingRadius = 10f;
            home.AngularVelocity = 1000;
            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;

            //projectile.gameObject.GetOrAddComponent<SMRTController>();

        }

        public static int ID;
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_WPN_beretta_shot_01", base.gameObject);

        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            
                

            base.PostProcessProjectile(projectile);

            
        }

        protected override void Update()
        {
            if (this.Owner == null)
            {
                UnityEngine.GameObject.Destroy(this);
            }
            if (this.Owner.CurrentGun != this.gun)
            {
                UnityEngine.GameObject.Destroy(this);
            }
            base.Update();
        }


        private bool HasReloaded;
        //This block of code allows us to change the reload sounds.
        
       
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_cata_reload_sfx", base.gameObject);


            }

        }
        
    }
}
