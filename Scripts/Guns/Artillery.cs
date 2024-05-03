using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using ItemAPI;
using Gungeon;
using System.Collections;
using HutongGames.PlayMaker.Actions;
using Dungeonator;
using Knives;

namespace Knives
{
    class Artillery_Revolver : MultiActiveReloadController
    {
       
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Artillery Revolver", "revolver_cannon");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:artillery_revolver", "ski:artillery_revolver");
            var Behav = gun.gameObject.AddComponent<Artillery_Revolver>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Overloading");
            gun.SetLongDescription("Active reload to double clipsize.\n\n" +
                "A one of a kind revolver for a one of a kind man. For most this hulking mound of metal is too much to wield effectively, but to Erik the Great, it fit his hands perfectly. " +
                "Erik towered over the battlefields of the frontier wars, his titanous size and artillery shell revolver could kill armies with panic alone." +
                "\n\n\n - Knife_to_a_Gunfight\n\n" +
                "Sprites by Lynceus");
            gun.SetupSprite(null, "revolver_cannon_idle_001", 1);
            gun.SetAnimationFPS(gun.shootAnimation, 6);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);


            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.5f;
            gun.DefaultModule.cooldownTime = .8f;
            gun.DefaultModule.numberOfShotsInClip = 4;
            gun.SetBaseMaxAmmo(300);
            
            gun.quality = PickupObject.ItemQuality.B;
            gun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed, -.3f, StatModifier.ModifyMethod.ADDITIVE);

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0].projectile);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.speed *= 1.3f;
            projectile.baseData.damage = 25f;
            projectile.AppliesKnockbackToPlayer = true;
            projectile.PlayerKnockbackForce = 15;
            PierceProjModifier poke = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            poke.penetratesBreakables = true;
            poke.penetration = 2;

            ImprovedAfterImage trail = projectile.gameObject.GetOrAddComponent<ImprovedAfterImage>();
           
            trail.shadowLifetime = .2f;
            trail.shadowTimeDelay = .001f;
            trail.dashColor = ExtendedColours.vibrantOrange;
            trail.spawnShadows = true;

            projectile.SetProjectileSpriteRight("BIG_projectile_001", 23, 16, false, tk2dBaseSprite.Anchor.MiddleCenter, 23, 16);
            projectile.shouldFlipVertically = true;
            projectile.shouldRotate = true;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "kirkcannon";


            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(157) as Gun).muzzleFlashEffects;
            gun.quality = PickupObject.ItemQuality.B;
            gun.gunClass = GunClass.PISTOL;
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(393) as Gun).gunSwitchGroup;
            gun.carryPixelOffset = new IntVector2(5,0);
            gun.barrelOffset.transform.localPosition = new Vector3(3f, 1.2f, 0f);
            gun.CanReloadNoMatterAmmo = true;
            Behav.activeReloadEnabled = true;
            Behav.canAttemptActiveReload = true;

            Behav.reloads = new List<MultiActiveReloadData>
            {
                new MultiActiveReloadData(0.55f, 60, 90, 32, 20, false, false, new ActiveReloadData
                {
                    damageMultiply = 1f,
                }, true, "OverLoad",6,2)

            };

            MiscToolMethods.TrimAllGunSprites(gun);
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            ID = gun.PickupObjectId;
        }

       
        public static int ID;

        public override void OnPickedUpByPlayer(PlayerController player)
        {

            base.OnPickedUpByPlayer(player);
        }
        public override void OnSwitchedAwayFromThisGun()
        {
            UpgradedClip = 0;
            gun.DefaultModule.numberOfShotsInClip = lastKnownNumberofShots;
            
            base.OnSwitchedAwayFromThisGun();
        }
        int lastKnownNumberofShots = 4;
        int UpgradedClip = 0;
        public override void OnActiveReloadSuccess(MultiActiveReload reload)
        {
            base.OnActiveReloadSuccess(reload);
            if (reload.Name == "OverLoad")
            {
                AkSoundEngine.PostEvent("Play_WPN_rpg_reload_01", base.gameObject);
                lastKnownNumberofShots = gun.DefaultModule.numberOfShotsInClip;
                UpgradedClip = lastKnownNumberofShots * 2;
                
            }
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
           

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

                if (UpgradedClip > 0)
                {
                    gun.DefaultModule.numberOfShotsInClip = UpgradedClip;
                }
                else
                {
                    gun.DefaultModule.numberOfShotsInClip = lastKnownNumberofShots;
                }

            }

            base.Update();
        }
       
       
        public override void OnReloadPressed(PlayerController player, Gun gun, bool manualReload)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                AkSoundEngine.PostEvent("Play_WPN_yarirocketlauncher_reload_01", base.gameObject);
                HasReloaded = false;
                base.OnReload(player, gun);
                gun.DefaultModule.numberOfShotsInClip = lastKnownNumberofShots;
                UpgradedClip = 0;
            }

        }

    }

}



