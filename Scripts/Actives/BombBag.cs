using ItemAPI;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using GungeonAPI;
using Dungeonator;
using Gungeon;


namespace Knives
{
    class BombBag : SpawnObjectPlayerItem
    {
        private static int[] spriteIDs;
        private static readonly string[] spritePaths = new string[]
        {
            "Knives/Resources/Pyro/Icons/Icon_cracker",
            "Knives/Resources/Pyro/Icons/Icon_cherry",
            "Knives/Resources/Pyro/Icons/Icon_flash",
            "Knives/Resources/Pyro/Icons/Icon_big"

        };
        public static void Init()
        {
            string itemName = "Pyros Party Popper Pouch";

            string resourceName = "Knives/Resources/Pyro/Icons/PPPP";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<BombBag>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Office Supplies";
            string longDesc = "Hold Use when on cooldown to select your next bomb.\n\n" +
                "What's in it? Gunpowder, nytaglycerin, notepads, a couple-a cherry bombs, fuzes, glue, single A batteries..." +
                "Road flare.. and uh paperclips, Big ones. You know All the normal stuff. \n\n" +
                "" +
                "Cherry is weak with short recharge\n\n" +
                "Angry is Strong but slow\n\n" +
                "Cracker has many small hits\n\n" +
                "Flash stuns enemies and player if they look towards it." +
                "\n\n\n - Knife_to_a_Gunfight";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 75);
            item.consumable = false;
            BombBag.spriteIDs = new int[BombBag.spritePaths.Length];
            cherry = BuildPrefabCherry();
            big = BuildPrefabBig();
            cracker = BuildPrefabCracker();
            flash = BuildPrefabFlash();
            SetupCollection();
            item.objectToSpawn = cherry; // default

            item.tossForce = 10;
            item.canBounce = true;
            item.IsCigarettes = false;
            item.RequireEnemiesInRoom = false;
            item.IsKageBunshinItem = false;

            item.quality = PickupObject.ItemQuality.B;
            BombBag.spriteIDs[0] = SpriteBuilder.AddSpriteToCollection(BombBag.spritePaths[0], item.sprite.Collection);
            BombBag.spriteIDs[1] = SpriteBuilder.AddSpriteToCollection(BombBag.spritePaths[1], item.sprite.Collection);
            BombBag.spriteIDs[2] = SpriteBuilder.AddSpriteToCollection(BombBag.spritePaths[2], item.sprite.Collection);
            BombBag.spriteIDs[3] = SpriteBuilder.AddSpriteToCollection(BombBag.spritePaths[3], item.sprite.Collection);
            item.sprite.SetSprite(spriteIDs[1]);


            ID = item.PickupObjectId;
        }

        public static int ID;

        public static GameObject cherry;
        public static GameObject big;
        public static GameObject cracker;
        public static GameObject flash;

