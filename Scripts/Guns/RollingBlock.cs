using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using HarmonyLib;
using System.Collections.Generic;

namespace Knives
{
	class RollingBlock : AdvancedGunBehaviour
	{

		public static void Add()
		{

			Gun gun = ETGMod.Databases.Items.NewGun("Rolling Block Rifle", "Rolling_block");
			Game.Items.Rename("outdated_gun_mods:rolling_block_rifle", "ski:rolling_block_rifle");
			gun.gameObject.AddComponent<RollingBlock>();
			gun.SetShortDescription("Roll Tide");
			gun.SetLongDescription("Roll to immediately reload. Reloading like this over an enemy bullet make the next shot a critical\n\n" +
				"A unique breech mechanism allows for speedy reloading durring a tumbling motion.\n" +
				"Not planned as a feature originally this speedy reload was discovered by a particularly limber gunslinger." +
				"\n\n- Knife_to_a_gunfight");
			gun.SetupSprite(null, "Rolling_block_idle_001", 1);
			gun.SetAnimationFPS(gun.shootAnimation, 8);
			gun.SetAnimationFPS(gun.reloadAnimation, 5);
			gun.SetAnimationFPS(gun.criticalFireAnimation, 8);


			gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(25) as Gun, true, false);
			gun.PreventNormalFireAudio = true;

			gun.gunHandedness = GunHandedness.TwoHanded;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.angleVariance = 0f;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			Gun gun3 = (Gun)ETGMod.Databases.Items["blunderbuss"];
			gun.muzzleFlashEffects = gun3.muzzleFlashEffects;
			gun.gunClass = GunClass.RIFLE;
			gun.reloadTime = 1.7f;
			gun.DefaultModule.cooldownTime = .7f;
			gun.DefaultModule.numberOfShotsInClip = 1;
			gun.SetBaseMaxAmmo(100);
			gun.quality = PickupObject.ItemQuality.B;
			


			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);

			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			gun.barrelOffset.transform.localPosition = new Vector3(39 / 16f, 7 / 16f, 0f);
			gun.carryPixelOffset = new IntVector2(10, 0);
			projectile.baseData.damage = 16f;
			projectile.baseData.speed *= .8f;
			projectile.baseData.range = 20f;
			

			gun.shellsToLaunchOnFire = 1;
			Gun gun2 = PickupObjectDatabase.GetById(84) as Gun;
			gun.shellCasing = gun2.shellCasing;


			ETGMod.Databases.Items.Add(gun, null, "ANY");
			ID = gun.PickupObjectId;
		}

		public static int ID;

		public System.Random rand = new System.Random();

		
		public override void PostProcessProjectile(Projectile projectile)
		{
			if(NextShotCrits == true)
            {
				ParticleSpewer spew = projectile.gameObject.GetOrAddComponent<ParticleSpewer>();
				spew.particlesPerSecond = 10;
				spew.particleType = GlobalSparksDoer.SparksType.SPARKS_ADDITIVE_DEFAULT;

				projectile.baseData.damage *= 1.5F;
				gun.spriteAnimator.ForceClearCurrentClip();
				gun.spriteAnimator.Play("Rolling_block_critical_fire");
				
				NextShotCrits = false;
				if (DoubleUp == true)
                {
					NextShotCrits = true;
					DoubleUp = false;
                }

			}
		}


		public override void OnReload(PlayerController player, Gun gun)
		{
			AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);

			doingrolload = false;
			base.OnReload(player, gun);

		}

		public bool DoingStandTracking = false;
        private bool doingrolload = false;
		public float RolloadCooldown = 0;
		public bool NextShotCrits = false;
        public override void Update()
		{
			base.Update();
			if (this.Owner != null)
			{
				PlayerController player = (this.Owner as PlayerController);
				if ((this.Owner as PlayerController).IsDodgeRolling)
				{

					if (doingrolload == false)
					{
						gun.spriteAnimator.ForceClearCurrentClip();
						tk2dSpriteAnimationClip clip = gun.spriteAnimator.GetClipByName("Rolling_block_reload");
						gun.spriteAnimator.Play(clip, 0, 10);
						gun.ForceImmediateReload();
						GameUIRoot.Instance.ForceClearReload(player.PlayerIDX);
						doingrolload = true;
					}


					AffectNearbyProjectiles(player.CenterPosition,1.8f);

				}
				if (doingrolload == true && !(this.Owner as PlayerController).IsDodgeRolling)
				{
					doingrolload = false;
				}

				if (player.HasActiveItem(663)) //RatPack
				{
					HasRatSack = true;
				}
				else
				{
					HasRatSack = false;
				}

				if (TriggerCollect != null)
				{
					NextShotCrits = true;
					TriggerCollect = null;
					if(player.PlayerHasActiveSynergy("Roll Tide"))
                    {
						DoubleUp = true;
                    }
				}

				if (NextShotCrits == true)
				{
					if (doingCritRoutine == false)
					{
						StartCoroutine(CritRoutine(this.gun));

					}
				}
			}

		}
		public bool DoubleUp = false;
		public override void OnSwitchedAwayFromThisGun()
		{
			if (doingCritRoutine)
			{
				StopCoroutine(CritRoutine(this.gun));
				if (origiColor != null)
				{
					this.gun.sprite.color = origiColor;
				}
				else
				{
					this.gun.sprite.color = new Color(1, 1, 1, 1);
				}

				doingCritRoutine = false;
			}
			base.OnSwitchedAwayFromThisGun();
		}

		public Color origiColor;
		public bool doingCritRoutine;
		private IEnumerator CritRoutine(Gun gun)
		{
			doingCritRoutine = true;
			origiColor = gun.sprite.color;
			while (NextShotCrits)
			{

				gun.sprite.color = new Color(.40f,.56f,1f);
				GlobalSparksDoer.DoRandomParticleBurst(1, gun.sprite.WorldBottomLeft, gun.sprite.WorldTopRight, new Vector3(1f, 1f, 0f), 120f, 0.2f, null, null, null, GlobalSparksDoer.SparksType.SPARKS_ADDITIVE_DEFAULT);
				yield return new WaitForSeconds(.1f);
				yield return null;
			}
			if (origiColor != null)
			{
				this.gun.sprite.color = origiColor;
			}
			else
			{
				this.gun.sprite.color = new Color(1, 1, 1, 1);
			}
			doingCritRoutine = false;
			yield return null;
		}

		
		private void AffectNearbyProjectiles(Vector2 center, float radius)
		{
			for (int i = 0; i < StaticReferenceManager.AllProjectiles.Count; i++)
			{
				Projectile projectile = StaticReferenceManager.AllProjectiles[i];
				if (projectile && projectile.Owner is AIActor)
				{
					float sqrMagnitude = (projectile.transform.position.XY() - center).sqrMagnitude;
					if (sqrMagnitude < radius)
					{
						NextShotCrits = true;
						if ((this.gun.CurrentOwner as PlayerController).PlayerHasActiveSynergy("Roll Tide"))
						{
							DoubleUp = true;
						}
					}
				}
			}
		}
		public static bool HasRatSack = false;
		public static Projectile TriggerCollect = null;

		[HarmonyPatch(typeof(RatPackItem), nameof(RatPackItem.EatBullet))]
		public class RatPatch
		{
			[HarmonyPrefix]
			public static void Prefix(Projectile p) //ProjEatten
			{
				if (HasRatSack)
				{
					TriggerCollect = p;
				}
			}
		}
	}
}