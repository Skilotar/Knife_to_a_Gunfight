using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;

namespace Knives
{
    class CharmMaker : PlayerItem
    {

        public static void Register()
        {
            //The name of the item
            string itemName = "Charm Machine";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Charm/Maker/CharmMachine";
            resource = resourceName;
            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<CharmMaker>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Charm Collector";
            string longDesc = "Consumes Material and produces Weapon Charm Capsules.\n\n" +
                "Highly Irradiated materials should not be inserted into the charm maker. " +
                "If an irradiated material is inserted your warranty will be voided.\n" +
                "Charming Charms Co. is not responsible for any illnesses or injuries causes by extraction of raw material needed to produce charms.\n" +
                "Do not operate while intoxicated.\n" +
                "Thank you.\n" +
                "\n\n\n - Knife_to_a_Gunfight";


            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, .25f);

            item.quality = PickupObject.ItemQuality.B;


            ID = item.PickupObjectId;
            ContaminationBomb = BuildPrefab();
            BuildVFX();
        }
        public static GameObject HappyVFX;
        public static GameObject ContaminationBomb;
        public static string resource;
        public static int ID;

        //applies damage on last use
        public override void DoEffect(PlayerController user)
        {
            if (user)
            {
                IPlayerInteractable nearestInteractable = user.CurrentRoom.GetNearestInteractable(user.sprite.WorldCenter, 1.5f, user);
                PickupObject pick = (nearestInteractable as PickupObject);
                if (CheckRadiationList(pick))
                {
                    user.StartCoroutine(RadiationContamination(pick,user));
                }
                else
                {
                    user.StartCoroutine(RandomRoll(pick,user));
                }
                
            }
        }

        private IEnumerator RandomRoll(PickupObject pick, PlayerController user)
        {
            // sound and visual
            AkSoundEngine.PostEvent("Play_CharmMachineHappy", base.gameObject);
            GameObject Parry = user.PlayEffectOnActor(HappyVFX, new Vector3(-2f, .0f), true);
            Parry.GetComponent<tk2dBaseSprite>().scale *= 1f;
            Parry.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(user.transform.position + new Vector3(0f, 2f), tk2dBaseSprite.Anchor.LowerLeft);
            Parry.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");
            UnityEngine.GameObject.Destroy(pick.gameObject);
            yield return new WaitForSeconds(2.5f);

            int NumBaubles = 1;
            if (pick.quality == ItemQuality.D) NumBaubles = 1;
            if (pick.quality == ItemQuality.C) NumBaubles = 2;
            if (pick.quality == ItemQuality.B) NumBaubles = 2;
            if (pick.quality == ItemQuality.A) NumBaubles = 4;
            if (pick.quality == ItemQuality.S) NumBaubles = 5;

           
            
            for (int i = 0; i < NumBaubles; i++)
            {
                LootEngine.SpawnItem(PickupObjectDatabase.GetById(CharmBauble.ID).gameObject, user.sprite.WorldCenter, Vector2.zero, 1, true, true);
            }
        }

        private IEnumerator RadiationContamination(PickupObject pick, PlayerController user)
        {
            yield return new WaitForSeconds(.01f);
            // sound and visual
            AkSoundEngine.PostEvent("Play_CharmMachineUnHappy", base.gameObject);
            AkSoundEngine.PostEvent("Play_CharmMachineUnHappy", base.gameObject);

            UnityEngine.GameObject.Destroy(pick.gameObject);
            // throw boom 
            GameObject bomb = UnityEngine.Object.Instantiate<GameObject>(ContaminationBomb, user.transform.position, Quaternion.identity);
            KTGStatusBomb stat = bomb.GetOrAddComponent<KTGStatusBomb>();
            stat.radioactiveContamination = true;
            tk2dBaseSprite component4 = bomb.GetComponent<tk2dBaseSprite>();
            if (component4)
            {
                component4.PlaceAtPositionByAnchor(user.transform.position, tk2dBaseSprite.Anchor.MiddleCenter);
                
            }
            RenderSettings.ambientLight = ExtendedColours.lime;
            Vector3 vector = user.unadjustedAimPoint - user.LockedApproximateSpriteCenter;
            DebrisObject debrisObject = LootEngine.DropItemWithoutInstantiating(bomb, this.gameObject.GetComponent<tk2dBaseSprite>().sprite.WorldCenter, vector, 0, false, false, true, false);
            user.RemoveActiveItem(ID);

        }

        private bool CheckRadiationList(PickupObject pick)
        {
            bool Bigif = false;
            foreach (int Id in irradiated)
            {
                if (Id == pick.PickupObjectId)
                {
                    Bigif = true;
                }
            }

            return Bigif;
        }

        private bool CheckCharmList(PickupObject pick)
        {
            bool Bigif = false;
            foreach (int Id in GlobalCharmList.Charmslist)
            {
                if(Id == pick.PickupObjectId)
                {
                    Bigif = true;
                }
            }

            return Bigif;
        }

