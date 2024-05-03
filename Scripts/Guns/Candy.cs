using System;
using Gungeon;
using ItemAPI;
using UnityEngine;

namespace Knives
{
	// Token: 0x02000214 RID: 532
	public class CandyCain : AdvancedGunBehavior
	{
		// Token: 0x06000A4C RID: 2636 RVA: 0x00085284 File Offset: 0x00083484
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("CandyCain", "CandyCain");
			Game.Items.Rename("outdated_gun_mods:candycain", "ski:candycain");
			CandyCain ranger = gun.gameObject.AddComponent<CandyCain>();
			GunExt.SetShortDescription(gun, "Pain Cane");
			GunExt.SetLongDescription(gun, "Alternates between buckshot and slugs.\n\n" +
				"A festively decorated model 1887 shotgun. There is nothing distinctly special about it other than a note that instructs its rounds to be loaded in alterating pattern. " +
			
				"\n\n\n - Knife_to_a_Gunfight");
			GunExt.SetupSprite(gun, null, "CandyCain_idle_001", 8);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 15);
			gun.barrelOffset.transform.localPosition = new Vector3(1.93f, .5f, 0f);
			for (int i = 0; i < 3; i++)
			{
				GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(86) as Gun, true, false);
			}
			//GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(694) as Gun, true, false);
			float cooldownTime = 0.6f;
			int numberOfShotsInClip = 6;


			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			projectile.baseData.damage *= 1.3f;
			projectile.baseData.speed *= 0.65f;
			projectile.baseData.range *= 1.5f;
			projectile.HasDefaultTint = true;
			projectile.DefaultTintColor = UnityEngine.Color.red;


			Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);
			projectile2.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile2);
			projectile2.baseData.damage *= 1.3f;
			projectile2.baseData.speed *= 0.65f;
			projectile2.baseData.range *= 1.5f;
			projectile2.HasDefaultTint = true;
			projectile2.DefaultTintColor = UnityEngine.Color.red;


			Projectile projectile3 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);
			projectile3.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile3.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile3);
			projectile3.baseData.damage *= 1.3f;
			projectile3.baseData.speed *= 0.65f;
			projectile3.baseData.range *= 1.5f;
			projectile3.HasDefaultTint = true;
			projectile3.DefaultTintColor = UnityEngine.Color.red;


			Projectile Slug = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(694) as Gun).DefaultModule.projectiles[0]);
			Slug.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(Slug.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(Slug);
			Slug.baseData.damage *= 1f;
			Slug.baseData.speed *= 1f;
			Slug.baseData.range *= 1.5f;
			Slug.HasDefaultTint = true;
			Slug.DefaultTintColor = UnityEngine.Color.white;



			Projectile Nothing = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(694) as Gun).DefaultModule.projectiles[0]);
			Nothing.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(Nothing.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(Nothing);
			Nothing.baseData.damage *= 0f;
			Nothing.baseData.speed *= 1000f;
			Nothing.baseData.range = 0f;
			Nothing.hitEffects.suppressMidairDeathVfx = true;
			ChildProjCleanup cleanup = Nothing.gameObject.GetOrAddComponent<ChildProjCleanup>();
			cleanup.delay = 0;
			cleanup.parentProjectile = null;
			cleanup.doColor = false;

			int num = 0;
			foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
			{
				projectileModule.numberOfShotsInClip = numberOfShotsInClip;
				projectileModule.cooldownTime = cooldownTime;
				projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Ordered;
				projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
				bool flag = num <= 0;
				if (flag)
				{
					projectileModule.ammoCost = 1;
					projectileModule.angleFromAim = 10f;
					projectileModule.angleVariance = .01f;
					projectileModule.projectiles[0] = projectile;
					projectileModule.projectiles.Add(Nothing);
					num++;
				}
				else
				{
					bool flag2 = num == 1;
					if (flag2)
					{
						projectileModule.ammoCost = 0;
						projectileModule.angleFromAim = 0f;
						projectileModule.angleVariance = .01f;
						projectileModule.projectiles.Add(Slug);
						projectileModule.projectiles[0] = projectile2;
						
						num++;
					}
					else
					{
						bool flag3 = num == 2;
						if (flag3)
						{
							projectileModule.ammoCost = 0;
							projectileModule.angleFromAim = -10f;
							projectileModule.angleVariance = .01f;
							projectileModule.projectiles[0] = projectile3;
							projectileModule.projectiles.Add(Nothing);
							num++;
						}
						
					}
				}
			}

			gun.reloadTime = 1.4f;
			gun.SetBaseMaxAmmo(150);
			gun.gunClass = GunClass.SHOTGUN;
			gun.quality = PickupObject.ItemQuality.B;
			gun.carryPixelOffset = new IntVector2(7, 0);
			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
			gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Candy_Shotty", "Knives/Resources/Candy_clipfull", "Knives/Resources/Candy_clipempty");
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(93) as Gun).gunSwitchGroup;
			ETGMod.Databases.Items.Add(gun, null, "ANY");
			CandyCain.ID = gun.PickupObjectId;
		}

		// Token: 0x040006D4 RID: 1748
		public static int ID;

        public override void OnReload(PlayerController player, Gun gun)
        {
			
            base.OnReload(player, gun);
        }


    }
}