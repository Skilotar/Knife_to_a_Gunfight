using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Gungeon;

namespace Knives
{ 
    class Ball : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Gyro Ball", "ball");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:gyro_ball", "ski:gyro_ball");
            gun.gameObject.AddComponent<Ball>();
            
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("High Speed Justice");
            gun.SetLongDescription("Putting just the right spin on this ball causes it to move in all sorts of weird ways.\n\n" +
                "___________________________________________\n\n" +
                "This ball will go where ever your cursor points it. Ball doubles in speed and damage for every redirection." +
                "\n\n\n - Knife_to_a_Gunfight");
            gun.SetupSprite(null, "ball_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 24);
            gun.SetAnimationFPS(gun.reloadAnimation, 24);

            gun.AddProjectileModuleFrom("Camera_Gun", true, true);
            gun.PreventNormalFireAudio = true;
            
            gun.gunHandedness = GunHandedness.HiddenOneHanded;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            Gun gun3 = (Gun)ETGMod.Databases.Items["wonderboy"];
            gun.muzzleFlashEffects = gun3.muzzleFlashEffects;
            gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1, StatModifier.ModifyMethod.ADDITIVE);
            gun.reloadTime = .5f;
            gun.DefaultModule.cooldownTime = .4f;
            gun.DefaultModule.numberOfShotsInClip = 6;
            gun.CurrentAmmo = 2000;
            gun.SetBaseMaxAmmo(400);
            gun.quality = PickupObject.ItemQuality.S;
                        
            gun.encounterTrackable.EncounterGuid = "Spiiiiin";
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
           
            projectile.baseData.damage = 0f;
            projectile.baseData.speed = 100000f;
            projectile.baseData.range = 0f;
            projectile.AdditionalScaleMultiplier = .00001f;
            gun.barrelOffset.transform.localPosition = new Vector3(-.30f,0,0);
            projectile.transform.parent = gun.barrelOffset;
            



            ETGMod.Databases.Items.Add(gun, null, "ANY");
            
        }

        public Projectile current_ball;
        public int shotcountcontroller = 1;
        public Vector2 targetPoint;


        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
            gun.PreventNormalFireAudio = true;
            AkSoundEngine.PostEvent("Play_BOSS_agunim_move_01", base.gameObject);
            targetPoint = player.unadjustedAimPoint;
            if (shotcountcontroller == 1)
            {
                Projectile projectile1 = ((Gun)ETGMod.Databases.Items[9]).DefaultModule.projectiles[0];
                GameObject gameObject1 = SpawnManager.SpawnProjectile(projectile1.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
                Projectile component1 = gameObject1.GetComponent<Projectile>();
                BounceProjModifier bounce = component1.GetComponent<BounceProjModifier>();
                bounce.chanceToDieOnBounce = 0;
                bounce.numberOfBounces = 6;
                PierceProjModifier stab = component1.GetComponent<PierceProjModifier>();
                stab.MaxBossImpacts = 4;
                stab.penetration = 6;
                

                bool flag = component1 != null;
                if (flag)
                {
                    component1.Owner = player;
                    component1.Shooter = player.specRigidbody;
                    component1.baseData.speed = 10f;
                    component1.baseData.range *= 1f;
                    component1.baseData.damage = 12f;
                    component1.angularVelocity = 1000f;
                    component1.projectileHitHealth = 50;
                    component1.collidesWithProjectiles = true;
                    component1.AdditionalScaleMultiplier = 1.2f;
                    component1.pierceMinorBreakables = true;
                    component1.HasDefaultTint = true;
                    component1.DefaultTintColor = UnityEngine.Color.cyan;
                    if (UnityEngine.Random.Range(1, 11) == 1)
                    {
                        player.DoPostProcessProjectile(component1);
                    }

                }
                current_ball = component1;
                shotcountcontroller = 0;
                Material outline = SpriteOutlineManager.GetOutlineMaterial(gun.sprite);
                outline.SetColor("_OverrideColor", new Color(63f, 236f, 165f));

            }
            else
            {
                if(current_ball != null)
                {
                    Vector2 vector = current_ball.sprite.WorldCenter;
                    Vector2 Aim = player.unadjustedAimPoint;
                    current_ball.SendInDirection(Aim - vector, false, true);

                    foreach (var projectile in GetBullets())
                    {
                        projectile.baseData.speed = projectile.baseData.speed + 5;
                        projectile.baseData.damage = projectile.baseData.damage * 1.75f;
                        projectile.angularVelocity = projectile.angularVelocity * -1;
                        projectile.Update();
                        projectile.UpdateSpeed();
                    }

                }
                else
                {
                    shotcountcontroller = 1;
                }
            }
        }

        private List<Projectile> GetBullets()
        {
            PlayerController player = this.gun.CurrentOwner as PlayerController;
            List<Projectile> list = new List<Projectile>();
            var allProjectiles = StaticReferenceManager.AllProjectiles;
            for (int i = 0; i < allProjectiles.Count; i++)
            {
                Projectile projectile = allProjectiles[i];
                if (projectile && projectile.sprite && !projectile.ImmuneToBlanks && !projectile.ImmuneToSustainedBlanks)
                {
                    if (projectile.Owner != null)
                    {
                        if (player)
                        {
                            if (projectile == current_ball)
                            {
                                list.Add(projectile);
                            }
                        }
                    }
                }
            }
            return list;
        }

        private bool HasReloaded;
        //This block of code allows us to change the reload sounds.
        public override void Update()
        {
           
            if (gun.CurrentOwner != null)
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
            
            base.Update();
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                HasReloaded = false;
                
                base.OnReloadPressed(player, gun, bSOMETHING);
                Material outline = SpriteOutlineManager.GetOutlineMaterial(gun.sprite);
                outline.SetColor("_OverrideColor", new Color(0f, 0f, 0f));

                shotcountcontroller = 1;
            }
        }

    }
}

