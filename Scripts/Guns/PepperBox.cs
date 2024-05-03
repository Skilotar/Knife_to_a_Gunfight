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

    public class PepperBox : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Spicy PepperBox", "Pepperbox");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:spicy_pepperbox", "ski:spicy_pepperbox");
            gun.gameObject.AddComponent<PepperBox>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("FireStorm");
            gun.SetLongDescription("Reload while doing a burstfire to fire wildly.\n" +
                "The final perfect version of the ancient pepperbox. Finally living up to its name this guns spews hot fire across the battlefield." +
                "" +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "Pepperbox_idle_001", 8);
            
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);


            
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(79) as Gun, true, false);

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 6;


            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.carryPixelOffset = new IntVector2(4, 0);
            gun.reloadTime = 1f;
            gun.DefaultModule.cooldownTime = 1.5f;
            gun.DefaultModule.numberOfShotsInClip = 12;
            gun.SetBaseMaxAmmo(600);
           
            gun.quality = PickupObject.ItemQuality.B;
            gun.barrelOffset.transform.localPosition = new Vector3(1.3f, .9f, 0f);

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 9f;
            projectile.baseData.speed *= 1f;
            projectile.AppliesFire = true;
            projectile.FireApplyChance = .20f;

            projectileStates stat = projectile.gameObject.GetOrAddComponent<projectileStates>();
            stat.PepperUn = true;

            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            gun.DefaultModule.projectiles[0] = projectile2;
            projectile2.baseData.damage = 10f;
            projectile2.baseData.speed *= 1f;
            projectile2.AppliesFire = true;
            projectile2.FireApplyChance = .20f;

            projectileStates stat2 = projectile2.gameObject.GetOrAddComponent<projectileStates>();
            stat2.PepperCharge = true;


            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 0f
            };
            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {

                Projectile = projectile2,
                ChargeTime = .65f
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
                item2
            };

            Projectile special = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(384) as Gun).DefaultModule.projectiles[0]);
            special.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(special.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(special);
            
            special.baseData.damage = 9f;
            special.baseData.speed *= 1f;
            special.AppliesFire = true;
            special.FireApplyChance = .20f;
            burst = special;


           
            gun.gunClass = GunClass.FIRE;
            ETGMod.Databases.Items.Add(gun, null, "ANY");


            ID = gun.PickupObjectId;
        }

        public static int ID;
        public static Projectile burst;

        public override void PostProcessProjectile(Projectile projectile)
        {
            AkSoundEngine.PostEvent("Play_WPN_beretta_shot_01", base.gameObject);
            Running = false;
            doing = false;
            if (projectile.GetComponent<projectileStates>().PepperUn == true && Running == false)
            {
                gun.StartCoroutine(normalBurst());
            }

            if (projectile.GetComponent<projectileStates>().PepperCharge == true && Running == false)
            {
                gun.StartCoroutine(chargeBurst());

            }

            base.PostProcessProjectile(projectile);
        }
        bool Running = false;
        private IEnumerator normalBurst()
        {
            Running = true;
            for (int i = 2; i <= 6; i++)
            {
                if(gun.ClipShotsRemaining > 0 )
                {
                    bool reload = gun.IsReloading;
                    yield return new WaitForSeconds(.07f / Player.stats.GetStatValue(PlayerStats.StatType.RateOfFire));
                    float var = UnityEngine.Random.Range(-25, 25);

                    var *= Player.stats.GetStatValue(PlayerStats.StatType.Accuracy);
                    if(reload){var = UnityEngine.Random.Range(-365, 365);}
                    Projectile proj = MiscToolMethods.SpawnProjAtPosi(burst, gun.barrelOffset.transform.position, (this.Owner as PlayerController), this.gun, var);
                    if (reload)
                    {
                        BounceProjModifier bnc = proj.gameObject.GetOrAddComponent<BounceProjModifier>();
                        bnc.bouncesTrackEnemies = true;
                        bnc.bounceTrackRadius = 10f;
                        bnc.numberOfBounces = 1;
                    }
                    (this.gun.CurrentOwner as PlayerController).DoPostProcessProjectile(proj);
                    gun.LoseAmmo(1);
                    gun.ClipShotsRemaining = gun.ClipShotsRemaining - 1;
                    AkSoundEngine.PostEvent("Play_WPN_beretta_shot_01", base.gameObject);

                }
                else
                {
                    gun.Reload();
                }
            }
            yield return new WaitForSeconds(.25f);
            if(gun.ClipShotsRemaining == 0)
            {
                gun.Reload();
            }
            gun.ClearCooldowns();
            Running = false;
        }
        
        private IEnumerator chargeBurst()
        {
            bool down = true;
            Running = true;
            float var = -6;
            for (int i = 2; i <= 12; i++)
            {
                if (gun.ClipShotsRemaining > 0)
                {
                    bool reload = gun.IsReloading;
                    yield return new WaitForSeconds(.05f / Player.stats.GetStatValue(PlayerStats.StatType.RateOfFire));
                    if (reload) { var = UnityEngine.Random.Range(-365, 365); }
                    Projectile proj = MiscToolMethods.SpawnProjAtPosi(burst, gun.barrelOffset.transform.position, (this.Owner as PlayerController), this.gun, var);
                    if (reload)
                    {
                        BounceProjModifier bnc = proj.gameObject.GetOrAddComponent<BounceProjModifier>();
                        bnc.bouncesTrackEnemies = true;
                        bnc.bounceTrackRadius = 10f;
                        bnc.numberOfBounces = 1;
                        proj.baseData.damage *= 2f;
                    }
                    gun.LoseAmmo(1);
                    gun.ClipShotsRemaining = gun.ClipShotsRemaining - 1;
                    (this.gun.CurrentOwner as PlayerController).DoPostProcessProjectile(proj);
                    AkSoundEngine.PostEvent("Play_WPN_beretta_shot_01", base.gameObject);
                    if (down)
                    {
                        var -= 6f;
                        if (var <= -18) down = false;
                    }
                    else
                    {
                        var += 6f;
                        if (var >= 18) down = true;
                    }
                }
                else
                {
                    gun.Reload();
                }
            }
            yield return new WaitForSeconds(.25f);
            if (gun.ClipShotsRemaining == 0)
            {
                gun.Reload();
            }
            gun.ClearCooldowns();
            Running = false;
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


                if(this.gun == gun.CurrentOwner.CurrentGun && doing == false && Running == false)
                {
                    StartCoroutine(Smonk());
                    if (gun.HasChargedProjectileReady)
                    {
                        Ember();
                    }
                }
                if (this.gun == gun.CurrentOwner.CurrentGun && Running == true)
                {
                    GlobalSparksDoer.DoRandomParticleBurst(1, gun.barrelOffset.transform.position, gun.barrelOffset.transform.position, OMITBMathsAndLogicExtensions.DegreeToVector2(gun.CurrentAngle), 20, 3, null, null, null, GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
                }

            }
            

        }

        private void Ember()
        {

            GlobalSparksDoer.DoRandomParticleBurst(2, gun.barrelOffset.transform.position, gun.barrelOffset.transform.position, Vector3.up, 20, 3, null, null, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
        }

        public bool doing;
        private IEnumerator Smonk()
        {
            doing = true;
            float wait = UnityEngine.Random.Range(.1f, .3f);

            GlobalSparksDoer.DoRandomParticleBurst(2, gun.barrelOffset.transform.position, gun.barrelOffset.transform.position,Vector3.up,20,3,null,null,null,GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
            yield return new WaitForSeconds(wait);
            yield return new WaitForEndOfFrame();
            
            doing = false;
        }
        public override void OnReload(PlayerController player, Gun gun)
        {
            if (gun.IsReloading && this.HasReloaded)
            {

                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                
                gun.StopCoroutine("FanTheHammer");
                Running = false;
                AkSoundEngine.PostEvent("Play_cata_reload_sfx", base.gameObject);

            }

            base.OnReload(player, gun);
        }



    }
}

