using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;



namespace Knives
{
    class Bab: AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Big Angry Bomb", "Bab");
                // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
                Game.Items.Rename("outdated_gun_mods:big_angry_bomb", "ski:big_angry_bomb");
            gun.gameObject.AddComponent<Bab>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Handle With Care");
            gun.SetLongDescription("A bomb so angry that it should not be thrown. Light the fuse, place it gently on the floor and then back away." +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "Bab_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 1);
            gun.SetAnimationFPS(gun.chargeAnimation, 12);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 5;


            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(79) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.gunHandedness = GunHandedness.OneHanded;
            
            gun.reloadTime = .75f;
            gun.DefaultModule.cooldownTime = 1.5f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.SetBaseMaxAmmo(25);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.B;
            gun.muzzleFlashEffects = new VFXPool { type = VFXPoolType.None, effects = new VFXComplex[0]
            };

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(15) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.AdditionalScaleMultiplier = .0001f;
            projectile.baseData.damage = 0f;
            projectile.baseData.speed *= .01f;

            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = .4f
            };

            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,

            };
            gun.gunClass = GunClass.EXPLOSIVE;

            Big_MAD = BuildPrefab();
            SpawnBombProjMod gameobj = projectile.gameObject.GetOrAddComponent<SpawnBombProjMod>();
            gameobj.gameobjecttospawn = Big_MAD;
            gameobj.tossforce = 0f;
            gameobj.doesSpecialVfx = true;
            gameobj.hitreallyhard = true;
            gameobj.VFX = EasyVFXDatabase.BAB;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;
        }
        
        public static int ID;
        public static GameObject Big_MAD;

        public override void OnPostFired(PlayerController player, Gun gun)
        {

        }

       


        private bool HasReloaded;

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

        public static GameObject BuildPrefab()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/BabBomb/Bab_001", new GameObject("Big_Bomb"));
            bomb.SetActive(false);
            FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BabBomb/Bab_001", collection),
                


            }, "babDeploy", tk2dSpriteAnimationClip.WrapMode.Once);
            deployAnimation.fps = 10;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                 SpriteBuilder.AddSpriteToCollection("Knives/Resources/BabBomb/Bab_005", collection),
                 SpriteBuilder.AddSpriteToCollection("Knives/Resources/BabBomb/Bab_006", collection),


            }, "babExplode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 10;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BabBomb/Bab_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BabBomb/Bab_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BabBomb/Bab_004", collection),


            }, "babArmed", tk2dSpriteAnimationClip.WrapMode.LoopSection);
            armedAnimation.fps = 6f;

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
                    damageRadius = 9f,
                    damageToPlayer = 0,
                    damage = 400f,
                    
                    breakSecretWalls = true,
                    secretWallsRadius = 3f,
                    forcePreventSecretWallDamage = false,
                    doDestroyProjectiles = true,
                    doForce = true,
                    pushRadius = 3,
                    force = 25,
                    debrisForce = 12.5f,
                    preventPlayerForce = false,
                    explosionDelay = 0.1f,
                    usesComprehensiveDelay = false,
                    comprehensiveDelay = 0,
                    playDefaultSFX = true,
                    
                    doScreenShake = true,
                    ss = new ScreenShakeSettings
                    {
                        magnitude = 4,
                        speed = 6.5f,
                        time = 0.5f,
                        falloff = .1f,
                        direction = new Vector2(0, 0),
                        vibrationType = ScreenShakeSettings.VibrationType.Auto,
                        simpleVibrationStrength = Vibration.Strength.Hard,
                        simpleVibrationTime = Vibration.Time.Normal
                    },
                    doStickyFriction = false,
                    doExplosionRing = true,
                    isFreezeExplosion = false,
                    freezeRadius = 5,
                    IsChandelierExplosion = false,
                    rotateEffectToNormal = false,
                    ignoreList = new List<SpeculativeRigidbody>(),
                    overrideRangeIndicatorEffect = null,
                    effect = null,
                    
                    freezeEffect = null,
                },
                explosionStyle = ProximityMine.ExplosiveStyle.TIMED,
                detectRadius = 3.5f,
                explosionDelay = 2.8f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "babDeploy",
                explodeAnimName = "babExplode",
                idleAnimName = "babArmed",
                



            };

            var boom = bomb.AddComponent<ProximityMine>(proximityMine);


            return bomb;
        }


    }
}
