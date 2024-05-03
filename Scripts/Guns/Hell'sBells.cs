using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using HutongGames.PlayMaker.Actions;
using ItemAPI;
using UnityEngine;

namespace Knives
{ 
    class Hells_bells :AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Hell's Bell", "bell");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:hell's_bell", "ski:hells_bell");
            gun.gameObject.AddComponent<Hells_bells>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Hell Tones");
            gun.SetLongDescription("This bell, dispite its name, does not come from bullet hell. It was the main clocktower bell of the leadlords cathedral. " +
                "The Bell's health was maintained for countless years by an unfortunatly named priest, Hell. " +
                "Hell died shortly after mixing up AC and DC currents when he was attempting to fix the bell's automated chiming system." +
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "bell_idle_001", 8);
            gun.SetAnimationFPS(gun.chargeAnimation, 5);

            gun.SetAnimationFPS(gun.shootAnimation, 20);
            gun.SetAnimationFPS(gun.reloadAnimation, 9);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(59) as Gun, true, false);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 4;

           
            

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.OneHanded;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 2f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.DefaultModule.cooldownTime = .5f;
            gun.SetBaseMaxAmmo(100);
            gun.quality = PickupObject.ItemQuality.B;
            gun.encounterTrackable.EncounterGuid = "Dong!";

           
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(0f, .5f, 0f);
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "planet";


            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 10f;
            projectile.baseData.speed = 3f;
            projectile.baseData.range = 3f;
            projectile.baseData.force = 5;
            

           
           
            Gun swipeFlash = (Gun)PickupObjectDatabase.GetById(417);
            gun.muzzleFlashEffects.type = VFXPoolType.None;

          


            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = .75f,
                AmmoCost = 0,


            };
        
         
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
                
               
            };
            projectile.transform.parent = gun.barrelOffset;
            //charge offests
            tk2dSpriteAnimationClip fireClip = gun.sprite.spriteAnimator.GetClipByName("bell_charge");
            float[] offsetsX = new float[] { 0.0f, 0.0f, 0.0f, -.5f, -.5f };
            float[] offsetsY = new float[] { 0f, .5f, 1.5f, 2f, 2f };
            for (int i = 0; i < offsetsX.Length && i < offsetsY.Length && i < fireClip.frames.Length; i++)
            {
                int id = fireClip.frames[i].spriteId;
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX[i];
                fireClip.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY[i];
            }

            //fire offests
            tk2dSpriteAnimationClip fireClip2 = gun.sprite.spriteAnimator.GetClipByName("bell_fire");
            float[] offsetsX2 = new float[] { 0.0f, 0.0f};
            float[] offsetsY2 = new float[] { .1f, 0f};
            for (int i = 0; i < offsetsX2.Length && i < offsetsY2.Length && i < fireClip2.frames.Length; i++)
            {
                int id = fireClip2.frames[i].spriteId;
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY2[i];
                
            }

            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;
        }

        public static int ID;

        public override void  OnPickedUpByPlayer(PlayerController player)
        {
            
            base.OnPickedUpByPlayer(player);
        }
   
        public override void PostProcessProjectile(Projectile projectile)
        {
            //swing hitbox
            ProjectileSlashingBehaviour slasher = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
            slasher.SlashDimensions = 120f;
            slasher.SlashRange = 3.4f;
            slasher.SlashDamage = 15f;

            StartCoroutine(Chimecontroller());

            base.PostProcessProjectile(projectile);
        }

        public IEnumerator Chimecontroller()
        {
           
            PlayerController player = (PlayerController)this.gun.CurrentOwner;
            
            yield return new WaitForSecondsRealtime(.2f);
            AkSoundEngine.PostEvent("Play_Hell_s_bell", base.gameObject);
            for (int i = 0; i < 18; i++)
            {
                Projectile projectile2 = ((Gun)ETGMod.Databases.Items[3]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + (i * 20)), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag = component != null;
                if (flag)
                {
                    if (player.PlayerHasActiveSynergy("New Waves"))
                    {
                        BounceProjModifier bounce = component.gameObject.GetOrAddComponent<BounceProjModifier>();
                        bounce.numberOfBounces = 1;
                        component.baseData.range *= 1.5f;
                    }
                    else
                    {
                        BounceProjModifier bounce = component.gameObject.GetOrAddComponent<BounceProjModifier>();
                        bounce.numberOfBounces = 0;
                        component.baseData.range *= 1f;
                    }
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    component.baseData.damage = 10f;
                    component.AdditionalScaleMultiplier *= 1.5f;
                    component.DefaultTintColor = UnityEngine.Color.yellow;
                    component.HasDefaultTint = true;
                }
            }
            if (player.PlayerHasActiveSynergy("Old Chimes"))
            {
                StartCoroutine(bulletspiral());
            }
            else
            {
                StartCoroutine(bulletpulse());
            }

        }

        public IEnumerator bulletspiral()
        {
            float rotationary = 0;
            float Dura = 1;
            PlayerController player = (PlayerController)this.gun.CurrentOwner;
            float elapsed = -BraveTime.DeltaTime;

            while (elapsed < Dura)
            {
                yield return new WaitForSecondsRealtime(.015f);
                elapsed += BraveTime.DeltaTime;

                Projectile projectile2 = ((Gun)ETGMod.Databases.Items[3]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + rotationary), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag = component != null;
                if (flag)
                {
                    if(player.PlayerHasActiveSynergy("New Waves"))
                    {
                        BounceProjModifier bounce = component.gameObject.GetOrAddComponent<BounceProjModifier>();
                        bounce.numberOfBounces = 1;
                        component.baseData.range *= 2f;
                    }
                    else
                    {
                        BounceProjModifier bounce = component.gameObject.GetOrAddComponent<BounceProjModifier>();
                        bounce.numberOfBounces = 0;
                        component.baseData.range *= 1f;
                    }
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    component.baseData.damage = 7f;
                    component.AdditionalScaleMultiplier *= 1.5f;
                    component.DefaultTintColor = UnityEngine.Color.yellow;
                    component.HasDefaultTint = true;
                }

                rotationary = rotationary + 20;
                yield return null;
            }
        }

        public IEnumerator bulletpulse()
        {
            
            PlayerController player = (PlayerController)this.gun.CurrentOwner;


            yield return new WaitForSeconds(.5f);

            for (int i = 0; i < 18; i++)
            {
                Projectile projectile2 = ((Gun)ETGMod.Databases.Items[3]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + (i * 20)), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag = component != null;
                if (flag)
                {
                    if (player.PlayerHasActiveSynergy("New Waves"))
                    {
                        BounceProjModifier bounce = component.gameObject.GetOrAddComponent<BounceProjModifier>();
                        bounce.numberOfBounces = 1;

                        component.baseData.range *= 1.5f;
                    }
                    else
                    {
                        BounceProjModifier bounce = component.gameObject.GetOrAddComponent<BounceProjModifier>();
                        bounce.numberOfBounces = 0;
                        component.baseData.range *= 1f;
                    }
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    component.AdditionalScaleMultiplier *= 1.5f;
                    component.baseData.damage = 10f;
                    component.DefaultTintColor = UnityEngine.Color.yellow;
                    component.HasDefaultTint = true;
                }
            }

            yield return new WaitForSeconds(.5f);

            for (int i = 0; i < 18; i++)
            {
                Projectile projectile2 = ((Gun)ETGMod.Databases.Items[3]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + (i * 20)), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag = component != null;
                if (flag)
                {
                    if (player.PlayerHasActiveSynergy("New Waves"))
                    {
                        BounceProjModifier bounce = component.gameObject.GetOrAddComponent<BounceProjModifier>();
                        bounce.numberOfBounces = 1;
                        component.baseData.range *= 1.5f;
                    }
                    else
                    {
                        BounceProjModifier bounce = component.gameObject.GetOrAddComponent<BounceProjModifier>();
                        bounce.numberOfBounces = 0;
                        component.baseData.range *= 1f;
                    }
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    component.baseData.damage = 5f;
                    component.DefaultTintColor = UnityEngine.Color.yellow;
                    component.HasDefaultTint = true;
                }
            }

           
            
            
            yield return null;
        }

        private bool HasReloaded;

        //This block of code allows us to change the reload sounds.
        public override void  Update()
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
                


            }
        }


    }
}
