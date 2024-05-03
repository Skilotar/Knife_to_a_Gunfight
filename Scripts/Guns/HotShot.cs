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
	class HotShot : AdvancedGunBehaviour
	{
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Sizzler", "Sizzler");
			Game.Items.Rename("outdated_gun_mods:sizzler", "ski:sizzler");
			gun.gameObject.AddComponent<HotShot>();
			gun.SetShortDescription("Spittin' Fire!");
			gun.SetLongDescription("Shotgun with some extra punch!, Hold to charge and delay the burst or don't charge to max out close range damage!" +
				"\n\n- Knife_to_a_gunfight");

			gun.SetupSprite(null, "Sizzler_idle_001", 1);
			gun.SetAnimationFPS(gun.chargeAnimation, 6);
			gun.SetAnimationFPS(gun.shootAnimation, 16);
			gun.SetAnimationFPS(gun.reloadAnimation, 14);
			gun.SetAnimationFPS(gun.idleAnimation, 1);

			gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(384) as Gun, true, false);
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 3;

			Gun shellingFlash = (Gun)PickupObjectDatabase.GetById(384);
			gun.muzzleFlashEffects = shellingFlash.muzzleFlashEffects;
			gun.PreventNormalFireAudio = true;
			gun.reloadTime = 1.5f;
			gun.DefaultModule.cooldownTime = .7f;
			
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

			gun.gunClass = GunClass.SHOTGUN;
			gun.SetBaseMaxAmmo(150);
			gun.CurrentAmmo = 150;
			
			gun.quality = PickupObject.ItemQuality.B;


			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
			gun.barrelOffset.transform.localPosition = new Vector3(1.5f, .5f, 0f);
			
			

			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.baseData.damage = 6f;
			projectile.baseData.speed = 15f;
			projectile.baseData.range = 8f;
			projectile.baseData.force = 5;
			projectile.AdditionalScaleMultiplier = 1;
			ProjectileSplitController split = projectile.gameObject.GetOrAddComponent<ProjectileSplitController>();
			split.distanceBasedSplit = true;
			split.distanceTillSplit = .3f;
			split.numberofsplits = 2;
			split.dmgMultAfterSplit = .8f;
			split.amtToSplitTo = 2;
			split.sizeMultAfterSplit = projectile.AdditionalScaleMultiplier * 1.06f;
			split.splitAngles = 15f;
			split.splitOnEnemy = false;

			split.removeComponentAfterUse = false;


			Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(125) as Gun).DefaultModule.projectiles[0]);
			projectile2.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile2);
			projectile2.baseData.damage = 7f;
			projectile2.baseData.force *= 0.1f;
			projectile2.baseData.range = 20f;
			projectile2.AdditionalScaleMultiplier = 1.5f;
			projectile2.baseData.speed *= .8f;
			projectile2.AppliesFire = true;
			projectile2.FireApplyChance = 100;


			ProjectileSplitController split2 = projectile2.gameObject.GetOrAddComponent<ProjectileSplitController>();
			split2.distanceBasedSplit = true;
			split2.distanceTillSplit = 6f;
			split2.numberofsplits = 1;
			split2.dmgMultAfterSplit = .8f;
			split2.amtToSplitTo = 3;
			split2.sizeMultAfterSplit = projectile2.AdditionalScaleMultiplier * 1.06f;
			split2.splitAngles = 45f;
			split2.splitOnEnemy = false;

			split.removeComponentAfterUse = false;


			gun.DefaultModule.projectiles[0] = projectile2;


			ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
			{
				Projectile = projectile,
				ChargeTime = 0f,
				

			};
			ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
			{
				Projectile = projectile2,
				ChargeTime = .6f,


			};
			gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
			{
				item,
				item2,
				
			};



			ETGMod.Databases.Items.Add(gun, null, "ANY");


		}
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
			AkSoundEngine.PostEvent("Play_WPN_shotgun_shot_01", base.gameObject);
		}
		
		public override void OnFinishAttack(PlayerController player, Gun gun)
		{
			

			
			base.OnFinishAttack(player, gun);
		}

	
		bool HasReloaded;
		public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{

				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				base.OnReloadPressed(player, gun, bSOMETHING);
				
				AkSoundEngine.PostEvent("Play_WPN_elephantgun_reload_01", base.gameObject);
			}

		}



	}
}
