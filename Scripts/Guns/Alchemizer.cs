using System;
using System.Collections;
using Gungeon;
using System.Collections.Generic;
using System.Linq;
using MonoMod;
using UnityEngine;
using Dungeonator;
using ItemAPI;
using Brave.BulletScript;
using Pathfinding;

namespace Knives
{

    public class Alchemizer : AdvancedGunBehaviour
    {

        
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Alchemizer", "Alchemizer");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:alchemizer", "ski:alchemizer");
            gun.gameObject.AddComponent<Alchemizer>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Hexplosion");
            gun.SetLongDescription("A ramshackle potion blaster made by a wizard with a gun.\n\n When dabbling with arcane magic some of the gunpowder from the" +
                 " air caused a violent reaction. This Blast reeked of decay and whispered hexing words into the air.\n\n" +
                 "Combining [BlastBlight] and [Hex] Statuses will cause blastblight explosions to transfer hex to nearby enemies." +
                "\n\n\n - Knife_to_a_Gunfight");
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "Alchemizer_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 6);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);
            gun.SetAnimationFPS(gun.idleAnimation, 6);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(181) as Gun, true, false);

            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Burst;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.DefaultModule.burstCooldownTime = .35f;
            gun.DefaultModule.burstShotCount = 2;
            gun.reloadTime = 1.2f;
            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.gunClass = GunClass.EXPLOSIVE;
            gun.DefaultModule.cooldownTime = 1f;
            gun.DefaultModule.numberOfShotsInClip = 3;
            gun.SetBaseMaxAmmo(120);
            
            
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.B;
             
            
            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.HasDefaultTint = true;
            projectile.DefaultTintColor = new Color(.50f, .07f, .04f);
            projectile.baseData.damage = 40f;
            projectile.baseData.speed *= .8f;
            projectile.baseData.range = 60;

            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(157) as Gun).DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            
            projectile2.HasDefaultTint = true;
            projectile2.DefaultTintColor = new Color(.50f, .07f, .04f);
            projectile2.baseData.damage = 6f;
            projectile2.baseData.speed *= .8f;
            projectile2.baseData.range = 60;
            ProjectileSplitController split = projectile2.gameObject.GetOrAddComponent<ProjectileSplitController>();
            split.distanceTillSplit = 0f;
            split.distanceBasedSplit = true;
            split.dmgMultAfterSplit = 1f;
            split.numberofsplits = 1;
            split.amtToSplitTo = 3;
            split.removeComponentAfterUse = true;
            split.splitAngles = 10;
            

            gun.DefaultModule.usesOptionalFinalProjectile = true;
            gun.DefaultModule.numberOfFinalProjectiles = 1;
            gun.DefaultModule.finalProjectile = projectile2;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Potions", "Knives/Resources/Alc_clipfull", "Knives/Resources/Alc_clipempty");

            gun.DefaultModule.finalAmmoType = GameUIAmmoType.AmmoType.BLUE_SHOTGUN;

            b_potion = BuildPrefabb();
            h_potion = BuildPrefabh();

            SpawnBombProjMod gameobj = projectile.gameObject.GetOrAddComponent<SpawnBombProjMod>();
            gameobj.gameobjecttospawn = b_potion;
            gameobj.tossforce = 12f;
            obj = gameobj;

            ETGMod.Databases.Items.Add(gun, null, "ANY");

            

        }

        public static GameObject b_potion;
        public static GameObject h_potion;

        public bool Flipper = true;
        public static SpawnBombProjMod obj;
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            if (Flipper)
            {
                obj.gameobjecttospawn = h_potion;
                obj.DoesHex = true;
                obj.DoesBlast = false;
                Flipper = false;
            }
            else
            {
                obj.gameobjecttospawn = b_potion;
                obj.DoesBlast = true;
                obj.DoesHex = false;
                Flipper = true;
            }
            

        }
        public override void PostProcessProjectile(Projectile projectile)
        {

            if(gun.ClipShotsRemaining != 1)
            {
                AkSoundEngine.PostEvent("Play_WPN_m79grenadelauncher_shot_01", base.gameObject);
            }
            else
            {
                AkSoundEngine.PostEvent("Play_WPN_elephantgun_shot_01", base.gameObject);
            }
           
            

            base.PostProcessProjectile(projectile);
        }



        private bool HasReloaded;

        protected void Update()
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
            else
            {

            }
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                /*
                Projectile projectile = ((Gun)ETGMod.Databases.Items[157]).DefaultModule.projectiles[0];
                AkSoundEngine.PostEvent("Play_WPN_elephantgun_shot_01", base.gameObject);
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag2 = component != null;
                if (flag2)
                {
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                }
                */
            }
        }

        public static GameObject BuildPrefabb()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/Blast_potion/blast_potion_001", new GameObject("blast_potion"));
            bomb.SetActive(false);
            FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Blast_potion/blast_potion_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Blast_potion/blast_potion_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Blast_potion/blast_potion_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Blast_potion/blast_potion_004", collection),
                

            }, "blastDeploy", tk2dSpriteAnimationClip.WrapMode.Once);
            deployAnimation.fps = 12;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Blast_potion/blast_potion_006", collection),



            }, "blastExplode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 30;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Blast_potion/blast_potion_005", collection),
                



            }, "blastArmed", tk2dSpriteAnimationClip.WrapMode.LoopSection);
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
                    damageRadius = 3f,
                    damageToPlayer = 0,
                    damage = 14f,
                    breakSecretWalls = false,
                    secretWallsRadius = 0f,
                    forcePreventSecretWallDamage = true,
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
                        magnitude = 2,
                        speed = 6.5f,
                        time = 0.22f,
                        falloff = 0,
                        direction = new Vector2(0, 0),
                        vibrationType = ScreenShakeSettings.VibrationType.Auto,
                        simpleVibrationStrength = Vibration.Strength.Medium,
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
                    effect = EasyVFXDatabase.RedFireBlastVFX,
                    freezeEffect = null,
                },
                explosionStyle = ProximityMine.ExplosiveStyle.TIMED,
                detectRadius = 3.5f,
                explosionDelay = .7f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "blastDeploy",
                explodeAnimName = "blastExplode",
                idleAnimName = "blastArmed",




            };

            var boom = bomb.AddComponent<ProximityMine>(proximityMine);


            return bomb;
        }

        public static GameObject BuildPrefabh()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/Hex_potion/Hex_potion_001", new GameObject("hex_potion"));
            bomb.SetActive(false);
            FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Hex_potion/Hex_potion_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Hex_potion/Hex_potion_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Hex_potion/Hex_potion_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Hex_potion/Hex_potion_004", collection),


            }, "hexDeploy", tk2dSpriteAnimationClip.WrapMode.Once);
            deployAnimation.fps = 12;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Hex_potion/Hex_potion_006", collection),



            }, "hexExplode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 30;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Hex_potion/Hex_potion_005", collection),




            }, "hexArmed", tk2dSpriteAnimationClip.WrapMode.LoopSection);
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
                    damageRadius = 3f,
                    damageToPlayer = 0,
                    damage = 14f,
                    breakSecretWalls = false,
                    secretWallsRadius = 0f,
                    forcePreventSecretWallDamage = true,
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
                        magnitude = 2,
                        speed = 6.5f,
                        time = 0.22f,
                        falloff = 0,
                        direction = new Vector2(0, 0),
                        vibrationType = ScreenShakeSettings.VibrationType.Auto,
                        simpleVibrationStrength = Vibration.Strength.Medium,
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
                    effect = EasyVFXDatabase.RedFireBlastVFX,
                    freezeEffect = null,
                },
                explosionStyle = ProximityMine.ExplosiveStyle.TIMED,
                detectRadius = 3.5f,
                explosionDelay = 0.7f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "hexDeploy",
                explodeAnimName = "hexExplode",
                idleAnimName = "hexArmed",




            };

            var boom = bomb.AddComponent<ProximityMine>(proximityMine);


            return bomb;
        }

    }
}
