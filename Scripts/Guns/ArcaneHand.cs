using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Knives
{
	class Arcane_Hand : AdvancedGunBehaviour
	{

		public static void Add()
		{

			Gun gun = ETGMod.Databases.Items.NewGun("Arcane Hand", "Arcane_Cannon");
			Game.Items.Rename("outdated_gun_mods:arcane_hand", "ski:arcane_hand");
			gun.gameObject.AddComponent<Arcane_Hand>();
			gun.SetShortDescription("Hand Cannon");
			gun.SetLongDescription("Hits repeat damage on reload. Enough hits summons lightning. \n\n" +
				"A runic gauntlet crackling with arcane power. " +
				"Carefully forged together by a wandering archeologist, this hand is highly coveted and must be protected." +
				"" +
				"\n\n- Knife_to_a_gunfight");


			gun.SetupSprite(null, "Arcane_Cannon_idle_001", 5);
			gun.SetAnimationFPS(gun.shootAnimation, 15);
			gun.SetAnimationFPS(gun.reloadAnimation, 7);

			gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(27) as Gun, true, false);

			gun.gunHandedness = GunHandedness.OneHanded;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.angleVariance = 0f;
			gun.gunHandedness = GunHandedness.HiddenOneHanded;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.DefaultModule.cooldownTime = .4f;
			gun.DefaultModule.numberOfShotsInClip = 8;
			gun.reloadTime = 1f;
			gun.SetBaseMaxAmmo(250);
			gun.CurrentAmmo = 250;
			gun.barrelOffset.localPosition = new Vector3(21/16f,9/16f, 0f);
			gun.quality = PickupObject.ItemQuality.B;
			gun.muzzleFlashEffects = null;
			gun.gunClass = GunClass.SILLY;

			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);

			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			gun.DefaultModule.usesOptionalFinalProjectile = false;
			projectile.baseData.damage = 6f;
			projectile.baseData.speed *= 1;
			projectile.baseData.range = 30f;
			projectile.gameObject.GetOrAddComponent<projectileStates>().arcane = true;
			projectile.damageTypes = CoreDamageTypes.Electric;
			

			ETGMod.Databases.Items.Add(gun, null, "ANY");

			ID = gun.PickupObjectId;


		}

		public static int ID;

		private void OnhitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2.aiActor)
            {
				AiactorSpecialStates state = arg2.aiActor.gameObject.GetOrAddComponent<AiactorSpecialStates>();
				state.ArcaneStacks++;
				
            }
        }

       
		public override void PostProcessProjectile(Projectile projectile)
		{
            if (projectile.gameObject.GetOrAddComponent<projectileStates>())
            {
				if(projectile.gameObject.GetOrAddComponent<projectileStates>().arcane == true)
                {
					projectile.OnHitEnemy += this.OnhitEnemy;
					AkSoundEngine.PostEvent("Play_WPN_plasmarifle_shot_01", base.gameObject);
					AkSoundEngine.PostEvent("Play_WPN_spellactionrevolver_shot_01", base.gameObject);
					
				}
				
			}
			
		}

		public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{

				HasReloaded = false;
				
				base.OnReloadPressed(player, gun, bSOMETHING);
				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				AkSoundEngine.PostEvent("Play_ENM_statue_charge_01", base.gameObject);
				
				if (player.CurrentRoom != null)
				{
					//ETGModConsole.Log("room good");
					if(player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null)
                    {
						List<AIActor> currentEnemies = player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
						for(int i = currentEnemies.Count -1 ; i >= 0; i--)
						{
							AIActor actor = currentEnemies[i];
							
							if (actor != null)
							{
								//ETGModConsole.Log("enemies not null");
								actor.gameObject.GetOrAddComponent<AiactorSpecialStates>();
								if (actor.gameObject.GetOrAddComponent<AiactorSpecialStates>().ArcaneStacks > 0)
								{
									//ETGModConsole.Log("arcanestacks");
									player.StartCoroutine(DamageChain(actor, actor.gameObject.GetOrAddComponent<AiactorSpecialStates>().ArcaneStacks,gun));
									
								}
							}

						}

					}

				}

			}

		}

     
        private IEnumerator DamageChain(AIActor actor, int arcaneStacks, Gun gun)
        {
            if (actor)
            {
				
				for (int i = 0; i < arcaneStacks; i++)
                {
					GlobalSparksDoer.EmitFromRegion(GlobalSparksDoer.EmitRegionStyle.RADIAL, 5, this.gun.reloadTime, this.gun.sprite.WorldBottomLeft, this.gun.sprite.WorldTopRight, Vector2.up, 0, 2, null, null, null, GlobalSparksDoer.SparksType.SOLID_SPARKLES);
					if (actor.healthHaver.IsAlive)
                    {
						if(i == 3)
                        {
							int damage = 0;
							if (arcaneStacks >= 4 && arcaneStacks < 8) damage = 40;
							if (arcaneStacks >= 8 && arcaneStacks < 12) damage = 80;
							if (arcaneStacks >= 12) damage = 120;
							AkSoundEngine.PostEvent("Play_Instant_Lightning", base.gameObject);
							doLightning(actor, damage);
							GlobalSparksDoer.EmitFromRegion(GlobalSparksDoer.EmitRegionStyle.RANDOM, 25, .2f, actor.specRigidbody.UnitBottomLeft + new Vector2(-2, 0), actor.specRigidbody.UnitBottomRight + new Vector2(2,0), Vector2.up, 0, 10, null, null, null, GlobalSparksDoer.SparksType.SOLID_SPARKLES);
							i = arcaneStacks;
						}
                        else
                        {
							actor.healthHaver.ApplyDamage(3, Vector2.zero, "Arcane Magic"); // small electricity
							AkSoundEngine.PostEvent("Play_ENV_puddle_zap_01", base.gameObject);
							GlobalSparksDoer.EmitFromRegion(GlobalSparksDoer.EmitRegionStyle.RANDOM, 15, .2f, actor.specRigidbody.UnitBottomLeft, actor.specRigidbody.UnitBottomRight, Vector2.up, 0, 3, null, null, null, GlobalSparksDoer.SparksType.SOLID_SPARKLES);
						}
						
						yield return new WaitForSeconds(.2f);
						GlobalSparksDoer.EmitFromRegion(GlobalSparksDoer.EmitRegionStyle.RANDOM, 15, .2f, actor.specRigidbody.UnitBottomLeft, actor.specRigidbody.UnitBottomRight, Vector2.up, 0, 3, null, null, null, GlobalSparksDoer.SparksType.SOLID_SPARKLES);
					}

				}
				actor.gameObject.GetOrAddComponent<AiactorSpecialStates>().ArcaneStacks = 0;
			}
			
			yield return null;

		}

        private void doLightning(AIActor actor, int damage)
        {
			GameObject Lightning = SpawnManager.SpawnVFX(EasyVFXDatabase.Lightning, actor.CenterPosition, Quaternion.identity);
			tk2dBaseSprite lig = Lightning.GetComponent<tk2dBaseSprite>();
			//if (UnityEngine.Random.Range(1, 3) < 2) lig.FlipX = true;
			Lightning.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(actor.specRigidbody.UnitBottomCenter, tk2dBaseSprite.Anchor.LowerCenter);
			Lightning.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");
			lig.HeightOffGround = 1f;
			lig.UpdateZDepth();

			RenderSettings.ambientLight = ExtendedColours.AmbientLightNoir;

			DoSafeExplosion(actor.CenterPosition, damage);

		}

		public void DoSafeExplosion(Vector3 position, int damage)
		{

			ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
			this.smallPlayerSafeExplosion.effect = null;
			this.smallPlayerSafeExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
			this.smallPlayerSafeExplosion.ss = defaultSmallExplosionData2.ss;
			this.smallPlayerSafeExplosion.damage = damage;
			Exploder.Explode(position, this.smallPlayerSafeExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);

		}
		private ExplosionData smallPlayerSafeExplosion = new ExplosionData
		{
			damageRadius = 4f,
			damageToPlayer = 0f,
			doDamage = true,
			damage = 40f,
			doDestroyProjectiles = false,
			doForce = true,
			debrisForce = 0f,
			preventPlayerForce = true,
			explosionDelay = 0.0f,
			usesComprehensiveDelay = false,
			doScreenShake = false,
			playDefaultSFX = true,
			breakSecretWalls = false,
			forcePreventSecretWallDamage = true,
			secretWallsRadius = 3,
			force = 20,


		};

		public bool HasReloaded = false;


		public override void Update()
		{
			base.Update();
			if (gun.CurrentOwner != null)
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
	}
}