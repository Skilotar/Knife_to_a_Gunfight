using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using System.Runtime.InteropServices;
using System.Text;
using Brave;
using Brave.BulletScript;
using Gungeon;
using HutongGames.PlayMaker.Actions;
using ItemAPI;
using UnityEngine;

namespace Knives
{
    class Maw : AdvancedGunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Maw of Vog", "Maw");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:maw_of_vog", "ski:maw_of_vog");
            gun.gameObject.AddComponent<Maw>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Through The Fire And Flames");
            gun.SetLongDescription("Charge for ring of fire.\n" +
                "Statbuffs while on fire\n\n" +
                "" +
                "This blade was forged with the blessing of the bullet hell's canine gaurdian, Vog. \n" +
                "This blade functions better while it is heated, swinging faster and without ceasing. \n" +
                "Weilders of the blade are protected, but not immune to the fires it makes" +
                "" +
                "" +
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "Maw_idle_001", 8);

            gun.SetAnimationFPS(gun.chargeAnimation, 12);
            gun.SetAnimationFPS(gun.shootAnimation, 20);
            gun.SetAnimationFPS(gun.reloadAnimation, 4);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(59) as Gun, true, false);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 9;



            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.OneHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = 1.8f;
            gun.DefaultModule.numberOfShotsInClip = 5;
            gun.DefaultModule.cooldownTime = .5f;



            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.B;
            //gun.encounterTrackable.EncounterGuid = "BURN BABY BURN!";

            //swipe
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(1f, .5f, 0f);
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "finished big";
            gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 1, StatModifier.ModifyMethod.ADDITIVE);

            gun.muzzleFlashEffects = null;



            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 9f;
            projectile.baseData.speed = 3f;
            projectile.baseData.range = 3f;
            projectile.baseData.force = 5;
            
            projectile.AppliesFire = true;
            projectile.FireApplyChance = 100f;



            //Hot HOT HOT 
            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);

            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            gun.DefaultModule.projectiles[0] = projectile2;
            projectile2.baseData.damage = 15f;
            projectile2.baseData.speed = 50f;
            projectile2.baseData.range = 10f;
           
            projectile2.name = "shelling";


           

            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 0f,
                AmmoCost = 0,


            };
            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile2,
                ChargeTime = .8f,


            };
           
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
                item2,
               
            };

            tk2dSpriteAnimationClip fireClip = gun.sprite.spriteAnimator.GetClipByName("Maw_reload");
            float[] offsetsX = new float[] { -.3f, -.4f, -.4f, -.4f, -.4f };
            float[] offsetsY = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, -0.0f };

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

            tk2dSpriteAnimationClip fireClip2 = gun.sprite.spriteAnimator.GetClipByName("Maw_fire");
            float[] offsetsX2 = new float[] { .1f, .4f, .3f, .2f ,.0f};
            float[] offsetsY2 = new float[] { 1.2f, .5f , .3f, -1.2f, -.6f};
            
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


            tk2dSpriteAnimationClip fireClip3 = gun.sprite.spriteAnimator.GetClipByName("Maw_charge");
            float[] offsetsX3 = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -0.6f, -0.7f, -0.7f };
            float[] offsetsY3 = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -0.0f };

            for (int i = 0; i < offsetsX3.Length && i < offsetsY3.Length && i < fireClip3.frames.Length; i++)
            {
                int id = fireClip3.frames[i].spriteId;
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX3[i];
                fireClip3.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY3[i];
            }



            //MiscToolMethods.TrimAllGunSprites(gun);
            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }



      

        public override void OnPostFired(PlayerController player, Gun gun)
        {
           
            gun.PreventNormalFireAudio = true;
           
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
            AkSoundEngine.PostEvent("Play_PET_wolf_bite_01", base.gameObject);
            
            if (projectile.GetCachedBaseDamage == 9f)
            {
                //slash

                PlayerController player = (PlayerController)this.gun.CurrentOwner;
                Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                ProjectileSlashingBehaviour slasher = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashDimensions = 90;
                slasher.SlashRange = 3f;
                slasher.playerKnockback = 0;

                if (player.CurrentFireMeterValue > 0)
                {
                    slasher.SlashDamage = 5;
                }
                slasher.InteractMode = SlashDoer.ProjInteractMode.IGNORE;


            }
            if (projectile.GetCachedBaseDamage == 15f)
            {

                PlayerController player = (PlayerController)this.gun.CurrentOwner;
                Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                ProjectileSlashingBehaviour slasher = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashDimensions = 90;
                slasher.SlashRange = 3f;
               
                slasher.playerKnockback = 0;
                
                slasher.InteractMode = SlashDoer.ProjInteractMode.IGNORE;
                
                StartCoroutine(RingOfFire());

            }
            StartCoroutine(AfterImages());

            base.PostProcessProjectile(projectile);
        }
        bool overhead = false;
        public IEnumerator AfterImages()
        {
            PlayerController player = (PlayerController)this.gun.CurrentOwner;


            yield return new WaitForSecondsRealtime(.1f);

            if (overhead)
            {
                Projectile projectile2 = ((Gun)ETGMod.Databases.Items[384]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + 120), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag = component != null;
                if (flag)
                {


                    
                    component.baseData.range = 4f;
                    component.baseData.speed = component.baseData.speed * -1 ;
                    component.baseData.speed = component.baseData.speed * 2f;
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    if(player.CurrentFireMeterValue > 0)
                    {
                        component.AppliesFire = true;
                        component.FireApplyChance = .5f;
                        player.IncreaseFire(.03f);
                    }
                    component.baseData.damage = 3f;
                    ImprovedAfterImage trail = component.gameObject.GetOrAddComponent<ImprovedAfterImage>();
                    trail.shadowLifetime = .2f;
                    trail.shadowTimeDelay = .0001f;
                    trail.dashColor = new Color(.67f, .08f, .01f);
                    trail.spawnShadows = true;

                }
                Spin(component);
                overhead = false;
            }
            else
            {
                Projectile projectile2 = ((Gun)ETGMod.Databases.Items[384]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle - 120), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag = component != null;
                if (flag)
                {


                   
                    component.baseData.range = 4f;
                    component.baseData.speed = component.baseData.speed * 2f;
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    if (player.CurrentFireMeterValue > 0)
                    {
                        component.AppliesFire = true;
                        component.FireApplyChance = .5f;
                        player.IncreaseFire(.03f);
                    }
                    component.baseData.damage = 3f;

                    ImprovedAfterImage trail = component.gameObject.GetOrAddComponent<ImprovedAfterImage>();
                    trail.shadowLifetime = .2f;
                    trail.shadowTimeDelay = .0001f;
                    trail.dashColor = new Color(.67f, .08f, .01f);
                    trail.spawnShadows = true;
                }
                Spin(component);
                overhead = true;
            }
           
        }

        public void Spin(Projectile proj)
        {
           

            BounceProjModifier bouncer = proj.gameObject.GetOrAddComponent<BounceProjModifier>();
            bouncer.name = "afterimage";

            int orbitersInGroup = OrbitProjectileMotionModule.GetOrbitersInGroup(101);
            if (orbitersInGroup >= 20)
            {
                return;
            }
            bouncer.projectile.specRigidbody.CollideWithTileMap = false;
            bouncer.projectile.ResetDistance();

            bouncer.projectile.baseData.range = Mathf.Max(bouncer.projectile.baseData.range, 17f);
           
            OrbitProjectileMotionModule orbitProjectileMotionModule = new OrbitProjectileMotionModule();
            orbitProjectileMotionModule.lifespan = 8f;
           
            
            if (bouncer.projectile.OverrideMotionModule != null && bouncer.projectile.OverrideMotionModule is HelixProjectileMotionModule)
            {
                orbitProjectileMotionModule.StackHelix = true;
                orbitProjectileMotionModule.ForceInvert = (bouncer.projectile.OverrideMotionModule as HelixProjectileMotionModule).ForceInvert;
            }
            
            
            orbitProjectileMotionModule.MinRadius = 3.5f;
            orbitProjectileMotionModule.MaxRadius = 3.5f;
            
            bouncer.projectile.OverrideMotionModule = orbitProjectileMotionModule;

        }

        public IEnumerator RingOfFire()
        {

            PlayerController player = (PlayerController)this.gun.CurrentOwner;

            yield return new WaitForSecondsRealtime(.1f);
            
            for (int i = 0; i < 36; i++)
            {
                
                Projectile projectile2 = ((Gun)ETGMod.Databases.Items[125]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle - (i * 10)), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag = component != null;
                if (flag)
                {


                    
                    component.baseData.range = 4.5f;
                    component.baseData.speed *= .25f;
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    component.baseData.damage = 1f;

                    
                }
            }

            

            player.IsOnFire = true;
            player.IncreaseFire(.01f);

        }



        private bool HasReloaded;

        public bool resist = false;
        public override void  Update()
        {
            
            if (gun.CurrentOwner)
            {
                PlayerController player = (PlayerController)this.gun.CurrentOwner;

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
                //statbuffs for fire
                
                if (player.CurrentFireMeterValue > 0)
                {

                    gun.DefaultModule.cooldownTime = .25f;
                    gun.reloadTime = .01f;

                }
                else
                {
                    gun.DefaultModule.cooldownTime = .5f;
                    gun.reloadTime = 2f;

                }
                //manual fire damage
                if (player.CurrentFireMeterValue > .95)
                {
                    player.healthHaver.ApplyDamage(.5f, Vector2.zero, "Fire", CoreDamageTypes.None, DamageCategory.Unstoppable);
                    player.CurrentFireMeterValue = .01f;
                }
                // fire resist apply
                if (player.CurrentFireMeterValue > 0 && resist == false)
                {
                    this.m_fireImmunity = new DamageTypeModifier();
                    this.m_fireImmunity.damageMultiplier = .29f;
                    this.m_fireImmunity.damageType = CoreDamageTypes.Fire;
                    player.healthHaver.damageTypeModifiers.Add(this.m_fireImmunity);
                    resist = true;
                }
                
              


            }
            
           


        }
      
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {

               
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                AkSoundEngine.PostEvent("Play_PET_dog_bark_01", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                

            }

        }

        

        public DamageTypeModifier m_fireImmunity;

    }
}

