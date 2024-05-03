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
	class Mozam : AdvancedGunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Mozambique", "Mozam");
			Game.Items.Rename("outdated_gun_mods:mozambique", "ski:mozambique");
			gun.gameObject.AddComponent<Mozam>();
			gun.SetShortDescription("Good Enough");
			gun.SetLongDescription("One to the head and two to the chest, a solid shotgun pistol. What it lacks in raw power it makes up for in adaptability. " +
				"Sporting a multitude of hop-ups and combos with other munitions. That's enough briefing cadet. Oscar Mike ladies!\n\n- Knife_to_a_gunfight");
			gun.SetupSprite(null, "Mozam_idle_001", 1);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 12);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 14);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 1);
			for (int i = 0; i < 3; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(157) as Gun, true, false);
				

			}
			foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
			{
				projectileModule.ammoCost = 1;
				projectileModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
				projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
				projectileModule.cooldownTime = .4f;
				projectileModule.angleVariance = 6f;
				projectileModule.numberOfShotsInClip = 6;
				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
				projectile.gameObject.SetActive(false);
				projectileModule.projectiles[0] = projectile;
				projectile.baseData.damage = 4.5f;
				projectile.AdditionalScaleMultiplier = .5f;
				FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);
				gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.BLUE_SHOTGUN;
				gun.DefaultModule.projectiles[0] = projectile;
				bool flag = projectileModule != gun.DefaultModule;
				if (flag)
				{
					projectileModule.ammoCost = 0;
				}

			}

			gun.DefaultModule.numberOfShotsInClip = 6;
			gun.reloadTime = 1.1f;
			gun.SetBaseMaxAmmo(400);
			
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(157) as Gun).muzzleFlashEffects;
			gun.quality = PickupObject.ItemQuality.D;
			gun.gunClass = GunClass.SHOTGUN;
			///gun.encounterTrackable.EncounterGuid = "Three buckshot on a single trigger pull.";
			ETGMod.Databases.Items.Add(gun, null, "ANY");
			ID = gun.PickupObjectId;


			Gun gun2 = (Gun)ETGMod.Databases.Items[15];
			Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun2.DefaultModule.projectiles[0].projectile);
			projectile2.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile2);
			projectile2.shouldRotate = true;
			projectile2.baseData.damage = 10f;
			projectile2.baseData.speed = 20f;
			projectile2.baseData.range = 25f;
			projectile2.angularVelocity = 1000;
			projectile2.hitEffects.suppressMidairDeathVfx = true;
			projectile2.pierceMinorBreakables = true;
			projectile2.SetProjectileSpriteRight("mozam", 19, 9, false, tk2dBaseSprite.Anchor.MiddleCenter, 19, 9);

			throwable = projectile2;
		}

		public static Projectile throwable;
		public static int ID;
		public System.Random rand = new System.Random();
        public override void OnPostFired(PlayerController player, Gun gun)
        {
			
			if(player.PlayerHasActiveSynergy("Hop-up: Hammer Point Rounds"))
			{
				int rng = UnityEngine.Random.Range(1, 3);
				switch (rng)
				{
					case 1:
						AkSoundEngine.PostEvent("Play_Mozam_hammer_001", base.gameObject);
						
						break;
					case 2:
						AkSoundEngine.PostEvent("Play_Mozam_hammer_002", base.gameObject);
						
						break;
					
				}

				hammer = true;

			}
            else
            {
				
				AkSoundEngine.PostEvent("Play_Mozam_fire_001", base.gameObject);
				
				hammer = false;

			}


			base.OnPostFired(player, gun);
        }

		bool hammer = false;

		public override void PostProcessProjectile(Projectile projectile)
		{
			PlayerController owner = this.gun.CurrentOwner as PlayerController;
			//hammerpoint
			
			if (hammer)
			{
				projectile.baseData.damage = 15 * owner.stats.GetStatModifier(PlayerStats.StatType.Damage);
				
			}
			else
			{
				projectile.baseData.damage = 4.5f * owner.stats.GetStatModifier(PlayerStats.StatType.Damage);
				
			}
		}

		
        public override void OnReload(PlayerController player, Gun gun)
        {
            base.OnReload(player, gun);
			gun.PreventNormalFireAudio = true;
			
			PlayerController owner = this.gun.CurrentOwner as PlayerController;
			bool throws = owner.PlayerHasActiveSynergy("Hop-up: Throw Away Joke");

			if (throws)
			{
				StartCoroutine(doThrow(player,gun));
			}
            
		}

        private IEnumerator doThrow(PlayerController player, Gun gun)
        {
			tk2dSpriteAnimationClip clip = gun.spriteAnimator.GetClipByName("Mozam_critical_fire");
			gun.spriteAnimator.Play(clip, 0, 10 / gun.reloadTime, true);

			AkSoundEngine.PostEvent("Play_ENM_bulletking_throw_01", base.gameObject);
			yield return new WaitForSeconds(.15f);
			GameObject gameObject = SpawnManager.SpawnProjectile(throwable.gameObject, gun.barrelOffset.position, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle));
			Projectile component = gameObject.GetComponent<Projectile>();
			component.Owner = player;
			component.angularVelocity = -400;


		}

		public bool fooltoggle = false;
		public bool once = false;

		public override void  Update()
		{
			base.Update();
			if (this.Owner != null)
            {
				PlayerController owner = this.gun.CurrentOwner as PlayerController;
				//April Fools
				bool fools = owner.PlayerHasActiveSynergy("Hop-up: April Fools");

				if (fools)
				{

					if(fooltoggle == false)
                    {
						gun.AddCurrentGunStatModifier(PlayerStats.StatType.AdditionalClipCapacityMultiplier, 1f, StatModifier.ModifyMethod.ADDITIVE);
						gun.AddCurrentGunStatModifier(PlayerStats.StatType.RateOfFire, .4f, StatModifier.ModifyMethod.ADDITIVE);
						owner.stats.RecalculateStats(owner);

						
						gun.SetBaseMaxAmmo(800);
						if (once == false)
						{
							gun.GainAmmo(800);
							once = true;
						}

						fooltoggle = true;
					}
					
				}
				else
				{
					if (fooltoggle == true)
					{
						gun.AddCurrentGunStatModifier(PlayerStats.StatType.AdditionalClipCapacityMultiplier, -1f, StatModifier.ModifyMethod.ADDITIVE);
						gun.AddCurrentGunStatModifier(PlayerStats.StatType.RateOfFire, -.4f, StatModifier.ModifyMethod.ADDITIVE);

						owner.stats.RecalculateStats(owner);

						gun.SetBaseMaxAmmo(400);
						
						fooltoggle = false;
					}
				}




			}

		}


		private void OnDestroy()
		{
			
		}
		
	}
}





