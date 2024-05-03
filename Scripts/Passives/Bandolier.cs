using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class Bandolier : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Shakey Bandolier";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Shoddy_bandolier";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Bandolier>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Petty Revenge";
            string longDesc = "An intentionally poorly crafted bandolier of grenades, designed to drop live grenades when shaken violently." +
                "\n\n\n - Knife_to_a_Gunfight"
                ;

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            //Set the rarity of the item

            item.quality = PickupObject.ItemQuality.B;

            Grenade = BuildPrefab();
            
        }

        public static GameObject Grenade;
        public override void Pickup(PlayerController player)
        {
            player.healthHaver.OnDamaged += HealthHaver_OnDamaged;
            base.Pickup(player);
        }

        private void HealthHaver_OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            martyrdom();
           
        }

        public void martyrdom()
        {
            GameObject bomb = UnityEngine.Object.Instantiate<GameObject>(Grenade, this.gameObject.GetComponent<tk2dBaseSprite>().sprite.WorldCenter, Quaternion.identity);
            tk2dBaseSprite component4 = bomb.GetComponent<tk2dBaseSprite>();
            if (component4)
            {
                component4.PlaceAtPositionByAnchor(this.gameObject.GetComponent<tk2dBaseSprite>().sprite.WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
                
                
            }

            DebrisObject debrisObject = LootEngine.DropItemWithoutInstantiating(bomb, this.gameObject.GetComponent<tk2dBaseSprite>().sprite.WorldCenter, this.Owner.CurrentGun.CurrentAngle.DegreeToVector2(), 2, false, false, true, false);

        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.healthHaver.OnDamaged -= HealthHaver_OnDamaged;
            return base.Drop(player);
        }

        private float randomizedAngle;
        public static GameObject BuildPrefab()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/Blast_potion/blast_potion_001", new GameObject("blast_potion"));
            bomb.SetActive(false);
            FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Grenade/grenade_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Grenade/grenade_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Grenade/grenade_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Grenade/grenade_004", collection),



            }, "blastDeploy", tk2dSpriteAnimationClip.WrapMode.Once);
            deployAnimation.fps = 12;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Grenade/grenade_006", collection),



            }, "blastExplode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 30;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

               SpriteBuilder.AddSpriteToCollection("Knives/Resources/Grenade/grenade_005", collection),




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
                    damageRadius = 4f,
                    damageToPlayer = 0,
                    damage = 20f,
                    breakSecretWalls = false,
                    secretWallsRadius = 0f,
                    forcePreventSecretWallDamage = true,
                    doDestroyProjectiles = true,
                    doForce = true,
                    pushRadius = 4,
                    force = 50,
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
                    effect = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<ProximityMine>().explosionData.effect,
                    freezeEffect = null,
                },
                explosionStyle = ProximityMine.ExplosiveStyle.TIMED,
                detectRadius = 3.5f,
                explosionDelay = 1f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "blastDeploy",
                explodeAnimName = "blastExplode",
                idleAnimName = "blastArmed",




            };

            var boom = bomb.AddComponent<ProximityMine>(proximityMine);


            return bomb;
        }



    }
}