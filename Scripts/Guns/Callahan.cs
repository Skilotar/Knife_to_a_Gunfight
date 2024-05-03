using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    class Callahan : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Callahan", "Callahan");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:callahan", "ski:callahan");
            gun.gameObject.AddComponent<Callahan>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Hole-Puncher");

            gun.SetLongDescription("Four charge cells power this beast of a gun. The bullet leaves the barrel with such force that it can bore through walls. \n\n\n - Knife_to_a_Gunfight");
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "Callahan_idle_001", 8);

            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 12);
            gun.SetAnimationFPS(gun.reloadAnimation, 4);
            gun.SetAnimationFPS(gun.chargeAnimation, 4);

            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(694) as Gun, true, false);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 2;

            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.PISTOL;
            gun.gunHandedness = GunHandedness.OneHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.2f;

            gun.DefaultModule.cooldownTime = .6f;
            gun.DefaultModule.numberOfShotsInClip = 4;
            gun.SetBaseMaxAmmo(250);
            gun.barrelOffset.transform.localPosition = new Vector3(1.5f, .7f, 0f);
            gun.quality = PickupObject.ItemQuality.B;
            
            
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 18f;
            projectile.baseData.speed = 20f;
            projectile.baseData.range = 20f;
            projectile.PlayerKnockbackForce = 4;
            projectile.AdditionalScaleMultiplier = 1f;

            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            gun.DefaultModule.projectiles[0] = projectile2;
            projectile2.baseData.damage = 50f;
            projectile2.baseData.speed = 20f;
            projectile2.baseData.range = 20f;
            projectile2.PlayerKnockbackForce = 8;
            projectile2.AdditionalScaleMultiplier = 2f;
            
            ImprovedAfterImage trail = projectile2.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            trail.shadowLifetime = .3f;
            trail.shadowTimeDelay = .0001f;
            trail.dashColor = new Color(.47f, .30f, .37f);
            trail.spawnShadows = true;

            projectile2.transform.parent = gun.barrelOffset;


            Projectile passed = UnityEngine.Object.Instantiate<Projectile>(((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0].projectile);
            passed.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(passed.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(passed);
            passed.SetProjectileSpriteRight("stonebulletman_shard", 11, 9, false, tk2dBaseSprite.Anchor.MiddleCenter, 11, 9);
            rocks = passed;

            Mole mole = projectile.gameObject.GetOrAddComponent<Mole>();
            mole.ischarged = false;
            mole.ShatterProj = rocks;
            Mole mole2 = projectile2.gameObject.GetOrAddComponent<Mole>();
            mole2.ischarged = true;
            mole2.ShatterProj = rocks;

            gun.gunSwitchGroup = ((Gun)PickupObjectDatabase.GetById(694)).gunSwitchGroup;

            Gun gun2 = (Gun)PickupObjectDatabase.GetById(694);
            gun.muzzleFlashEffects = gun2.muzzleFlashEffects;
            
            gun.carryPixelOffset = new IntVector2(5, 0);
            

            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 0f
            };
            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                AmmoCost = 1, 
                Projectile = projectile2,
                ChargeTime = 1f
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
                item2
            };


            ETGMod.Databases.Items.Add(gun, null, "ANY");

            Bloody = VFXToolbox.CreateVFXPool("BloodyMuzzleflash",
                new List<string>()
                {
                    "Knives/Resources/Muzzleflashes/Bloody/Bloody_muzzleflash_001",
                    "Knives/Resources/Muzzleflashes/Bloody/Bloody_muzzleflash_002",
                    "Knives/Resources/Muzzleflashes/Bloody/Bloody_muzzleflash_003",
                    "Knives/Resources/Muzzleflashes/Bloody/Bloody_muzzleflash_004",
                },
                12, //FPS
                new IntVector2(55, 27), //Dimensions
                tk2dBaseSprite.Anchor.MiddleLeft, //Anchor
                false, //Uses a Z height off the ground
                0, //The Z height, if used
                false,
               VFXAlignment.Fixed
                  );
            ID = gun.PickupObjectId;
        }

        public static int ID;
        public static VFXPool Bloody;
        public static Projectile rocks;

        public override void OnPostFired(PlayerController player, Gun gun)
        {

        }
        public System.Random rng = new System.Random();
        public override void PostProcessProjectile(Projectile projectile)
        {
           
            base.PostProcessProjectile(projectile);
        }



        private bool HasReloaded;
       
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
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                

            }
        }


    }

    public class Mole: MonoBehaviour
    {
        public Mole()
        {

        }

        private void Start()
        {
            AkSoundEngine.PostEvent("Play_half_gauge_fire", base.gameObject);

            this.m_projectile = base.GetComponent<Projectile>();
            if (this.m_projectile.Owner is PlayerController)
            {
                this.projOwner = this.m_projectile.Owner as PlayerController;
            }
            SpeculativeRigidbody specRigidBody = this.m_projectile.specRigidbody;
            this.m_projectile.BulletScriptSettings.surviveTileCollisions = true;
            this.m_projectile.BulletScriptSettings.surviveRigidbodyCollisions = true;
            this.m_projectile.pierceMinorBreakables = true;
            m_projectile.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(m_projectile.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreCollisontile));
            m_projectile.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(m_projectile.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison));
            
        }
        
        public bool isinwall = false;
        private void OnPreCollisontile(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
        {
            if(PiercedOneWall != true)
            {
                
                isinwall = true;

                PhysicsEngine.SkipCollision = true;
            }
            else
            {
                m_projectile.DieInAir();

            }

        }
        public AIActor Stabbed = null;
        private void OnPreCollison(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)//Non Wall Collision
        {
            if (otherRigidbody.aiActor)
            {
                if(this.projOwner.PlayerHasActiveSynergy("Puncture Wound"))
                {
                    if(otherRigidbody.aiActor != Stabbed)
                    {
                        m_projectile.AdditionalScaleMultiplier = 1.2f;
                        m_projectile.baseData.damage *= 1.2f;
                        m_projectile.ResetDistance();
                       
                        GameObject gameObject = SpawnManager.SpawnProjectile(ShatterProj.gameObject, m_projectile.LastPosition, Quaternion.Euler(0f, 0f, (projOwner.CurrentGun == null) ? 0f : m_projectile.transform.eulerAngles.magnitude), true);
                        Projectile component = gameObject.GetComponent<Projectile>();
                        component.Owner = this.projOwner;
                        ProjectileSlashingBehaviour Blood = component.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
                        Blood.SlashDamage = 8f;
                        Blood.SlashDimensions = 45f;
                        Blood.SlashVFX = Callahan.Bloody;
                        Blood.DestroyBaseAfterFirstSlash = true;
                        Blood.soundToPlay = null;
                        Blood.SlashRange = 5f;
                        Stabbed = otherRigidbody.aiActor;
                        AkSoundEngine.PostEvent("Play_OBJ_candle_splat_01", base.gameObject);
                        AkSoundEngine.PostEvent("Play_OBJ_candle_splat_01", base.gameObject);
                        AkSoundEngine.PostEvent("Play_OBJ_candle_splat_01", base.gameObject);
                    }

                }
            }
            else
            {
                if (otherRigidbody.minorBreakable == false && otherRigidbody.projectile == false)
                {
                    if (PiercedOneWall != true)
                    {
                        PhysicsEngine.SkipCollision = true;
                    }
                    else
                    {
                        m_projectile.DieInAir();

                    }
                }
                   
            }

        }

       
        private void Update()
        {
            keeptrackofinwallstate();
        }
        bool Once = false;
        bool PiercedOneWall = false;
        private void keeptrackofinwallstate()
        {
            if(isinwall == true) // if was in wall
            {
                if(m_projectile.specRigidbody.HasTriggerCollisions == false) // check if leaves the wall because no collisions
                {
                    isinwall = false;
                    Once = true;
                }
            }
            else
            {
                
                if(m_projectile.LastPosition.GetAbsoluteRoom() != null)
                {

                    if (m_projectile.LastPosition.GetAbsoluteRoom() == this.projOwner.CurrentRoom)
                    {
                        if (Once == true)
                        {
                            if (ischarged == true)
                            {
                                int angle = -8;

                                for (int i = 0; i < 9; i++)
                                {
                                    float angleadjusted = angle;
                                    if (angle == -6)// set top side pattern
                                    {
                                        angleadjusted = -7.5f;
                                    }

                                    if (angle == 6)// set bottom side pattern
                                    {
                                        angleadjusted = 7.5f;
                                    }

                                    GameObject gameObject = SpawnManager.SpawnProjectile(ShatterProj.gameObject, m_projectile.LastPosition, Quaternion.Euler(0f, 0f, (projOwner.CurrentGun == null) ? 0f : m_projectile.transform.eulerAngles.magnitude + angleadjusted), true);
                                    Projectile component = gameObject.GetComponent<Projectile>();
                                    bool flag2 = component != null;
                                    if (flag2)
                                    {

                                        component.Owner = projOwner;
                                        component.Shooter = projOwner.specRigidbody;
                                        component.baseData.damage = 4f;
                                        component.baseData.speed = 15f;
                                        component.AdditionalScaleMultiplier = (1 * UnityEngine.Random.Range(.4f, 1.1f));
                                        component.baseData.range = 7f;
                                        component.baseData.range += UnityEngine.Random.Range(-1, 8);
                                        component.baseData.speed *= (1f - (Math.Abs(angle) / 40));
                                        component.transform.eulerAngles.RotateBy(Quaternion.Euler(0, 0, UnityEngine.Random.Range(-180f, 180f)));
                                        component.baseData.speed += UnityEngine.Random.Range(1, 8);
                                        component.UpdateSpeed();
                                        if (UnityEngine.Random.Range(1, 11) == 1)
                                        {
                                            projOwner.DoPostProcessProjectile(component);
                                        }
                                    }
                                    angle++;
                                    angle++; 
                                }

                            }
                            else
                            {
                                int angle = -12;

                                for (int i = 0; i < 6; i++)
                                {
                                    float angleadjusted = angle;


                                    GameObject gameObject = SpawnManager.SpawnProjectile(ShatterProj.gameObject, m_projectile.LastPosition, Quaternion.Euler(0f, 0f, (projOwner.CurrentGun == null) ? 0f : m_projectile.transform.eulerAngles.magnitude + angleadjusted), true);
                                    Projectile component = gameObject.GetComponent<Projectile>();
                                    bool flag2 = component != null;
                                    if (flag2)
                                    {

                                        component.Owner = projOwner;
                                        component.Shooter = projOwner.specRigidbody;
                                        component.baseData.damage = 3f;
                                        component.baseData.speed = 15f;
                                        component.AdditionalScaleMultiplier = (1 * UnityEngine.Random.Range(.3f, 1f));
                                        component.baseData.range = 7f;
                                        component.baseData.range += UnityEngine.Random.Range(-1, 8);
                                        component.baseData.speed *= (1f - (Math.Abs(angle) / 40));
                                        component.transform.eulerAngles.RotateBy(Quaternion.Euler(0, 0, UnityEngine.Random.Range(-180f, 180f)));
                                        component.baseData.speed += UnityEngine.Random.Range(1, 8);
                                        component.UpdateSpeed();
                                        if (UnityEngine.Random.Range(1, 11) == 1)
                                        {
                                            projOwner.DoPostProcessProjectile(component);
                                        }
                                    }
                                    angle++;
                                    angle++;
                                    angle++;
                                    angle++;
                                }

                            }

                            if (projOwner.PlayerHasActiveSynergy("Paper Pusher"))
                            {
                                int angle = -6;

                                for (int i = 0; i < 3; i++)
                                {
                                    float angleadjusted = angle;
                                    Gun paper = (Gun)PickupObjectDatabase.GetById(477);

                                    GameObject gameObject = SpawnManager.SpawnProjectile(paper.DefaultModule.projectiles[0].projectile.gameObject, m_projectile.LastPosition, Quaternion.Euler(0f, 0f, (projOwner.CurrentGun == null) ? 0f : m_projectile.transform.eulerAngles.magnitude + angleadjusted), true);
                                    Projectile component = gameObject.GetComponent<Projectile>();
                                    bool flag2 = component != null;
                                    if (flag2)
                                    {

                                        component.Owner = projOwner;
                                        component.Shooter = projOwner.specRigidbody;
                                        component.baseData.speed = 15f;
                                        component.AdditionalScaleMultiplier = .75f;

                                    }
                                    angle += 6;

                                }
                            }
                            m_projectile.baseData.damage *= 1.7f;
                            AkSoundEngine.PostEvent("Play_OBJ_rock_break_01", base.gameObject);

                            PiercedOneWall = true;


                            Once = false;
                        }
                    }
                    else
                    {
                        m_projectile.DieInAir();
                    }
                }
              
            }
        }

        public Projectile ShatterProj;
        public bool ischarged = false;
        private PlayerController projOwner;
        private Projectile m_projectile;
    }
}