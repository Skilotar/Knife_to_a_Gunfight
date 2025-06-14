using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using ItemAPI;
using System.Reflection;

namespace Knives
{
    class FizzleNade : PlayerItem
    {
        // Only real use is for Monk
        public static void Register()
        {

            string itemName = "Frag Grenade";

            string resourceName = "Knives/Resources/Cook_grenade_001";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<FizzleNade>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Cooked to Perfection";
            string longDesc = "Hold to cook grenade.\n\n" +
                "A genade for the brave of heart. Holding onto a live grenade is scary but a well cooked frag can serve up a perfect impact." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 100);

            item.quality = PickupObject.ItemQuality.C;
            item.UsesNumberOfUsesBeforeCooldown = true;
            item.numberOfUses = 2;
            nade = BuildPrefab();

        }

        public static GameObject BuildPrefab()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/Cook_grenade_001", new GameObject("Nade"));
            bomb.SetActive(false);
            FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Cook_grenade_001", collection),
                
            }, "blastDeploy", tk2dSpriteAnimationClip.WrapMode.Once);
            deployAnimation.fps = 12;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Cook_grenade_001", collection),



            }, "blastExplode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 30;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Cook_grenade_001", collection),

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
                    damageRadius = 2f,
                    damageToPlayer = 0,
                    damage = 25f,
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
                    doStickyFriction = true,
                    doExplosionRing = true,
                    isFreezeExplosion = false,
                    freezeRadius = 5,
                    IsChandelierExplosion = false,
                    rotateEffectToNormal = false,
                    ignoreList = new List<SpeculativeRigidbody>(),
                    overrideRangeIndicatorEffect = null,
                    effect = Explosions.ExplosiveRounds.effect,
                    freezeEffect = null,
                },
                explosionStyle = ProximityMine.ExplosiveStyle.TIMED,
                detectRadius = 3.5f,
                explosionDelay = 1.5f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "blastDeploy",
                explodeAnimName = "blastExplode",
                idleAnimName = "blastArmed",
            };

            var boom = bomb.AddComponent<ProximityMine>(proximityMine);


            return bomb;
        }

        public static GameObject nade;
        public static GameObject reticle;

        public override void Pickup(PlayerController player)
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/Nade_reticle_001", null);
            gameObject.SetActive(false);
            ItemAPI.FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            reticle = gameObject;
            base.Pickup(player);
        }

        public override void Update()
        {
            if (this.LastOwner != null && held == true)
            {
                BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(this.LastOwner.PlayerIDX);
                if (!instanceForPlayer.ActiveActions.UseItemAction.IsPressed && Time.timeScale != 0 && !instanceForPlayer.ActiveActions.MapAction.IsPressed)
                {
                    this.IsCurrentlyActive = false;
                    held = false;
                }
                else
                {
                    
                }

            }

            base.Update();
        }

        
        public bool held = false;
        public override void DoEffect(PlayerController user)
        {
            
            if(held == false)
            {
                user.DidUnstealthyAction();
                base.StartCoroutine(ItemBuilder.HandleDuration(this, 1.5f, user, new Action<PlayerController>(this.EndEffect)));
                

                held = true;
            }
            
        }
        public override void OnPreDrop(PlayerController user)
        {
            held = false;
            base.OnPreDrop(user); 
        }

        
        private void EndEffect(PlayerController obj)
        {
            Vector2 dir = this.LastOwner.unadjustedAimPoint - this.LastOwner.CurrentGun.barrelOffset.transform.position;
            GameObject tumble = UnityEngine.Object.Instantiate<GameObject>(FizzleNade.nade, this.LastOwner.transform.position, Quaternion.identity);
            DebrisObject debrisObject = LootEngine.DropItemWithoutInstantiating(tumble, this.LastOwner.transform.position + new Vector3(.5f, 0), dir, 12, false, false, true, false);
            ProximityMine mine = debrisObject.GetComponent<ProximityMine>();
            float elapsed = GetPrivateType<PlayerItem, float>(this, "m_activeElapsed");
            float dur = GetPrivateType<PlayerItem, float>(this, "m_activeDuration");
            float remain = dur - elapsed;
            mine.explosionDelay = remain;
            BurstOnDeath burst = tumble.GetOrAddComponent<BurstOnDeath>();
            burst.player = this.LastOwner;
        }
        public class BurstOnDeath : MonoBehaviour
        {

            public BurstOnDeath()
            {


            }


            private void Start()
            {
               
            }
            private void OnDestroy()
            {
                float baseang = UnityEngine.Random.Range(0, 360);
                float var = 0;
                for(int i = 0;i < 5; i++)
                {
                    MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, base.gameObject.transform.position, player, baseang, var);
                    var += 72;
                }
            }
            protected void Update()
            {
                if (player != null)
                {

                }
            }

            public PlayerController player;

        }


        private static T2 GetPrivateType<T, T2>(T obj, string field)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T2)f.GetValue(obj);
        }
    }




}