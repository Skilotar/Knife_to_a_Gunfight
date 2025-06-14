using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.BreakableAPI;
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
	class HeartBreaker : AdvancedGunBehaviour
	{

		public static void Add()
		{

			Gun gun = ETGMod.Databases.Items.NewGun("Heartbreaker", "HeartBreaker");
			Game.Items.Rename("outdated_gun_mods:heartbreaker", "ski:heartbreaker");
			gun.gameObject.AddComponent<HeartBreaker>();
			gun.SetShortDescription("Loves Me Not");
			gun.SetLongDescription("chance to charm, Crits charmed enemies.\n\n" +
				"A lovely bouquet of roses shrouding a powerful coachgun.\n" +
				"A not so subtle way to give unwanted love intrests the hint." +
				"\n\n- Knife_to_a_gunfight");
			gun.SetupSprite(null, "HeartBreaker_idle_001", 1);
			gun.SetAnimationFPS(gun.shootAnimation, 10);
			gun.SetAnimationFPS(gun.reloadAnimation, 12);

			for (int i = 0; i < 12; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(157) as Gun, true, false);
			}
			int counter = 0;
			foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
			{
				if(counter <= 5)
                {
					projectileModule.ammoCost = 1;
					projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
					projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
					projectileModule.cooldownTime = .4f;
					projectileModule.numberOfShotsInClip = 2;
					Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
					projectile.gameObject.SetActive(false);
					projectile.baseData.damage = 3f;
					projectileModule.angleVariance = 15f;
					projectile.baseData.range = 10 * (1 + UnityEngine.Random.Range(-.2f, .2f));
					projectile.baseData.speed = 20 * (1 + UnityEngine.Random.Range(-.3f, .3f));
					projectile.AdditionalScaleMultiplier = .7f;
					FakePrefab.MarkAsFakePrefab(projectile.gameObject);
					UnityEngine.Object.DontDestroyOnLoad(projectile);
					projectileModule.projectiles[0] = projectile;
					gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.SHOTGUN;
					gun.DefaultModule.projectiles[0] = projectile;
					bool flag = projectileModule != gun.DefaultModule;
					if (flag)
					{
						projectileModule.ammoCost = 0;
					}
					counter++;
                }
                else 
				{
					projectileModule.ammoCost = 0;
					projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
					projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
					projectileModule.cooldownTime = .4f;
					projectileModule.numberOfShotsInClip = 2;
					Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
					projectile.gameObject.SetActive(false);
					projectile.baseData.damage = 2f;
					projectileModule.angleVariance = 17f;
					projectile.baseData.range = 10 * (1 + UnityEngine.Random.Range(-.2f, .2f));
					projectile.baseData.speed = 20 * (1 + UnityEngine.Random.Range(-.3f, .3f));

					projectile.AppliesCharm = true;
					projectile.CharmApplyChance = .25f;
					projectile.charmEffect = StaticStatusEffects.charmingRoundsEffect;

					SlowDownOverTimeModifier slowDown = projectile.gameObject.AddComponent<SlowDownOverTimeModifier>();
					slowDown.extendTimeByRangeStat = true;
					slowDown.activateDriftAfterstop = true;
					slowDown.doRandomTimeMultiplier = true;
					slowDown.killAfterCompleteStop = true;
					slowDown.timeTillKillAfterCompleteStop = 1f;
					slowDown.timeToSlowOver = 0.5f;
					slowDown.targetSpeed = projectile.baseData.speed * 0.1f;

					DriftModifier drift = projectile.gameObject.AddComponent<DriftModifier>();
					MiscToolMethods.EasyAnimate(projectile, new List<string>() {
					"petal_001",
					"petal_002",
					"petal_003",
					"petal_004",
					"petal_005",
					"petal_006",
					"petal_007",
					"petal_006",
					"petal_005",
					"petal_004",
					"petal_003",
					"petal_002",

					}, 12, new IntVector2(9, 7), 6, false, tk2dSpriteAnimationClip.WrapMode.Loop);

					GameObject petalDebris = BreakableAPIToolbox.GenerateDebrisObject($"Knives/Resources/Heartbreaker/petal_004.png", true, 1, 1, 0, 0).gameObject;
					projectile.hitEffects = new ProjectileImpactVFXPool()
					{
						suppressHitEffectsIfOffscreen = false,
						suppressMidairDeathVfx = false,
						overrideMidairZHeight = -1,
						overrideEarlyDeathVfx = null,
						overrideMidairDeathVFX = petalDebris,
						midairInheritsVelocity = true,
						midairInheritsFlip = true,
						midairInheritsRotation = true,
						alwaysUseMidair = true,
						CenterDeathVFXOnProjectile = false,
						HasProjectileDeathVFX = false
					};


					FakePrefab.MarkAsFakePrefab(projectile.gameObject);
					UnityEngine.Object.DontDestroyOnLoad(projectile);
					projectileModule.projectiles[0] = projectile;
					gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.SHOTGUN;
					gun.DefaultModule.projectiles[0] = projectile;
					counter++;
				}

			}

			gun.reloadTime = 1f;
			gun.SetBaseMaxAmmo(200);
			gun.CurrentAmmo = 200;
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(379) as Gun).muzzleFlashEffects;
			gun.quality = PickupObject.ItemQuality.B;
			gun.shellsToLaunchOnReload = 2;

			gun.shellsToLaunchOnFire = 0;
			Gun gun2 = PickupObjectDatabase.GetById(51) as Gun;
			gun.shellCasing = gun2.shellCasing;

			gun.gunClass = GunClass.SHOTGUN;
			gun.carryPixelOffset = new IntVector2(5, 0);
			gun.barrelOffset.transform.localPosition = new Vector3(26/16f, 8/16f, 0f);
			ETGMod.Databases.Items.Add(gun, null, "ANY");

			ID = gun.PickupObjectId;
		}

		public static int ID;
		public override void OnPostFired(PlayerController player, Gun gun)
		{
			AkSoundEngine.PostEvent("Play_WPN_golddoublebarrelshotgun_shot_01", base.gameObject);
			AkSoundEngine.PostEvent("Play_WPN_wandbundle_shot_01", base.gameObject);

			base.OnPostFired(player, gun);
		}

		public static bool LovesMeNot = true;

        public override void PostProcessProjectile(Projectile projectile)
        {
			if(projectile.GetCachedBaseDamage == 3)
            {
				projectile.OnHitEnemy += this.crit;
            }
            base.PostProcessProjectile(projectile);
        }

        private void crit(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if(arg2.aiActor != null)
            {
                if (CheckIfCharmed(arg2) == true)
                {
					if ((this.gun.CurrentOwner as PlayerController).PlayerHasActiveSynergy("Toxic Romance"))
					{
						arg2.aiActor.ApplyEffect(StaticStatusEffects.irradiatedLeadEffect);
					}
					arg2.healthHaver.ApplyDamage(15, Vector2.zero, "HeartBroken");
					arg2.aiActor.RemoveEffect(StaticStatusEffects.charmingRoundsEffect);
					AkSoundEngine.PostEvent("Play_Neon_critical", base.gameObject);
				}
            }
        }

		private bool CheckIfCharmed(SpeculativeRigidbody arg2)
		{
			AIActor boi = arg2.aiActor;

			GameActorEffect effect = boi.GetEffect(StaticStatusEffects.charmingRoundsEffect.effectIdentifier); // uses effect Identifier    // Hey Future me. DONT use the resistnaceType on the get effect method. it don't work fam.
			if (effect != null)
			{
				//ETGModConsole.Log(effect.effectIdentifier.ToString());
				return true;
			}
			else
			{
				//ETGModConsole.Log("Null effect");
				return false;
			}



		}

		public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{

			
			AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
			
		}
		
		public bool HasReloaded { get; private set; }

		public override void Update()
		{
			base.Update();
			if (this.Owner != null)
			{


				if (!gun.IsReloading && !HasReloaded)
				{
					this.HasReloaded = true;
					
				}

			}

		}


	}
}


