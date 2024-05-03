using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;

namespace Knives
{

    public class Followup : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Follow_Up", "Followup");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:follow_up", "ski:follow_up");
            gun.gameObject.AddComponent<Followup>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Pay It Forward");
            gun.SetLongDescription("Instead of Fixing the terrible spread on his firearm. Wigfred, added microscopic technology to each bullet to impulse targets towards the next bullet in the burst." +
                
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "Followup_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 8);
            gun.SetAnimationFPS(gun.reloadAnimation, 4);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(79) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 14f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Burst;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.DefaultModule.burstCooldownTime = .12f;
            gun.DefaultModule.burstShotCount = 5;
            gun.reloadTime = 1f;
            gun.DefaultModule.cooldownTime = .4f;
            gun.DefaultModule.numberOfShotsInClip = 15;
            gun.SetBaseMaxAmmo(500);
            gun.quality = PickupObject.ItemQuality.C;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(199) as Gun).muzzleFlashEffects;

            
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.SetProjectileSpriteRight("pointer", 16, 13, false, tk2dBaseSprite.Anchor.LowerCenter, 16, 13);
            projectile.AdditionalScaleMultiplier = .5f;
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 9;
            projectile.baseData.force = 0;
            projectile.baseData.speed *= .6f;
            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            ID = gun.PickupObjectId;
        }
        
        public static int ID;


        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_shot_01", base.gameObject);

        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            PlayerController player = (PlayerController)gun.CurrentOwner;
            if (player.PlayerHasActiveSynergy("Plasma Spray"))
            {
                projectile.baseData.damage *= 1.5f;
                ChainLightningModifier chain = projectile.gameObject.GetOrAddComponent<ChainLightningModifier>();
                chain.maximumLinkDistance = 12f;
                chain.RequiresSameProjectileClass = true;
                chain.LinkVFXPrefab = (PickupObjectDatabase.GetById(298) as ComplexProjectileModifier).ChainLightningVFX;
            }

            projectile.gameObject.GetOrAddComponent<FollowProjMod>();
            base.PostProcessProjectile(projectile);
        }

        private bool HasReloaded;
        
        public override void  Update()
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

                if (player.PlayerHasActiveSynergy("Plasma Spray"))
                {
                    gun.DefaultModule.angleVariance = 32f;
                }
                else
                {
                    gun.DefaultModule.angleVariance = 14;
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
                AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_reload", base.gameObject);


            }

        }

    }
}
