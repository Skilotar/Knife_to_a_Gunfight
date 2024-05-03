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
    class Spark : SpawnObjectPlayerItem
    {
        public static void Init()
        {
            string itemName = "Sp4rc Snare";

            string resourceName = "Knives/Resources/Sp4rc_snare";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Spark>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Bound!";
            string longDesc = "A Sp4rk Brand hunting snare used to bind enemies in place." +
                "\n\n\n - Knife_to_a_Gunfight";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 125);
            item.consumable = false;

            item.objectToSpawn = BuildPrefab();

            item.tossForce = 20;
            
            item.canBounce = false;
            
            item.IsCigarettes = false;
            item.RequireEnemiesInRoom = true;


            item.AudioEvent = "Play_Snare_throw";
            item.IsKageBunshinItem = false;

            item.quality = PickupObject.ItemQuality.B;
            ID = item.PickupObjectId;
        }

        public static int ID;
        public static GameObject BuildPrefab()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/Spark/Sp4rc_001", new GameObject("Spark"));
            bomb.SetActive(false);
            FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Spark/Sp4rc_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Spark/Sp4rc_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Spark/Sp4rc_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Spark/Sp4rc_004", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Spark/Sp4rc_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Spark/Sp4rc_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Spark/Sp4rc_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Spark/Sp4rc_004", collection),
               
                



            }, "SprkDeploy", tk2dSpriteAnimationClip.WrapMode.Once);
            deployAnimation.fps = 12;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Spark/Sp4rc_008", collection),


            }, "SprkExplode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 30;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Spark/Sp4rc_005", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Spark/Sp4rc_006", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Spark/Sp4rc_007", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Spark/Sp4rc_008", collection),



            }, "SprkArmed", tk2dSpriteAnimationClip.WrapMode.LoopSection);
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
                explosionDelay = 5f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "SprkDeploy",
                explodeAnimName = "SprkExplode",
                idleAnimName = "SprkArmed",

            };

            var boom = bomb.AddComponent<ProximityMine>(proximityMine);

            
            return bomb;
        }

        public Vector2 hitpos = new Vector2();
        
        public Vector2 centerPosition;
        public bool vangaurd;
        public override void Update()
        {
            if (this.LastOwner != null)
            {
                PlayerController Owner = this.LastOwner;
                if (this.spawnedPlayerObject != null)
                {

                    centerPosition = this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter;

                    List<AIActor> activeEnemies = Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);

                    foreach (AIActor aiactor in activeEnemies)
                    {

                        bool flag3 = Vector2.Distance(aiactor.CenterPosition, centerPosition) < 4f && aiactor.healthHaver.GetMaxHealth() > 0f && aiactor != null && aiactor.specRigidbody != null && Owner != null;
                        bool flag4 = flag3;
                        aiactor.gameObject.GetOrAddComponent<AiactorSpecialStates>();
                        if (flag4 && !aiactor.gameObject.GetComponent<AiactorSpecialStates>().Snared)
                        {
                            float timescale = aiactor.LocalTimeScale;
                            aiactor.StartCoroutine(SlowDuration(aiactor,timescale));
                            aiactor.LocalTimeScale = .25f;
                            attach(aiactor.specRigidbody);
                            AkSoundEngine.PostEvent("Play_Snare_hit", base.gameObject);
                            aiactor.gameObject.GetComponent<AiactorSpecialStates>().Snared = true;
                        }
                    }

                    if (vangaurd)
                    {
                        HandleRadialIndicator();
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

        
        private IEnumerator SlowDuration(AIActor ai, float time)
        {
            yield return new WaitForSeconds(4f);
            ai.LocalTimeScale = time;
            Destroy(ai.GetComponent<ArbitraryCableDrawer>());
        }

        public System.Random rng = new System.Random();
        public IEnumerator bombPreDet()
        {
            yield return new WaitForSeconds(4.999f);

            PlayerController Owner = this.LastOwner;
            if (this.spawnedPlayerObject != null)
            {
                UnhandleRadialIndicator();
                hitpos = Vector2.zero;
                foreach(AIActor ai in Bound)
                {

                    Destroy(ai.GetComponent<ArbitraryCableDrawer>());
                    
                }
                Bound.Clear();
            }
        }
        public List<AIActor> Bound = new List<AIActor>
        {
        };
        public void attach(SpeculativeRigidbody arg2)
        {

            this.cable = arg2.aiActor.gameObject.AddComponent<ArbitraryCableDrawer>();
            this.cable.Attach1Offset = this.spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter - new Vector2(this.spawnedPlayerObject.GetComponent<tk2dSprite>().transform.position.x, this.spawnedPlayerObject.GetComponent<tk2dSprite>().transform.position.y);
            this.cable.Attach2Offset = arg2.aiActor.CenterPosition - arg2.aiActor.transform.position.XY();
            this.cable.Initialize(this.spawnedPlayerObject.GetComponent<tk2dSprite>().transform, arg2.aiActor.transform);
            Bound.Add(arg2.aiActor);
            
        }
       
        private void HandleRadialIndicator()
        {

            bool flag = !this.m_indicator;
            if (flag)
            {

                this.m_indicator = ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), centerPosition, Quaternion.identity, this.spawnedPlayerObject.transform)).GetComponent<HeatIndicatorController>();
                this.m_indicator.CurrentRadius = 4f;
                this.m_indicator.IsFire = false;
                this.m_indicator.CurrentColor = new Color(1f, 0.54f, 0.16f);
            }
        }

        private void UnhandleRadialIndicator()
        {
            bool flag = this.m_indicator;
            if (flag)
            {
                this.m_indicator.EndEffect();
                this.m_indicator = null;
            }
        }
        public static BlinkPassiveItem m_BlinkPassive = PickupObjectDatabase.GetById(436).GetComponent<BlinkPassiveItem>();
        public GameObject BlinkpoofVfx = m_BlinkPassive.BlinkpoofVfx;
        public HeatIndicatorController m_indicator;
        private ArbitraryCableDrawer cable;
        
    }
}
