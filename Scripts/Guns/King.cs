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
	class King : AdvancedGunBehaviour
	{

		public static void Add()
		{

			Gun gun = ETGMod.Databases.Items.NewGun("Kingly Shotgun", "Kingly_Shotgun");
			Game.Items.Rename("outdated_gun_mods:kingly_shotgun", "ski:kingly_shotgun");
			gun.gameObject.AddComponent<King>();
			gun.SetShortDescription("Checkmate");
			gun.SetLongDescription("Fires octagonally and gains increased damage while standing still.\n\n" +
				"A shotgun that Leon Chessovski used to win the international chess boxing championship... \n" +
				"For about 20 minutes until it was ruled that shotguns are not a legal strategy for boxing.\n\n" +
				"" +
				"\n\n- Knife_to_a_gunfight");
			gun.SetupSprite(null, "Kingly_Shotgun_idle_001", 1);
			gun.SetAnimationFPS(gun.shootAnimation, 8);
			gun.SetAnimationFPS(gun.reloadAnimation, 5);


			for (int i = 0; i < 6; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(124) as Gun, true, false);
				
			}

			foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
			{
				projectileModule.ammoCost = 1;
				projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
				projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
				projectileModule.cooldownTime = .6f;

				projectileModule.numberOfShotsInClip = 2;
				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
				projectile.gameObject.SetActive(false);
				projectileModule.projectiles[0] = projectile;
				projectile.baseData.damage = 8f;
				projectile.baseData.range = 10 * (1 + UnityEngine.Random.Range(-.2f, .2f));
				projectile.baseData.speed = 20 * (1 + UnityEngine.Random.Range(-.3f, .3f));
				projectileModule.angleVariance = 24f;
				projectile.sprite.color = UnityEngine.Color.red;
				projectile.AdditionalScaleMultiplier = 1.5f;
				FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);
				ImprovedAfterImage trail = projectile.gameObject.GetOrAddComponent<ImprovedAfterImage>();
				trail.shadowLifetime = .1f;
				trail.shadowTimeDelay = .0001f;
				trail.dashColor = new Color(.67f, .08f, .01f);
				trail.spawnShadows = true;
				gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.SHOTGUN;
				gun.DefaultModule.projectiles[0] = projectile;
				bool flag = projectileModule != gun.DefaultModule;
				if (flag)
				{
					projectileModule.ammoCost = 0;
				}

			}
			
			gun.barrelOffset.transform.localPosition = new Vector3(2.3f, .5f, 0f);
			gun.reloadTime = 1f;
			gun.SetBaseMaxAmmo(300);
			gun.CurrentAmmo = 300;
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(157) as Gun).muzzleFlashEffects;
			gun.quality = PickupObject.ItemQuality.B;
			gun.shellsToLaunchOnReload = 1;
			gun.CustomBossDamageModifier = 2f;
			gun.shellsToLaunchOnFire = 1;
			Gun gun2 = PickupObjectDatabase.GetById(51) as Gun;
			gun.shellCasing = gun2.shellCasing;
			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(54) as Gun).gunSwitchGroup;
			gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
			gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("Pixel_Shotty", "Knives/Resources/king_clipfull", "Knives/Resources/king_clipempty");
			//PierceProjModifier stab = gun.projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
			//stab.penetratesBreakables = true;
			gun.gunClass = GunClass.SHOTGUN;
			
			ETGMod.Databases.Items.Add(gun, null, "ANY");

			ID = gun.PickupObjectId;
			thisone = gun;
		}
		public static Gun thisone;
		public static int ID;

		public System.Random rand = new System.Random();

		public override void OnPostFired(PlayerController player, Gun gun)
		{
			AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
			AkSoundEngine.PostEvent("Play_King_shot_001", base.gameObject);
			AkSoundEngine.PostEvent("Play_King_shot_001", base.gameObject);
			AkSoundEngine.PostEvent("Play_King_shot_001", base.gameObject);
			AkSoundEngine.PostEvent("Play_King_shot_001", base.gameObject);
			StartCoroutine(postShotFlashy());

			base.OnPostFired(player, gun);
		}

		bool PostShotFlash = false;


        public override void OnDropped()
        {
			
			if(m_laser1!=null)UnityEngine.GameObject.Destroy(m_laser1);
			if(m_laser2!=null)UnityEngine.GameObject.Destroy(m_laser2);
			DoingStandTracking = false;


			base.OnDropped();
        }



        public override void OnGunsChanged(Gun previous, Gun current, bool newGun)
        {
			if(this.gun == previous || this.gun == current)
            {
				if (m_laser1 != null) UnityEngine.GameObject.Destroy(m_laser1);
				if (m_laser2 != null) UnityEngine.GameObject.Destroy(m_laser2);
				DoingStandTracking = false;

			}

			base.OnGunsChanged(previous, current, newGun);
        }
        public override void PostProcessProjectile(Projectile projectile)
		{
			if (this.Owner.Velocity.magnitude <= .1f)
			{

				if((this.Owner as PlayerController).PlayerHasActiveSynergy("King and Queen"))
                {
					projectile.baseData.damage *= 4f;
				}
                else
                {
					projectile.baseData.damage *= 2f;
				}

			}
		}


		public override void OnReload(PlayerController player, Gun gun)
		{
			AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
			AkSoundEngine.PostEvent("Play_King_reload_001", base.gameObject);
			AkSoundEngine.PostEvent("Play_King_reload_001", base.gameObject);
			doingrolload = false;
			base.OnReload(player, gun);
			
		}

		public bool postshot = false;
        private IEnumerator postShotFlashy()
        {

			postshot = true;
			yield return new WaitForSeconds(.2f);
			postshot = false;
           
        }

        public bool DoingStandTracking = false;
		public override void Update()
		{
			base.Update();
			if (this.Owner != null)
			{
				
				if((this.Owner as PlayerController).PlayerHasActiveSynergy("King and Queen"))
                {
					gun.OverrideAngleSnap = 1f;
				}
                else
                {
					gun.OverrideAngleSnap = 45f;
				}

				if ((this.Owner as PlayerController).PlayerHasActiveSynergy("Bisharpshooter"))
				{
					if (this.Owner.Velocity.magnitude <= .1f && DoingStandTracking == false)
					{

						StartCoroutine(DoingStandTrack());
					}
                    
				}
				if ((this.Owner as PlayerController).PlayerHasActiveSynergy("Rolload"))
				{
					if ((this.Owner as PlayerController).IsDodgeRolling && doingrolload == false)
					{

						StartCoroutine(Rolload());
					}

				}
				if (m_laser2 == null && m_laser1 == null && this.gun.IsReloading == false && (this.Owner as PlayerController).IsDodgeRolling == false && this.gun.ClipShotsRemaining != 0 && postshot != true)
                {
					

					StartCoroutine(doFunkyLaserSightGarbage(this.Owner as PlayerController));

				}

				

			}
            else
            {
				
			}

		}

        private IEnumerator Rolload()
        {
			doingrolload = true;

			gun.MoveBulletsIntoClip(2);
			AkSoundEngine.PostEvent("Play_King_reload_001", base.gameObject);
			AkSoundEngine.PostEvent("Play_King_reload_001", base.gameObject);
			yield return new WaitForSeconds(1f);

			doingrolload = false;
        }

        public bool doingrolload = false;
		public bool doingStat = false;
        private IEnumerator DoingStandTrack()
        {
			DoingStandTracking = true;
			PlayerController EmergencyPlayerGone = this.Owner as PlayerController;
			EmergencyPlayerGone.stats.RecalculateStats(EmergencyPlayerGone, true);
			while (this.Owner.Velocity.magnitude <= .1f && this.Owner != null && GameManager.Instance.IsLoadingLevel == false)
            {

				if (doingStat == false)
                {
					gun.AddCurrentGunStatModifier(PlayerStats.StatType.Accuracy, -.8f, StatModifier.ModifyMethod.ADDITIVE);
					gun.AddCurrentGunStatModifier(PlayerStats.StatType.RangeMultiplier, 2f, StatModifier.ModifyMethod.ADDITIVE);
					(gun.CurrentOwner as PlayerController).stats.RecalculateStats((gun.CurrentOwner as PlayerController), true);
					doingStat = true;
					yield return null;
					
				}

				yield return null;
			}
			gun.AddCurrentGunStatModifier(PlayerStats.StatType.Accuracy, .8f, StatModifier.ModifyMethod.ADDITIVE);
			gun.AddCurrentGunStatModifier(PlayerStats.StatType.RangeMultiplier, -2f, StatModifier.ModifyMethod.ADDITIVE);
			EmergencyPlayerGone.stats.RecalculateStats(EmergencyPlayerGone, true);
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			DoingStandTracking = false;
			doingStat = false;
			yield return null;

        }

        public GameObject m_laser1;
		public GameObject m_laser2;
		private IEnumerator doFunkyLaserSightGarbage(PlayerController player) // this is trash... 
        {


			Color color = UnityEngine.Color.red;
			
	
			UnityEngine.GameObject laser1 = RenderLaserSight(gun.barrelOffset.transform.position, 100, 1, player.CurrentGun.CurrentAngle + (player.CurrentGun.DefaultModule.angleVariance * player.stats.GetStatModifier(PlayerStats.StatType.Accuracy)), true, color);
			UnityEngine.GameObject laser2 = RenderLaserSight(gun.barrelOffset.transform.position, 100, 1, player.CurrentGun.CurrentAngle - (player.CurrentGun.DefaultModule.angleVariance * player.stats.GetStatModifier(PlayerStats.StatType.Accuracy)), true, color);
			m_laser1 = laser1;
			m_laser2 = laser2;
			
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			UnityEngine.GameObject.Destroy(laser1);
			UnityEngine.GameObject.Destroy(laser2);
			m_laser1 = null;
			m_laser2 = null;
		}

		public static GameObject RenderLaserSight(Vector2 position, float length, float width, float angle, bool alterColour = false, Color? colour = null)
		{
			GameObject laserSightPrefab = LoadHelper.LoadAssetFromAnywhere("assets/resourcesbundle/global vfx/vfx_lasersight.prefab") as GameObject;

			GameObject gameObject = SpawnManager.SpawnVFX(laserSightPrefab, position, Quaternion.Euler(0, 0, angle));

			tk2dTiledSprite component2 = gameObject.GetComponent<tk2dTiledSprite>();
			float newWidth = 1f;
			if (width != -1) newWidth = width;
			component2.dimensions = new Vector2(length, newWidth);
			if (alterColour && colour != null)
			{
				component2.usesOverrideMaterial = true;
				component2.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
				component2.sprite.renderer.material.SetColor("_OverrideColor", (Color)colour);
				component2.sprite.renderer.material.SetColor("_EmissiveColor", (Color)colour);
				component2.sprite.renderer.material.SetFloat("_EmissivePower", 100);
				component2.sprite.renderer.material.SetFloat("_EmissiveColorPower", 1.55f);
			}
			return gameObject;
		}

	
	}
}