using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using System.Reflection;
using Gungeon;
using Alexandria.BreakableAPI;
using Dungeonator;
using System.Collections.Generic;

namespace Knives
{
    class WreckingBall : PlayerItem
    {
        public static void Register()
        {
            string itemName = "Wrecking Ball";

            string resourceName = "Knives/Resources/Wrecking_Ball";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<WreckingBall>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Off The Chain";
            string longDesc =
                "A powerful Wrecking Ball that leaves death and speedboosts in its wake.\n\n" +
                "The weight and concussive explosions are able to bust down doors and bust through defenses. " +
                "The favorite tool of a savlonian freedom fighter this wrecking ball has a bright future of toppling oppression. " +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 40f);


            Ball = BuildPrefab();
            boostPad = BreakableAPIToolbox.GenerateDebrisObject("Knives/Resources/wreckingball/boostPad", true, 10, 20, 100, 20, null, .2f, null, null, 0);
           
            item.quality = PickupObject.ItemQuality.B;
            ID = item.PickupObjectId;

            SpeedyModifier = BuildModifier();
            
        }
        public static StatModifier BuildModifier()
        {
            StatModifier SpeedyMod = new StatModifier();
            SpeedyMod.statToBoost = PlayerStats.StatType.MovementSpeed;
            SpeedyMod.modifyType = StatModifier.ModifyMethod.ADDITIVE;
            SpeedyMod.amount = 1.5f;
            return SpeedyMod;
        }

        public static StatModifier SpeedyModifier;

        public static DebrisObject boostPad;
        public static GameObject Ball;
        public static int ID;

