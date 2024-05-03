using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using System.CodeDom;

namespace Knives
{
    class TaurenTails : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Tauren Tails", "tt");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:tauren_tails", "ski:tauren_tails");
            gun.gameObject.AddComponent<TaurenTails>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Full Charge");
            gun.SetLongDescription("A strange variation of the timeless sling. This varient was forged from the tail of a taurous and has three slots for ammo and can fire up to two at a time. \n" +
                "\n- Knife_to_a_Gunfight");
            
            gun.SetupSprite(null, "tt_idle_001", 4);
            gun.SetAnimationFPS(gun.shootAnimation, 8);
            gun.SetAnimationFPS(gun.reloadAnimation, 24);
            gun.SetAnimationFPS(gun.chargeAnimation, 12);
            
            gun.AddProjectileModuleFrom((PickupObjectDatabase.GetById(382) as Gun), true, true);
            Gun gun2 = (PickupObjectDatabase.GetById(382) as Gun);
            gun.muzzleFlashEffects = gun2.muzzleFlashEffects;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SHITTY;
            gun.gunHandedness = GunHandedness.OneHanded;
           
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 2f;
            
            gun.DefaultModule.cooldownTime = 1f;
            gun.DefaultModule.numberOfShotsInClip = 3;
            gun.SetBaseMaxAmmo(900);
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(382) as Gun).gunSwitchGroup;
            gun.quality = PickupObject.ItemQuality.C;
            



            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.chargeProjectiles[0].Projectile);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 15f;
            projectile.baseData.speed *= 1f;
            projectile.BossDamageMultiplier = 1.2f;
           
            projectile.transform.parent = gun.barrelOffset;
            PierceProjModifier stab = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            stab.penetration = 1;
            

            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.chargeProjectiles[0].Projectile);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            gun.DefaultModule.projectiles[0] = projectile2;
            projectile2.baseData.damage = 16f;
            projectile2.baseData.speed *= 1f;
            projectile2.BossDamageMultiplier = 2.5f;
           

            projectile2.transform.parent = gun.barrelOffset;
            PierceProjModifier stab2 = projectile2.gameObject.GetOrAddComponent<PierceProjModifier>();
            stab2.penetration = 1;

            tk2dSpriteAnimationClip fireClip = gun.sprite.spriteAnimator.GetClipByName("tt_idle");
            float[] offsetsX = new float[] { -0f, -.05f, -.1f, -.05f, 0.0f, .05f, .1f,.05f };
            float[] offsetsY = new float[] { .1f, .1f, .1f, .1f, .1f, .1f, .1f, .1f };
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

            tk2dSpriteAnimationClip fireClip2 = gun.sprite.spriteAnimator.GetClipByName("tt_charge");
            float[] offsetsX2 = new float[] { -.2f, -.2f };
            float[] offsetsY2 = new float[] { .2f, .2f };
            for (int i = 0; i < offsetsX2.Length && i < offsetsY2.Length && i < fireClip2.frames.Length; i++)
            {
                int id = fireClip2.frames[i].spriteId;
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX2[i];
                fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY2[i];
            }

           

            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = .5f
            };
            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile2,
                
                ChargeTime = 1.3f
            };
            
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
                item2,
                
            };
            gun.PreventNormalFireAudio = true;

            ETGMod.Databases.Items.Add(gun, null, "ANY");


        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            AkSoundEngine.PostEvent("Play_sling_fire_001", base.gameObject);
            AkSoundEngine.PostEvent("Play_sling_fire_001", base.gameObject);
            AkSoundEngine.PostEvent("Play_sling_fire_001", base.gameObject);
            PlayerController player = this.gun.CurrentOwner as PlayerController;
            if (projectile.GetCachedBaseDamage == 15)
            {
               
            }
            if (projectile.GetCachedBaseDamage == 16)
            {
                if (this.gun.ClipShotsRemaining >= 2)
                {
                    int getclip = this.gun.ClipShotsRemaining;
                    Projectile projectile1 = ((PickupObjectDatabase.GetById(382) as Gun)).DefaultModule.chargeProjectiles[0].Projectile;
                    GameObject gameObject1 = SpawnManager.SpawnProjectile(projectile1.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
                    Projectile component1 = gameObject1.GetComponent<Projectile>();
                    component1.Owner = player;
                    component1.baseData.damage = 16;
                    component1.Shooter = player.specRigidbody;
                    component1.BossDamageMultiplier = 3.3f;
                    PierceProjModifier stab3 = component1.gameObject.GetOrAddComponent<PierceProjModifier>();
                    AkSoundEngine.PostEvent("Play_sling_fire_001", base.gameObject);
                    AkSoundEngine.PostEvent("Play_sling_fire_001", base.gameObject);
                    AkSoundEngine.PostEvent("Play_sling_fire_001", base.gameObject);
                    stab3.penetration = 1;
                    this.gun.ClipShotsRemaining = getclip - 1;
                    this.gun.GainAmmo(-1);
                   
                }
            }
        }
        public bool toggle = true;
        public override void OnFinishAttack(PlayerController player, Gun gun)
        {
            toggle = true;
            
        }
        private bool HasReloaded;
       
        public override void  Update()
        {
            if (this.gun.CurrentOwner)
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
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                
            }
        }


    }
}


