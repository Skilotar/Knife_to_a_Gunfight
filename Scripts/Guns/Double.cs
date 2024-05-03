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
    class messenger :AdvancedGunBehaviour
    {
		
		public static void Add()
		{
			
			Gun gun = ETGMod.Databases.Items.NewGun("Messenger", "tap");
			Game.Items.Rename("outdated_gun_mods:messenger", "ski:messenger");
			gun.gameObject.AddComponent<messenger>();
			gun.SetShortDescription("There And Back");
			gun.SetLongDescription("Trigger fires on press and on release. A high action coach gun used by the postal messengers of the gundrominian region in order to transfer parcels safely. \n\n- Knife_to_a_gunfight");
			gun.SetupSprite(null, "tap_idle_001", 1);
			gun.SetAnimationFPS(gun.shootAnimation, 10);
			gun.SetAnimationFPS(gun.reloadAnimation, 5);
			

			for (int i = 0; i < 10; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(157) as Gun, true, false);
				

			}
			
			foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
			{	
				projectileModule.ammoCost = 1;
				projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
				projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
				projectileModule.cooldownTime = 1;
				
				projectileModule.numberOfShotsInClip = 2;
				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
				projectile.gameObject.SetActive(false);
				projectileModule.projectiles[0] = projectile;
				projectile.baseData.damage = 4f;
				projectile.baseData.range = 10 * (1 + UnityEngine.Random.Range(-.2f, .2f));
				projectile.baseData.speed = 20 * (1 + UnityEngine.Random.Range(-.3f, .3f));
				projectileModule.angleVariance = 8f;
				
				projectile.AdditionalScaleMultiplier = .5f;
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
			gun.SetBaseMaxAmmo(200);
			gun.CurrentAmmo = 200;
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(157) as Gun).muzzleFlashEffects;
			gun.quality = PickupObject.ItemQuality.B;
			gun.shellsToLaunchOnReload = 2;
			gun.barrelOffset.transform.localPosition = new Vector3(1.25f, .8f, 0f);
			gun.shellsToLaunchOnFire = 0;
			Gun gun2 = PickupObjectDatabase.GetById(51) as Gun;
			gun.shellCasing = gun2.shellCasing;

			
			gun.gunClass = GunClass.SHOTGUN;
			/*
			tk2dSpriteAnimationClip fireClip = gun.sprite.spriteAnimator.GetClipByName("tap_reload"); // 4 frames
			float[] offsetsX = new float[] { 0.0f, 0.1f, 0.1f, 0.1f };
			float[] offsetsY = new float[] { 0f, -.4f, -.4f, -.4f};
			for (int i = 0; i < offsetsX.Length && i < offsetsY.Length && i < fireClip.frames.Length; i++)
			{
				int id = fireClip.frames[i].spriteId;
				fireClip.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX[i];
				fireClip.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY[i];
				fireClip.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX[i];
				fireClip.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY[i];
				fireClip.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX[i];
				fireClip.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY[i];
				fireClip.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX[i];
				fireClip.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY[i];
			}
			*/
			tk2dSpriteAnimationClip fireClip2 = gun.sprite.spriteAnimator.GetClipByName("tap_reload"); // 7 frames
			float[] offsetsX2 = new float[] { 0.0f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0f };
			float[] offsetsY2 = new float[] { 0f, -.4f, -.4f, -.4f, -.4f, -.4f, -.4f };
			for (int i = 0; i < offsetsX2.Length && i < offsetsY2.Length && i < fireClip2.frames.Length; i++)
			{
				int id = fireClip2.frames[i].spriteId;
				fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position0.x += offsetsX2[i];
				fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position0.y += offsetsY2[i];
				fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position1.x += offsetsX2[i];
				fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position1.y += offsetsY2[i];
				fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position2.x += offsetsX2[i];
				fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position2.y += offsetsY2[i];
				fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position3.x += offsetsX2[i];
				fireClip2.frames[i].spriteCollection.spriteDefinitions[id].position3.y += offsetsY2[i];
			}

			gun.carryPixelOffset = new IntVector2(6, -1);


			ETGMod.Databases.Items.Add(gun, null, "ANY");


		}

		public System.Random rand = new System.Random();

		public override void OnPostFired(PlayerController player, Gun gun)
		{

			AkSoundEngine.PostEvent("Play_WPN_golddoublebarrelshotgun_shot_01", base.gameObject);

			base.OnPostFired(player, gun);
		}


		public override void PostProcessProjectile(Projectile projectile)
		{
			
		}


        public override void OnReload(PlayerController player, Gun gun)
        {
			gun.shellsToLaunchOnReload = gun.ClipCapacity - gun.ClipShotsRemaining;
			base.OnReload(player, gun);
			gun.reloadTime = 1.5f;
		}

		
		public bool holding;
		
		public override void  Update()
		{
			base.Update();
			if (this.Owner != null)
			{

				BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer((this.gun.CurrentOwner as PlayerController).PlayerIDX);
				if (instanceForPlayer.ActiveActions.ShootAction.IsPressed && Time.timeScale != 0 && gun.ClipShotsRemaining != 0 && !instanceForPlayer.ActiveActions.MapAction.IsPressed)
				{
					holding = true;
					gun.reloadTime = .75f;

				}
				if (!instanceForPlayer.ActiveActions.ShootAction.IsPressed && holding && Time.timeScale != 0 && !instanceForPlayer.ActiveActions.MapAction.IsPressed)
				{
					gun.reloadTime = 1.5f;
					if(gun.ClipShotsRemaining > 0)
                    {
						gun.ClearCooldowns();
						gun.Attack();
					}
                    else
                    {
						AkSoundEngine.PostEvent("Play_WPN_magnum_empty_01", base.gameObject);
                    }

					holding = false;
				}

			}

		}


		private void OnDestroy()
		{

		}

	}
}



