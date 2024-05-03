using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using Alexandria.ItemAPI;
using MultiplayerBasicExample;
using System.Collections.Generic;

namespace Knives
{

    public class Zapistol : AdvancedGunBehavior
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Zapistol", "Zap");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:zapistol", "ski:zapistol");
            gun.gameObject.AddComponent<Zapistol>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Minimalistic");
            gun.SetLongDescription("This SP4RK brand zapper was in development prior to the corporation's appropriation of technology from the ARC private security company. " +
                "Due to being in te early stages of development the design consists of manually triggering the priming cap of a single burst battery.\n\n" +
                "Developed By SP4RK in the early 23 hundreds the burst battery is able to unload its entire electric storage at once. " +
                "" +
                "\n\n\n - Knife_to_a_Gunfight");
            
            gun.SetupSprite(null, "Zap_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 1);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(79) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = .75f;
            gun.DefaultModule.cooldownTime = .5f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.SetBaseMaxAmmo(300);
            
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.D;
            gun.barrelOffset.transform.localPosition = new Vector3(.5f, .2f, 0f);
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 17f;
            projectile.baseData.speed = 60;
            projectile.PenetratesInternalWalls = true;
            projectile.AdditionalScaleMultiplier *= .5f;
            PierceProjModifier stabby = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            stabby.penetration = 4;
            
            projectile.transform.parent = gun.barrelOffset;
            LightningProjectileComp zappy = projectile.gameObject.GetOrAddComponent<LightningProjectileComp>();

            List<string> BeamAnimPaths = new List<string>()
            {

                "Knives/Resources/BeamSprites/Sp4rk_001",
                "Knives/Resources/BeamSprites/Sp4rk_002",
                "Knives/Resources/BeamSprites/Sp4rk_003",
            };
            projectile.AddTrailToProjectile(
                 "Knives/Resources/BeamSprites/Sp4rk_001",
                new Vector2(3, 2),
                new Vector2(1, 1),
                BeamAnimPaths, 20,
                BeamAnimPaths, 20,
                -1,
                0.0001f,
                5,
                true
                );
            Gun muzzle = (Gun)PickupObjectDatabase.GetById(142);
            gun.muzzleFlashEffects = muzzle.muzzleFlashEffects;
            projectile.damageTypes = CoreDamageTypes.Electric;
            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_ENM_pop_shot_01", base.gameObject);
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
           
            base.PostProcessProjectile(projectile);
        }

        private bool HasReloaded;
        //This block of code allows us to change the reload sounds.
        protected override void  Update()
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
                
            }



        }

    }
}