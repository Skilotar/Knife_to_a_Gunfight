using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Collections.Generic;
using MultiplayerBasicExample;

namespace Knives
{
    class Bison : AdvancedGunBehaviour
    {

        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Bizon 19", "Bizon19");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:bizon_19", "ski:bizon_19");
            gun.gameObject.AddComponent<Bison>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Seeing Red");
            gun.SetLongDescription("A Custom gun made for the infamous Buffammo. His hulking stature and unchecked rage were a threat to friend and foe alike. \n" +
                "He has yet to come pick it up from the gungeon blacksmith. Surely it can't hurt to make sure it works for him... Right? " +
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "Bizon19_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 26);
            gun.SetAnimationFPS(gun.reloadAnimation, 5);
            //gun.SetAnimationFPS(gun.criticalFireAnimation, 5);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 5;
            gun.gunClass = GunClass.FULLAUTO;
            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.5f;
            gun.DefaultModule.numberOfShotsInClip = 34;
            gun.DefaultModule.cooldownTime = .15f;
            gun.SetBaseMaxAmmo(1000);
            gun.ammo = 1000;
            gun.quality = PickupObject.ItemQuality.B;
            gun.carryPixelOffset = new IntVector2(5, 0);
            
            gun.PreventNormalFireAudio = true;
            //gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 2, StatModifier.ModifyMethod.ADDITIVE);
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(2.2f, .4f, 0f);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 3f;
            projectile.baseData.speed *= 1.5f;
            projectile.baseData.range *= 1f;
            

            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            gun.DefaultModule.usesOptionalFinalProjectile = true;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.finalProjectile = projectile2;
            gun.DefaultModule.numberOfFinalProjectiles = 1;
            projectile2.baseData.damage = 5f;
            projectile2.baseData.speed *= .5f;
            projectile2.baseData.range *= 1f;
            projectile2.SetProjectileSpriteRight("Bison", 31, 19, false, tk2dBaseSprite.Anchor.MiddleCenter, 31, 19);
            projectile2.shouldFlipVertically = true;
            projectile2.shouldRotate = true;
            projectile2.gameObject.GetOrAddComponent<BulletPopOnDie>();
            ImprovedAfterImage trail = projectile2.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            trail.shadowLifetime = .2f;
            trail.shadowTimeDelay = .001f;
            trail.dashColor = ExtendedColours.gildedBulletsGold;
            trail.spawnShadows = true;

            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        public override void OnPickedUpByPlayer(PlayerController player)
        {
            player.GunChanged += this.OnGunChanged;
            base.OnPickedUpByPlayer(player);
        }

        public System.Random rng = new System.Random();

        public override void OnPostFired(PlayerController player, Gun gun)
        {

           

        }
        public override void OnFinishAttack(PlayerController player, Gun gun)
        {


            base.OnFinishAttack(player, gun);
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            AkSoundEngine.PostEvent("Play_WPN_spma_shot_01", base.gameObject);
            if (gun.ClipShotsRemaining == 1) AkSoundEngine.PostEvent("Play_Bizon_finalshot", base.gameObject);



            base.PostProcessProjectile(projectile);
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            this.HasReloaded = false;
            if(gun.ClipShotsRemaining < gun.ClipCapacity)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_rpg_reload_01", base.gameObject);

            }

        }

        public override void OnPostDrop(GameActor owner)
        {
            PlayerController player = (PlayerController)owner;
            player.GunChanged -= this.OnGunChanged;
        }
        private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
        {
           
        }

        private bool HasReloaded;
        public override void Update()
        {
            if (gun.CurrentOwner && GameManager.Instance.CurrentFloor != 0 && GameManager.Instance.IsLoadingLevel == false)
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

        
    }
}

