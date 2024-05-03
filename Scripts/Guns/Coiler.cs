using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.ItemAPI;
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
	class Coiler : AdvancedGunBehavior
	{

		public static void Add()
		{

			Gun gun = ETGMod.Databases.Items.NewGun("Coiler", "Coiler");
			Game.Items.Rename("outdated_gun_mods:coiler", "ski:coiler");
			gun.gameObject.AddComponent<Coiler>();
			gun.SetShortDescription("OverCharged");
			gun.SetLongDescription("Enemies hit with this coiling shot will detonate an electric pulse after 2 seconds.\n\n" +
				"An attempt by the Sp4rk company to create a recharging station for their burst batteries." +
				"" +
				"\n\n- Knife_to_a_gunfight");


			gun.SetupSprite(null, "Coiler_idle_001", 1);
			gun.SetAnimationFPS(gun.shootAnimation, 4);
			gun.SetAnimationFPS(gun.reloadAnimation, 7);

			gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);


			gun.gunHandedness = GunHandedness.OneHanded;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.angleVariance = 0f;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.DefaultModule.cooldownTime = 1f;
			gun.DefaultModule.numberOfShotsInClip = 2;
			gun.reloadTime = 1.3f;
			gun.SetBaseMaxAmmo(200);
			gun.CurrentAmmo = 200;

			gun.quality = PickupObject.ItemQuality.B;

			gun.gunClass = GunClass.PISTOL;

			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);

			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;

			projectile.baseData.damage = 5f;
			projectile.baseData.speed *= 1;
			projectile.baseData.range = 20f;
			projectile.AdditionalScaleMultiplier *= .5f;
			projectile.damageTypes = CoreDamageTypes.Electric;
			projectile.AdditionalScaleMultiplier *= .5f;
			LightningProjectileComp zappy = projectile.gameObject.GetOrAddComponent<LightningProjectileComp>();

			List<string> BeamAnimPaths = new List<string>()
			{

				"Knives/Resources/BeamSprites/Sp4rk_001",
				"Knives/Resources/BeamSprites/Sp4rk_002",
				"Knives/Resources/BeamSprites/Sp4rk_003",
			};
			projectile.AddTrailToProjectile(
				 "Knives/Resources/BeamSprites/Sp4rk_001",
				new Vector2(3, 2),
				new Vector2(1, 1),
				BeamAnimPaths, 20,
				BeamAnimPaths, 20,
				-1,
				0.0001f,
				5,
				true
				);
			PierceProjModifier stabby = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
			stabby.penetration = 0;

			ETGMod.Databases.Items.Add(gun, null, "ANY");


		}

		public System.Random rng = new System.Random();
		public override void PostProcessProjectile(Projectile projectile)
		{
			AkSoundEngine.PostEvent("Play_ENM_rubber_shock_01", base.gameObject);
			projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(OnHitEnemy));
		}

		private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
		{
			if(arg2.aiActor != null)
            {
				StartCoroutine(BurstDelay(arg2.aiActor));
			}
			
		}

        private IEnumerator BurstDelay(AIActor aiactor)
        {
			AkSoundEngine.PostEvent("Play_BOSS_dragun_charge_01", base.gameObject);

			aiactor.SetOutlines(true); 
			aiactor.SetOverrideOutlineColor(Color.yellow);
			yield return new WaitForSeconds(2);
			if(aiactor != null)
            {

				PlayerController player = (PlayerController)this.gun.CurrentOwner;
				for (int i = 0; i < 6; i++)
				{
					Projectile projectile2 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
					GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, aiactor.specRigidbody.UnitCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + (i * 10)), true);
					Projectile component = gameObject.GetComponent<Projectile>();
					bool flag = component != null;
					if (flag)
					{

						component.gameObject.GetOrAddComponent<LightningProjectileComp>();
						PierceProjModifier stabby = component.gameObject.GetOrAddComponent<PierceProjModifier>();
						stabby.penetration = 1;
						component.Owner = player;
						component.Shooter = player.specRigidbody;
						component.baseData.damage = 9f;
						component.DefaultTintColor = UnityEngine.Color.yellow;
						component.HasDefaultTint = true;

					}
				}

				
				AkSoundEngine.PostEvent("Play_BOSS_DragunGold_Thunder_01", base.gameObject);
				



				
			}
			
		}

        public override void OnReload(PlayerController player, Gun gun)
        {
			
			AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
			//AkSoundEngine.PostEvent("Play_neon_reload", base.gameObject);
			base.OnReload(player, gun);
        }
        
		public bool HasReloaded = false;


		protected override void Update()
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