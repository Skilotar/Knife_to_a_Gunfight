
using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;
using Alexandria.SoundAPI;
using System.Collections.Generic;

namespace Knives
{

    public class Prawncher : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Prawncher", "Prawncher");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:prawncher", "ski:prawncher");
            gun.gameObject.AddComponent<Prawncher>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Bubble Buddy");
            gun.SetLongDescription("A subspecies of pistol shrimp, blaster prawns use their innate magic abilities " +
                "to bless seawater with extreme tensile strength, perfect for blowing bounties of beautiful bubbles. \n\n" +
                "Shelly here is more than happy to help your on your oceanic escapades." +

                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "Prawncher_idle_001", 6);
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
           
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 10f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.gunHandedness = GunHandedness.HiddenOneHanded;
            gun.reloadTime = 1.3f;
            gun.DefaultModule.cooldownTime = .14f;
            gun.DefaultModule.numberOfShotsInClip = 16;
            gun.SetBaseMaxAmmo(400);
            gun.quality = PickupObject.ItemQuality.B;
           
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(199) as Gun).muzzleFlashEffects;

            gun.barrelOffset.transform.localPosition = new Vector3(21/16f, 13/16f, 0f);
            gun.carryPixelOffset = new IntVector2(5, 0);


            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            
            PierceProjModifier stabby = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            stabby.penetration = 9;
            projectile.baseData.damage = 4f;
            projectile.baseData.force = 0;
            projectile.baseData.range *= 1f;
            projectile.baseData.speed *= 1f;
            MiscToolMethods.EasyAnimate(projectile, new List<string>() {
                "bubble_projectile_big_001",
                "bubble_projectile_big_002",
                "bubble_projectile_big_003",
            }, 3, new IntVector2(18, 18), 3, false, tk2dSpriteAnimationClip.WrapMode.Loop);
            projectile.enemyImpactEventName = "";
            projectile.objectImpactEventName = "";
            gun.OverrideNormalFireAudioEvent = (PickupObjectDatabase.GetById(599) as Gun).OverrideNormalFireAudioEvent;
            projectile.hitEffects = (PickupObjectDatabase.GetById(599) as Gun).DefaultModule.projectiles[0].projectile.hitEffects;
            
            SlowDownOverTimeModifier slow = projectile.gameObject.GetOrAddComponent<SlowDownOverTimeModifier>();
            slow.extendTimeByRangeStat = true;
            slow.doRandomTimeMultiplier = true;
            slow.killAfterCompleteStop = true;
            slow.timeTillKillAfterCompleteStop = 2;

            PrawncherBubbles bubbles = projectile.gameObject.GetOrAddComponent<PrawncherBubbles>();

            gun.DefaultModule.projectiles[0] = projectile;
            gun.DefaultModule.usesOptionalFinalProjectile = true;
            gun.DefaultModule.numberOfFinalProjectiles = 1;
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(507) as Gun).gunSwitchGroup;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "Tear";

            gun.DefaultModule.finalAmmoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.finalCustomAmmoType = "evo1";

            Projectile final = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(229) as Gun).DefaultModule.projectiles[0].projectile);
            final.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(final.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(final);
            
            PierceProjModifier stabby2 = final.gameObject.GetOrAddComponent<PierceProjModifier>();
            stabby2.penetration = 2;
            final.baseData.damage = 20f;
            final.baseData.force = 0;
            final.baseData.range *= 1.5f;
            final.baseData.speed *= 1.5f;
            final.baseData.range *= 2f;

            gun.DefaultModule.finalProjectile = final;

            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.FULLAUTO;
            ETGMod.Databases.Items.Add(gun, null, "ANY");


            ID = gun.PickupObjectId;

        }

        public static int ID;

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            if(projectile.GetCachedBaseDamage == 3)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                if (UnityEngine.Random.Range(0,2) < 1)
                {
                    AkSoundEngine.PostEvent("Play_Bubble_shot_001", base.gameObject);
                }
                else
                {
                    AkSoundEngine.PostEvent("Play_Bubble_shot_002", base.gameObject);
                }
               
            }

            if(projectile.GetCachedBaseDamage == 20) // final shot
            {
                FixBubbleBullets(this.gun.CurrentOwner as PlayerController, projectile);
                AkSoundEngine.PostEvent("Play_WPN_superdowser_shot_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_WPN_superdowser_shot_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_WPN_superdowser_shot_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_WPN_superdowser_shot_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_WPN_megablaster_shot_01", base.gameObject);
            }
            base.PostProcessProjectile(projectile);
        }

        public static void FixBubbleBullets(PlayerController player, Projectile pop_proj)
        {
            List<Projectile> list = new List<Projectile>();
            var allProjectiles = StaticReferenceManager.AllProjectiles;
            for (int i = 0; i < allProjectiles.Count; i++)
            {
                Projectile projectile = allProjectiles[i];
                if(projectile != null)
                {
                    if (projectile.Owner != null)
                    {
                        if (projectile.Owner == player)
                        {
                            if (projectile.gameObject.GetComponent<PrawncherBubbles>())
                            {
                                PrawncherBubbles pop = projectile.gameObject.GetComponent<PrawncherBubbles>();
                                pop.Pop_proj = pop_proj;
                            }
                        }
                    }
                }
               
            }
        }

        private bool HasReloaded;

        public override void Update()
        {
            if (gun.CurrentOwner)
            {

                if (!gun.PreventNormalFireAudio)
                {
                    //this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded) 
                {
                    this.HasReloaded = true;
                   
                }
                if(gun.ClipShotsRemaining == 1)
                {
                    gun.DefaultModule.angleVariance = 0f;
                }
                else
                {
                    gun.DefaultModule.angleVariance = 14f;
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
                AkSoundEngine.PostEvent("Play_OBJ_water_flow_01", base.gameObject);

            }

        }

    }

    
    public class PrawncherBubbles : MonoBehaviour
    {

        public PrawncherBubbles()
        {

        }
        private void Start()
        {
            try
            {
                proj = base.gameObject.GetComponent<Projectile>();
                this.parentOwner = this.proj.ProjectilePlayerOwner();
                if (this.parentOwner.PlayerHasActiveSynergy("Catch and Release"))
                {
                    SlowDownOverTimeModifier slow = proj.gameObject.GetOrAddComponent<SlowDownOverTimeModifier>();
                    slow.extendTimeByRangeStat = true;
                    slow.doRandomTimeMultiplier = true;
                    slow.targetSpeed = 1;
                    slow.killAfterCompleteStop = true;
                    slow.timeTillKillAfterCompleteStop = 1;
                    this.proj.collidesWithProjectiles = true;
                    this.proj.UpdateCollisionMask();
                    SpeculativeRigidbody specRigidbody = this.proj.specRigidbody;
                    SpeculativeRigidbody speculativeRigidbody = specRigidbody;
                    speculativeRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(speculativeRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision));

                }
            }
            catch (Exception e)
            {
                ETGModConsole.Log(e.Message);
                ETGModConsole.Log(e.StackTrace);
            }
        }

        

        private void Update()
        {

            if(Pop_proj != null)
            {
                
                if (Vector2.Distance(Pop_proj.LastPosition, proj.LastPosition) <= 1.2f)
                {
                    
                    for (int i = 0; i <= 1; i++)
                    {
                        
                        Projectile pop = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, proj.LastPosition, parentOwner, UnityEngine.Random.Range(-360f, 360f));
                        pop.DefaultTintColor = ExtendedColours.MikuBlue;
                        pop.HasDefaultTint = true;
                        pop.baseData.damage = 10;
                        pop.enemyImpactEventName = "Play_WPN_fish_impact_01";
                        pop.objectImpactEventName = "Play_WPN_fish_impact_01";

                        GoopModifier puddle = pop.gameObject.GetOrAddComponent<GoopModifier>();
                        puddle.SpawnGoopOnCollision = true;
                        puddle.CollisionSpawnRadius = 2;
                        puddle.goopDefinition = EasyGoopDefinitions.WaterGoop;

                        ImprovedAfterImage trail = pop.gameObject.GetOrAddComponent<ImprovedAfterImage>();
                        trail.shadowLifetime = .05f;
                        trail.shadowTimeDelay = .01f;
                        trail.dashColor = ExtendedColours.MikuBlue;
                        trail.spawnShadows = true;
                        trail.maxEmission = 100;
                        trail.minEmission = 50;
                        
                    }
                    if (this.parentOwner.PlayerHasActiveSynergy("Catch and Release"))
                    {

                        proj.DieInAir();
                    }
                    else
                    {
                        proj.DieInAir();

                    }
                }
               
            }
        }

        private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            bool flag = otherRigidbody && otherRigidbody.projectile;
            if (flag)
            {
                bool flag2 = otherRigidbody.projectile.Owner is AIActor;
                if (flag2)
                {
                    
                    if (otherRigidbody.projectile.gameObject.GetComponent<BubbleBulletMod>() ==  null)
                    {
                        
                        BubbleBulletMod bubbly = otherRigidbody.projectile.gameObject.GetOrAddComponent<BubbleBulletMod>();
                        bubbly.player = this.parentOwner;

                        SpawnManager.PoolManager.Remove(otherRigidbody.projectile.transform);
                        PassiveReflectItem.ReflectBullet(otherRigidbody.projectile, true, parentOwner, 2f, 1f, .5f, 0f);
                        otherRigidbody.projectile.RemoveBulletScriptControl();
                        otherRigidbody.projectile.AdjustPlayerProjectileTint(ExtendedColours.honeyYellow, 1);

                        otherRigidbody.projectile.BulletScriptSettings.preventPooling = true;
                        otherRigidbody.projectile.baseData.speed *= .3f;

                        otherRigidbody.projectile.baseData.damage *= 0.5f;
                        otherRigidbody.projectile.UpdateSpeed();
                        otherRigidbody.projectile.persistTime = 7;
                        otherRigidbody.projectile.Speed = 0;
                        otherRigidbody.projectile.specRigidbody.enabled = false;

                        ChildProjCleanup cleanup = otherRigidbody.projectile.gameObject.GetOrAddComponent<ChildProjCleanup>();
                        cleanup.delay = 7f;
                        cleanup.parentProjectile = proj;
                        cleanup.doColorCleanup = false;
                        cleanup.doColor = false;

                        bubbly.m_bubble = proj;
                        proj.baseData.speed *= .5f;
                        proj.AdditionalScaleMultiplier *= 2;
                        proj.Update();
                        this.proj.collidesWithProjectiles = false;
                        this.proj.UpdateCollisionMask();
                    }
                }
                PhysicsEngine.SkipCollision = true;
            }
        }

        private Projectile lastCollidedProjectile;
        public Projectile proj;
        private PlayerController parentOwner;
        public Projectile Pop_proj;
    }
    

}
