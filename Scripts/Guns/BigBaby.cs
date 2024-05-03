using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;


namespace Knives
{

    public class BigBaby : AdvancedGunBehaviour
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Big Baby", "BigBaby");
            Game.Items.Rename("outdated_gun_mods:big_baby", "ski:big_baby");
            gun.gameObject.AddComponent<BigBaby>();
            gun.SetShortDescription("You Woke Up The Baby");
            gun.SetLongDescription("A massive firey laser shotgun used by an even massive-er man. Fires two shots at once." +
                "\n\n\n - Knife_to_a_Gunfight");

            gun.SetupSprite(null, "BigBaby_idle_001", 8);
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(51) as Gun).gunSwitchGroup;
            gun.SetAnimationFPS(gun.shootAnimation, 12);
            gun.SetAnimationFPS(gun.reloadAnimation, 10);
            gun.SetAnimationFPS(gun.idleAnimation, 5);
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            for (int i = 0; i < 2; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(81) as Gun, true, false);
            }
            
            //GUN STATS
            foreach (ProjectileModule mod in gun.Volley.projectiles)
            {

                mod.ammoCost = 2;
                mod.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
                mod.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                mod.cooldownTime = .9f;
                mod.angleVariance = 0f;
                mod.numberOfShotsInClip = 3;
                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(mod.projectiles[0]);
                mod.projectiles[0] = projectile;
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                projectile.baseData.damage = 3f;
                projectile.baseData.speed *= 2f;
                projectile.shouldRotate = true;
                projectile.SetProjectileSpriteRight("Baby", 15, 9, false, tk2dBaseSprite.Anchor.MiddleCenter, 17, 11);
                

                mod.positionOffset = new Vector2(0, -0.3f);
                if (mod != gun.DefaultModule)
                {
                    mod.positionOffset = new Vector2(0, 0.2f);
                    mod.ammoCost = 0;
                }
                projectile.transform.parent = gun.barrelOffset;
            }

            gun.reloadTime = 1.4f;
            gun.barrelOffset.transform.position = new Vector3(1.5f, 1f, 0f);
            gun.SetBaseMaxAmmo(60);
            gun.ammo = 60;
            gun.gunClass = GunClass.SHOTGUN;
            gun.quality = PickupObject.ItemQuality.B;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Suck_On_This", "Knives/Resources/Baby_clipfull", "Knives/Resources/Baby_clipempty");

            ID = gun.PickupObjectId;
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            
            base.PostProcessProjectile(projectile);
        }
        public static int ID;
        
    }
}
