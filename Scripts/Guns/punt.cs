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
	class punt : AdvancedGunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Half Gauge Shotgun", "half_gauge");
			Game.Items.Rename("outdated_gun_mods:half_gauge_shotgun", "ski:half_gauge_shotgun");
			gun.gameObject.AddComponent<punt>();
			gun.SetShortDescription("Screw That General Direction!");
			gun.SetLongDescription("Invented by McGee Larvey Oddballed this gun was designed to aid the vision impared in their duck hunting adventures. " +
				"While the idea was good at heart McGee failed to comprehend the full weight of his actions until his sight damaged brother shot out the entire side of his barn. " +
				"But at least he hit something... " +
				"\n\n\n- Knife_to_a_gunfight");
			
			
			gun.SetAnimationFPS(gun.shootAnimation, 30);
			gun.SetAnimationFPS(gun.reloadAnimation, 13);
			gun.SetAnimationFPS(gun.idleAnimation, 1);
			gun.SetupSprite(null, "half_gauge_idle_001", 1);

			
			for (int i = 0; i < 75; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
				
			}
			foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
			{
				projectileModule.ammoCost = 1;
				projectileModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
				projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
				projectileModule.cooldownTime = .1f;
				
				projectileModule.angleVariance = 55;
				projectileModule.numberOfShotsInClip = 1;
				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
				projectile.gameObject.SetActive(false);
				projectileModule.projectiles[0] = projectile;
				
				projectile.baseData.range = 20 * (1 + UnityEngine.Random.Range(-.2f, .2f));
				projectile.baseData.speed = 20 * (1 + UnityEngine.Random.Range(-.3f, .3f));
				projectile.baseData.damage = 6f;
				projectile.AdditionalScaleMultiplier = .3f;
				FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);
				projectile.AppliesKnockbackToPlayer = true;
				projectile.PlayerKnockbackForce = 2f;
				gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.BLUE_SHOTGUN;
				gun.DefaultModule.projectiles[0] = projectile;
				bool flag = projectileModule != gun.DefaultModule;
				if (flag)
				{
					projectileModule.ammoCost = 0;
				}
				gun.PreventNormalFireAudio = true;
			}

			gun.DefaultModule.angleVariance = 40;
			gun.PreventNormalFireAudio = true;
			gun.reloadTime = 1.3f;
			gun.DefaultModule.cooldownTime = .20f;
			gun.gunClass = GunClass.SHOTGUN;
			gun.SetBaseMaxAmmo(40);
			gun.CurrentAmmo = 40;
			gun.barrelOffset.transform.localPosition = new Vector3(3f, .5f, 0f);
			gun.muzzleFlashEffects = null;
			gun.quality = PickupObject.ItemQuality.C;
			gun.encounterTrackable.EncounterGuid = "I can hit the broad side of the barn";
			ETGMod.Databases.Items.Add(gun, null, "ANY");

			ID = gun.PickupObjectId;
		}
		public static int ID;
		public override void  OnPickedUpByPlayer(PlayerController player)
        {
			this.gun.HasBeenPickedUp = true;
			
            base.OnPickedUpByPlayer(player);
        }
		public override void  Update()
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

		public override void PostProcessProjectile(Projectile projectile)
		{


		}

		public override void OnFinishAttack(PlayerController player, Gun gun)
		{
			AkSoundEngine.PostEvent("Play_half_gauge_fire", base.gameObject);

			base.OnFinishAttack(player, gun);
		}

		bool HasReloaded;
		public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{

				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				base.OnReloadPressed(player, gun, bSOMETHING);

			}

		}



	}
}