        public static GameObject BuildPrefab()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/wreckingball/Roll/Wrecking_ball_001", new GameObject("WreckingBall"));
            bomb.SetActive(false);
            FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;
            
            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/wreckingball/Throw/Wrecking_ball_throw_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/wreckingball/Throw/Wrecking_ball_throw_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/wreckingball/Throw/Wrecking_ball_throw_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/wreckingball/Throw/Wrecking_ball_throw_004", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/wreckingball/Throw/Wrecking_ball_throw_005", collection),

            }, "blastDeploy", tk2dSpriteAnimationClip.WrapMode.Once);
            deployAnimation.fps = 12;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/wreckingball/Roll/Wrecking_ball_001", collection),
                

            }, "blastExplode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 30;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/wreckingball/Roll/right/Wrecking_ball_right_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/wreckingball/Roll/right/Wrecking_ball_right_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/wreckingball/Roll/right/Wrecking_ball_right_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/wreckingball/Roll/right/Wrecking_ball_right_004", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/wreckingball/Roll/right/Wrecking_ball_right_005", collection),

            }, "blastArmed", tk2dSpriteAnimationClip.WrapMode.Loop);
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

            var RollLeftAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/wreckingball/Roll/left/Wrecking_ball_left_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/wreckingball/Roll/left/Wrecking_ball_left_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/wreckingball/Roll/left/Wrecking_ball_left_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/wreckingball/Roll/left/Wrecking_ball_left_004", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/wreckingball/Roll/left/Wrecking_ball_left_005", collection),

            }, "blastArmedLeft", tk2dSpriteAnimationClip.WrapMode.Loop);

            ProximityMine proximityMine = new ProximityMine
            {
                explosionData = new ExplosionData
                {
                    useDefaultExplosion = false,
                    doDamage = true,
                    forceUseThisRadius = false,
                    damageRadius = 3.2f,
                    damageToPlayer = 0,
                    damage = 35f,
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
                    playDefaultSFX = false,
                    

                    doScreenShake = true,
                    ss = new ScreenShakeSettings
                    {
                        magnitude = 1,
                        speed = 6.5f,
                        time = 0.22f,
                        falloff = 0,
                        direction = new Vector2(0, 0),
                        vibrationType = ScreenShakeSettings.VibrationType.Auto,
                        simpleVibrationStrength = Vibration.Strength.UltraLight,
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
                explosionDelay = 5f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "blastDeploy",
                explodeAnimName = "blastExplode",
                idleAnimName = "blastArmed",

            };

            var boom = bomb.AddComponent<ProximityMine>(proximityMine);
            SpeculativeRigidbody body = bomb.GetOrAddComponent<SpeculativeRigidbody>();
            PixelCollider pixelCollider = new PixelCollider();
            pixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
            pixelCollider.CollisionLayer = CollisionLayer.Projectile;
            pixelCollider.ManualWidth = 6;
            pixelCollider.ManualHeight = 13;
            pixelCollider.ManualOffsetX = 2;
            pixelCollider.ManualOffsetY = 0;
            body.PixelColliders = new List<PixelCollider>()
            {
                pixelCollider
            };
            body.CollideWithOthers = true;
            body.CollideWithTileMap = true;
            body.ForceAlwaysUpdate = true;

            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.SetColor("_EmissiveColor", new Color32(0, 234, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 4f);
            mat.SetFloat("_EmissivePower", 4);
            mat.SetFloat("_EmissiveThresholdSensitivity", .20f);

            MeshRenderer component = bomb.GetComponent<MeshRenderer>();
            if (!component)
            {
                ETGModConsole.Log("nope");
                return bomb;
            }
            Material[] sharedMaterials = component.sharedMaterials;
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                if (sharedMaterials[i].shader == mat)
                {
                    return bomb;
                }
            }
            Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
            Material material = new Material(mat);
            material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
            sharedMaterials[sharedMaterials.Length - 1] = material;
            component.sharedMaterials = sharedMaterials;


            return bomb;
        }


    

        public override void DoEffect(PlayerController player)
        {
            if (player != null)
            {
                StartCoroutine(LetHerRIPPPP());
                player.DidUnstealthyAction();
            }
        }
        
        private IEnumerator LetHerRIPPPP()
        {
            
            Vector2 dir = this.LastOwner.unadjustedAimPoint - this.LastOwner.CurrentGun.barrelOffset.transform.position;
            GameObject tumble = UnityEngine.Object.Instantiate<GameObject>(Ball.gameObject, this.LastOwner.transform.position, Quaternion.identity);
            DebrisObject debrisObject = LootEngine.DropItemWithoutInstantiating(tumble, this.LastOwner.transform.position + new Vector3(.5f,0), dir, 6, false, false, true, false);
            debrisObject.decayOnBounce = -.1f;
            debrisObject.bounceCount = 9;
            
            
            WreckingBallController comp = debrisObject.gameObject.GetOrAddComponent<WreckingBallController>();
            comp.boomcount = 0;
            comp.player = this.LastOwner;
            comp.boostpad = boostPad;

            var ang = this.LastOwner.CurrentGun.CurrentAngle;
            if (ang > 135 || ang < -135)
            {
                debrisObject.sprite.FlipX = true;
            }

            ImprovedAfterImage zoomy = debrisObject.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            zoomy.dashColor = new Color(220f / 255f, 220f / 255f, 220f / 255f);
            zoomy.spawnShadows = true;
            zoomy.shadowTimeDelay = .1f;
            zoomy.shadowLifetime = .25f;
            zoomy.minTranslation = 0.1f;
            zoomy.OverrideImageShader = ShaderCache.Acquire("Brave/Internal/DownwellAfterImage");

            AkSoundEngine.PostEvent("Play_Wreck_Rip", debrisObject.gameObject);

            /*
            AdditionalBraveLight light = debrisObject.gameObject.GetOrAddComponent<AdditionalBraveLight>();
            light.transform.position = debrisObject.sprite.WorldCenter;
            light.LightColor = new Color(0, .91f, 1);
            light.LightIntensity = 2f;
            light.LightRadius = 2f;
            */

            yield return null;
        }


       


        protected void EndEffect(PlayerController user)
        {

        }

        public override void Update()
        {

            if (this.LastOwner != null)
            {

                this.CanBeUsed(this.LastOwner);

            }
            base.Update();
        }

       
        
    }


    public class WreckingBallController : MonoBehaviour
    {

        public WreckingBallController()
        {


        }


        private void Start()
        {
            debrisObject = this.gameObject.GetComponent<DebrisObject>();
            SpeculativeRigidbody spec = this.gameObject.GetComponent<SpeculativeRigidbody>();
            spec.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(spec.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision));
            spec.CollideWithOthers = true;
            spec.CollideWithTileMap = true;
            debrisObject.OnTouchedGround += this.floor;
            

            animator = debrisObject.gameObject.GetComponent<tk2dSpriteAnimator>();
            AkSoundEngine.PostEvent("Play_Wreck_Loop", debrisObject.gameObject);
            

        }

        

        public tk2dSpriteAnimator animator;
        private void floor(DebrisObject obj)
        {
            touchFloor = true;
            AkSoundEngine.PostEvent("Play_ENM_ironmaiden_stomp_01", obj.gameObject);

            int number = UnityEngine.Random.Range(1, 3);
            dropboosts(number, 2);
        }
        public RoomHandler lastRoomReset;
        public bool CanReset = true;
        private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (otherPixelCollider != null && otherRigidbody.aiActor == null)
            {
                //You cant run from a trainer battle!

                bool inBossFight = false;
                if (room != null && room.HasActiveEnemies(RoomHandler.ActiveEnemyType.All))
                {
                    foreach (AIActor aiactor in room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                    {
                        if (aiactor != null && aiactor.healthHaver != null && aiactor.healthHaver.IsBoss)
                        {
                            inBossFight = true;
                        }

                    }
                }

                // skip player collision
                if (otherRigidbody == this.player.specRigidbody)
                {
                    PhysicsEngine.SkipCollision = true;
                }

                //for secret rooms
                if (otherRigidbody.GetComponent<MajorBreakable>() != null)
                {
                    bool isSecretDoor = otherRigidbody.GetComponent<MajorBreakable>().IsSecretDoor;
                    if (isSecretDoor)
                    {

                        AkSoundEngine.PostEvent("Play_OBJ_boulder_crash_01", base.gameObject);
                        AkSoundEngine.PostEvent("Play_OBJ_boulder_crash_01", base.gameObject);
                        AkSoundEngine.PostEvent("Play_OBJ_boulder_crash_01", base.gameObject);
                        otherRigidbody.GetComponent<MajorBreakable>().ApplyDamage(100, Vector2.zero, false, false, true);
                        DoSafeExplosion(debrisObject.sprite.WorldCenter);
                    }
                }

                //for door
                if (otherRigidbody.gameObject.GetComponentInParent<DungeonDoorController>() != null && !inBossFight && CanReset)
                {

                    DungeonDoorController door = otherRigidbody.gameObject.GetComponentInParent<DungeonDoorController>();

                    if (door.IsSealed)
                    {

                        if (player.IsInCombat)
                        {

                            lastRoomReset = player.CurrentRoom;
                            GameObject roomResetter = new GameObject("room resetter");
                            roomResetter.transform.position = player.CurrentRoom.GetCenterCell().ToVector2();
                            LeaveRoomResetter leave = roomResetter.AddComponent<LeaveRoomResetter>();
                            leave.parentRoom = player.CurrentRoom;
                            leave.player = player;

                            player.CurrentRoom.UnsealRoom();
                            CanReset = false;
                            if (door.OneWayDoor)
                            {
                                door.DoUnseal(lastRoomReset);
                            }
                            DoSafeExplosion(debrisObject.sprite.WorldCenter);
                        }


                        Vector3 vel = GetPrivateType<DebrisObject, Vector3>(debrisObject, "m_velocity");
                        Vector2 dir = vel;
                        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        //ETGModConsole.Log(ang);
                        if (ang > 135 || ang < -45)
                        {
                            door.Open(true);
                        }
                        else
                        {
                            door.Open();

                        }
                    }
                    else
                    {
                        Vector3 vel = GetPrivateType<DebrisObject, Vector3>(debrisObject, "m_velocity");
                        Vector2 dir = vel;
                        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        //ETGModConsole.Log(ang);
                        if (ang > 135 || ang < -45)
                        {
                            door.Open(true);
                        }
                        else
                        {
                            door.Open();

                        }
                    }




                }

                //for flipped tables
                if (otherRigidbody.gameObject.GetComponent<FlippableCover>() != null)
                {
                    FlippableCover Table = otherRigidbody.GetComponent<FlippableCover>();
                    if (!Table.IsBroken)
                    {
                        if (touchFloor)
                        {
                            DoTableExplosion(debrisObject.sprite.WorldCenter);
                        }

                    }
                }
            }

        }

        public void ResetRoomOnLeave()
        {
            lastRoomReset?.ResetPredefinedRoomLikeDarkSouls();
            lastRoomReset = null;
        }

        public Vector2 posinow;
        public Vector2 lastposi;
        protected void Update()
        {
            if (boomcount <= 4)
            {
                if (BoomCooldown <= 0)
                {
                    if (GameManager.Instance.Dungeon.data != null)
                    {
                        room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(debrisObject.sprite.WorldCenter.ToIntVector2());
                        if (room != null)
                        {
                            //ETGModConsole.Log("room good");
                            if (room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null)
                            {
                                List<AIActor> currentEnemies = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                                for (int i = currentEnemies.Count - 1; i >= 0; i--)
                                {
                                    AIActor actor = currentEnemies[i];

                                    if (actor != null)
                                    {
                                        if (Vector2.Distance(actor.CenterPosition, debrisObject.sprite.WorldCenter) < 3.5f && doBoom == false)
                                        {
                                            if (doBoom != true)
                                            {
                                                StartCoroutine(DelayBoom(currentEnemies));

                                            }

                                        }
                                    }
                                }


                            }

                            //for locks
                            IPlayerInteractable nearestInteractable = room.GetNearestInteractable(debrisObject.sprite.WorldCenter, 2f, player);
                            if (nearestInteractable != null)
                            {
                                if (nearestInteractable is InteractableLock)
                                {

                                    InteractableLock interactableLock = nearestInteractable as InteractableLock;
                                    interactableLock.ForceUnlock();
                                    UnityEngine.Object.Instantiate<GameObject>(EasyVFXDatabase.TeleporterPrototypeTelefragVFX, interactableLock.sprite.WorldCenter, Quaternion.identity);
                                    DoSafeExplosion(debrisObject.sprite.WorldCenter);
                                }
                            }

                            if (nearestInteractable != null)
                            {

                                if (nearestInteractable is FlippableCover)
                                {
                                    FlippableCover Table = nearestInteractable as FlippableCover;
                                    if (!Table.IsBroken)
                                    {
                                        if (touchFloor)
                                        {
                                            DoTableExplosion(debrisObject.sprite.WorldCenter);
                                        }

                                    }
                                }

                            }

                        }

                    }


                }
                else
                {
                    BoomCooldown -= BraveTime.DeltaTime;
                }



            }
            else
            {
                DoSafeExplosion(debrisObject.sprite.WorldCenter);
                
                
                UnityEngine.GameObject.Destroy(this.gameObject);
            }

            if (touchFloor)
            {
                touchFloor = false;
            }

            if (debrisObject != null)
            {
                Vector3 vel = GetPrivateType<DebrisObject, Vector3>(debrisObject, "m_velocity");
                if (KnownVel != vel && animator.CurrentClip.name != "blastDeploy")
                {
                    Vector2 dir = vel;
                    float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    //ETGModConsole.Log(ang);

                    if (ang >= 90 || ang <= -90)
                    {
                        animator.Play("blastArmedLeft");
                        right = false;
                    }
                    else
                    {
                        animator.Play("blastArmed");
                        right = true;
                    }
                    KnownVel = vel;
                }

            }



        }

        private void OnDestroy()
        {
            AkSoundEngine.PostEvent("Play_Wreck_boom", debrisObject.gameObject);
            AkSoundEngine.PostEvent("Stop_Wreck_Loop", debrisObject.gameObject);
            dropboosts(5, 5);
        }
        public void dropboosts(int num, float force)
        {
            for (int f = 0; f <= num; f++)
            {
                float Var = UnityEngine.Random.Range(-1, 2);
                Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(boostpad.gameObject, debrisObject.sprite.WorldCenter, UnityEngine.Random.rotation);

                DebrisObject boost = LootEngine.DropItemWithoutInstantiating(gameObject, debrisObject.sprite.WorldCenter, randomDirection, force + Var, false, false, true, false);
                boost.sprite.TweenRotateTo(randomDirection);

                AdditionalBraveLight light = boost.gameObject.GetOrAddComponent<AdditionalBraveLight>();
                light.transform.position = boost.sprite.WorldCenter;
                light.LightColor = new Color(0, .91f, 1);
                light.LightIntensity = 3f;
                light.LightRadius = 1.5f;

                BoostPadController go = boost.gameObject.GetOrAddComponent<BoostPadController>();
                go.player = this.player;



            }
        }

        private IEnumerator DelayBoom(List<AIActor> currentEnemies)
        {
            doBoom = true;

            yield return new WaitForSeconds(.20f);

            BoomCooldown = 1f;
            DoSafeExplosion(debrisObject.sprite.WorldCenter);
            boomcount++;

            for (int i = currentEnemies.Count - 1; i >= 0; i--)
            {
                AIActor actor = currentEnemies[i];

                if (actor != null)
                {
                    if (Vector2.Distance(actor.CenterPosition, debrisObject.sprite.WorldCenter) < 3.5f)
                    {
                        actor.behaviorSpeculator.Stun(4f);
                    }
                }
            }
            doBoom = false;
        }

        public Vector3 KnownVel;
        public bool right = true;
        public bool Once = false;
        public bool doBoom = false;
        public void DoTableExplosion(Vector3 position)
        {

            ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
            this.smallPlayerSafeExplosion.effect = defaultSmallExplosionData2.effect;
            this.smallPlayerSafeExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
            this.smallPlayerSafeExplosion.ss = defaultSmallExplosionData2.ss;
            this.smallPlayerSafeExplosion.damage = 30;
            this.smallPlayerSafeExplosion.damageRadius = 1;
            Exploder.Explode(position, this.smallPlayerSafeExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);
            boomcount += 1;

        }

        public void DoSafeExplosion(Vector3 position)
        {

            ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
            this.smallPlayerSafeExplosion.effect = defaultSmallExplosionData2.effect;
            this.smallPlayerSafeExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
            this.smallPlayerSafeExplosion.ss = defaultSmallExplosionData2.ss;
            Exploder.Explode(position, this.smallPlayerSafeExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);
            boomcount += 1;

        }

        private ExplosionData smallPlayerSafeExplosion = new ExplosionData
        {
            damageRadius = 4f,
            damageToPlayer = 0f,
            doDamage = true,
            damage = 20f,
            doDestroyProjectiles = true,
            doForce = true,
            debrisForce = 0f,
            preventPlayerForce = true,
            explosionDelay = 0f,
            usesComprehensiveDelay = false,
            doScreenShake = false,
            playDefaultSFX = true,
            breakSecretWalls = true,
            secretWallsRadius = 3,
            force = 2,
            forceUseThisRadius = true,


        };


        private static T2 GetPrivateType<T, T2>(T obj, string field)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T2)f.GetValue(obj);
        }
        public float BoomCooldown;
        public bool touchFloor = false;
        public bool touchWall = false;
        public int bounces;
        DebrisObject debrisObject;
        public RoomHandler room;
        public PlayerController player;
        public int boomcount;
        public DebrisObject boostpad;
    }

    public class BoostPadController : MonoBehaviour
    {

        public BoostPadController()
        {


        }


        private void Start()
        {
            boostpad = this.gameObject.GetComponent<DebrisObject>();
            state = player.gameObject.GetOrAddComponent<WreckingSpeedBooster>();
            SpeedyModifier = WreckingBall.SpeedyModifier;


        }
        public StatModifier SpeedyModifier = null;

        protected void Update()
        {
            if(player != null)
            {

                if (boostpad != null)
                {
                    if (Vector2.Distance(boostpad.sprite.WorldCenter, player.specRigidbody.UnitCenter) <= 1.5f)
                    {
                        state.WreckingBoosted = 2.5f;
                    }

                    if(Existance <= 0)
                    {
                        UnityEngine.GameObject.Destroy(boostpad.gameObject);
                    }
                    else
                    {
                        Existance -= BraveTime.DeltaTime;
                    }
                   
                }
                
            }
        }
       
        public WreckingSpeedBooster state;
        public PlayerController player;
        public DebrisObject boostpad;
        public float Existance = 20;
    }
}