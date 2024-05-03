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
	class Watch_Charged : AdvancedGunBehaviour
	{

		public static void Add()
		{

			Gun gun = ETGMod.Databases.Items.NewGun("Watchman_charged", "Watchman_charged");
			Game.Items.Rename("outdated_gun_mods:watchman_charged", "ski:watchman_c");
			gun.gameObject.AddComponent<Watch_Charged>();
			gun.SetShortDescription("IronBreaker");
			gun.SetLongDescription("The Watchman :tm: super rifle was used as a mainstay armament for patrol ships guarding hegemony controlled planets in the frontier wars. \n\n" +
				"It has the ability to re-route shield power into an amplifier for its rounds. \n\n" +
				"Reload at full ammo to charge up losing one armor." +
				"" +
				"\n\n- Knife_to_a_gunfight");


			gun.SetupSprite(null, "Watchman_charged_idle_001", 1);

			gun.SetAnimationFPS(gun.shootAnimation, 12);
			gun.SetAnimationFPS(gun.reloadAnimation, 6);

			gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(370) as Gun, true, false);


			gun.gunHandedness = GunHandedness.TwoHanded;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.angleVariance = 0f;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.DefaultModule.cooldownTime = 2f;
			gun.DefaultModule.numberOfShotsInClip = 4;
			gun.reloadTime = 2.5f;
			gun.SetBaseMaxAmmo(100);
			gun.CurrentAmmo = 100;

			gun.quality = PickupObject.ItemQuality.EXCLUDED;

			gun.gunClass = GunClass.RIFLE;

			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.chargeProjectiles[1].Projectile);
			gun.barrelOffset.transform.localPosition = new Vector3(2f, .7f, 0f);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.AppliesKnockbackToPlayer = true;
			projectile.PlayerKnockbackForce = 15f;
			projectile.baseData.damage = 50f;
			projectile.baseData.speed *= 1.5f;
			projectile.baseData.range = 40f;
			gun.shellsToLaunchOnFire = 1;
			Gun gun2 = PickupObjectDatabase.GetById(84) as Gun;
			gun.shellCasing = gun2.shellCasing;
			ETGMod.Databases.Items.Add(gun, null, "ANY");

			Watch_Charged.Charged = gun.PickupObjectId;
		}

		public static int Charged;
		public override void PostProcessProjectile(Projectile projectile)
		{

		}

		public override void OnReloadPressed(PlayerController player, Gun gun, bool manualReload)
		{



			base.OnReloadPressed(player, gun, manualReload);
		}

		public bool HasReloaded = false;


		public override void  Update()
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