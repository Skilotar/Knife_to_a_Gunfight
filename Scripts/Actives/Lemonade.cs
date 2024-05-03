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
    class Lemonade : SpawnObjectPlayerItem
    {
        private static int[] spriteIDs;
        private static readonly string[] spritePaths = new string[]
        {
            "Knives/Resources/Lemonade/Lemonade_001",
            "Knives/Resources/Lemonade/LemonadeP_001",
           

        };
        public static void Init()
        {
            string itemName = "Lemonade";

            string resourceName = "Knives/Resources/Lemonade/Lemonade_001";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Lemonade>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "When Life Gives You Lemons";
            string longDesc = "A number of lemons packaged in a wooden crate with the lable. Experimental Lemons. DO NOT EAT.\n\n" +
                "There is a bloody note inside with angry ravings about lemons and houses and a cave of some sort? " +
                "Truely a strange find." +
                "\n\n\n - Knife_to_a_Gunfight";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 200);
            item.consumable = false;

            Lemon = BuildPrefab();
            pinkLemon = BuildPrefabP();
            item.objectToSpawn = Lemon;

            Lemonade.spriteIDs = new int[Lemonade.spritePaths.Length];
            Lemonade.spriteIDs[0] = SpriteBuilder.AddSpriteToCollection(Lemonade.spritePaths[0], item.sprite.Collection);
            Lemonade.spriteIDs[1] = SpriteBuilder.AddSpriteToCollection(Lemonade.spritePaths[1], item.sprite.Collection);
            item.sprite.SetSprite(spriteIDs[0]);
            item.tossForce = 10;
            item.canBounce = true;

            item.IsCigarettes = false;
            item.RequireEnemiesInRoom = false;


            item.AudioEvent = "Play_OBJ_mine_beep_01";
            item.IsKageBunshinItem = false;
            item.numberOfUses = 3;
            item.UsesNumberOfUsesBeforeCooldown = true;
            item.quality = PickupObject.ItemQuality.D;

            ID = item.PickupObjectId;
        }
        public static int ID;
        public static GameObject pinkLemon;
        public static GameObject Lemon;
        public static GameObject BuildPrefab()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/Lemonade/Lemonade_001", new GameObject("LemonGrenade"));
            bomb.SetActive(false);
            FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/Lemonade_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/Lemonade_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/Lemonade_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/Lemonade_004", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/Lemonade_005", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/Lemonade_006", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/Lemonade_007", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/Lemonade_008", collection),

            }, "LemonDeploy", tk2dSpriteAnimationClip.WrapMode.Once);
            deployAnimation.fps = 12;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/Lemonade_012", collection),
                


            }, "LemonExplode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 30;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/Lemonade_009", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/Lemonade_010", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/Lemonade_011", collection),
                


            }, "LemonArmed", tk2dSpriteAnimationClip.WrapMode.LoopSection);
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
                    damage = 10f,  
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
                explosionDelay = 1f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "LemonDeploy",
                explodeAnimName = "LemonExplode",
                idleAnimName = "LemonArmed",




            };

            var boom = bomb.AddComponent<ProximityMine>(proximityMine);


            return bomb;
        }

        public static GameObject BuildPrefabP()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/Lemonade/LemonadeP_001", new GameObject("LemonPGrenade"));
            bomb.SetActive(false);
            FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/LemonadeP_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/LemonadeP_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/LemonadeP_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/LemonadeP_004", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/LemonadeP_005", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/LemonadeP_006", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/LemonadeP_007", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/LemonadeP_008", collection),

            }, "LemonPDeploy", tk2dSpriteAnimationClip.WrapMode.Once);
            deployAnimation.fps = 12;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/LemonadeP_012", collection),



            }, "LemonPExplode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 30;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/LemonadeP_009", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/LemonadeP_010", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Lemonade/LemonadeP_011", collection),



            }, "LemonPArmed", tk2dSpriteAnimationClip.WrapMode.LoopSection);
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
                    damage = 15f, 
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
                explosionDelay = 1f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "LemonPDeploy",
                explodeAnimName = "LemonPExplode",
                idleAnimName = "LemonPArmed",




            };

            var boom = bomb.AddComponent<ProximityMine>(proximityMine);


            return bomb;
        }

        public Vector2 centerPosition;
        public bool vangaurd;
        public override void Update()
        {
            if (this.LastOwner != null)
            {
                if (this.spawnedPlayerObject != null)
                {
                    if (vangaurd)
                    {

                        StartCoroutine(bombPreDet());
                        vangaurd = false;
                    }

                }


                if (this.spawnedPlayerObject == null)
                {
                    centerPosition = new Vector2(0, 0);
                    if (!vangaurd)
                    {

                        vangaurd = true;
                    }

                }

                if(this.LastOwner.PlayerHasActiveSynergy("Pink Lemonade"))
                {
                    this.objectToSpawn = pinkLemon;
                    this.sprite.SetSprite(spriteIDs[1]);
                }
                else
                {
                    this.objectToSpawn = Lemon;
                    this.sprite.SetSprite(spriteIDs[0]);
                }
            }


            base.Update();
        }
        public System.Random rng = new System.Random();
        public IEnumerator bombPreDet()
        {
            yield return new WaitForSeconds(.999f);

            PlayerController Owner = this.LastOwner;
            if (this.spawnedPlayerObject != null)
            {
                centerPosition = this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter;

                List<AIActor> activeEnemies = Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);

                foreach (AIActor aiactor in activeEnemies)
                {

                    bool flag3 = Vector2.Distance(aiactor.CenterPosition, centerPosition) < 3f && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && Owner != null;

                    if (flag3)
                    {
                        aiactor.ApplyEffect(StaticStatusEffects.hotLeadEffect);
                        if (this.LastOwner.PlayerHasActiveSynergy("Pink Lemonade"))
                        {
                            aiactor.ApplyEffect(StaticStatusEffects.charmingRoundsEffect);
                        }
                    }

                }

            }
        }



    }
}