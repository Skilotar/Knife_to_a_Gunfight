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

    public class EvilEye : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Evil Eye", "EvilEye");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:evil_eye", "ski:evil_eye");
            gun.gameObject.AddComponent<EvilEye>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("All Eyes On YOU!");
            gun.SetLongDescription("Reload to mark an enemy and gain perfect accuracy on them.\n\n" +
                "A rifle designed by an obsessive hitman who would stop at nothing to get his mark. " +
                "" +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "EvilEye_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 17);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(345) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 10f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = .8f;
            gun.DefaultModule.cooldownTime = .17f;
            gun.DefaultModule.numberOfShotsInClip = 15;
            gun.SetBaseMaxAmmo(600);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.B;

            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            gun.barrelOffset.transform.localPosition = new Vector3(2.3f, .5f, 0f);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.AdditionalScaleMultiplier *= .5f;
            projectile.baseData.damage = 10f;
            projectile.baseData.speed *= .9f;
           
            projectile.gameObject.GetOrAddComponent<EvilEyeSpecialTracking>();
            gun.shellsToLaunchOnFire = 1;
            Gun gun2 = PickupObjectDatabase.GetById(84) as Gun;
            gun.shellCasing = gun2.shellCasing;

            gun.muzzleFlashEffects.type = VFXPoolType.None;

            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.FULLAUTO;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;
           

        }

        public static int ID;
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_shot_01", base.gameObject);

        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            if(projectile.GetComponent<EvilEyeSpecialTracking>() != null)
            {
                EvilEyeSpecialTracking eye = projectile.GetComponent<EvilEyeSpecialTracking>();
                if((this.gun.CurrentOwner as PlayerController).PlayerHasActiveSynergy("Expanded Ram"))
                {
                    eye.targetActor = TargetAIList;

                }
                else
                {
                    eye.targetActor.Add(TargetAI);

                }

            }

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
                if ((this.gun.CurrentOwner as PlayerController).PlayerHasActiveSynergy("Expanded Ram"))
                {
                   
                    if(TargetAIList != null)
                    {
                        
                        if (TargetAIList.Count > 0 )
                        {
                            if (TargetAIList[TargetAIList.Count - 1].healthHaver != null)
                            {
                                if (TargetAIList[TargetAIList.Count - 1].healthHaver.IsDead == true)
                                {

                                    TargetAIList.RemoveAt(TargetAIList.Count - 1);
                                    if (m_extantLockOnSprite != null)
                                    {
                                        SpawnManager.Despawn(this.m_extantLockOnSprite);
                                    }
                                    if (TargetAIList.Count > 1) // move down one
                                    {
                                        
                                        if(TargetAIList[TargetAIList.Count - 1].healthHaver.IsDead != true)
                                        {
                                           
                                            this.m_extantLockOnSprite = TargetAIList[TargetAIList.Count - 1].PlayEffectOnActor((GameObject)BraveResources.Load("Global VFX/VFX_LockOn_Predator", ".prefab"), Vector3.zero, true, true, true);
                                        }
                                       
                                    }

                                }

                            }
                        }
                        else
                        {
                            if (m_extantLockOnSprite != null)
                            {
                                SpawnManager.Despawn(this.m_extantLockOnSprite);
                            }
                            
                        }
                       

                    }

                }
                else
                {
                    if (TargetAI != null)
                    {
                        if (TargetAI.healthHaver.IsDead == true)
                        {
                            if (m_extantLockOnSprite != null)
                            {
                                SpawnManager.Despawn(this.m_extantLockOnSprite);
                            }
                            
                        }
                    }

                }


            }

        }



        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            
            if (gun.IsReloading && this.HasReloaded)
            {
                if(m_extantLockOnSprite != null)
                {
                    SpawnManager.Despawn(this.m_extantLockOnSprite);
                }

                foreach(AIActor bois in TargetAIList)
                {
                    if(bois != null)
                    {
                        if (bois.healthHaver.IsDead != true)
                        {
                            bois.ClearOverrideOutlineColor();
                        }
                       
                    }
                   
                }

                TargetAIList.Clear();
                TargetAI = null;
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_zapper_shot_01", base.gameObject);

                if ((this.gun.CurrentOwner as PlayerController).PlayerHasActiveSynergy("Expanded Ram"))
                {
                    Projectile projectile1 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
                    GameObject gameObject1 = SpawnManager.SpawnProjectile(projectile1.gameObject, gun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
                    Projectile component1 = gameObject1.GetComponent<Projectile>();

                    bool flag2 = component1 != null;
                    if (flag2)
                    {
                        component1.Owner = player;
                        component1.Shooter = player.specRigidbody;
                        component1.baseData.damage = 1f;

                        ProjectileSlashingBehaviour slashy = component1.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
                        slashy.OnSlashHitEnemy += Slashy_OnSlashHitEnemy;
                        slashy.SlashVFX.type = VFXPoolType.None;
                        slashy.DoSound = false;
                        slashy.SlashDamage = 1;
                        slashy.SlashRange = 15;
                        slashy.SlashDimensions = 22;
                        Color color = new Color(0, 1, 0);


                        if (player.CurrentGun.sprite.FlipY == false)
                        {
                            UnityEngine.GameObject laser = RenderLaserSight(gun.barrelOffset.transform.TransformPoint(-.1f, .25f, 0), 500, 2, player.CurrentGun.CurrentAngle - 22, true, color);
                            StartCoroutine(LaserDespawn(laser));
                            UnityEngine.GameObject laser2 = RenderLaserSight(gun.barrelOffset.transform.TransformPoint(-.1f, .25f, 0), 500, 2, player.CurrentGun.CurrentAngle + 22, true, color);
                            StartCoroutine(LaserDespawn(laser2));
                        }
                        else
                        {
                            UnityEngine.GameObject laser = RenderLaserSight(gun.barrelOffset.transform.TransformPoint(-.1f, -.25f, 0), 500, 2, player.CurrentGun.CurrentAngle - 22, true, color);
                            StartCoroutine(LaserDespawn(laser));
                            UnityEngine.GameObject laser2 = RenderLaserSight(gun.barrelOffset.transform.TransformPoint(-.1f, -.25f, 0), 500, 2, player.CurrentGun.CurrentAngle + 22, true, color);
                            StartCoroutine(LaserDespawn(laser2));
                        }
                       
                    }
                }
                else
                {
                    Projectile projectile = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
                    GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, gun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
                    Projectile component = gameObject.GetComponent<Projectile>();
                    component.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(component.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy));

                    bool flag2 = component != null;
                    if (flag2)
                    {
                        component.Owner = player;
                        component.Shooter = player.specRigidbody;
                        component.baseData.damage = 1f;
                        component.baseData.speed *= 10;
                        component.AdditionalScaleMultiplier = .5f;
                        component.sprite.renderer.enabled = false;
                        component.pierceMinorBreakables = true;
                        Color color = new Color(0, 1, 0);
                        if (player.CurrentGun.sprite.FlipY == false)
                        {
                            UnityEngine.GameObject laser = RenderLaserSight(gun.barrelOffset.transform.TransformPoint(-.1f, .25f, 0), 500, 2, player.CurrentGun.CurrentAngle, true, color);
                            StartCoroutine(LaserDespawn(laser));
                        }
                        else
                        {
                            UnityEngine.GameObject laser = RenderLaserSight(gun.barrelOffset.transform.TransformPoint(-.1f, -.25f, 0), 500, 2, player.CurrentGun.CurrentAngle, true, color);
                            StartCoroutine(LaserDespawn(laser));
                        }


                    }



                }
            }

        }

        private void Slashy_OnSlashHitEnemy(PlayerController player, AIActor hitEnemy, Vector2 forceDirection)
        {
            if (hitEnemy.healthHaver.IsDead != true)
            {
                TargetAIList.Add(hitEnemy);
                Color color = new Color(0, 1, 0);
                hitEnemy.SetOverrideOutlineColor(color);
                this.m_extantLockOnSprite = TargetAIList[TargetAIList.Count -1].PlayEffectOnActor((GameObject)BraveResources.Load("Global VFX/VFX_LockOn_Predator", ".prefab"), Vector3.zero, true, true, true);

            }

        }

        private IEnumerator LaserDespawn(GameObject laser)
        {
            yield return new WaitForSeconds(.1f);
            UnityEngine.GameObject.Destroy(laser);
        }

        private void HandleHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2.aiActor.healthHaver.IsDead != true)
            {
                TargetAI = arg2.aiActor;
                
                this.m_extantLockOnSprite = TargetAI.PlayEffectOnActor((GameObject)BraveResources.Load("Global VFX/VFX_LockOn_Predator", ".prefab"), Vector3.zero, true, true, true);
                    
            }
        }

        public static GameObject RenderLaserSight(Vector2 position, float length, float width, float angle, bool alterColour = false, Color? colour = null)
        {
            GameObject laserSightPrefab = LoadHelper.LoadAssetFromAnywhere("assets/resourcesbundle/global vfx/vfx_lasersight.prefab") as GameObject;

            GameObject gameObject = SpawnManager.SpawnVFX(laserSightPrefab, position, Quaternion.Euler(0, 0, angle));

            tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
            float newWidth = 1f;
            if (width != -1) newWidth = width;
            component2.dimensions = new Vector2(length, newWidth);
            if (alterColour && colour != null)
            {
                component2.usesOverrideMaterial = true;
                component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                component2.sprite.renderer.material.SetColor("_OverrideColor", (Color)colour);
                component2.sprite.renderer.material.SetColor("_EmissiveColor", (Color)colour);
                component2.sprite.renderer.material.SetFloat("_EmissivePower", 100);
                component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f);
            }
            return gameObject;
        }

        public List<AIActor> TargetAIList = new List<AIActor>
        { };
        public AIActor TargetAI;
        private GameObject m_extantLockOnSprite;
    }
}
