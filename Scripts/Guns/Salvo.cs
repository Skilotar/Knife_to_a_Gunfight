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
	class Salvo : AdvancedGunBehaviour
	{
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Salvo", "Salvo");
            Game.Items.Rename("outdated_gun_mods:salvo", "ski:salvo");
            gun.gameObject.AddComponent<Salvo>();
            gun.SetShortDescription("The Perfect Partner");
            gun.SetLongDescription("The preferred weapon of a mighty warlord, deadset on toppling oppression." +
                "\n\n- Knife_to_a_gunfight");

            gun.SetupSprite(null, "Salvo_idle_001", 1);

            gun.SetAnimationFPS(gun.shootAnimation, 6);
            gun.SetAnimationFPS(gun.reloadAnimation, 3);
            gun.SetAnimationFPS(gun.idleAnimation, 1);


            for (int i = 0; i < 3; i++)
            {

                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(16) as Gun, true, false);

            }
            foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
            {
                projectileModule.ammoCost = 1;
                projectileModule.shootStyle = ProjectileModule.ShootStyle.Burst;
                projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                projectileModule.burstShotCount = 2;
                projectileModule.burstCooldownTime = .01f;
                projectileModule.cooldownTime = 1f;
                projectileModule.angleVariance = 30f;
                projectileModule.numberOfShotsInClip = 8;
                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                projectileModule.projectiles[0] = projectile;
                projectile.baseData.damage = 5f;
                projectile.AdditionalScaleMultiplier = .5f;

                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);


                gun.DefaultModule.projectiles[0] = projectile;
                bool flag = projectileModule != gun.DefaultModule;
                if (flag)
                {
                    projectileModule.ammoCost = 0;
                }

            }


            GunGradualReload reload = gun.gameObject.GetOrAddComponent<GunGradualReload>();
            reload.emptyclipPunishtime = 0f;
            reload.mincliptofire = 2;
            reload.reloadnumber = 1;
            reload.hasRepeatingAnimation = true;
            reload.frameToRepeatAnimationFrom = 1;
            gun.gunClass = GunClass.SHOTGUN;
            gun.SetBaseMaxAmmo(80);
            gun.CurrentAmmo = 80;
            gun.carryPixelOffset = new IntVector2(6, 0);
            gun.quality = PickupObject.ItemQuality.A;
            gun.reloadTime = 4f;
            gun.barrelOffset.transform.localPosition = new Vector3(2f, .5f, 0f);

            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;

        }
        public static int ID;
		public override void OnPickedUpByPlayer(PlayerController player)
		{
			this.gun.HasBeenPickedUp = true;

			base.OnPickedUpByPlayer(player);
		}
		public override void Update()
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
			
			AkSoundEngine.PostEvent("Play_WPN_comm4nd0_shot_01", base.gameObject);
			AkSoundEngine.PostEvent("Play_half_gauge_fire", base.gameObject);
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

