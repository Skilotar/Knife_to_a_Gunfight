using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;
using System.Collections.Generic;
//using Alexandria.ItemAPI; 

namespace Knives
{

    public class Rampage : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Rampage", "Rampage");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:rampage", "ski:rampage");
            gun.gameObject.AddComponent<Rampage>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Heat Sink");
            gun.SetLongDescription("The Internal mechanisms of this LMG Function much better when heated. " +
                "Forged by one of the childern of the sun it would use the heat from their flaming bodies to loosen the firing pin. " +
                "Light yourself ablaze or reload at full clip to cram a thermite canister into the barrel. \n\n" +
                "Safety messures have been added for non plasma based lifeforms. Reloading will flush all fire from the system to allow for emergency extingushes. " +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "Rampage_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 17);
            gun.SetAnimationFPS(gun.introAnimation, 12);
            gun.SetAnimationFPS(gun.criticalFireAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(182) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 6f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1f;
            gun.DefaultModule.cooldownTime = .34f;
            gun.DefaultModule.numberOfShotsInClip = 20;
            gun.SetBaseMaxAmmo(650);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.B;

            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            gun.barrelOffset.transform.localPosition = new Vector3(1.6f, .5f, 0f);
            ItemAPI.FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.AdditionalScaleMultiplier *= .6f;
            projectile.baseData.damage = 15f;
            projectile.baseData.speed *= 1f;

            bomber = BuildPrefab();

           
            Projectile Fin_projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            Fin_projectile.gameObject.SetActive(false);
            
            ItemAPI.FakePrefab.MarkAsFakePrefab(Fin_projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(Fin_projectile);
            
            Fin_projectile.AdditionalScaleMultiplier *= .6f;
            Fin_projectile.baseData.damage = 15f;
            Fin_projectile.baseData.speed *= 1f;
            SpawnBombProjMod therm = Fin_projectile.gameObject.GetOrAddComponent<SpawnBombProjMod>();
            therm.Therm = true;
            therm.gameobjecttospawn = bomber;
            therm.tossforce = 8f;
           

            gun.PreventOutlines = true;
            gun.shellsToLaunchOnFire = 1;
            Gun gun2 = PickupObjectDatabase.GetById(84) as Gun;
            gun.shellCasing = gun2.shellCasing;

            //gun.DefaultModule.finalProjectile = Fin_projectile;
            

            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.FULLAUTO;

            synProj = Fin_projectile;

            
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;
        }
        public static GameObject bomber;
        public static Projectile synProj;

        public static int ID;
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_WPN_smileyrevolver_shot_01", base.gameObject);

        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            PlayerController player = (gun.CurrentOwner as PlayerController);
            if (player.IsOnFire)
            {
                projectile.baseData.speed *= 1.5f;
                projectile.UpdateSpeed();
                ImprovedAfterImage trail = projectile.gameObject.GetOrAddComponent<ImprovedAfterImage>();
                trail.shadowLifetime = .1f;
                trail.shadowTimeDelay = .00001f;
                trail.dashColor = new Color(.67f, .08f, .01f);
                trail.spawnShadows = true;

                if(player.CurrentFireMeterValue > .15f)
                {
                    player.CurrentFireMeterValue = player.CurrentFireMeterValue - .15f;
                }

                if (player.CurrentFireMeterValue <= .02f)
                {
                    player.IsOnFire = false;
                }

                if (player.PlayerHasActiveSynergy("Paint The Town RED"))
                {
                    projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy));
                    projectile.OnDestruction += Projectile_OnDestruction;
                }

                if(player.PlayerHasActiveSynergy("Thermal Linkage"))
                {
                    projectile.damageTypes = CoreDamageTypes.Electric;
                    ChainLightningModifier chain = projectile.gameObject.GetOrAddComponent<ChainLightningModifier>();
                    chain.maximumLinkDistance = 12f;
                    chain.RequiresSameProjectileClass = true;
                    chain.LinkVFXPrefab = (PickupObjectDatabase.GetById(298) as ComplexProjectileModifier).ChainLightningVFX;
                }
            }

            


            base.PostProcessProjectile(projectile);
        }

        private void Projectile_OnDestruction(Projectile obj)
        {
            if (obj != null)
            {
                
                DeadlyDeadlyGoopManager goop = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(PickupObjectDatabase.GetById(242).GetComponent<DirectionalAttackActiveItem>().goopDefinition);
                goop.TimedAddGoopCircle(obj.sprite.WorldCenter, 1f, 0.5f, true);
            }
        }

       
        private void HandleHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            try
            {
                if (arg2 != null)
                {
                    DeadlyDeadlyGoopManager goop = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(PickupObjectDatabase.GetById(242).GetComponent<DirectionalAttackActiveItem>().goopDefinition);
                    if (arg3 != true)
                    {

                        goop.TimedAddGoopCircle(arg2.sprite.WorldCenter, 1f, 0.5f, true);
                    }
                    else
                    {
                        
                        goop.TimedAddGoopCircle(arg2.sprite.WorldCenter, 1.5f, 1f, true);
                    }

                }

            }
            catch(Exception e)
            {
                ETGModConsole.Log(e.ToString());
            }


        }

        private bool HasReloaded;
        public bool WasrecentlyStealthed = false;
        public bool doingCoroutine;
        public bool doingstealthtimer = false;
        public bool house = false;
        public bool limiter = false;
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
                if((gun.CurrentOwner as PlayerController).IsOnFire)
                {
                    gun.DefaultModule.cooldownTime = .18f;

                    if(gun.ClipShotsRemaining == 0)
                    {
                        PlayerController player = this.gun.CurrentOwner as PlayerController;
                        player.CurrentFireMeterValue = player.CurrentFireMeterValue - 1f;
                        player.IsOnFire = false;
                    }
                }
                else
                {
                    gun.DefaultModule.cooldownTime = .34f;
                }

                
                
               
            }
                
        }
        
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.ClipCapacity == gun.ClipShotsRemaining && !player.IsOnFire && Bruh == false)
            {
                StartCoroutine(DoFireUpAnimation());
                Bruh = true;
            }

            if(gun.ClipShotsRemaining == 0)
            {
                if ((this.gun.CurrentOwner as PlayerController).PlayerHasActiveSynergy("Burn The House Down!") || (this.gun.CurrentOwner as PlayerController).HasPassiveItem(815))
                {
                    MiscToolMethods.SpawnProjAtPosi(synProj, player.CenterPosition, player, gun);
                }
            }

            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_yarirocketlauncher_reload_01", base.gameObject);
                if (player.IsOnFire)
                {
                    player.CurrentFireMeterValue = player.CurrentFireMeterValue -1f;
                    player.IsOnFire = false;
                    
                }
                
            }

        }

        public bool Bruh = false;
        private IEnumerator DoFireUpAnimation()
        {

            PlayerController player = (gun.CurrentOwner as PlayerController);
            gun.spriteAnimator.Play("Rampage_critical_fire");

            yield return new WaitForSeconds(1.25f);
            yield return new WaitForEndOfFrame();
            gun.LoseAmmo(10);
            player.IsOnFire = true;
            player.IncreaseFire(.20f);
            AkSoundEngine.PostEvent("Play_OverheatUp", base.gameObject);
            Bruh = false;
        }


        public static GameObject BuildPrefab()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/Therm/therm_spin_001", new GameObject("Therm"));
            bomb.SetActive(false);
            FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Therm/therm_spin_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Therm/therm_spin_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Therm/therm_spin_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Therm/therm_spin_004", collection),
                


            }, "ThermDeploy", tk2dSpriteAnimationClip.WrapMode.LoopSection);
            deployAnimation.fps = 12;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Therm/therm_burst_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Therm/therm_burst_002", collection),


            }, "ThermExplode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 30;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Therm/therm_spin_001", collection),


            }, "ThermArmed", tk2dSpriteAnimationClip.WrapMode.LoopSection);
            armedAnimation.fps = 10.0f;

            foreach (var frame in armedAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var audioListener = bomb.AddComponent<AudioAnimatorListener>();
            audioListener.animationAudioEvents = new ActorAudioEvent[]
            {
                new ActorAudioEvent
                {
                    eventName = "Play_OBJ_mine_beep_01",
                    eventTag = "beep"
                }
            };

            ProximityMine proximityMine = new ProximityMine
            {
                explosionData = new ExplosionData
                {
                    useDefaultExplosion = false,
                    doDamage = true,
                    forceUseThisRadius = false,
                    damageRadius = 2f,
                    damageToPlayer = 0,
                    damage = 5f,
                    breakSecretWalls = false,
                    secretWallsRadius = 0f,
                    forcePreventSecretWallDamage = true,
                    doDestroyProjectiles = false,
                    doForce = true,
                    pushRadius = 0,
                    force = 25,
                    debrisForce = 0f,
                    preventPlayerForce = false,
                    explosionDelay = 0.1f,
                    usesComprehensiveDelay = false,
                    comprehensiveDelay = 0,
                    playDefaultSFX = true,

                    doScreenShake = false,
                    ss = new ScreenShakeSettings
                    {
                        magnitude = 2,
                        speed = 6.5f,
                        time = 0.22f,
                        falloff = 0,
                        direction = new Vector2(0, 0),
                        vibrationType = ScreenShakeSettings.VibrationType.Auto,
                        simpleVibrationStrength = Vibration.Strength.Medium,
                        simpleVibrationTime = Vibration.Time.Normal
                    },
                    doStickyFriction = true,
                    doExplosionRing = true,
                    isFreezeExplosion = false,
                    freezeRadius = 5,
                    IsChandelierExplosion = false,
                    rotateEffectToNormal = false,
                    ignoreList = new List<SpeculativeRigidbody>(),
                    overrideRangeIndicatorEffect = null,
                    effect = EasyVFXDatabase.RedFireBlastVFX,
                    freezeEffect = null,
                },
                explosionStyle = ProximityMine.ExplosiveStyle.TIMED,
                detectRadius = 3.5f,
                explosionDelay = .7f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "ThermDeploy",
                explodeAnimName = "ThermExplode",
                idleAnimName = "ThermArmed",




            };

            var boom = bomb.AddComponent<ProximityMine>(proximityMine);
            

            return bomb;
        }



    }
}


