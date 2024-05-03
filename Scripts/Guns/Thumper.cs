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
	class Thumper : AdvancedGunBehaviour
	{

		public static void Add()
		{

			Gun gun = ETGMod.Databases.Items.NewGun("Thumper", "Thumper");
			Game.Items.Rename("outdated_gun_mods:thumper", "ski:thumper");
			gun.gameObject.AddComponent<Thumper>();
			gun.SetShortDescription("Short Fuse");
			gun.SetLongDescription("Press to fire, release to detonate. A decendant of the ancient flare gun used to detonate flares into space to flag down planetary" +
				" rescue teams. \n\n- Knife_to_a_gunfight");

			
			gun.SetupSprite(null, "Thumper_idle_001", 1);
			gun.SetAnimationFPS(gun.shootAnimation, 9);
			gun.SetAnimationFPS(gun.reloadAnimation, 7);

			gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(275) as Gun, true, false);
			

			gun.gunHandedness = GunHandedness.OneHanded;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.angleVariance = 0f;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			Gun flare = (Gun)PickupObjectDatabase.GetById(275);
			gun.gunSwitchGroup = flare.gunSwitchGroup;
			
			gun.DefaultModule.cooldownTime = .75f;
			
			gun.DefaultModule.numberOfShotsInClip = 1;
			
			gun.reloadTime = .75f;
			gun.SetBaseMaxAmmo(100);
			gun.CurrentAmmo = 100;
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(275) as Gun).muzzleFlashEffects;
			gun.quality = PickupObject.ItemQuality.B;
			
			gun.gunClass = GunClass.FIRE;

			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);

			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			
			projectile.baseData.damage = 5f;
			projectile.baseData.speed *= .7f;
			projectile.baseData.range = 20f;
			//ExplosiveModifier boom = projectile.gameObject.GetOrAddComponent<ExplosiveModifier>();
			//boom.explosionData = smallPlayerSafeExplosion;
			

			ETGMod.Databases.Items.Add(gun, null, "ANY");


		}

		public System.Random rand = new System.Random();
		public override void PostProcessProjectile(Projectile projectile)
		{
			pressDetProjModifier press = projectile.gameObject.GetOrAddComponent<pressDetProjModifier>();
			press.isThumperRocket = true;
			projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(OnHitEnemy));
			//AkSoundEngine.PostEvent("Play_WPN_flare_shot_01", base.gameObject);

		}

        private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
			if(arg2.aiActor != null)
            {
				pressDetProjModifier press = arg1.gameObject.GetOrAddComponent<pressDetProjModifier>();
				press.isThumperRocket = false;

				DoSafeExplosion(arg1.LastPosition);
				PlayerController player = this.gun.CurrentOwner as PlayerController;
				RoomHandler room = player.CurrentRoom;
				if (room != null)
				{
					foreach (var enemy in room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
					{
						if (enemy.aiActor != null)
						{
							if (Vector2.Distance(enemy.Position, arg1.LastPosition) < 5)
							{
								enemy.ApplyEffect(PickupObjectDatabase.GetById(295).GetComponent<BulletStatusEffectItem>().FireModifierEffect);
							}
						}

					}
				}

			}


		}

		public void DoSafeExplosion(Vector3 position)
		{

			ExplosionData defaultSmallExplosionData2 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData;
			this.smallPlayerSafeExplosion.effect = defaultSmallExplosionData2.effect;
			this.smallPlayerSafeExplosion.ignoreList = defaultSmallExplosionData2.ignoreList;
			this.smallPlayerSafeExplosion.ss = defaultSmallExplosionData2.ss;
			Exploder.Explode(position, this.smallPlayerSafeExplosion, Vector2.zero, null, false, CoreDamageTypes.None, false);

		}
		public override void OnReload(PlayerController player, Gun gun)
		{
			
			base.OnReload(player, gun);
			
		}


		public override void  Update()
		{
			base.Update();
			if (this.Owner != null)
			{

			}

		}

		private ExplosionData smallPlayerSafeExplosion = new ExplosionData
		{
			damageRadius = 5f,
			damageToPlayer = 0f,
			doDamage = true,
			damage = 10f,
			doDestroyProjectiles = true,
			doForce = true,
			debrisForce = 0f,
			preventPlayerForce = false,
			explosionDelay = 0.0f,
			usesComprehensiveDelay = false,
			doScreenShake = false,
			playDefaultSFX = true,
			breakSecretWalls = true,
			secretWallsRadius = 3,
			force = 20,
			forceUseThisRadius = true,


		};

	}
}
