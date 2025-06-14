using System;
using System.Collections;
using Gungeon;
using ItemAPI;
using UnityEngine;

namespace Knives
{
	// Token: 0x02000214 RID: 532
	public class Regundant : AdvancedGunBehavior
	{
		// Token: 0x06000A4C RID: 2636 RVA: 0x00085284 File Offset: 0x00083484
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("Regundant", "regundant");
			Game.Items.Rename("outdated_gun_mods:regundant", "ski:regundant");
			Regundant ranger = gun.gameObject.AddComponent<Regundant>();
			GunExt.SetShortDescription(gun, "All In One");
			GunExt.SetLongDescription(gun, "Reloads one bullet at a time. Can fire loaded bullets to interupt reload. \n" +
				"A nightmarish collection of loading mechanisms feed off of one single tube magazine.\n" +
				"By some marvel of engineering it is chambered in every caliber. Try not to think about it too much..." +
				"\n\n\n - Knife_to_a_Gunfight");

			GunExt.SetupSprite(gun, null, "regundant_idle_001", 8);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 8);
			GunExt.SetAnimationFPS(gun, gun.chargeAnimation, 8);
			GunExt.SetAnimationFPS(gun, gun.criticalFireAnimation, 8);
			GunExt.SetAnimationFPS(gun, gun.dischargeAnimation, 8);
			GunExt.SetAnimationFPS(gun, gun.introAnimation, 8);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 6);

			gun.barrelOffset.transform.localPosition = new Vector3(36/16f, 11/16f, 0f);
			gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(378) as Gun, true, false);


			//added modules get rarer as they go down.

			Projectile Split = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);
			Split.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(Split.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(Split);
			Split.baseData.damage *= 1.3f;
			Split.baseData.speed *= 0.65f;
			Split.baseData.range *= 1.5f;
			ProjectileSplitController pop = Split.gameObject.GetOrAddComponent<ProjectileSplitController>();
			pop.distanceBasedSplit = true;
			pop.distanceTillSplit = 0;
			pop.amtToSplitTo = 2;
			pop.numberofsplits = 1;
			pop.removeComponentAfterUse = true;
			pop.splitAngles = 20;
			gun.DefaultModule.projectiles.Add(Split);
			gun.DefaultModule.projectiles.Add(Split);
			gun.DefaultModule.projectiles.Add(Split);
			gun.DefaultModule.projectiles.Add(Split);
			gun.DefaultModule.projectiles.Add(Split);
			gun.DefaultModule.projectiles.Add(Split);
			gun.DefaultModule.projectiles.Add(Split);
			gun.DefaultModule.projectiles.Add(Split);

			Projectile Slug = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(694) as Gun).DefaultModule.projectiles[0]);
			Slug.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(Slug.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(Slug);
			Slug.baseData.damage *= 1f;
			Slug.baseData.speed *= 1f;
			Slug.baseData.range *= 1.5f;
			gun.DefaultModule.projectiles.Add(Slug);
			gun.DefaultModule.projectiles.Add(Slug);
			gun.DefaultModule.projectiles.Add(Slug);
			gun.DefaultModule.projectiles.Add(Slug);

			Projectile rifle = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(25) as Gun).DefaultModule.projectiles[0]);
			rifle.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(rifle.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(rifle);
			rifle.baseData.damage *= 1f;
			rifle.baseData.speed *= 1f;
			rifle.baseData.range *= 1.5f;
			gun.DefaultModule.projectiles.Add(rifle);
			gun.DefaultModule.projectiles.Add(rifle);
			gun.DefaultModule.projectiles.Add(rifle);
			gun.DefaultModule.projectiles.Add(rifle);

			Projectile explode = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(81) as Gun).DefaultModule.projectiles[0]);
			explode.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(explode.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(explode);
			explode.baseData.damage *= 1f;
			explode.baseData.speed *= 1f;
			explode.baseData.range *= 1.5f;
			gun.DefaultModule.projectiles.Add(explode);

			Projectile old = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(9) as Gun).DefaultModule.projectiles[0]);
			old.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(old.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(old);
			old.baseData.damage *= 1f;
			old.baseData.speed *= 1f;
			old.baseData.range *= 1.5f;
			gun.DefaultModule.projectiles.Add(old);

			Projectile Arrow = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(8) as Gun).DefaultModule.chargeProjectiles[0].Projectile);
			Arrow.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(Arrow.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(Arrow);
			Arrow.baseData.damage *= 1f;
			Arrow.baseData.speed *= 1f;
			Arrow.baseData.range *= 1.5f;
			gun.DefaultModule.projectiles.Add(Arrow);

			Projectile SArrow = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(8) as Gun).DefaultModule.projectiles[0]);
			SArrow.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(SArrow.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(SArrow);
			SArrow.baseData.damage *= 1f;
			SArrow.baseData.speed *= 1f;
			SArrow.baseData.range *= 1.5f;
			gun.DefaultModule.projectiles.Add(SArrow);

			Projectile fish = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(7) as Gun).DefaultModule.projectiles[0]);
			fish.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(fish.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(fish);
			fish.baseData.damage *= 1f;
			fish.baseData.speed *= 1f;
			fish.baseData.range *= 1.5f;
			gun.DefaultModule.projectiles.Add(fish);

			Projectile Pea = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(197) as Gun).DefaultModule.projectiles[0]);
			Pea.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(Pea.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(Pea);
			Pea.baseData.damage *= 1f;
			Pea.baseData.speed *= 1f;
			Pea.baseData.range *= 1.5f;
			gun.DefaultModule.projectiles.Add(Pea);
			
			Projectile Hammer = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(91) as Gun).DefaultModule.finalProjectile);
			Hammer.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(Hammer.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(Hammer);
			Hammer.baseData.damage *= 1f;
			Hammer.baseData.speed *= 1f;
			Hammer.baseData.range *= 1.5f;
			gun.DefaultModule.projectiles.Add(Hammer);

			Projectile Gold = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(53) as Gun).DefaultModule.projectiles[0]);
			Gold.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(Gold.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(Gold);
			Gold.baseData.damage *= 1f;
			Gold.baseData.speed *= 1f;
			Gold.baseData.range *= 1.5f;
			gun.DefaultModule.projectiles.Add(Gold);

			Projectile Magic = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(385) as Gun).DefaultModule.projectiles[0]);
			Magic.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(Magic.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(Magic);
			Magic.baseData.damage *= 1f;
			Magic.baseData.speed *= 1f;
			Magic.baseData.range *= 1.5f;
			gun.DefaultModule.projectiles.Add(Magic);

			Projectile staple = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(26) as Gun).DefaultModule.projectiles[0]);
			staple.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(staple.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(staple);
			staple.baseData.damage *= 1f;
			staple.baseData.speed *= 1f;
			staple.baseData.range *= 1.5f;
			gun.DefaultModule.projectiles.Add(staple);
			ProjectileSplitController pop2 = staple.gameObject.GetOrAddComponent<ProjectileSplitController>();
			pop2.distanceBasedSplit = true;
			pop2.distanceTillSplit = 0;
			pop2.amtToSplitTo = 4;
			pop2.numberofsplits = 1;
			pop2.removeComponentAfterUse = true;
			pop2.splitAngles = 15;

			gun.DefaultModule.numberOfShotsInClip = 6;
			gun.DefaultModule.cooldownTime = .8f;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
			gun.reloadTime = 3.6f;

			gun.SetBaseMaxAmmo(300);
			gun.gunClass = GunClass.NONE;
			gun.quality = PickupObject.ItemQuality.B;
			gun.carryPixelOffset = new IntVector2(5, 0);
			GunGradualReload gradual = gun.gameObject.GetOrAddComponent<GunGradualReload>();
			gradual.hasRepeatingAnimation = true;
			//gradual.additionalAudioEventsPerReload.Add("Play_WPN_colt1851_reload_01");
			//gradual.reloadEndSound = "Play_WPN_elephantgun_reload_01";
			gradual.mincliptofire = 2;
			gradual.reloadnumber = 1;

			gun.gunSwitchGroup = (PickupObjectDatabase.GetById(93) as Gun).gunSwitchGroup;
			ETGMod.Databases.Items.Add(gun, null, "ANY");
			Regundant.ID = gun.PickupObjectId;
			MiscToolMethods.TrimAllGunSprites(gun);
		}

		bool rerack = false;
		
        public override void Update()
        {
			if(this.gun.CurrentOwner != null)
            {
                if (gun.IsFiring == true && rerack == false && gun.IsReloading != true)
                {
					int RNG = UnityEngine.Random.Range(0, 4);
					StartCoroutine(RerackAnimate(RNG));
				}
            }

            base.Update();
        }

        private IEnumerator RerackAnimate(int RNG)
        {
			rerack = true;
            while (gun.spriteAnimator.IsPlaying("regundant_fire"))
			{
				yield return new WaitForEndOfFrame();
				yield return null;
            }

			
			if(gun.ClipShotsRemaining != 1)
            {
				if (RNG == 0)
				{
					gun.spriteAnimator.PlayForDuration("regundant_charge", 5 / 8f, "regundant_idle");
					AkSoundEngine.PostEvent("Play_WPN_elephantgun_reload_01", base.gameObject);

				}
				if (RNG == 1)
				{
					gun.spriteAnimator.PlayForDuration("regundant_discharge", 6 / 8f, "regundant_idle");
					AkSoundEngine.PostEvent("Play_WPN_m79grenadelauncher_reload_01", base.gameObject);

				}
				if (RNG == 2)
				{
					gun.spriteAnimator.PlayForDuration("regundant_critical_fire", 4 / 8f, "regundant_idle");
					AkSoundEngine.PostEvent("Play_WPN_shotgun_reload", base.gameObject);
				}
				if (RNG == 3)
				{
					gun.spriteAnimator.PlayForDuration("regundant_intro", 5 / 8f, "regundant_idle");
					AkSoundEngine.PostEvent("Play_WPN_elephantgun_reload_01", base.gameObject);
					AkSoundEngine.PostEvent("Play_ENM_wizardred_swing_01", base.gameObject);
				}
			}
			
			rerack = false;
		}

        public static int ID;

		public override void OnReload(PlayerController player, Gun gun)
		{
			
			
			base.OnReload(player, gun);
		}


	}
}