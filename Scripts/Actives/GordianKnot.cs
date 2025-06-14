using ItemAPI;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

using Dungeonator;
using Gungeon;
using System.Reflection;

namespace Knives
{
    class GordianKnot : SpawnObjectPlayerItem
    {
        public static void Init()
        {
            string itemName = "Gordian Knot";

            string resourceName = "Knives/Resources/Knot/Gordian_Knot";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<GordianKnot>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Simple Solutions";
            string longDesc = "Ensnares Nearby Enemies.\n\n" +
                "A legendary knot once said to have been solved by the bromans greatest miliary leader by cutting it in half. The Knot refused to give up so easily, re-tying itself around the blade which cut it.\n\n" +
                "Ever greedy to expand, it draws in unsuspecting creatures into its mass." +
                "\n\n\n - Knife_to_a_Gunfight";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 300);
            item.consumable = false;

            item.objectToSpawn = BuildPrefab();
            item.tossForce = 7;
            item.canBounce = true;
            item.IsKageBunshinItem = false;

            item.quality = PickupObject.ItemQuality.B;
            ID = item.PickupObjectId;
        }

        public static int ID;
        public static GameObject BuildPrefab()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/Knot/Gordian_Knot_001", new GameObject("Knot"));
            bomb.SetActive(false);
            FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Knot/Gordian_Knot_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Knot/Gordian_Knot_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Knot/Gordian_Knot_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Knot/Gordian_Knot_004", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Knot/Gordian_Knot_005", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Knot/Gordian_Knot_006", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Knot/Gordian_Knot_007", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Knot/Gordian_Knot_008", collection),

            }, "KnotDeploy", tk2dSpriteAnimationClip.WrapMode.Once);
            deployAnimation.fps = 8;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Knot/Gordian_Knot_010", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Knot/Gordian_Knot_011", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Knot/Gordian_Knot_012", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Knot/Gordian_Knot_013", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Knot/Gordian_Knot_014", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Knot/Gordian_Knot_015", collection),
               

            }, "KnotExplode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 8;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {
                 SpriteBuilder.AddSpriteToCollection("Knives/Resources/Knot/Gordian_Knot_009", collection),


            }, "KnotArmed", tk2dSpriteAnimationClip.WrapMode.Once);
            armedAnimation.fps = 8f;

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
                    damage = 40f,
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
                    effect = null,
                    freezeEffect = null,
                },
                explosionStyle = ProximityMine.ExplosiveStyle.TIMED,
                detectRadius = 3.5f,
                explosionDelay = 8f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "KnotDeploy",
                explodeAnimName = "KnotExplode",
                idleAnimName = "KnotArmed",

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
                    if (GetPrivateType<DebrisObject, bool>(spawnedPlayerObject.GetComponent<DebrisObject>(), "onGround"))
                    {
                        RoomHandler room = spawnedPlayerObject.transform.position.GetAbsoluteRoom();
                        if (room != null)
                        {
                            List<AIActor> activeEnemies = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                            if (activeEnemies.Count > 0 && activeEnemies != null)
                            {
                                for (int i = activeEnemies.Count - 1; i >= 0; i--)
                                {
                                    if (Vector2.Distance(activeEnemies[i].Position, spawnedPlayerObject.GetComponent<tk2dSprite>().sprite.WorldCenter) < 6)
                                    {
                                        if (activeEnemies[i].GetComponent<TetherEnemyToSpot>() == null)
                                        {
                                            if(activeEnemies[i].healthHaver.IsBoss != true)
                                            {
                                                TetherEnemyToSpot rope = activeEnemies[i].gameObject.GetOrAddComponent<TetherEnemyToSpot>();
                                                rope.obj = spawnedPlayerObject;
                                                rope.freewalkRadius = 1;
                                                rope.UpdateToObject = true;
                                                rope.color = new Color(.012f, .12f, .43f);
                                                grabbed.Add(activeEnemies[i]);
                                            }
                                           
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (vangaurd == false)
                    {
                        vangaurd = true;
                        StartCoroutine(Decouple());
                    }

                }
                else
                {
                    if (vangaurd == true)
                    {
                        vangaurd = false;
                    }
                }
            }

            base.Update();
        }
        public List<AIActor> grabbed = new List<AIActor>();
        private IEnumerator Decouple()
        {
            float timetowait = this.spawnedPlayerObject.GetComponent<ProximityMine>().explosionDelay - .5f;
            float elapsed = 0;
            while(elapsed < timetowait)
            {
                elapsed += .25f;
                yield return new WaitForSeconds(.25f);
                for (int i = grabbed.Count - 1; i >= 0; i--)
                {
                    if (grabbed[i] != null)
                    {
                        grabbed[i].healthHaver.ApplyDamage(3f,Vector2.zero,"Knot");
                        
                    }
                }
                
            }
        }
        private static T2 GetPrivateType<T, T2>(T obj, string field)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T2)f.GetValue(obj);
        }
    }
}