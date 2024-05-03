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
    class Bombushka : SpawnObjectPlayerItem
    {
  
        public static void Init()
        {
            string itemName = "Bombushka";

            string resourceName = "Knives/Resources/Bombushka/bombushka_001";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Bombushka>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Mama Bomba";
            string longDesc = "A prized posession of the Tzar Bomba depicting his mother. This hardened C4 matryoshka was designed as a gift from the tzar to other rulers he did not like." +
                "\n\n\n - Knife_to_a_Gunfight";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 200);
            item.consumable = false;
            granny = BuildPrefab();
            BombushkaController bomb = granny.gameObject.GetOrAddComponent<BombushkaController>();
            bomb.gameobjecttospawn = granny;
            bomb.thisBombNumber = 1;
            
            item.objectToSpawn = granny;
            item.tossForce = 8;
            item.canBounce = true;
            item.IsCigarettes = false;
            item.RequireEnemiesInRoom = false;
            item.AudioEvent = "Play_OBJ_mine_beep_01";
            item.IsKageBunshinItem = false;
            item.quality = PickupObject.ItemQuality.C;
            ID = item.PickupObjectId;

        }
        public static GameObject granny;
        public static int ID;
        public static GameObject BuildPrefab()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/Bombushka/bombushka_001", new GameObject("GrandmaGrenade"));
            bomb.SetActive(false);
            FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Bombushka/bombushka_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Bombushka/bombushka_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Bombushka/bombushka_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Bombushka/bombushka_004", collection),
                

            }, "bombushkaDeploy", tk2dSpriteAnimationClip.WrapMode.Once);
            deployAnimation.fps = 12;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Bombushka/bombushka_007", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Bombushka/bombushka_008", collection),


            }, "bombushkaExplode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 30;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

               SpriteBuilder.AddSpriteToCollection("Knives/Resources/Bombushka/bombushka_005", collection),
               SpriteBuilder.AddSpriteToCollection("Knives/Resources/Bombushka/bombushka_006", collection),




            }, "bombushkaArmed", tk2dSpriteAnimationClip.WrapMode.LoopSection);
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
                    force = 20,
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
                explosionDelay = 1.2f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "bombushkaDeploy",
                explodeAnimName = "bombushkaExplode",
                idleAnimName = "bombushkaArmed",




            };

            var boom = bomb.AddComponent<ProximityMine>(proximityMine);


            return bomb;
        }



        public override void Update()
        {
            if (this.LastOwner != null)
            {
                if(this.LastOwner.PlayerHasActiveSynergy("Angry Mama"))
                {
                    BombushkaController bomb = granny.gameObject.GetOrAddComponent<BombushkaController>();
                    bomb.DoesSynergy = true;
                }
                else
                {
                    BombushkaController bomb = granny.gameObject.GetOrAddComponent<BombushkaController>();
                    bomb.DoesSynergy = false;
                }
               
              
            }


            base.Update();
        }



    }
}
