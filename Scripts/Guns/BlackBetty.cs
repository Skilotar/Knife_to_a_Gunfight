using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;
using System.Collections.Generic;

namespace Knives
{

    public class Black_Betty : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Black Betty", "Black_Betty");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:black_betty", "ski:black_betty");
            gun.gameObject.AddComponent<Black_Betty>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("BAM-Ba-Lam");
            gun.SetLongDescription("Successful hits increase the damage enemies take; this effect stacks. \n\n" +
                "A pistol made of pure shadow. Its bullets weaken the resolve of its victims causing them to fall prey to dark forces. " +
                "" +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "Black_Betty_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 7);
            gun.SetAnimationFPS(gun.reloadAnimation, 10);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(79) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 5f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.3f;
            gun.DefaultModule.cooldownTime = .3f;
            gun.DefaultModule.numberOfShotsInClip = 8;
            gun.SetBaseMaxAmmo(250);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.B;

            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            gun.carryPixelOffset = new IntVector2(6, 0);
            gun.barrelOffset.transform.localPosition = new Vector3(1.2f, .6f, 0f);
            projectile.baseData.damage = 8f;
            projectile.baseData.speed *= 1.3f;
            projectile.DefaultTintColor = UnityEngine.Color.black;
            projectile.HasDefaultTint = true;
            ImprovedAfterImage trail = projectile.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            trail.shadowLifetime = .2f;
            trail.shadowTimeDelay = .0001f;
            trail.dashColor = UnityEngine.Color.black;
            trail.spawnShadows = true;
            gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 2, StatModifier.ModifyMethod.ADDITIVE);         
            gun.shellsToLaunchOnFire = 0;

            gun.muzzleFlashEffects = VFXToolbox.CreateVFXPool("BlackBetty Muzzleflash",
                new List<string>()
                {
                    "Knives/Resources/Muzzleflashes/BlackBetty/BlackBetty_muzzleflash_001",
                    "Knives/Resources/Muzzleflashes/BlackBetty/BlackBetty_muzzleflash_002",
                    "Knives/Resources/Muzzleflashes/BlackBetty/BlackBetty_muzzleflash_003",
                    "Knives/Resources/Muzzleflashes/BlackBetty/BlackBetty_muzzleflash_004",
                    "Knives/Resources/Muzzleflashes/BlackBetty/BlackBetty_muzzleflash_005",
                },
                15, //FPS
                new IntVector2(27, 16), //Dimensions
                tk2dBaseSprite.Anchor.MiddleLeft, //Anchor
                false, //Uses a Z height off the ground
                0, //The Z height, if used
                false,
               VFXAlignment.Fixed
               );

           
            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }


        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_BlackBetty_fire_001", base.gameObject);


        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(OnHitEnemy));
            base.PostProcessProjectile(projectile);
        }

        private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2.aiActor != null)
            {
                if(arg2.aiActor.healthHaver != null)
                {
                    arg2.aiActor.healthHaver.AllDamageMultiplier = arg2.aiActor.healthHaver.AllDamageMultiplier * 1.05f;
                }
            }
        }

        private bool HasReloaded;
        //This block of code allows us to change the reload sounds.
        public override void Update()
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
                AkSoundEngine.PostEvent("Play_WPN_vertebraek47_reload_01", base.gameObject);


            }

        }

    }
}
