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

namespace Knives
{
    class EndoThermic : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Endothermic Shotgun", "Endothermic");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:endothermic_shotgun", "ski:endothermic_shotgun");
            gun.gameObject.AddComponent<EndoThermic>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Energy Intake");
            gun.SetLongDescription("Shotgun auto-reloads over time.\n\n" +
                "Infantry Armament Experimentation: Number 273.15c\n\n" +
                "Thermal core calibration inverted from expectations. Device has plummeted in temperature and is holding stable around .01 degrees kelvin.\n" +
                "Further destablization may cause catastrophic damage.\n" +
                "This has been placed into permanent containment for facility saftey. If you are reading this note you are getting fired.\n" +
                "- Hmon Armaments Department." +
                "\n\n\n - Knife_to_a_Gunfight");
            gun.SetupSprite(null, "Endothermic_idle_001", 1);
            gun.SetAnimationFPS(gun.shootAnimation, 20);
            gun.SetAnimationFPS(gun.reloadAnimation, 1);
            gun.SetAnimationFPS(gun.criticalFireAnimation, 1);
            gun.SetAnimationFPS(gun.outOfAmmoAnimation, 1);
            
            for (int i = 0; i < 5; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(223) as Gun, true, false);


            }
            foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
            {
                projectileModule.ammoCost = 1;
                projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
                projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                projectileModule.cooldownTime = .5f;
                projectileModule.angleVariance = 9f;
                projectileModule.numberOfShotsInClip = 4;
                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                projectileModule.projectiles[0] = projectile;
                projectile.baseData.damage = 6f;
                projectile.AdditionalScaleMultiplier = .7f;
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                projectile.FreezeApplyChance = .2f;
                gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.BLUE_SHOTGUN;
                gun.DefaultModule.projectiles[0] = projectile;
                bool flag = projectileModule != gun.DefaultModule;
                if (flag)
                {
                    projectileModule.ammoCost = 0;
                }

            }
            gun.barrelOffset.transform.localPosition = new Vector3(1.5f, .5f, 0f);
            gun.reloadTime = 4f;
            
            gun.SetBaseMaxAmmo(4);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(157) as Gun).muzzleFlashEffects;
            gun.quality = PickupObject.ItemQuality.B;
            gun.gunClass = GunClass.SHOTGUN;
            gun.CanGainAmmo = false;

            
            
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            ID = gun.PickupObjectId;
        }

        public static int ID;
        public static PlayerController Loneranger;
        public override void OnPickup(GameActor owner)
        {
            PlayerController player = (PlayerController)owner;
            try
            {
                base.OnPickup(player);
                player.GunChanged += this.OnGunChanged;
            }
            catch (Exception e)
            {
                Tools.Print("Copper OnPickup", "FFFFFF", true);
                Tools.PrintException(e);
            }
        }

        public override void OnPostDrop(GameActor owner)

        {
            PlayerController player = (PlayerController)owner;
            try
            {
                player.GunChanged -= this.OnGunChanged;
                base.OnPostDrop(player);
            }
            catch (Exception e)
            {
                Tools.Print("Copper OnPostDrop", "FFFFFF", true);
                Tools.PrintException(e);
            }
        }

        // Token: 0x06000392 RID: 914 RVA: 0x000261A4 File Offset: 0x000243A4
        private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
        {
            try
            {
                isdoing = false;
            }
            catch (Exception e)
            {
                Tools.Print("Copper OnGunChanged", "FFFFFF", true);
                Tools.PrintException(e);
            }
        }

        public override void OnPickedUpByPlayer(PlayerController player)
        {
            Loneranger = player;
            base.OnPickedUpByPlayer(player);
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            if ((this.Owner as PlayerController).PlayerHasActiveSynergy("Proper Kalibration"))
            {
                pause = .3f;
            }
            else
            {
                pause = .4f;
            }
            fireamindelay = .2f;
            //AkSoundEngine.PostEvent("Play_ENM_iceslime_blast_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_half_gauge_fire", base.gameObject);
            base.OnPostFired(player, gun);
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

                if(gun.ClipShotsRemaining < (gun.ClipCapacity + gun.AdditionalClipCapacity))
                {
                    if(isdoing == false)
                    {
                        StartCoroutine(gradualReload());
                    }
                }
                if (gun.ClipCapacity + gun.AdditionalClipCapacity != gun.GetBaseMaxAmmo())
                {
                    gun.SetBaseMaxAmmo(gun.ClipCapacity + gun.AdditionalClipCapacity);
                }
                if(pause > 0)
                {
                    pause -= Time.deltaTime;
                }
                if(fireamindelay >0)
                {
                    fireamindelay -= Time.deltaTime;
                }

                if(gun.IsFiring != true && fireamindelay <=0)
                {
                    if((this.Owner as PlayerController).PlayerHasActiveSynergy("Proper Kalibration"))
                    {
                        switch (gun.ClipShotsRemaining)
                        {
                            case 0:
                                gun.spriteAnimator.Play("Exothermic_out_of_ammo");
                                break;
                            case 1:
                                gun.spriteAnimator.Play("Exothermic_final_fire");
                                break;
                            case 2:
                                gun.spriteAnimator.Play("Exothermic_reload");
                                break;
                            case 3:
                                gun.spriteAnimator.Play("Exothermic_critical_fire");
                                break;
                            case 4:
                                gun.spriteAnimator.Play("Exothermic_idle");
                                break;
                            default:
                                gun.spriteAnimator.Play("Exothermic_idle");
                                break;

                        }

                    }
                    else
                    {
                        switch (gun.ClipShotsRemaining)
                        {
                            case 0:
                                gun.spriteAnimator.Play("Endothermic_out_of_ammo");
                                break;
                            case 1:
                                gun.spriteAnimator.Play("Endothermic_final_fire");
                                break;
                            case 2:
                                gun.spriteAnimator.Play("Endothermic_reload");
                                break;
                            case 3:
                                gun.spriteAnimator.Play("Endothermic_critical_fire");
                                break;
                            case 4:
                                gun.spriteAnimator.Play("Endothermic_idle");
                                break;
                            default:
                                gun.spriteAnimator.Play("Endothermic_idle");
                                break;

                        }

                    }

                }


            }

            base.Update();
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool manualReload)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                HasReloaded = false;
                base.OnReload(player, gun);
            }

        }
        public float perload = .7f;
        bool isdoing = false;
        public float pause = 0;
        private float fireamindelay = .2f;

        public IEnumerator gradualReload()
        {
            
            isdoing = true;
            PlayerController player = (PlayerController)this.gun.CurrentOwner;
            float Reloadtime = perload * player.stats.GetStatValue(PlayerStats.StatType.ReloadSpeed);
            float time = Reloadtime;
            while (gun.ClipShotsRemaining < (gun.ClipCapacity + gun.AdditionalClipCapacity))
            {
                
                
                if(time > 0 )
                {
                    if(pause <= 0)
                    {
                        time -= Time.deltaTime;
                    }
                    
                }
                else
                {
                    gun.CurrentAmmo += 1;
                    gun.MoveBulletsIntoClip(1);
                    if (!(this.Owner as PlayerController).PlayerHasActiveSynergy("Proper Kalibration")) AkSoundEngine.PostEvent("Play_WPN_accent_ice_01", base.gameObject);
                    AkSoundEngine.PostEvent("Play_ITM_Crisis_Stone_Impact_01", base.gameObject);
                    time = Reloadtime;
                }

                yield return null;
            }
            yield return null;
            isdoing = false;
        }

    }
}
