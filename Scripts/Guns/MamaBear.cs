using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;
using System.Collections.Generic;

namespace Knives
{

    public class MamaBear : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Mama Bear", "MamaBear");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:mama_bear", "ski:mama_bear");
            gun.gameObject.AddComponent<MamaBear>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Magna Ursa");
            gun.SetLongDescription("Bullets detonate after contact.\n\n" +

                "The favored firearm of Helena Shivarnova, Great Empress of space Russia.\n" +
                "The rounds it fires are non explosive in nature, but they explode anyway for fear of disappointing their Empress." +

                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "MamaBear_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 7);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 3f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.6f;
            gun.DefaultModule.cooldownTime = .8f;
            gun.DefaultModule.numberOfShotsInClip = 8;
            gun.SetBaseMaxAmmo(100);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.B;

            
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            
            projectile.baseData.damage = 12f;
            projectile.baseData.speed *= 1.2f;
            projectile.baseData.range *= 5;
            

            projectile.SetProjectileSpriteRight("bear", 5, 4, false, tk2dBaseSprite.Anchor.MiddleCenter, 5, 4);
            
            projectile.shouldFlipVertically = true;
            projectile.shouldRotate = true;
            projectile.baseData.force = 20;
            projectile.AppliesKnockbackToPlayer = true;
            projectile.PlayerKnockbackForce = 5;

            BecomeBomb bomb = projectile.gameObject.GetOrAddComponent<BecomeBomb>();
            bomb.gameobjecttospawn = BuildPrefab();
            
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "golden_gun";

            gun.shellsToLaunchOnFire = 0;
            gun.shellsToLaunchOnReload = 8;
            gun.reloadShellLaunchFrame = 3;
            Gun gun3 = PickupObjectDatabase.GetById(37) as Gun;
            gun.muzzleFlashEffects = gun3.muzzleFlashEffects;
            Gun gun2 = PickupObjectDatabase.GetById(84) as Gun;
            gun.shellCasing = gun2.shellCasing;
            gun.barrelOffset.transform.localPosition = new Vector3(40 / 16f, 28 / 16f, 0f);

            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.EXPLOSIVE;


            ETGMod.Databases.Items.Add(gun, null, "ANY");
            MiscToolMethods.TrimAllGunSprites(gun);
            ID = gun.PickupObjectId;
        }
        public static int ID;
        public static GameObject BuildPrefab()
        {

            var bomb = SpriteBuilder.SpriteFromResource("Knives/Resources/Blast_potion/blast_potion_001", new GameObject("blast_potion"));
            bomb.SetActive(false);
            FakePrefab.MarkAsFakePrefab(bomb);

            var animator = bomb.AddComponent<tk2dSpriteAnimator>();
            var collection = (PickupObjectDatabase.GetById(108) as SpawnObjectPlayerItem).objectToSpawn.GetComponent<tk2dSpriteAnimator>().Library.clips[0].frames[0].spriteCollection;

            var deployAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/MamaBear/bear_bomb_001", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/MamaBear/bear_bomb_002", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/MamaBear/bear_bomb_003", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/MamaBear/bear_bomb_004", collection),
                

            }, "blastDeploy", tk2dSpriteAnimationClip.WrapMode.Once);
            deployAnimation.fps = 12;
            foreach (var frame in deployAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var explodeAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/MamaBear/bear_bomb_006", collection),

            }, "blastExplode", tk2dSpriteAnimationClip.WrapMode.Once);
            explodeAnimation.fps = 30;
            foreach (var frame in explodeAnimation.frames)
            {
                frame.eventLerpEmissiveTime = 0.5f;
                frame.eventLerpEmissivePower = 30f;
            }

            var armedAnimation = SpriteBuilder.AddAnimation(animator, collection, new List<int>
            {

                SpriteBuilder.AddSpriteToCollection("Knives/Resources/MamaBear/bear_bomb_005", collection),
                SpriteBuilder.AddSpriteToCollection("Knives/Resources/MamaBear/bear_bomb_006", collection),

            }, "blastArmed", tk2dSpriteAnimationClip.WrapMode.LoopSection);
            armedAnimation.fps = 1f;

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
                    damage = 15f,
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
                        magnitude = 1,
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
                explosionDelay = .7f,
                usesCustomExplosionDelay = false,
                customExplosionDelay = 0.1f,
                deployAnimName = "blastDeploy",
                explodeAnimName = "blastExplode",
                idleAnimName = "blastArmed",




            };

            var boom = bomb.AddComponent<ProximityMine>(proximityMine);


            return bomb;
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_WPN_seriouscannon_shot_01", base.gameObject);
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            
            base.PostProcessProjectile(projectile);
        }

        private bool HasReloaded;
        //This block of code allows us to change the reload sounds.
        public override void Update()
        {
            if (gun.CurrentOwner)
            {

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;

                }


            }

        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_mailbox_reload_01", base.gameObject);
                //kSoundEngine.PostEvent("Play_WPN_rpg_reload_01", base.gameObject);
                

            }

        }

	}


    public class BecomeBomb : MonoBehaviour
	{

		public BecomeBomb()
		{

		}


		private void Start()
		{
			this.projectile = base.GetComponent<Projectile>();
			this.parentOwner = this.projectile.ProjectilePlayerOwner();
			this.gun = this.projectile.PossibleSourceGun;
            BounceProjModifier bnc = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
            bnc.numberOfBounces = 1;

            PierceProjModifier poke =projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            poke.penetration = 1;
			projectile.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(projectile.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreCollisontile));
			projectile.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(projectile.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnHitbody));
			
		}
		
		private void OnHitbody(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
		{
            AkSoundEngine.PostEvent("Play_WPN_seriouscannon_impact_01", base.gameObject);
           myRigidbody.projectile.StartCoroutine(Bombify(myRigidbody.projectile, false));
            
        }

		private void OnPreCollisontile(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
		{
            AkSoundEngine.PostEvent("Play_WPN_seriouscannon_impact_01", base.gameObject);
            myRigidbody.projectile.StartCoroutine(Bombify(myRigidbody.projectile, true));
		}

        private IEnumerator Bombify(Projectile projectile, bool wall)
        {
            projectile.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Remove(projectile.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreCollisontile));
            projectile.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Remove(projectile.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnHitbody));

            if (wall) yield return new WaitForSeconds(.05f);

            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(gameobjecttospawn, projectile.LastPosition, Quaternion.identity);
            tk2dBaseSprite component4 = gameObject.GetComponent<tk2dBaseSprite>();
            if (component4)
            {
                component4.PlaceAtPositionByAnchor(projectile.LastPosition, tk2dBaseSprite.Anchor.MiddleCenter);
            }
            
            DebrisObject debis = LootEngine.DropItemWithoutInstantiating(gameObject, projectile.LastPosition, projectile.Direction, projectile.Speed / 4, false, false, true, false);
            var ang = projectile.Direction.ToAngle();
            if (ang > 135 || ang < -135)
            {
                debis.sprite.FlipX = true;
            }
            projectile.SuppressHitEffects = true;
            projectile.hitEffects.suppressMidairDeathVfx = true;
            projectile.DieInAir();
            
            yield return null;
        }

        public bool once = false;


		public bool hasbeenBuffed = false;

		public bool hasbeenupdated = false;
        public GameObject gameobjecttospawn = null;

        private Projectile projectile;
		public Projectile shatterto;
		public Gun gun;

		private PlayerController parentOwner;

		private Projectile TargetProjectile;
	}
}


