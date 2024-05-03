using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using Dungeonator;
using System.Collections;
using MonoMod.RuntimeDetour;
using System.Reflection;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;

namespace Knives
{
    class EmergencyDanceParty : PlayerItem
    {
        public static void Register()
        {
            string itemName = "Emergency Dance Party";

            string resourceName = "Knives/Resources/EmergencyDanceParty";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<EmergencyDanceParty>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Life Of The Party";
            string longDesc =

                "Deploy a Holographic party to confuse your enemies. Decoys only have a chance to confuse an enemy. As with most partys getting shot brings it to an end." +
                "\n\n" +
                "A boombox fitted with technology from a holopilot suit. This gross misuse of secret military technology makes for an amazing impromtu party to mask your movements." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            objectToSpawn = BuildPrefab();
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 60f);

            item.quality = PickupObject.ItemQuality.B;
            ID = item.PickupObjectId;
        }
        public static int ID;

        public static GameObject objectToSpawn;

        public static GameObject BuildPrefab()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/BoomBox/BoomBox_001", new GameObject("BoomBox"));
            bomb.SetActive(false);
            FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BoomBox/BoomBox_001", collection),
                

            }, "Deploy", tk2dSpriteAnimationClip.WrapMode.Once);
            deployAnimation.fps = 12;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BoomBox/BoomBox_009", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/BoomBox/BoomBox_010", collection),

            }, "Explode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 10;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

               SpriteBuilder.AddSpriteToCollection("Knives/Resources/BoomBox/BoomBox_001", collection),
               SpriteBuilder.AddSpriteToCollection("Knives/Resources/BoomBox/BoomBox_002", collection),
               SpriteBuilder.AddSpriteToCollection("Knives/Resources/BoomBox/BoomBox_003", collection),
               SpriteBuilder.AddSpriteToCollection("Knives/Resources/BoomBox/BoomBox_004", collection),
               SpriteBuilder.AddSpriteToCollection("Knives/Resources/BoomBox/BoomBox_005", collection),
               SpriteBuilder.AddSpriteToCollection("Knives/Resources/BoomBox/BoomBox_006", collection),
               SpriteBuilder.AddSpriteToCollection("Knives/Resources/BoomBox/BoomBox_007", collection),
               SpriteBuilder.AddSpriteToCollection("Knives/Resources/BoomBox/BoomBox_008", collection),



            }, "Armed", tk2dSpriteAnimationClip.WrapMode.LoopSection);
            armedAnimation.fps = 15f;

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
                    damage = 6f,  // exactly enough to always kill the arrowkin it makes unless they are jammed or buffed
                    breakSecretWalls = false,
                    secretWallsRadius = 0,
                    forcePreventSecretWallDamage = false,
                    doDestroyProjectiles = false,
                    doForce = false,
                    pushRadius = 0,
                    force = 0,
                    debrisForce = 2f,
                    preventPlayerForce = false,
                    explosionDelay = 15f,
                    usesComprehensiveDelay = false,
                    comprehensiveDelay = 15,
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
                explosionDelay = 13f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "Deploy",
                explodeAnimName = "Explode",
                idleAnimName = "Armed",




            };

            var boom = bomb.AddComponent<ProximityMine>(proximityMine);


            return bomb;
        }


        public override void Pickup(PlayerController player)
        {
            playertrail = player.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            playertrail.shadowLifetime = .15f;
            playertrail.shadowTimeDelay = .0001f;
            playertrail.dashColor = new Color(.01f, .50f, .67f);
            playertrail.spawnShadows = false;

            Hooks.OnReinforcementWave = (System.Action<RoomHandler>)Delegate.Combine(Hooks.OnReinforcementWave, new System.Action<RoomHandler>(this.NewWave));

            player.OnReceivedDamage += Player_OnReceivedDamage;


            base.Pickup(player);
        }

        private void Player_OnReceivedDamage(PlayerController player)
        {
            if (player.healthHaver.IsAlive && doingParty)
            {

                GlobalSparksDoer.DoRandomParticleBurst(20, boombox.transform.position , boombox.transform.position, new Vector3(1f, 1f, 0f), 120f, 0.75f, null, null, null, GlobalSparksDoer.SparksType.SPARKS_ADDITIVE_DEFAULT);
                UnityEngine.GameObject.Destroy(boombox.gameObject);
                AkSoundEngine.PostEvent("Stop_DanceParty_00" + CurrentPlaying, base.gameObject);
                AkSoundEngine.PostEvent("Play_DanceParty_AbruptStop", base.gameObject);
                runClearDudes();
                playertrail.spawnShadows = false;
                doingParty = false;
                this.ForceApplyCooldown(player);
            }
        }

        private void runClearDudes()
        {
            foreach (KageBunshinController item in m_extantKages)
            {
                if(item!= null)
                {
                    if(item.gameObject != null)
                    {
                        GlobalSparksDoer.DoRandomParticleBurst(20, item.transform.position, item.transform.position, new Vector3(1f, 1f, 0f), 120f, 0.75f, null, null, null, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                        UnityEngine.Object.Destroy(item.gameObject);
                    }
                }
            }
            m_extantKages.Clear();
           
        }

        private void NewWave(RoomHandler room)
        {
           
            if (doingParty)
            {
                StartCoroutine(AttractNewWave(room));
            }
        }

        private IEnumerator AttractNewWave(RoomHandler room)
        {
            yield return new WaitForSeconds(.5f);
            foreach (KageBunshinController kage in m_extantKages)
            {
                AttractEnemies(kage, room, 2);
            }
        }

        public int CurrentPlaying = 1;
        public List<KageBunshinController> m_extantKages = new List<KageBunshinController>
        {
        };
        public override void DoEffect(PlayerController user)
        {
            int Rotation = 0;
            for (int i = 0; i <= 5; i++)
            {
                
                if(i != 0)
                {
                    GameObject shadowclone = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(820) as SpawnObjectPlayerItem).objectToSpawn, user.specRigidbody.UnitCenter, Quaternion.identity);
                    KageBunshinController kageBunshin = shadowclone.GetOrAddComponent<KageBunshinController>();
                    kageBunshin.UsesRotationInsteadOfInversion = true;
                    kageBunshin.RotationAngle = Rotation;
                    kageBunshin.Duration = 13f;
                    OverrideInit(user, kageBunshin);

                    AttractEnemies(kageBunshin,user.CurrentRoom,i);
                    kageBunshin.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox, CollisionLayer.Projectile, CollisionLayer.PlayerCollider, CollisionLayer.LowObstacle));
                    
                    m_extantKages.Add(kageBunshin);
                    ImprovedAfterImage trail = shadowclone.GetOrAddComponent<ImprovedAfterImage>();
                    trail.shadowLifetime = .15f;
                    trail.shadowTimeDelay = .0001f;
                    trail.dashColor = new Color(.01f, .50f, .67f);
                    trail.spawnShadows = true;
                }
                Rotation += 60;
            }

            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(objectToSpawn, user.specRigidbody.UnitCenter, Quaternion.identity);
            Vector3 vector = user.unadjustedAimPoint - user.LockedApproximateSpriteCenter;
            Vector3 vector2 = user.specRigidbody.UnitCenter;
            if (vector.y > 0f)
            {
                vector2 += Vector3.up * 0.25f;
            }
            tk2dBaseSprite component4 = gameObject.GetComponent<tk2dBaseSprite>();
            if (component4)
            {
                component4.PlaceAtPositionByAnchor(vector2, tk2dBaseSprite.Anchor.MiddleCenter);
            }
            Vector2 vector3 = user.unadjustedAimPoint - user.LockedApproximateSpriteCenter;
            DebrisObject debrisObject = LootEngine.DropItemWithoutInstantiating(gameObject, user.specRigidbody.UnitCenter, vector3, .5f, false, false, true, false);
            boombox = gameObject;

            playertrail.spawnShadows = true;
            float dura = 13f;

            StartCoroutine(ItemBuilder.HandleDuration(this, dura, user, EndEffect));
            doingParty = true;
            StartCoroutine(DanceParty());
            int rng = UnityEngine.Random.Range(1, 6);
            CurrentPlaying = rng;

            AkSoundEngine.PostEvent("Play_DanceParty_00" + CurrentPlaying , base.gameObject);
            

            
        }
        public GameObject boombox;
        
        public bool doingParty = false;
        private IEnumerator DanceParty()
        {
            while (doingParty)
            {

                int RNG = UnityEngine.Random.Range(0, validColors.Count);
                RenderSettings.ambientLight = validColors[RNG];
                CameraController camera = GameManager.Instance.MainCameraController;
                camera.DoScreenShake(shake, this.LastOwner.CenterPosition);
                yield return new WaitForSeconds(.5f);
            }
        }
       

        public ScreenShakeSettings shake = new ScreenShakeSettings
        {
            magnitude = .5f,
            speed = 2f,
            time = 0f,
            falloff = .1f,
            direction = new Vector2(0, 0),
            vibrationType = ScreenShakeSettings.VibrationType.Auto,
            simpleVibrationStrength = Vibration.Strength.Medium,
            simpleVibrationTime = Vibration.Time.Instant
        };

        public List<Color> validColors = new List<Color>
        {
            new Color(1,.78f,0), // yellow
            new Color(.89f,0f,.54f), // pink
            new Color(.1f,.80f,.84f), // blue
            new Color(.34f,.90f,.15f), //green
            new Color(.84f,.1f,.1f), //red
            new Color(.58f,.15f,.90f), //purple
        };

        private void AttractEnemies(KageBunshinController kageBunshin, RoomHandler room, float ChanceMod)
        {
            
            List<AIActor> activeEnemies = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            if (activeEnemies != null)
            {
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    if (activeEnemies[i].OverrideTarget == null)
                    {
                        if(1.5f >= UnityEngine.Random.Range(1, 4.5f - (ChanceMod/2)))
                        {
                            activeEnemies[i].OverrideTarget = kageBunshin.specRigidbody;
                        }
                        
                    }
                }
            }
        }

        private void OverrideInit(PlayerController user, KageBunshinController kage)
        {
            kage.Owner = user;
            kage.sprite = kage.GetComponentInChildren<tk2dSprite>();
            kage.GetComponentInChildren<Renderer>().material.SetColor("_FlatColor", new Color(0.25f, 0.25f, 0.25f, 1f));
            kage.sprite.usesOverrideMaterial = true;
            if (kage.Duration > 0f)
            {
                UnityEngine.Object.Destroy(kage.gameObject, kage.Duration);
            }
        }
        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && user.IsInCombat;
        }


        public override void Update()
        {

            if (this.LastOwner != null)
            {

                this.CanBeUsed(this.LastOwner);



            }
            base.Update();
        }

        protected void EndEffect(PlayerController user)
        {
            playertrail.spawnShadows = false;
            doingParty = false;
            m_extantKages.Clear();
        }

        public override void OnPreDrop(PlayerController user)
        {
            AkSoundEngine.PostEvent("Stop_DanceParty_00" + CurrentPlaying, base.gameObject);
            EndEffect(user);
            user.OnReceivedDamage -= Player_OnReceivedDamage;

            Hooks.OnReinforcementWave = (System.Action<RoomHandler>)Delegate.Remove(Hooks.OnReinforcementWave, new System.Action<RoomHandler>(this.NewWave));


            base.OnPreDrop(user);
        }
        ImprovedAfterImage playertrail;

    }
}

