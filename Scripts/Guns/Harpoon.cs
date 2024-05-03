using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Gungeon;
using System.Collections;
using HutongGames.PlayMaker.Actions;

namespace Knives
{
    class harpoon : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Death to Bayshore", "harp");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:death_to_bayshore", "ski:death_to_bayshore");
            gun.gameObject.AddComponent<harpoon>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Wanted Dead Or Alive");
            gun.SetLongDescription("A terrifying rifle created by devious engeineer Caleb Quinn, built for the purpose of dragging in wanted criminals by spearing them through their cores.\n\n" +
                "___________________________________________\n\n" +
                "If an enemy is speared the next attack will pull them in and slash." +
                "\n\n\n - Knife_to_a_Gunfight");
            gun.SetupSprite(null, "harp_idle_001", 1);
            gun.SetAnimationFPS(gun.shootAnimation, 1);
            gun.SetAnimationFPS(gun.reloadAnimation, 1);

            gun.AddProjectileModuleFrom("Camera_Gun", true, true);
            gun.PreventNormalFireAudio = true;

            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            Gun gun3 = (Gun)ETGMod.Databases.Items["blunderbuss"];
            gun.muzzleFlashEffects = gun3.muzzleFlashEffects;
            gun.gunClass = GunClass.RIFLE;
            gun.reloadTime = .8f;
            gun.DefaultModule.cooldownTime = .5f;
            gun.DefaultModule.numberOfShotsInClip = 2;
            gun.SetBaseMaxAmmo(200);
            gun.quality = PickupObject.ItemQuality.B;

            
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            projectile.baseData.damage = 10f;
            projectile.baseData.speed = 100000f;
            projectile.baseData.range = 0f;
            projectile.AdditionalScaleMultiplier = .00001f;



            MiscToolMethods.TrimAllGunSprites(gun);
            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }

        public Projectile current_ball;
        public int shotcountcontroller = 1;
        public Vector2 targetPoint;
        public Vector2 last_position;
        int setup = 0;
        public string playerName;
        public override void  OnPickedUpByPlayer(PlayerController player)
        {

            if (player.IsPrimaryPlayer)
            {
                playerName = "primaryplayer";
            }
            else
            {
                playerName = "secondaryplayer";
            }
            base.OnPickedUpByPlayer(player);
        }


        public override void OnPostFired(PlayerController player, Gun gun)
        {
            
            AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
            gun.PreventNormalFireAudio = true;
            
            targetPoint = player.unadjustedAimPoint;
            if (shotcountcontroller == 1)
            {
                AkSoundEngine.PostEvent("Play_OBJ_hook_shot_01", base.gameObject);
                Projectile projectile1 = ((Gun)ETGMod.Databases.Items[25]).DefaultModule.projectiles[0];
                GameObject gameObject1 = SpawnManager.SpawnProjectile(projectile1.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
                Projectile component1 = gameObject1.GetComponent<Projectile>();
                setup = setup + 1;
                PierceProjModifier stab = component1.GetComponent<PierceProjModifier>();
                stab.MaxBossImpacts = 0;
                stab.penetration = 0;

                bool flag = component1 != null;
                if (flag)
                {
                    component1.Owner = player;
                    component1.Shooter = player.specRigidbody;
                    
                    component1.baseData.speed = 40f;
                    component1.baseData.range *= 1f;
                    component1.baseData.damage = 10f;
                    component1.AppliedStunDuration = 1f;
                    component1.AppliesStun = true;
                    
                    
                    component1.pierceMinorBreakables = true;
                  
                }
                current_ball = component1;
                shotcountcontroller = 0;

                Gun gun3 = (Gun)PickupObjectDatabase.GetById(335);
                gun.muzzleFlashEffects = gun3.muzzleFlashEffects;
               
            }
            else
            {
                Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                float zRotation = BraveMathCollege.Atan2Degrees(vector);
                player.StartCoroutine(this.HandleSwing(player, vector, 100f, 2.25f));
                if(Hooked_enemy != null)
                {
                    Hooked_enemy.knockbackDoer.ApplyKnockback(-1 * vector, 100f);
                    
                }
               

                Gun gun3 = (Gun)PickupObjectDatabase.GetById(335);
                gun.muzzleFlashEffects = gun3.muzzleFlashEffects;
                gun.Update();

            }
            current_ball.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(current_ball.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.attach));
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            if(shotcountcontroller != 1)
            {
                Gun swipeFlash = (Gun)PickupObjectDatabase.GetById(335);
                PlayerController player = (PlayerController)this.gun.CurrentOwner;
                Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                ProjectileSlashingBehaviour slasher = projectile.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashDimensions = 60;
                slasher.SlashRange = 3f;
                slasher.playerKnockback = 0;
                slasher.SlashVFX = swipeFlash.muzzleFlashEffects;
                player.knockbackDoer.ApplyKnockback(vector, 25, true);
                slasher.InteractMode = SlashDoer.ProjInteractMode.IGNORE;
                
            }
            base.PostProcessProjectile(projectile);
        }

        private IEnumerator HandleSwing(PlayerController user, Vector2 aimVec, float rayDamage, float rayLength)
        {
            float elapsed = 0f;
            while (elapsed < 1)
            {
                elapsed += BraveTime.DeltaTime;
                SpeculativeRigidbody hitRigidbody = this.IterativeRaycast(user.CenterPosition, aimVec, rayLength, int.MaxValue, user.specRigidbody);
                if (hitRigidbody && hitRigidbody.aiActor && hitRigidbody.aiActor.IsNormalEnemy)
                {

                    if (user.IsPrimaryPlayer)
                    {
                        
                        hitRigidbody.aiActor.healthHaver.ApplyDamage(rayDamage, aimVec, "primaryplayer", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                          

                    }
                    else
                    {
                     
                        hitRigidbody.aiActor.healthHaver.ApplyDamage(rayDamage, aimVec, "secondaryplayer", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                        

                    }
                }
                SpeculativeRigidbody hitRigidbody2 = this.IterativeRaycast(user.CenterPosition, aimVec, rayLength, int.MaxValue, user.specRigidbody);
                if (hitRigidbody2 && hitRigidbody2.projectile && hitRigidbody2.projectile.Owner != this.gun.CurrentOwner)
                {
                    PassiveReflectItem.ReflectBullet(hitRigidbody2.projectile, true, this.gun.CurrentOwner, 15f, 1f, 1.5f, 0f);
                }
                
                    
                yield return null;
            }
            yield break;
        }

        protected SpeculativeRigidbody IterativeRaycast(Vector2 rayOrigin, Vector2 rayDirection, float rayDistance, int collisionMask, SpeculativeRigidbody ignoreRigidbody)
        {
            int num = 0;
            RaycastResult raycastResult;
            while (PhysicsEngine.Instance.Raycast(rayOrigin, rayDirection, rayDistance, out raycastResult, true, true, collisionMask, new CollisionLayer?(CollisionLayer.BulletBlocker), false, null, ignoreRigidbody))
            {
                num++;
                SpeculativeRigidbody speculativeRigidbody = raycastResult.SpeculativeRigidbody;
                if (num < 3 && speculativeRigidbody != null)
                {
                    MinorBreakable component = speculativeRigidbody.GetComponent<MinorBreakable>();
                    if (component != null)
                    {
                        component.Break(rayDirection.normalized * 3f);
                        RaycastResult.Pool.Free(ref raycastResult);
                        continue;
                    }
                }
                RaycastResult.Pool.Free(ref raycastResult);
                return speculativeRigidbody;
            }
            return null;
        }

        public AIActor Hooked_enemy = null;

       

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

                last_position = this.gun.CurrentOwner.CenterPosition;
            }
            
            base.Update();
        }
        public void attach(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            
                this.cable = arg2.aiActor.gameObject.AddComponent<ArbitraryCableDrawer>();
                this.cable.Attach1Offset = this.gun.CurrentOwner.CenterPosition - (this.gun.CurrentOwner.transform.position.XY() + new Vector2(.5f,.5f));
                this.cable.Attach2Offset = arg2.aiActor.CenterPosition - arg2.aiActor.transform.position.XY();
                this.cable.Initialize(this.gun.CurrentOwner.transform, arg2.aiActor.transform);
                Hooked_enemy = arg2.aiActor;
            
        }
        public void unhook()
        {
            if(Hooked_enemy != null)
            {
                Destroy(Hooked_enemy.GetComponent<ArbitraryCableDrawer>());
                Hooked_enemy = null;
            }
            
        }
       
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                HasReloaded = false;

                base.OnReloadPressed(player, gun, bSOMETHING);
                unhook();

                shotcountcontroller = 1;
                
                Gun gun3 = (Gun)ETGMod.Databases.Items["blunderbuss"];
                gun.muzzleFlashEffects = gun3.muzzleFlashEffects;
                gun.Update();
            }
        }
        private ArbitraryCableDrawer cable;
    }
}