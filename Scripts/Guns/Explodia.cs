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

    public class Explodia : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Explodia", "Explodia");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:explodia", "ski:explodia");
            gun.gameObject.AddComponent<Explodia>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Obliterate!");
            gun.SetLongDescription("A great beast ravenous and filled with need for vengance. Slaughter them all! Show Kaliber her time has come! " +
                "\nThe world will look on in awe as it combusts into Glorious Stardust!!" +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "Explodia_idle_001", 2);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 17);
            
            
            gun.SetAnimationFPS(gun.reloadAnimation, 9);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(81) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 10f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.5f;
            gun.DefaultModule.cooldownTime = .3f;
            gun.DefaultModule.numberOfShotsInClip = 25;
            gun.SetBaseMaxAmmo(555);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.EXCLUDED;

            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            gun.barrelOffset.transform.localPosition = new Vector3(2.25f, 1f, 0f);
            ItemAPI.FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.AdditionalScaleMultiplier *= .6f;
            projectile.baseData.damage = 2f;
            projectile.baseData.speed *= 1.3f;
            projectile.PlayerKnockbackForce = 3f;
            projectile.gameObject.GetOrAddComponent<projectileStates>().Explodia = true;
            gun.carryPixelOffset = new IntVector2(4, 0);
            gun.shellsToLaunchOnFire = 1;
            Gun gun2 = PickupObjectDatabase.GetById(84) as Gun;
            gun.shellCasing = gun2.shellCasing;
            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.FULLAUTO;
           
            gun.AddPassiveStatModifier(PlayerStats.StatType.Accuracy, -.15f, StatModifier.ModifyMethod.ADDITIVE);
            gun.AddPassiveStatModifier(PlayerStats.StatType.Damage, .15f, StatModifier.ModifyMethod.ADDITIVE);
            gun.AddPassiveStatModifier(PlayerStats.StatType.MovementSpeed, .15f, StatModifier.ModifyMethod.ADDITIVE);
            gun.AddPassiveStatModifier(PlayerStats.StatType.RateOfFire, .15f, StatModifier.ModifyMethod.ADDITIVE);
            gun.AddPassiveStatModifier(PlayerStats.StatType.AdditionalClipCapacityMultiplier, .25f, StatModifier.ModifyMethod.ADDITIVE);


            ImprovedAfterImage trail = projectile.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            trail.shadowLifetime = .3f;
            trail.shadowTimeDelay = .001f;
            trail.dashColor = new Color(1.0f, .69f, .0f); 
            trail.spawnShadows = true;

            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;



        }


        public static int ID;
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_WPN_deck4rd_shot_01", base.gameObject); 
            AkSoundEngine.PostEvent("Play_WPN_deserteagle_shot_01", base.gameObject);
            CameraController camera = GameManager.Instance.MainCameraController;
            camera.DoScreenShake(shake, player.CenterPosition);
        }
        public ScreenShakeSettings shake = new ScreenShakeSettings
        {
            magnitude = .7f,
            speed = 2f,
            time = 0f,
            falloff = .1f,
            direction = new Vector2(0, 0),
            vibrationType = ScreenShakeSettings.VibrationType.Auto,
            simpleVibrationStrength = Vibration.Strength.Medium,
            simpleVibrationTime = Vibration.Time.Instant
        };

        public override void PostProcessProjectile(Projectile sourceProjectile)
        {
            
            if (sourceProjectile.gameObject.GetOrAddComponent<projectileStates>().Explodia == true)
            {
                sourceProjectile.OnHitEnemy += this.OnHitEnemy;
                if (sourceProjectile is InstantDamageOneEnemyProjectile != true)
                {
                    
                    if (sourceProjectile is InstantlyDamageAllProjectile != true)
                    {
                        
                        if (sourceProjectile.GetComponent<ArtfulDodgerProjectileController>() != true)
                        {
                            PlayerController playerController = sourceProjectile.ProjectilePlayerOwner();
                            Projectile proj = ((Gun)PickupObjectDatabase.GetById(15)).DefaultModule.projectiles[0].projectile;

                            for (int i = 0; i < 2; i++)
                            {
                               
                                GameObject gameObject = SpawnManager.SpawnProjectile(proj.gameObject, sourceProjectile.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (base.Owner.CurrentGun == null) ? 0f : base.Owner.CurrentGun.CurrentAngle), true);
                                Projectile component = gameObject.GetComponent<Projectile>();
                                bool flag6 = component != null;
                                if (flag6)
                                {
                                    if (i == 1)
                                    {
                                        component.baseData.speed *= -1;
                                    }
                                    component.Owner = playerController;
                                    component.Shooter = playerController.specRigidbody;
                                    component.baseData.damage *= playerController.stats.GetStatValue(PlayerStats.StatType.Damage);
                                    component.baseData.speed *= playerController.stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed);
                                    component.baseData.force *= playerController.stats.GetStatValue(PlayerStats.StatType.KnockbackMultiplier);
                                    component.UpdateSpeed();
                                    component.specRigidbody.CollideWithTileMap = true;
                                    BulletLifeTimer orAddComponent = component.gameObject.GetOrAddComponent<BulletLifeTimer>();
                                    orAddComponent.secondsTillDeath = 4f;
                                    OrbitProjectileMotionModule orbitProjectileMotionModule = new OrbitProjectileMotionModule();
                                    orbitProjectileMotionModule.lifespan = 50f;
                                    orbitProjectileMotionModule.MinRadius = 1f;
                                    orbitProjectileMotionModule.MaxRadius = 1f;
                                    orbitProjectileMotionModule.usesAlternateOrbitTarget = true;
                                    orbitProjectileMotionModule.OrbitGroup = -555;
                                    orbitProjectileMotionModule.alternateOrbitTarget = sourceProjectile.specRigidbody;
                                    bool flag7 = component.OverrideMotionModule != null && component.OverrideMotionModule is HelixProjectileMotionModule;
                                    if (flag7)
                                    {
                                        orbitProjectileMotionModule.StackHelix = true;
                                        orbitProjectileMotionModule.ForceInvert = (component.OverrideMotionModule as HelixProjectileMotionModule).ForceInvert;
                                    }
                                    component.OverrideMotionModule = orbitProjectileMotionModule;
                                    component.OnHitEnemy += this.OnHitEnemy;
                                    playerController.DoPostProcessProjectile(component);

                                    ImprovedAfterImage trail = component.gameObject.GetOrAddComponent<ImprovedAfterImage>();
                                    trail.shadowLifetime = .2f;
                                    trail.shadowTimeDelay = .01f;
                                    trail.dashColor = new Color(1.0f, .69f, .0f);
                                    trail.spawnShadows = true;

                                    ChildProjCleanup clean = component.gameObject.GetOrAddComponent<ChildProjCleanup>();
                                    clean.parentProjectile = sourceProjectile;
                                    clean.doColor = false;
                                }
                            }
                            
                        }
                    }
                }
            }
            base.PostProcessProjectile(projectile);
        }

        private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if(arg2.aiActor != null)
            {
                BlastBlightedStatusController status = arg2.aiActor.gameObject.GetOrAddComponent<BlastBlightedStatusController>();
                status.statused = true;
            }
        }

        private bool HasReloaded;
        
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
                AkSoundEngine.PostEvent("Play_WPN_yarirocketlauncher_reload_01", base.gameObject);
                

            }

        }


    }
}


