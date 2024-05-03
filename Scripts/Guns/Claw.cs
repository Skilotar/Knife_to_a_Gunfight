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
    class Claw : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Kralle", "Claw");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:kralle", "ski:kralle");
            gun.gameObject.AddComponent<Claw>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Helping Hand");
            gun.SetLongDescription("Grab and Throw enemy projectiles. \n" +
                "An electromagnetically propelled freight loading arm extension. This was an early mockup design made for assisting cargo starship loaders. Later it was abandoned for the use of large exoskeletons called Loaders." +
                "\n\n\n - Knife_to_a_Gunfight");

            gun.SetupSprite(null, "Claw_idle_001", 8);
            gun.SetAnimationFPS(gun.chargeAnimation, 5);
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);
            
            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(15) as Gun, true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.DefaultModule.preventFiringDuringCharge = true;
            
            gun.reloadTime = 1.2f;
            gun.DefaultModule.cooldownTime = .0f;
            gun.gunClass = GunClass.CHARGE;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.SetBaseMaxAmmo(100);
            gun.quality = PickupObject.ItemQuality.A;

            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            gun.carryPixelOffset = new IntVector2(13, -3);
            gun.barrelOffset.transform.localPosition = new Vector3(22 / 16f, 10 / 16f, 0f);

            projectile.baseData.damage = 0f;
            projectile.baseData.speed = 1f;
            projectile.baseData.range = .001f;
            projectile.sprite.renderer.enabled = false;
            projectile.hitEffects.suppressMidairDeathVfx = true;

            projectile.transform.parent = gun.barrelOffset;
            gun.gameObject.GetOrAddComponent<GunSpecialStates>().ReturnHitList = true;

            
            catcher = projectile;

            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = .15f,
                AmmoCost = 0,
            };
            
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
            };

            MiscToolMethods.TrimAllGunSprites(gun);
            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }

        public static Projectile catcher;
        public override void OnDropped()
        {

            if (m_laser1 != null) UnityEngine.GameObject.Destroy(m_laser1);
            if (m_laser2 != null) UnityEngine.GameObject.Destroy(m_laser2);
            if (m_laser3 != null) UnityEngine.GameObject.Destroy(m_laser3);
            if (m_laser4 != null) UnityEngine.GameObject.Destroy(m_laser4);
            if (m_laser5 != null) UnityEngine.GameObject.Destroy(m_laser5);

            GunSpecialStates state = gun.gameObject.GetOrAddComponent<GunSpecialStates>();
            if (state.returnList != null)
            {
                for (int i = state.returnList.Count - 1; i >= 0; i--)
                {
                    if (state.returnList[i] != null)
                    {
                        state.returnList[i].DieInAir();
                    }
                }
                state.returnList.Clear();
            }

                base.OnDropped();
        }



        public override void OnGunsChanged(Gun previous, Gun current, bool newGun)
        {
            if (this.gun == previous || this.gun == current)
            {
                if (m_laser1 != null) UnityEngine.GameObject.Destroy(m_laser1);
                if (m_laser2 != null) UnityEngine.GameObject.Destroy(m_laser2);
                if (m_laser3 != null) UnityEngine.GameObject.Destroy(m_laser3);
                if (m_laser4 != null) UnityEngine.GameObject.Destroy(m_laser4);
                if (m_laser5 != null) UnityEngine.GameObject.Destroy(m_laser5);

                GunSpecialStates state = gun.gameObject.GetOrAddComponent<GunSpecialStates>();
                if (state.returnList != null)
                {
                    for (int i = state.returnList.Count - 1; i >= 0; i--)
                    {
                        if (state.returnList[i] != null)
                        { 
                            state.returnList[i].DieInAir();
                        }
                    }
                    state.returnList.Clear();
                }

            }


            base.OnGunsChanged(previous, current, newGun);
        }
        public bool postshot = false;

        private bool HasReloaded;
        private bool once = true;
        public Color color = UnityEngine.Color.red;


        private IEnumerator postShotFlashy()
        {
            color = UnityEngine.Color.white;
            yield return new WaitForSeconds(.2f);
            color = UnityEngine.Color.red;
        }

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
                if (gun.GetChargeFraction() == 1)
                {

                    if (once)
                    {
                        //Catch collection
                        once = false;
                        Projectile proj = MiscToolMethods.SpawnProjAtPosi(catcher, gun.barrelOffset.transform.position, Owner as PlayerController, gun);
                        ProjectileSlashingBehaviour stabby = proj.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
                        stabby.InteractMode = SlashDoer.ProjInteractMode.RETURNPROJ;
                        stabby.DestroyBaseAfterFirstSlash = true;
                        stabby.SlashDamageUsesBaseProjectileDamage = false;
                        stabby.SlashDamage = 1;
                        stabby.SlashDimensions = 30;
                        stabby.SlashRange = 5;

                        AkSoundEngine.PostEvent("Play_WPN_radiationlaser_shot_01", base.gameObject);
                        AkSoundEngine.PostEvent("Play_WPN_tachyon_loop_01", base.gameObject);
                        StartCoroutine(postShotFlashy());
                        postshot = false;

                    }

                    if (gun.gameObject.GetOrAddComponent<GunSpecialStates>().ReturnHitList == true)
                    {
                        GunSpecialStates state = gun.gameObject.GetOrAddComponent<GunSpecialStates>();
                        if (state.returnList != null)
                        {
                            for (int i = state.returnList.Count - 1; i >= 0; i--)
                            {
                                if (state.returnList[i] != null)
                                {
                                    Projectile caught = state.returnList[i];
                                    caught.baseData.damage = (5 + (40 / (state.returnList.Count))) * (Owner as PlayerController).stats.GetStatModifier(PlayerStats.StatType.Damage);
                                    caught.transform.parent = gun.barrelOffset.transform;
                                    caught.specRigidbody.enabled = false;
                                    caught.specRigidbody.Reinitialize();
                                    caught.ResetDistance();
                                }
                            }
                        }
                       
                    }

                    if (m_laser2 == null && m_laser1 == null && this.gun.IsReloading == false && (this.Owner as PlayerController).IsDodgeRolling == false && this.gun.ClipShotsRemaining != 0 && postshot != true)
                    {
                        StartCoroutine(doFunkyLaserSightGarbage(this.Owner as PlayerController));
                    }


                }
                else
                {
                    if(once == false)
                    {
                        
                        once = true;
                        postshot = true;
                        AkSoundEngine.PostEvent("Stop_WPN_tachyon_loop_01", base.gameObject);
                        AkSoundEngine.PostEvent("Play_Beew_001", base.gameObject);
                        if (gun.gameObject.GetOrAddComponent<GunSpecialStates>().ReturnHitList == true)
                        {
                            GunSpecialStates state = gun.gameObject.GetOrAddComponent<GunSpecialStates>();
                            if (state.returnList != null)
                            {
                                for (int i = state.returnList.Count - 1; i >= 0; i--)
                                {
                                    if (state.returnList[i] != null)
                                    {
                                        Projectile shoot = state.returnList[i];
                                        shoot.transform.SetParent(null, true);
                                        shoot.specRigidbody.enabled = true;
                                        shoot.specRigidbody.Reinitialize();
                                        Vector2 vec = gun.barrelOffset.transform.TransformPoint(new Vector3(10, 0, 0)) - shoot.LastPosition;
                                        shoot.baseData.speed = 10 + (30 / (state.returnList.Count));
                                        
                                        shoot.UpdateSpeed();
                                        shoot.SendInDirection(vec, true, true);
                                       
                                    }
                                }
                            }
                           
                            state.returnList.Clear();
                        }
                    }
                }

            }



            base.Update();
        }

        public GameObject m_laser1;
        public GameObject m_laser2;
        public GameObject m_laser3;
        public GameObject m_laser4;
        public GameObject m_laser5;

        private IEnumerator doFunkyLaserSightGarbage(PlayerController player) // this is trash... 
        {


            
            UnityEngine.GameObject laser1 = RenderLaserSight(gun.barrelOffset.transform.position, 120, 1, player.CurrentGun.CurrentAngle + 30, true, color);
            
            UnityEngine.GameObject laser2 = RenderLaserSight(gun.barrelOffset.transform.position, 120, 1, player.CurrentGun.CurrentAngle - 30, true, color);

            UnityEngine.GameObject laser3 = RenderLaserSight(gun.barrelOffset.transform.position, 80, 1, player.CurrentGun.CurrentAngle, true, color);
            UnityEngine.GameObject laser4 = RenderLaserSight(gun.barrelOffset.transform.position, 50, 1, player.CurrentGun.CurrentAngle + 15, true, color);
            UnityEngine.GameObject laser5 = RenderLaserSight(gun.barrelOffset.transform.position, 50, 1, player.CurrentGun.CurrentAngle - 15 , true, color);
            m_laser1 = laser1;
            m_laser2 = laser2;
            m_laser3 = laser3;
            m_laser4 = laser4;
            m_laser5 = laser5;

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            UnityEngine.GameObject.Destroy(laser1);
            UnityEngine.GameObject.Destroy(laser2);
            UnityEngine.GameObject.Destroy(laser3);
            UnityEngine.GameObject.Destroy(laser4);
            UnityEngine.GameObject.Destroy(laser5);

            m_laser1 = null;
            m_laser2 = null;
            m_laser3 = null;
            m_laser4 = null;
            m_laser5 = null;
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
