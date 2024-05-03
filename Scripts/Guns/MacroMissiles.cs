using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;

namespace Knives
{

    public class MacroMissiles : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Macro Missiles", "Macro");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:macro_missiles", "ski:macro_missiles");
            gun.gameObject.AddComponent<MacroMissiles>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Full Send");
            gun.SetLongDescription("The further they fly the stronger they are.\n\n" +
                "A marvel of compression technology this launcher manages to store more missiles than ever before! " +
                "Just add water and BOOM your salvo is fully loaded." +

                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "Macro_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 18);
            gun.SetAnimationFPS(gun.reloadAnimation, 7);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(16) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 14f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Burst;
            
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.DefaultModule.burstCooldownTime = .1f;
            gun.DefaultModule.burstShotCount = 20;
            gun.reloadTime = 1f;
            gun.DefaultModule.cooldownTime = .4f;
            gun.DefaultModule.numberOfShotsInClip = 10;
            gun.SetBaseMaxAmmo(350);
            gun.quality = PickupObject.ItemQuality.B;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(16) as Gun).muzzleFlashEffects;

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.barrelOffset.transform.position = new Vector3( 24/ 16f, 9/ 16f,0/16f);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 1f;
            projectile.baseData.force = 0;
            projectile.baseData.speed *= .3f;
            projectile.BossDamageMultiplier *= .1f;
            
            projectile.DefaultTintColor = ExtendedColours.lime;
            projectile.HasDefaultTint = true;

            ExplosiveModifier boom = projectile.gameObject.GetOrAddComponent<ExplosiveModifier>();
            boom.explosionData.damage = 5f;
            
            ReverseSpeedRampMod ram = projectile.gameObject.GetOrAddComponent<ReverseSpeedRampMod>();
            ram.doeffects = false;
            ram.isGun = true;

            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.EXPLOSIVE;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            ID = gun.PickupObjectId;
        }

        public static int ID;


        public override void OnPostFired(PlayerController player, Gun gun)
        {
            
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            AkSoundEngine.PostEvent("Play_BOSS_RatMech_Missile_01", base.gameObject);
            base.PostProcessProjectile(projectile);
        }

        private bool HasReloaded;

        public override void Update()
        {
            if (gun.CurrentOwner)
            {
                PlayerController player = (PlayerController)gun.CurrentOwner;
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
                AkSoundEngine.PostEvent("Play_WPN_yarirocketlauncher_reload_01",base.gameObject);
            }

        }

    }
}
