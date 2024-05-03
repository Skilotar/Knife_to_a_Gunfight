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
    class NewHolyGrenade : SpawnObjectPlayerItem
    {
        public static void Init()
        {
            string itemName = "Purifier";

            string resourceName = "Knives/Resources/Holy/pur_001";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<NewHolyGrenade>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Armaments 2:9-21";
            string longDesc = "Oh Kaliber Bless this thy hand grenade, That with it thous may blowest thine enemies to tiny bits... \n\n In thy mercy." +
                "\n\n-----------------------------" +
                "Deals massive damage to jammed enemies" +
                "\n\n\n - Knife_to_a_Gunfight";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 325);
            item.consumable = false;

            item.objectToSpawn = BuildPrefab();

            item.tossForce = 10;
            item.canBounce = true;

            item.IsCigarettes = false;
            item.RequireEnemiesInRoom = false;


            item.AudioEvent = "Play_hallelujah";
            item.IsKageBunshinItem = false;

            item.quality = PickupObject.ItemQuality.A;
            ID = item.PickupObjectId;
        }
        public static int ID;

        public static GameObject BuildPrefab()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/Holy/pur_001", new GameObject("HolyGrenade"));
            bomb.SetActive(false);
            FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Holy/pur_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Holy/pur_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Holy/pur_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Holy/pur_004", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Holy/pur_005", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Holy/pur_006", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Holy/pur_007", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Holy/pur_008", collection),

            }, "HolyDeploy", tk2dSpriteAnimationClip.WrapMode.Once);
            deployAnimation.fps = 12;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Holy/pur_013", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Holy/pur_014", collection),


            }, "HolyExplode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 30;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Holy/pur_009", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Holy/pur_010", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Holy/pur_011", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Holy/pur_012", collection),


            }, "HolyArmed", tk2dSpriteAnimationClip.WrapMode.LoopSection);
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
                    damage = 25f,  // exactly enough to always kill the arrowkin it makes unless they are jammed or buffed
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
                explosionDelay = 1f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "HolyDeploy",
                explodeAnimName = "HolyExplode",
                idleAnimName = "HolyArmed",




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

                    bool flag3 = Vector2.Distance(aiactor.CenterPosition, centerPosition) < 5f && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && Owner != null;
                    bool flag4 = flag3;
                    aiactor.gameObject.GetOrAddComponent<AiactorSpecialStates>();
                    if (flag4 && aiactor.IsBlackPhantom)
                    {
                        if (aiactor.healthHaver.IsBoss)
                        {
                            aiactor.healthHaver.ApplyDamage(250, Vector2.zero, "Kaliber's mercy",CoreDamageTypes.Magic,DamageCategory.Unstoppable,true,null,true);
                        }
                        else
                        {

                            aiactor.healthHaver.ApplyDamage(200, Vector2.zero, "Kaliber's mercy");
                        }
                    }
                }

            }
        }

      
     
    }
}