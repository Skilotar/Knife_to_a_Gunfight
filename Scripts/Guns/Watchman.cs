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
	class Watch_Standard : AdvancedGunBehaviour
	{

		public static void Add()
		{

			Gun gun = ETGMod.Databases.Items.NewGun("Watchman", "Watchman");
			Game.Items.Rename("outdated_gun_mods:watchman", "ski:watchman");
			gun.gameObject.AddComponent<Watch_Standard>();
			gun.SetShortDescription("Iron Breaker");
			gun.SetLongDescription("Reload at full clip to charge up losing one armor.\n\n" +
				"" +
				"The Watchman :tm: super rifle was used as a mainstay armament for patrol ships guarding hegemony controlled planets in the frontier wars. \n\n" +
				"It has the ability to re-route shield power into an amplifier for its rounds." +
				"" +
				"" +
				"\n\n- Knife_to_a_gunfight");


			gun.SetupSprite(null, "Watchman_idle_001", 1);
			gun.SetAnimationFPS(gun.criticalFireAnimation, 5);
			gun.SetAnimationFPS(gun.shootAnimation, 12);
			gun.SetAnimationFPS(gun.reloadAnimation, 4);

			gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(25) as Gun, true, false);


			gun.gunHandedness = GunHandedness.TwoHanded;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.angleVariance = 0f;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.DefaultModule.cooldownTime = 1.5f;
			gun.DefaultModule.numberOfShotsInClip = 4;
			gun.reloadTime = 2.5f;
			gun.SetBaseMaxAmmo(100);
			gun.CurrentAmmo = 100;

			gun.quality = PickupObject.ItemQuality.B;

			gun.gunClass = GunClass.RIFLE;

			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			gun.barrelOffset.transform.localPosition = new Vector3(2f, .7f, 0f);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.AppliesKnockbackToPlayer = true;
			projectile.PlayerKnockbackForce = 7f;
			projectile.baseData.damage = 35f;
			projectile.baseData.speed *= 1.5f;
			projectile.baseData.range = 40f;
			PierceProjModifier stabby = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
			stabby.penetration = 4;
			ETGMod.Databases.Items.Add(gun, null, "ANY");

			gun.shellsToLaunchOnFire = 1;
			Gun gun2 = PickupObjectDatabase.GetById(84) as Gun;
			gun.shellCasing = gun2.shellCasing;


			Watch_Standard.StandardID = gun.PickupObjectId;
		}

		public static int StandardID;
		public static bool isCharged = false;
		public override void PostProcessProjectile(Projectile projectile)
		{
			
            if (!Watch_Standard.isCharged) // not charged 
            {
				projectile.baseData.damage *= 1f;
				projectile.BossDamageMultiplier *= 1f;
				if(1 == UnityEngine.Random.Range(1, 2))
                {
					AkSoundEngine.PostEvent("Play_Watch_shot_001", base.gameObject);
				}
                else
                {
					AkSoundEngine.PostEvent("Play_Watch_shot_002", base.gameObject);
				}
			}
            else // charged
            {
				PlayerController player = (PlayerController)gun.CurrentOwner;
				if(player.PlayerHasActiveSynergy("Thermal Linkage") && player.IsOnFire)
				{
					player.CurrentFireMeterValue = .01f;
					StartCoroutine(HeatBuffer(player,1.2f));
                }
				projectile.damageTypes = CoreDamageTypes.Electric;
				projectile.baseData.damage *= 9f;
				projectile.BossDamageMultiplier *= 2f;
				
				projectile.OnHitEnemy += this.OnHitEnemy;
				timer = timer - 5;
				int sound = UnityEngine.Random.Range(1, 3);
				if (1 == sound)
				{
					AkSoundEngine.PostEvent("Play_WatchC_shot_001", base.gameObject);
				}
				if(2 == sound)
                {
					AkSoundEngine.PostEvent("Play_WatchC_shot_002", base.gameObject);
				}
				if(3 == sound)
                {
					AkSoundEngine.PostEvent("Play_WatchC_shot_003", base.gameObject);
				}
			}
		}

        private IEnumerator HeatBuffer(PlayerController player,float buffertimer)
        {
			float elap = 0;
            while(elap < buffertimer)
            {
				elap += Time.deltaTime;
				player.CurrentFireMeterValue = .01f;
				yield return null;
            }
			yield return null;
        }

        private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
		{
			if (arg2.aiActor != null)
            {
                if (arg2.aiActor.healthHaver.IsBoss)
                {
					arg2.aiActor.healthHaver.ApplyDamage(20, Vector2.zero, "Amped Watchman",CoreDamageTypes.Electric,DamageCategory.Unstoppable,true,null,true);

				}
            }
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool manualReload)
        {
			if (gun.ClipCapacity != gun.ClipShotsRemaining)
			{
				AkSoundEngine.PostEvent("Play_Watch_Reload_001", base.gameObject);
			}

			if (!Watch_Standard.isCharged)
			{
				if (gun.ClipCapacity == gun.ClipShotsRemaining && player.healthHaver.Armor > 0 && bruh == false)//full clip & armor
				{
					if (player.healthHaver.GetCurrentHealth() == 0)// Robot or armor bases person
					{
						if (player.healthHaver.Armor > 1)
						{
							gun.spriteAnimator.Play("Watchman_critical_fire");
							AkSoundEngine.PostEvent("Play_Watch_charge", base.gameObject);
							StartCoroutine(armorlost(player));
							bruh = true;
						}

					}
					else
					{
						gun.spriteAnimator.Play("Watchman_critical_fire");
						AkSoundEngine.PostEvent("Play_Watch_charge", base.gameObject);
						StartCoroutine(armorlost(player));
						bruh = true;
					}

				}
			}
			if (player.PlayerHasActiveSynergy("Thermal Linkage"))
			{
				player.CurrentFireMeterValue = 0;
				player.IsOnFire = false;
			
			}
				base.OnReloadPressed(player, gun, manualReload);
		}
		public bool bruh = false;
		private IEnumerator armorlost(PlayerController player)
		{
			bruh = true;
			player.IsGunLocked = true;
			player.inventory.GunLocked.SetOverride("ChargingWatchman", true);
			yield return new WaitForSeconds(3.2f);
			yield return new WaitForEndOfFrame();
			if(player.CurrentGun.PickupObjectId == this.gun.PickupObjectId)
            {
				//loose armor
				float armor = this.Owner.healthHaver.Armor;
				this.Owner.healthHaver.Armor *= 0;
				int adjarmor = (int)armor - 1;
				this.Owner.healthHaver.Armor = adjarmor;

				//change form
				Watch_Standard.isCharged = true;
				timer = 120;
				
			}
			player.inventory.GunLocked.RemoveOverride("ChargingWatchman");
			player.IsGunLocked = false;
			bruh = false;
		}

        public bool HasReloaded = false;
		public float timer = 120;
		public bool synergySpecialOverrideState = false;
		public bool KnownPriorState = false;
		public bool once = false;
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

                if (Watch_Standard.isCharged && !synergySpecialOverrideState) // count down but not while on fire
                {
					if(timer > 0)
                    {
						timer -= Time.deltaTime;
                    }
                    else
                    {
						Watch_Standard.isCharged = false;
                    }
                }

                if (((PlayerController)gun.CurrentOwner).IsOnFire && ((PlayerController)gun.CurrentOwner).PlayerHasActiveSynergy("Thermal Linkage"))
                {
					if(once == false)
                    {
						KnownPriorState = Watch_Standard.isCharged;
						Watch_Standard.isCharged = true;
						synergySpecialOverrideState = true;
						timer += 1;
						once = true;
					}

				}
                else
                {
					if (once == true)
					{
						Watch_Standard.isCharged = KnownPriorState;
						synergySpecialOverrideState = false;
						once = false;
					}
				}

			}


		}
	}
}
