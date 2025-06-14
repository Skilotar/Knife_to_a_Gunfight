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

    public class Plustle : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Plustle", "plustle");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:plustle", "ski:plustle");
            gun.gameObject.AddComponent<Plustle>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Plus");
            gun.SetLongDescription("Positive Charge, Yearns for Companionship.\n\n" +
                "A small proton blaster developed by cleo of sylph co. This blaster was inspired by her twin sister and their strong bond.\n" +
                "When joined together they are greater than their sum." +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "plustle_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 8);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(79) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 10f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1f;
            gun.DefaultModule.cooldownTime = .3f;
            gun.DefaultModule.numberOfShotsInClip = 10;
            gun.SetBaseMaxAmmo(250);
            gun.barrelOffset.transform.position = new Vector3(24 / 16f, 9 / 16f, 0);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.C;

            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(36) as Gun).muzzleFlashEffects;

            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage = 7f;
            projectile.baseData.speed *= .8f;

            BounceProjModifier bnc = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
            bnc.numberOfBounces = 1;

            MiscToolMethods.EasyAnimate(projectile, new List<string>() {
                "Plus_001",
                "Plus_002",
                
            }, 4, new IntVector2(15, 15), 6, false, tk2dSpriteAnimationClip.WrapMode.Loop);


            projectile.transform.parent = gun.barrelOffset;
            projectile.gameObject.GetOrAddComponent<projectileStates>().PlusMinus = true;
            projectile.gameObject.GetOrAddComponent<projectileStates>().Married = false;
            gun.gunClass = GunClass.PISTOL;
            gun.carryPixelOffset = new IntVector2(5, 0);
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            ID = gun.PickupObjectId;

            mod = new LootModData
            {
                AssociatedPickupId = ID,
                DropRateMultiplier = 15,
            };
        }

        public static LootModData mod;
        public static int ID;

        public override void OnPostFired(PlayerController player, Gun gun)
        {
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            AkSoundEngine.PostEvent("Play_Beew_001", base.gameObject);
            base.PostProcessProjectile(projectile);
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
                PlayerController player = gun.CurrentOwner as PlayerController;
                if (player.PlayerHasActiveSynergy("Better Together"))
                {
                    gun.DefaultModule.angleFromAim = 7;
                    gun.DefaultModule.angleVariance = 0;
                }
                else
                {
                    gun.DefaultModule.angleFromAim = 0;
                    gun.DefaultModule.angleVariance = 10;
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
                AkSoundEngine.PostEvent("Play_WPN_screechgun_reload_01", base.gameObject);
            }

        }

    }
}
