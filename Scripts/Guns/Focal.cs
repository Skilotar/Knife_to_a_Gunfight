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
	class Focal : AdvancedGunBehaviour
	{

		public static void Add()
		{

			Gun gun = ETGMod.Databases.Items.NewGun("Focus", "Focal_point");
			Game.Items.Rename("outdated_gun_mods:focus", "ski:focus");
			gun.gameObject.AddComponent<Focal>();
			gun.SetShortDescription("Center Of Attention");
			gun.SetLongDescription("An inverted spread shotgun designed to place every pellet at a single point in space. It can be dialed in for precision, but if you miss there is always the security that a stray shot could land." +
				"\n\n- Knife_to_a_gunfight");
			gun.SetupSprite(null, "Focal_point_idle_001", 1);
			gun.SetAnimationFPS(gun.shootAnimation, 10);
			gun.SetAnimationFPS(gun.reloadAnimation, 1);


			for (int i = 0; i < 7; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(56) as Gun, true, false);


			}

			foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
			{
				projectileModule.ammoCost = 1;
				projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
				projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
				projectileModule.cooldownTime = 1;
				
				projectileModule.numberOfShotsInClip = 6;
				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
				projectile.gameObject.SetActive(false);
				projectileModule.projectiles[0] = projectile;
				projectile.baseData.damage = 7f;
				
				projectileModule.angleVariance = 56;
				projectile.baseData.speed *= .5f;
				projectile.AdditionalScaleMultiplier = .7f;
				FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);

				gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.SHOTGUN;
				gun.DefaultModule.projectiles[0] = projectile;
				bool flag = projectileModule != gun.DefaultModule;
				if (flag)
				{
					projectileModule.ammoCost = 0;
				}

			}

			gun.reloadTime = 1.5f;
			gun.SetBaseMaxAmmo(300);
			gun.CurrentAmmo = 300;
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(157) as Gun).muzzleFlashEffects;
			gun.quality = PickupObject.ItemQuality.C;
			gun.reloadShellLaunchFrame = 1;
			gun.shellsToLaunchOnReload = 6;
			gun.shellsToLaunchOnFire = 0;
			Gun gun2 = PickupObjectDatabase.GetById(51) as Gun;
			gun.shellCasing = gun2.shellCasing;
			gun.carryPixelOffset = new IntVector2(12,0);

			gun.gunClass = GunClass.SHOTGUN;

			Projectile projectile3 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			gun.barrelOffset.transform.localPosition = new Vector3(2.2f, 1f, 0f);
			projectile3.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile3.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile3);

			projectile3.baseData.damage = 4f;
			projectile3.baseData.speed = 10f;
			projectile3.baseData.range = 6f;
			projectile3.baseData.force = 5;
			projectile3.SuppressHitEffects = true;
			projectile3.hitEffects.suppressMidairDeathVfx = true;

			projectile3.SetProjectileSpriteRight("LoneStar", 16, 16, false, tk2dBaseSprite.Anchor.MiddleCenter, 16, 16);
			gun.muzzleFlashEffects = null;
			specialproj = projectile3;

			ETGMod.Databases.Items.Add(gun, null, "ANY");


		}

		public static Projectile specialproj;

		public System.Random rand = new System.Random();
		
		public override void OnPostFired(PlayerController player, Gun gun)
		{
			
			AkSoundEngine.PostEvent("Play_WPN_golddoublebarrelshotgun_shot_01", base.gameObject);
			StartCoroutine(processfocal(player));
			base.OnPostFired(player, gun);
		}
		public Vector3 pointInSpace;
		private IEnumerator processfocal(PlayerController player)
		{
			pointInSpace = this.gun.barrelOffset.transform.TransformPoint(5.5f, 0, 0);

			
			GameObject gameObject2 = SpawnManager.SpawnProjectile(specialproj.gameObject, pointInSpace, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
			Projectile component2 = gameObject2.GetComponent<Projectile>();
			component2.baseData.damage = 0f;
			component2.baseData.range = 20f;
			component2.baseData.speed = .001f;
			component2.Owner = player;


			yield return new WaitForSeconds(.25f);

			foreach (Projectile proj in GetFocusBullets())
            {
				proj.SendInDirection(pointInSpace - proj.LastPosition, true, true);
				proj.baseData.speed *= 1.2f;
				proj.UpdateSpeed();
				proj.gameObject.GetComponent<projectileStates>().isfocusshot = false;
            }
			yield return new WaitForSeconds(.25f);
			if(component2 != null)
            {
				component2.DieInAir();
			}
			
		}

		public List<Projectile> GetFocusBullets()
		{
			List<Projectile> list = new List<Projectile>();
			var allProjectiles = StaticReferenceManager.AllProjectiles;
			for (int i = 0; i < allProjectiles.Count; i++)
			{
				Projectile projectile = allProjectiles[i];
				if (projectile.Owner != null)
				{
					if (projectile.Owner == this.gun.CurrentOwner)
					{
						if (projectile.GetComponent<projectileStates>())
						{
							if (projectile.GetComponent<projectileStates>().isfocusshot == true)
							{
								list.Add(projectile);
							}
						}
					}
				}
			}
			return list;
		}

		public override void PostProcessProjectile(Projectile projectile)
		{
			projectile.gameObject.GetOrAddComponent<projectileStates>().isfocusshot = true;
		}


		public override void OnReload(PlayerController player, Gun gun)
		{
			
		}


		public override void  Update()
		{
			base.Update();
			if (this.Owner != null)
			{

				
			}

		}

        public override void OnDropped()
        {
			
            base.OnDropped();
        }


    }
}
