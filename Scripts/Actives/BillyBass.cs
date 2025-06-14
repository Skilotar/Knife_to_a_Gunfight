using ItemAPI;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

using Dungeonator;
using Gungeon;


namespace Knives
{
    class SingingFish : SpawnObjectPlayerItem
    {
        public static void Init()
        {
            string itemName = "Billy Bass";

            string resourceName = "Knives/Resources/BillyBass/SingyFish_001";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<SingingFish>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Don't Worry";
            string longDesc = "Be Happy\n\n" +
                "A rubber fish mounted to a plastic base that sings music. " +
                "The wiring isn't the most sound; its liable to go at any moment." +
                "\n\n\n - Knife_to_a_Gunfight";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 400);
            item.consumable = false;

            item.objectToSpawn = BuildPrefab();
            item.tossForce = 7;
            item.canBounce = true;
            item.IsKageBunshinItem = false;

            item.quality = PickupObject.ItemQuality.C;
            ID = item.PickupObjectId;
        }

        public static int ID;
        public static GameObject BuildPrefab()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/BillyBass/SingyFish_001", new GameObject("Fish"));
            bomb.SetActive(false);
            FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BillyBass/SingyFish_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BillyBass/SingyFish_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BillyBass/SingyFish_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BillyBass/SingyFish_004", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BillyBass/SingyFish_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BillyBass/SingyFish_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BillyBass/SingyFish_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BillyBass/SingyFish_004", collection),


            }, "FishDeploy", tk2dSpriteAnimationClip.WrapMode.Once);
            deployAnimation.fps = 8;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BillyBass/SingyFish_011", collection),

            }, "FishExplode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 30;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BillyBass/SingyFish_005", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BillyBass/SingyFish_006", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BillyBass/SingyFish_007", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BillyBass/SingyFish_008", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BillyBass/SingyFish_009", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BillyBass/SingyFish_010", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BillyBass/SingyFish_011", collection),


            }, "FishArmed", tk2dSpriteAnimationClip.WrapMode.LoopSection);
            armedAnimation.fps = 4f;

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
                    damageRadius = 5f,
                    damageToPlayer = 0,
                    damage = 20f,
                    breakSecretWalls = true,
                    secretWallsRadius = 3.5f,
                    forcePreventSecretWallDamage = false,
                    doDestroyProjectiles = true,
                    doForce = true,
                    pushRadius = 6,
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
                    effect = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<ProximityMine>().explosionData.effect,
                    freezeEffect = null,
                },
                explosionStyle = ProximityMine.ExplosiveStyle.TIMED,
                detectRadius = 3.5f,
                explosionDelay = 11f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "FishDeploy",
                explodeAnimName = "FishExplode",
                idleAnimName = "FishArmed",

            };


            SpeculativeRigidbody body = bomb.GetOrAddComponent<SpeculativeRigidbody>();
            PixelCollider pixelCollider = new PixelCollider();
            pixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
            pixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
            pixelCollider.ManualWidth = 6;
            pixelCollider.ManualHeight = 13;
            pixelCollider.ManualOffsetX = 2;
            pixelCollider.ManualOffsetY = 0;
            body.PixelColliders = new List<PixelCollider>()
            {
                pixelCollider
            };

            var boom = bomb.AddComponent<ProximityMine>(proximityMine);
           
            return bomb;
        }
        
        public bool vangaurd = false;
        public override void Update()
        {
            if (this.LastOwner != null)
            {
                PlayerController Owner = this.LastOwner;
                if (this.spawnedPlayerObject != null)
                {
                    if(vangaurd == false && this.LastOwner.CurrentRoom != null)
                    {
                        vangaurd = true;
                        AkSoundEngine.PostEvent("Play_BillyBass_001", this.spawnedPlayerObject);
                        CharmProxyCharmBomb charmer = spawnedPlayerObject.GetOrAddComponent<CharmProxyCharmBomb>();
                        charmer.room = this.LastOwner.CurrentRoom;
                        AttractEnemies(spawnedPlayerObject.GetComponent<SpeculativeRigidbody>(), this.LastOwner.CurrentRoom,5);
                    }
                    
                }
                else
                {
                    if(vangaurd == true)
                    {
                        vangaurd = false;
                    }
                }
            }

                base.Update();
        }

        private void AttractEnemies(SpeculativeRigidbody kageBunshin, RoomHandler room, float ChanceMod)
        {
            if(room != null)
            {
                List<AIActor> activeEnemies = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                if (activeEnemies != null)
                {
                    if(activeEnemies.Count > 0)
                    {
                        for (int i = 0; i < activeEnemies.Count; i++)
                        {
                            if (activeEnemies[i].OverrideTarget == null)
                            {
                                if (1.5f >= UnityEngine.Random.Range(1, 4.5f - (ChanceMod / 2)))
                                {
                                    activeEnemies[i].OverrideTarget = kageBunshin.specRigidbody;
                                }

                            }
                        }
                    }
                }
            }
            
        }
    }
}