using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Collections.Generic;
using MultiplayerBasicExample;
using Alexandria.BreakableAPI;

namespace Knives
{
    class Bomislav : AdvancedGunBehaviour
    {

        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Bomislav", "Bomislav");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:bomislav", "ski:bomislav");
            gun.gameObject.AddComponent<Bomislav>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Fire In The Hole!");
            gun.SetLongDescription("A Proud Russian Firearm.\n" +
                "Favorite of the great soviet space colonies local miners, its ammo can be used for blasting out minerals or shooting down space pirates." +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "Bomislav_idle_001", 8);


            gun.SetAnimationFPS(gun.shootAnimation, 15);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 2;

            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 2f;
            gun.DefaultModule.numberOfShotsInClip = 200;
            gun.DefaultModule.cooldownTime = .25f;

            gun.SetBaseMaxAmmo(400);
            gun.quality = PickupObject.ItemQuality.B;
            gun.gunSwitchGroup = ((Gun)PickupObjectDatabase.GetById(53)).gunSwitchGroup;


            Gun shellingFlash = (Gun)PickupObjectDatabase.GetById(96);
            gun.muzzleFlashEffects = shellingFlash.muzzleFlashEffects;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(32 / 16f, 10 / 16f, 0f);
            gun.carryPixelOffset = new IntVector2(4, 0);

            gun.shellsToLaunchOnFire = 1;
            Gun gun2 = PickupObjectDatabase.GetById(84) as Gun;
            gun.shellCasing = gun2.shellCasing;
            Casing = gun2.shellCasing;

            gun.shellsToLaunchOnReload = 1;
            
            Mag = BreakableAPIToolbox.GenerateDebrisObject("Knives/Resources/Casings/Bomislav_clip.png", false, 15, 15, 0, 0, null, 3f, null, null, 1).gameObject;
            Mag.gameObject.GetOrAddComponent<ExplodeInFire>();

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 5f;
            projectile.baseData.speed *= 1f;
            projectile.baseData.range *= 1f;
            projectile.baseData.force = 3f;

            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;

            MiscToolMethods.TrimAllGunSprites(gun);
        }
        public static GameObject Casing;
        public static GameObject Mag;
        public static PlayerController owner;
        public static int ID;

        public override void OnPickedUpByPlayer(PlayerController player)
        {
            this.m_fireImmunity = new DamageTypeModifier();
            this.m_fireImmunity.damageMultiplier = 0f;
            this.m_fireImmunity.damageType = CoreDamageTypes.Fire;
            player.healthHaver.damageTypeModifiers.Add(this.m_fireImmunity);
            owner = player;
            base.OnPickedUpByPlayer(player);
        }
        public override void OnGunsChanged(Gun previous, Gun current, bool newGun)
        {

            base.OnGunsChanged(previous, current, newGun);
        }
        public override void OnDropped()
        {
            if(owner != null)
            {
                owner.healthHaver.damageTypeModifiers.Remove(this.m_fireImmunity);
            }
            
            base.OnDropped();
        }
        public override void OnPostFired(PlayerController player, Gun gun)
        {


            //gun.PreventNormalFireAudio = true;
        }
        public override void PostProcessProjectile(Projectile projectile)
        {

            base.PostProcessProjectile(projectile);
        }
        public bool HasReloaded = true;
        public bool doing = false;
        public override void Update()
        {
            if (this.gun.CurrentOwner)
            {

                if (!gun.PreventNormalFireAudio)
                {
                    //this.gun.PreventNormalFireAudio = true;

                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    
                }

                if (this.gun.IsReloading)
                {
                    gun.shellCasing = Mag;
                }
                else
                {
                    gun.shellCasing = Casing;

                }

            }

            base.Update();
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_yarirocketlauncher_reload_01", base.gameObject);
                player.StartCoroutine(BombAndFuse(player));
                gun.LoseAmmo(10);
            }

        }

        private IEnumerator BombAndFuse(PlayerController player)
        {
            GoopDefinition oil = EasyGoopDefinitions.OilDef;
            oil.lifespan = 10f;
            
            DeadlyDeadlyGoopManager goop = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(oil);
            goop.TimedAddGoopCircle(this.Owner.sprite.WorldBottomCenter, 2f, 3f, true);

            while (player.CurrentGun.IsReloading)
            {
                goop.TimedAddGoopCircle(this.Owner.sprite.WorldBottomCenter, .5f, 2.5f, true);
                
                yield return null;
            }

            goop.IgniteGoopCircle(this.Owner.sprite.WorldBottomCenter, 2);

        }

        public DamageTypeModifier m_fireImmunity;
    }
}