        public override bool CanBeUsed(PlayerController user)
        {

            return base.CanBeUsed(user) && checkStuff(user) == false;
        }

        private bool checkStuff(PlayerController user)
        {
            bool failChecks = false;

            IPlayerInteractable nearestInteractable = user.CurrentRoom.GetNearestInteractable(user.sprite.WorldCenter, 1.5f, user);
            PickupObject pick = (nearestInteractable as PickupObject);
            if (nearestInteractable != null) // Anything Nearby? 
            {
                if (nearestInteractable is PickupObject == false) failChecks = true;
                if (pick != null)
                {
                    if (pick.gameObject.GetComponent<CharmBauble>() != null) failChecks = true;
                    if (pick.gameObject.GetComponent<MasterCharmComp>() != null) failChecks = true;
                    if (CheckCharmList(pick)) failChecks = true;
                }
               
            }
            else
            {
                failChecks = true;
            }

            return failChecks;
        }

        public override void Update()
        {

            if (this.LastOwner)
            {
                this.CanBeUsed(this.LastOwner);
            }


            base.Update();
        }

        public List<int> irradiated = new List<int>
        {
            87, // gama ray
            443, // big boy
            204, // irradiated lead
            313, // Monster Blood
            GunZilla_Tail.ID,
            449, // teleporter prototype
            331, // Science Canon
        };

        public static GameObject BuildPrefab()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/Charm/Maker/bomb/CharmMachine_001", new GameObject("RadiationGrenade"));
            bomb.SetActive(false);
            FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Charm/Maker/bomb/CharmMachine_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Charm/Maker/bomb/CharmMachine_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Charm/Maker/bomb/CharmMachine_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Charm/Maker/bomb/CharmMachine_004", collection),


            }, "CharmDeploy", tk2dSpriteAnimationClip.WrapMode.Once);
            deployAnimation.fps = 12;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/Charm/Maker/bomb/CharmMachine_008", collection),
               
            }, "CharmExplode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 30;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

               SpriteBuilder.AddSpriteToCollection("Knives/Resources/Charm/Maker/bomb/CharmMachine_005", collection),
               SpriteBuilder.AddSpriteToCollection("Knives/Resources/Charm/Maker/bomb/CharmMachine_006", collection),




            }, "CharmArmed", tk2dSpriteAnimationClip.WrapMode.LoopSection);
            armedAnimation.fps = 10.0f;

            foreach (var frame in armedAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

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
                        magnitude = 5,
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
                    effect = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData.effect,
                    freezeEffect = null,
                },
                explosionStyle = ProximityMine.ExplosiveStyle.TIMED,
                detectRadius = 3.5f,
                explosionDelay = 2.5f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "CharmDeploy",
                explodeAnimName = "CharmExplode",
                idleAnimName = "CharmArmed",




            };

            var boom = bomb.AddComponent<ProximityMine>(proximityMine);


            return bomb;
        }
        public static void BuildVFX()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/Charm/Maker/happy/CharmMachine_001", null);
            gameObject.SetActive(false);
            ItemAPI.FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            tk2dSpriteAnimator animator = gameObject.AddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimationClip animationClip = new tk2dSpriteAnimationClip
            {
                fps = 6,
                wrapMode = tk2dSpriteAnimationClip.WrapMode.Once
            };
            GameObject spriteObject = new GameObject("spriteObject");
            ItemBuilder.AddSpriteToObject("spriteObject", "Knives/Resources/Charm/Maker/happy/CharmMachine_001", spriteObject); //add 1
            tk2dSpriteAnimationFrame starterFrame = new tk2dSpriteAnimationFrame
            {
                spriteId = spriteObject.GetComponent<tk2dSprite>().spriteId,
                spriteCollection = spriteObject.GetComponent<tk2dSprite>().Collection
            };
            tk2dSpriteAnimationFrame[] frameArray = new tk2dSpriteAnimationFrame[]
            {
                starterFrame
            };
            animationClip.frames = frameArray;
            for (int i = 1; i < 16; i++)
            {
                GameObject spriteForObject = new GameObject("spriteForObject");
                if (i <= 9)
                {
                    ItemBuilder.AddSpriteToObject("spriteForObject", $"Knives/Resources/Charm/Maker/happy/CharmMachine_00{i}", spriteForObject);
                }
                else
                {
                    ItemBuilder.AddSpriteToObject("spriteForObject", $"Knives/Resources/Charm/Maker/happy/CharmMachine_0{i}", spriteForObject);
                }

                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame
                {
                    spriteId = spriteForObject.GetComponent<tk2dBaseSprite>().spriteId,
                    spriteCollection = spriteForObject.GetComponent<tk2dBaseSprite>().Collection
                };
                animationClip.frames = animationClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
            animationClip.name = "start";
            animator.Library.clips = new tk2dSpriteAnimationClip[] { animationClip };
            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.playAutomatically = true;
            HappyVFX = gameObject;
        }

    }
}