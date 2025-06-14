using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;

using MultiplayerBasicExample;
using System.Collections.Generic;
using Alexandria.ItemAPI;

namespace Knives
{

    public class BioticRifle : AdvancedGunBehavior
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Biotic Rifle", "Biotic");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:biotic_rifle", "ski:biotic_rifle");
            gun.gameObject.AddComponent<BioticRifle>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Hush Little Baby");
            gun.SetLongDescription("Final shot fires a sleep dart. Sleeping enemies will be inactive until they take damage or wake up naturally.\n\n" +
                "The rifle of Captain Amari. Her methods may be unorthodox in somes eyes, but no one argues with the results." +
                "\n\n\n - Knife_to_a_Gunfight");
            gun.SetupSprite(null, "Biotic_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 8);
            gun.SetAnimationFPS(gun.finalShootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(124) as Gun, true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.3f;
            gun.DefaultModule.cooldownTime = .8f;
            gun.DefaultModule.numberOfShotsInClip = 7;
            gun.SetBaseMaxAmmo(450);
            gun.carryPixelOffset = new IntVector2(8, 0);
            gun.barrelOffset.transform.position = new Vector3(36 / 16f, 7 / 16f, 0);
            gun.quality = PickupObject.ItemQuality.B;


            gun.muzzleFlashEffects = ((Gun)PickupObjectDatabase.GetById(26)).muzzleFlashEffects;
            gun.finalMuzzleFlashEffects = ((Gun)PickupObjectDatabase.GetById(539)).muzzleFlashEffects;


            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(((Gun)PickupObjectDatabase.GetById(124)).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 15f;
            projectile.baseData.speed *= 2f;
            projectile.baseData.range = 50;
            projectile.AdditionalScaleMultiplier *= .7f;
            projectile.HasDefaultTint = true;
            projectile.DefaultTintColor = ExtendedColours.honeyYellow;


            gun.DefaultModule.finalAmmoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.finalCustomAmmoType = "ghost_small";

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.SMALL_BULLET;
           
            
            ImprovedAfterImage trail = projectile.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            trail.shadowLifetime = .2f;
            trail.shadowTimeDelay = .00001f;
            trail.dashColor = ExtendedColours.honeyYellow;
            trail.spawnShadows = true;

            Projectile fin = UnityEngine.Object.Instantiate<Projectile>(((Gun)PickupObjectDatabase.GetById(124)).DefaultModule.projectiles[0]);
            fin.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(fin.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(fin);
            fin.baseData.damage = 0f;
            fin.baseData.speed *= 1f;
            fin.baseData.range = 40;
            fin.SuppressHitEffects = true;
            fin.hitEffects.suppressMidairDeathVfx = true;
            sleepdart = fin;
            projectileStates state = fin.gameObject.GetOrAddComponent<projectileStates>();
            state.sleepy = true;

            projectileStates state2 = projectile.gameObject.GetOrAddComponent<projectileStates>();
            state2.biotic = true;
            BioticProjMod bio = projectile.gameObject.GetOrAddComponent<BioticProjMod>();
            

            ImprovedAfterImage trail2 = fin.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            trail2.shadowLifetime = .2f;
            trail2.shadowTimeDelay = .00001f;
            trail2.dashColor = UnityEngine.Color.white;
            trail2.spawnShadows = true;

            gun.gunSwitchGroup = ((Gun)PickupObjectDatabase.GetById(124)).gunSwitchGroup;
            gun.DefaultModule.finalProjectile = fin;
            gun.DefaultModule.numberOfFinalProjectiles = 1;
            gun.DefaultModule.usesOptionalFinalProjectile = true;

            MiscToolMethods.TrimAllGunSprites(gun);
            gun.gunClass = GunClass.RIFLE;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;
        }
        public static Projectile sleepdart;
        public static int ID;

        public override void PostProcessProjectile(Projectile projectile)
        {

            if (projectile.gameObject.GetComponent<projectileStates>())
            {
                if (projectile.gameObject.GetComponent<projectileStates>().sleepy)
                {
                    StartCoroutine(slightdelay());
                    projectile.hitEffects.suppressMidairDeathVfx = true;
                    projectile.DieInAir();


                }
                if (projectile.gameObject.GetComponent<projectileStates>().biotic)
                {
                    AkSoundEngine.PostEvent("Play_WPN_e12_shot_01", projectile.gameObject);
                    AkSoundEngine.PostEvent("Play_WPN_dartgun_shot_01", projectile.gameObject);

                    if ((this.Owner as PlayerController).PlayerHasActiveSynergy("Hush Big Baby"))
                    {
                        if (UnityEngine.Random.Range(0, 10f) < 1)
                        {
                            projectile.OnHitEnemy += Boom;
                        }
                    }
                } 
            }
            this.gun.DefaultModule.numberOfFinalProjectiles = 1;
            base.PostProcessProjectile(projectile);
        }

        private void Boom(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            DoSafeExplosion(arg1.LastPosition);
        }
        public void DoSafeExplosion(Vector3 position)
        {

            ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
            this.smallPlayerSafeExplosion.effect = defaultSmallExplosionData2.effect;
            this.smallPlayerSafeExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
            this.smallPlayerSafeExplosion.ss = defaultSmallExplosionData2.ss;
            Exploder.Explode(position, this.smallPlayerSafeExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);

        }

        private ExplosionData smallPlayerSafeExplosion = new ExplosionData
        {
            damageRadius = 4f,
            damageToPlayer = 0f,
            doDamage = true,
            damage = 10f,
            doDestroyProjectiles = false,
            doForce = false,
            debrisForce = 10f,
            preventPlayerForce = true,
            explosionDelay = 0.1f,
            usesComprehensiveDelay = false,
            doScreenShake = false,
            playDefaultSFX = true,
            breakSecretWalls = false,
        };

        private IEnumerator slightdelay()
        {
            while(gun.spriteAnimator.CurrentFrame != 3 && gun.IsCurrentGun())
            {
                yield return null;
            }

            Projectile projectile = MiscToolMethods.SpawnProjAtPosi(BioticRifle.sleepdart, gun.barrelOffset.transform.position, gun.CurrentOwner as PlayerController, gun);
            ((Gun)PickupObjectDatabase.GetById(385)).muzzleFlashEffects.SpawnAtPosition(gun.barrelOffset.transform.position, gun.CurrentAngle);
            AkSoundEngine.PostEvent("Play_WPN_nailgun_shot_01", projectile.gameObject);
            AkSoundEngine.PostEvent("Play_WPN_nailgun_shot_01", projectile.gameObject);
            AkSoundEngine.PostEvent("Play_WPN_e12_shot_01", projectile.gameObject);
            projectile.OnHitEnemy += SleepyStatus;

            yield return null;
        }

        private void SleepyStatus(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if(arg2.aiActor != null)
            {
                SleepStatus status =  arg2.aiActor.gameObject.GetOrAddComponent<SleepStatus>();
                status.statused = true;
                status.duration = 6;
            }
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {

        }

        private bool HasReloaded;
        public bool doingCo;

        public PlayerController LastKnownOwner;
        public bool Startup;

        protected override void Update()
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

                if (Startup == false)
                {
                    PlayerController player = gun.CurrentOwner as PlayerController;
                    player.inventory.OnGunChanged += Inventory_OnGunChanged;
                    
                    LastKnownOwner = player;
                    Startup = true;
                }

                if (((gun.CurrentOwner) as PlayerController).PlayerHasActiveSynergy("Eye Of Horus"))
                {
                    PlayerController player = this.gun.CurrentOwner as PlayerController;
                    if (player.IsDodgeRolling)
                    {
                        if(doingCo == false)
                        {
                            StartCoroutine(NextShotIsSleep(player));
                        }
                    }
                    else
                    {
                        doingCo = false;
                    }
                }

            }

        }
        public override void OnDropped()
        {
            LastKnownOwner.inventory.OnGunChanged -= Inventory_OnGunChanged;
            gun.StopAllCoroutines();
            doingCo = false;
            this.gun.DefaultModule.numberOfFinalProjectiles = 1;
            Startup = false;
            base.OnDropped();
        }

        private void Inventory_OnGunChanged(Gun previous, Gun current, Gun previousSecondary, Gun currentSecondary, bool newGun)
        {
            gun.StopAllCoroutines();
            doingCo = false;
            gun.DefaultModule.numberOfFinalProjectiles = 1;
        }

        private IEnumerator NextShotIsSleep(PlayerController player)
        {
            doingCo = true;
            this.gun.DefaultModule.numberOfFinalProjectiles = gun.DefaultModule.numberOfShotsInClip;
            while (player.IsDodgeRolling)
            {
                yield return null;
            }
            yield return new WaitForSeconds(.5f);
            this.gun.DefaultModule.numberOfFinalProjectiles = 1;

            doingCo = false;
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_dartgun_reload_01", base.gameObject);
                


            }



        }

    }
}