        public static GameObject BuildPrefabCherry()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/Pyro/Bombs/cherry/cherry_001", new GameObject("CherryBomb"));
            bomb.SetActive(false);
            ItemAPI.FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cherry/cherry_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cherry/cherry_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cherry/cherry_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cherry/cherry_004", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cherry/cherry_005", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cherry/cherry_006", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cherry/cherry_007", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cherry/cherry_008", collection),

            }, "CherryDeploy", tk2dSpriteAnimationClip.WrapMode.Once);
            deployAnimation.fps = 12;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cherry/cherry_013", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cherry/cherry_014", collection),


            }, "CherryExplode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 30;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cherry/cherry_009", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cherry/cherry_010", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cherry/cherry_011", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cherry/cherry_012", collection),


            }, "CherryArmed", tk2dSpriteAnimationClip.WrapMode.LoopSection);
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
                    playDefaultSFX = false,

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
                explosionDelay = 1.25f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "CherryDeploy",
                explodeAnimName = "CherryExplode",
                idleAnimName = "CherryArmed",




            };

            var boom = bomb.AddComponent<ProximityMine>(proximityMine);


            return bomb;
        }

        public static GameObject BuildPrefabBig()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/Pyro/Bombs/big/Big_001", new GameObject("BigBomb"));
            bomb.SetActive(false);
            ItemAPI.FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/big/Big_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/big/Big_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/big/Big_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/big/Big_004", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/big/Big_005", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/big/Big_006", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/big/Big_007", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/big/Big_008", collection),

            }, "BigDeploy", tk2dSpriteAnimationClip.WrapMode.Once);
            deployAnimation.fps = 12;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/big/Big_012", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/big/Big_013", collection),


            }, "BigExplode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 20;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/big/Big_009", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/big/Big_010", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/big/Big_011", collection),
                


            }, "BigArmed", tk2dSpriteAnimationClip.WrapMode.LoopSection);
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
                    damageRadius = 10f,
                    damageToPlayer = 0,
                    damage = 100f,
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
                explosionDelay = 4f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "BigDeploy",
                explodeAnimName = "BigExplode",
                idleAnimName = "BigArmed",




            };

            var boom = bomb.AddComponent<ProximityMine>(proximityMine);


            return bomb;
        }

        public static GameObject BuildPrefabCracker()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/Pyro/Bombs/cracker/cracker_001", new GameObject("CrackerBomb"));
            bomb.SetActive(false);
            ItemAPI.FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cracker/cracker_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cracker/cracker_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cracker/cracker_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cracker/cracker_004", collection),
                

            }, "CrackerDeploy", tk2dSpriteAnimationClip.WrapMode.Once);
            deployAnimation.fps = 12;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cracker/cracker_010", collection),
              

            }, "CrackerExplode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 30;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cracker/cracker_005", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cracker/cracker_006", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cracker/cracker_007", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cracker/cracker_008", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/cracker/cracker_009", collection),


            }, "CrackerArmed", tk2dSpriteAnimationClip.WrapMode.LoopSection);
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
                    playDefaultSFX = false,

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
                    effect = EasyVFXDatabase.BloodiedScarfPoofVFX,
                    freezeEffect = null,
                },
                explosionStyle = ProximityMine.ExplosiveStyle.TIMED,
                detectRadius = 3.5f,
                explosionDelay = 5.5f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "CrackerDeploy",
                explodeAnimName = "CrackerExplode",
                idleAnimName = "CrackerArmed",




            };

            var boom = bomb.AddComponent<ProximityMine>(proximityMine);


            return bomb;
        }


        public static GameObject BuildPrefabFlash()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/Pyro/Bombs/flash/flash_001", new GameObject("FlashBomb"));
            bomb.SetActive(false);
            ItemAPI.FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/flash/flash_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/flash/flash_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/flash/flash_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/flash/flash_004", collection),
                

            }, "FlashDeploy", tk2dSpriteAnimationClip.WrapMode.Once);
            deployAnimation.fps = 12;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/flash/flash_008", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/flash/flash_009", collection),

            }, "FlashExplode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 30;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/flash/flash_005", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/flash/flash_006", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/Bombs/flash/flash_007", collection),
               


            }, "FlashArmed", tk2dSpriteAnimationClip.WrapMode.LoopSection);
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
                    damageRadius = 5f,
                    damageToPlayer = 0,
                    damage = 4f,
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
                    playDefaultSFX = false,

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
                    effect = EasyVFXDatabase.WhiteCircleVFX,
                    freezeEffect = null,
                },
                explosionStyle = ProximityMine.ExplosiveStyle.TIMED,
                detectRadius = 3.5f,
                explosionDelay = 1f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "FlashDeploy",
                explodeAnimName = "FlashExplode",
                idleAnimName = "FlashArmed",




            };

            var boom = bomb.AddComponent<ProximityMine>(proximityMine);


            return bomb;
        }
        public int usedSelect = 0;
        public override void DoEffect(PlayerController user)
        {
            switch (Current_Select)
            {

                case 0:
                    ItemBuilder.SetCooldownType(this, ItemBuilder.CooldownType.Damage, 150); //cracker
                    usedSelect = 0;
                    break;
                case 1:
                    ItemBuilder.SetCooldownType(this, ItemBuilder.CooldownType.Damage, 75); //cherry
                    usedSelect = 1;
                    AkSoundEngine.PostEvent("Play_Cherry_001", base.gameObject);
                    break;
                case 2:
                    ItemBuilder.SetCooldownType(this, ItemBuilder.CooldownType.Damage, 150); //flash
                    usedSelect = 2;
                    break;
                case 3:
                    ItemBuilder.SetCooldownType(this, ItemBuilder.CooldownType.Damage, 200); // big
                    usedSelect = 3;
                    break;
            }
            
            base.DoEffect(user);
        }

        public void DoSafeExplosion(Vector3 position)
        {

            ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
            this.smallPlayerSafeExplosion.effect = EasyVFXDatabase.SmallMagicPuffVFX;
            this.smallPlayerSafeExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
            this.smallPlayerSafeExplosion.ss = defaultSmallExplosionData2.ss;
            
            Exploder.Explode(position, this.smallPlayerSafeExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);

        }
        private ExplosionData smallPlayerSafeExplosion = new ExplosionData
        {
            damageRadius = 2.5f,
            damageToPlayer = 0f,
            doDamage = true,
            damage = 3f,
            doDestroyProjectiles = true,
            doForce = false,
            debrisForce = 5f,
            preventPlayerForce = true,
            explosionDelay = 0.1f,
            usesComprehensiveDelay = false,
            doScreenShake = false,
            playDefaultSFX = false
        };

        public bool locked = false;
        public bool shown = false;
        public override void Pickup(PlayerController player)
        {
            m_player = player;
            base.Pickup(player);
        }
        public override void Update()
        {
            if (this.LastOwner != null)
            {
                PlayerController player = this.LastOwner as PlayerController;
                //selector
                if (this.IsOnCooldown && player.CurrentItem == this)
                {

                
                   

                    if (Key(GungeonActions.GungeonActionType.UseItem) && KeyTime(GungeonActions.GungeonActionType.UseItem) > .33f && !locked)
                    {
                        shown = true;
                        locked = true;
                    }

                    if (!Key(GungeonActions.GungeonActionType.UseItem) && locked == true)
                    {
                        locked = false;
                        shown = false;
                    }


                    if (shown)
                    {

                        if (player.CurrentGun.CurrentAngle <= 45 && player.CurrentGun.CurrentAngle >= -45)// right
                        {
                            Current_Select = 0;
                            this.objectToSpawn = cracker;
                            this.tossForce = 7;
                            this.canBounce = true;
                            this.sprite.SetSprite(spriteIDs[0]);
                        }
                        if (player.CurrentGun.CurrentAngle > 45 && player.CurrentGun.CurrentAngle <= 135)// up
                        {
                            Current_Select = 1;
                            this.objectToSpawn = cherry;
                            this.tossForce = 10;
                            this.canBounce = true;
                            this.sprite.SetSprite(spriteIDs[1]);
                        }
                        if (player.CurrentGun.CurrentAngle > 135 || player.CurrentGun.CurrentAngle <= -135)// left
                        {
                            Current_Select = 2;
                            this.objectToSpawn = flash;
                            this.tossForce = 6;
                            this.canBounce = true;
                            this.sprite.SetSprite(spriteIDs[2]);
                        }
                        if (player.CurrentGun.CurrentAngle < -45 && player.CurrentGun.CurrentAngle >= -135)// down
                        {
                            Current_Select = 3;
                            this.objectToSpawn = big;
                            this.tossForce = 6;
                            this.canBounce = true;
                            this.sprite.SetSprite(spriteIDs[3]);
                        }


                        player.StartCoroutine(ShowChargeLevel(player, Current_Select));
                    }
                }


                //effects
                if(this.spawnedPlayerObject != null)
                {
                    if(usedSelect == 0)//cracker
                    {
                        if(UseLocker == false)
                        {
                            player.StartCoroutine(CrackerGoPopPop());
                            
                            UseLocker = true;
                        }
                       
                    }
                    if(usedSelect == 2)// flash
                    {
                        if (UseLocker == false)
                        {
                            player.StartCoroutine(AUGHMYEYES());
                            
                            UseLocker = true;
                        }
                    }
                    if(usedSelect == 3)//big
                    {
                        if (UseLocker == false)
                        {
                            player.StartCoroutine(BossRipper());
                            
                            UseLocker = true;
                        }
                    }

                }
                else
                {
                    usedSelect = 10;
                    
                    UseLocker = false;
                }

                

            }


            base.Update();
        }
        

        private IEnumerator AUGHMYEYES()
        {
            yield return new WaitForSeconds(.9999f);
            if (this.spawnedPlayerObject != null)
            {
                
                float FlashHoldtime = 0.1f;
                float FlashFadetime = 0.5f;
                Pixelator.Instance.FadeToColor(FlashFadetime, Color.white, true, FlashHoldtime);
                AkSoundEngine.PostEvent("Play_Flash_001", base.gameObject);
                //play flash sound
                this.LastOwner.CurrentRoom.ApplyActionToNearbyEnemies(this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter, 10f, new Action<AIActor, float>(this.ProcessEnemyStun));
                Exploder.DoDistortionWave(this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter, .5f, 0.04f, 20, .6f);
                float b = (LastOwner.CenterPosition - this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter).ToAngle();
                if (BraveMathCollege.AbsAngleBetween(this.LastOwner.FacingDirection, b) >= 30f && !this.LastOwner.IsDodgeRolling)
                {
                    this.LastOwner.CurrentStoneGunTimer = 3;
                }
            }
           
        }

        private void ProcessEnemyStun(AIActor arg1, float arg2)
        {
            arg1.behaviorSpeculator.Stun(3f, true);
        }

        private IEnumerator BossRipper()
        {
            yield return new WaitForSeconds(4.99f);
            if (this.LastOwner.CurrentRoom != null && this.spawnedPlayerObject != null)
            {
                this.LastOwner.CurrentRoom.ApplyActionToNearbyEnemies(this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter, 8f, new Action<AIActor, float>(this.ProcessEnemy));
            }
        }
           

        private void ProcessEnemy(AIActor arg1, float arg2)
        {
            if (arg1.healthHaver.IsBoss)
            {
                arg1.healthHaver.ApplyDamage(100, Vector2.zero, "BigBoom", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
            }
        }

        public bool UseLocker = false;
        private IEnumerator CrackerGoPopPop()
        {

            if (this.spawnedPlayerObject != null)
            {

                
                yield return new WaitForSeconds(1f);
                AkSoundEngine.PostEvent("Play_Cracker_001", base.gameObject);
                DoSafeExplosion(this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter);
                yield return new WaitForSeconds(.5f);
                DoSafeExplosion(this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter);
                yield return new WaitForSeconds(.4f);
                DoSafeExplosion(this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter);
                yield return new WaitForSeconds(.3f);
                DoSafeExplosion(this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter);
                yield return new WaitForSeconds(.2f);
                DoSafeExplosion(this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter);
                yield return new WaitForSeconds(.3f);
                DoSafeExplosion(this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter);
                yield return new WaitForSeconds(.2f);
                DoSafeExplosion(this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter);
                yield return new WaitForSeconds(.2f);
                DoSafeExplosion(this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter);
                yield return new WaitForSeconds(.2f);
                DoSafeExplosion(this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter);
                yield return new WaitForSeconds(.2f);
                DoSafeExplosion(this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter);
                yield return new WaitForSeconds(.2f);
                DoSafeExplosion(this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter);
                yield return new WaitForSeconds(.2f);
                DoSafeExplosion(this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter);
                yield return new WaitForSeconds(.2f);
                DoSafeExplosion(this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter);
                yield return new WaitForSeconds(.2f);
                DoSafeExplosion(this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter);
            }
        }
        public int Current_Select = 1;
        public List<GameObject> extantSprites;
        private static tk2dSpriteCollectionData BagVFXCollection;
        private static GameObject VFXScapegoat;

        private static int Meter1ID;
        private static int Meter2ID;
        private static int Meter3ID;
        private static int Meter4ID;

        private static void SetupCollection()
        {
            VFXScapegoat = new GameObject();
            UnityEngine.Object.DontDestroyOnLoad(VFXScapegoat);
            BombBag.BagVFXCollection = SpriteBuilder.ConstructCollection(VFXScapegoat, "bagVFX_Collection");
            UnityEngine.Object.DontDestroyOnLoad(BombBag.BagVFXCollection);

            Meter1ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/radials/Bomb_radial_cracker", BombBag.BagVFXCollection); //right
            Meter2ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/radials/Bomb_radial_cherry", BombBag.BagVFXCollection); // top
            Meter3ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/radials/Bomb_radial_flash", BombBag.BagVFXCollection); // left
            Meter4ID = SpriteBuilder.AddSpriteToCollection("Knives/Resources/Pyro/radials/Bomb_radial_big", BombBag.BagVFXCollection); // botom

        }
        public PlayerController m_player;
        public float KeyTime(GungeonActions.GungeonActionType action)
        {
            if (!GameManager.Instance.IsLoadingLevel)
            {
                return BraveInput.GetInstanceForPlayer(m_player.PlayerIDX).ActiveActions.GetActionFromType(action).PressedDuration;
            }
            else
            {
                return 0;
            }

        }

        public bool KeyDown(GungeonActions.GungeonActionType action)
        {
            if (!GameManager.Instance.IsLoadingLevel)
            {
                return BraveInput.GetInstanceForPlayer(m_player.PlayerIDX).ActiveActions.GetActionFromType(action).WasPressed;
            }
            else
            {
                return false;
            }

        }

        public bool Key(GungeonActions.GungeonActionType action)
        {
            if (!GameManager.Instance.IsLoadingLevel)
            {
                return BraveInput.GetInstanceForPlayer(m_player.PlayerIDX).ActiveActions.GetActionFromType(action).IsPressed;
            }
            else
            {
                return false;
            }

        }
        private IEnumerator ShowChargeLevel(GameActor gunOwner, int chargeLevel)
        {
            if (extantSprites.Count > 0)
            {
                for (int i = extantSprites.Count - 1; i >= 0; i--)
                {
                    UnityEngine.Object.Destroy(extantSprites[i].gameObject);
                }
                extantSprites.Clear();
            }
            GameObject newSprite = new GameObject("Level Popup", new Type[] { typeof(tk2dSprite) }) { layer = 0 };
            newSprite.transform.position = (gunOwner.transform.position + new Vector3(0.7f, -1.5f));
            tk2dSprite m_ItemSprite = newSprite.AddComponent<tk2dSprite>();
            extantSprites.Add(newSprite);
            int spriteID = -1;
            switch (chargeLevel)
            {

                case 0:
                    spriteID = Meter1ID;
                    break;
                case 1:
                    spriteID = Meter2ID;
                    break;
                case 2:
                    spriteID = Meter3ID;
                    break;
                case 3:
                    spriteID = Meter4ID;
                    break;


            }
            m_ItemSprite.SetSprite(BombBag.BagVFXCollection, spriteID);
            m_ItemSprite.PlaceAtPositionByAnchor(newSprite.transform.position, tk2dBaseSprite.Anchor.LowerCenter);
            m_ItemSprite.transform.localPosition = m_ItemSprite.transform.localPosition.Quantize(0.0625f);
            newSprite.transform.parent = gunOwner.transform;
            if (m_ItemSprite)
            {
                //sprite.AttachRenderer(m_ItemSprite);
                m_ItemSprite.depthUsesTrimmedBounds = true;
                m_ItemSprite.UpdateZDepth();
            }
            //sprite.UpdateZDepth();
            yield return new WaitForSeconds(.5f);
            if (newSprite)
            {
                extantSprites.Remove(newSprite);
                Destroy(newSprite.gameObject);
            }
            yield break;
        }



        public int LastChargeLevel = 0;
       
    }
}
