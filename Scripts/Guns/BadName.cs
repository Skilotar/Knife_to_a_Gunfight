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
	class Bad_Name : AdvancedGunBehaviour
	{

		public static void Add()
		{

			Gun gun = ETGMod.Databases.Items.NewGun("Bad Name", "Bad_Name");
			Game.Items.Rename("outdated_gun_mods:bad_name", "ski:bad_name");
			gun.gameObject.AddComponent<Bad_Name>();
			gun.SetShortDescription("Shot Through The Heart");
			gun.SetLongDescription("Chance to charm, perminant 2 damage buff after killing a charmed enemy.\n\n" +
				"" +
				"This gun revels in the pain it brings, it enjoys most when it can break both the body and the heart. " +
				"" +
				"\n\n- Knife_to_a_gunfight");


			gun.SetupSprite(null, "Bad_Name_idle_001", 1);
			gun.SetAnimationFPS(gun.shootAnimation, 12);
			gun.SetAnimationFPS(gun.reloadAnimation, 10);

			gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(25) as Gun, true, false);


			gun.gunHandedness = GunHandedness.TwoHanded;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.angleVariance = 0f;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.DefaultModule.cooldownTime = .6f;
			gun.DefaultModule.numberOfShotsInClip = 7;
			gun.reloadTime = 2f;
			gun.SetBaseMaxAmmo(300);
			gun.CurrentAmmo = 300;
			gun.carryPixelOffset = new IntVector2(6, 0);
			gun.quality = PickupObject.ItemQuality.B;

			gun.gunClass = GunClass.CHARM;

			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);

			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;

			gun.shellsToLaunchOnFire = 1;
			Gun gun2 = PickupObjectDatabase.GetById(84) as Gun;
			gun.shellCasing = gun2.shellCasing;

			projectile.baseData.damage = 14f;
			projectile.baseData.speed *= 1.5f;
			projectile.baseData.range = 20f;
			projectile.AppliesCharm = true;
			projectile.CharmApplyChance = .15f;
			projectile.charmEffect = StaticStatusEffects.charmingRoundsEffect;

			gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("HeartBreak", "Knives/Resources/Jovi_clipfull", "Knives/Resources/Jovi_clipempty");
			ETGMod.Databases.Items.Add(gun, null, "ANY");

			ID = gun.PickupObjectId;
		}

		public static int ID;
		public override void PostProcessProjectile(Projectile projectile)
		{
			AkSoundEngine.PostEvent("Play_WPN_m1911_shot_01", base.gameObject);
			projectile.baseData.damage *= 1 + (.1f * boost_number);
			projectile.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(OnHitEnemy));
		}
		public int boost_number = 0;
		private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
		{
			if(arg2.aiActor != null)
            {
                if (arg3)
                {
                    if (arg2.aiActor.CanTargetEnemies && !arg2.aiActor.CanTargetPlayers)
                    {
						if(arg2.aiActor.healthHaver && !arg2.aiActor.healthHaver.IsBoss && arg2.aiActor.GetComponent<CompanionController>() == null)
                        {
							boost_number++;
							boost_number++;
							AkSoundEngine.SetRTPCValue("PIT_SFX", boost_number);
							AkSoundEngine.PostEvent("Play_Stat_Up", base.gameObject);
                        }
                    }
                }
            }
		}

		public override void OnReload(PlayerController player, Gun gun)
		{

			AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
			AkSoundEngine.PostEvent("Play_WPN_yarirocketlauncher_reload_01", base.gameObject);

			base.OnReload(player, gun);
		}

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
