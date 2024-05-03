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
	class DoomShotty : AdvancedGunBehaviour
	{

		public static void Add()
		{

			Gun gun = ETGMod.Databases.Items.NewGun("Super Shotgun", "Doom");
			Game.Items.Rename("outdated_gun_mods:super_shotgun", "ski:super_shotgun");
			gun.gameObject.AddComponent<DoomShotty>();
			gun.SetShortDescription("Rip And Tear");
			gun.SetLongDescription("Criticals to Jammed enemies, press reload again while reloading to grapple.\n\n" +
				"Demons run at the sound of this gun. An unassuming distant click of the hammer sends shudders down the spines of the unholy. Give them HELL!" +
				"\n\n- Knife_to_a_gunfight");
			gun.SetupSprite(null, "Doom_idle_001", 1);
			gun.SetAnimationFPS(gun.shootAnimation, 10);
			gun.SetAnimationFPS(gun.reloadAnimation, 5);
			gun.SetAnimationFPS(gun.criticalFireAnimation, 3);
			gun.AddPassiveStatModifier(PlayerStats.StatType.Curse, 3, StatModifier.ModifyMethod.ADDITIVE);
			gun.AddCurrentGunStatModifier(PlayerStats.StatType.MovementSpeed,1, StatModifier.ModifyMethod.ADDITIVE);

			
			for (int i = 0; i < 7; i++)
			{
				gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(157) as Gun, true, false);

			}

			foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
			{
				projectileModule.ammoCost = 1;
				projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
				projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
				projectileModule.cooldownTime = .4f;

				projectileModule.numberOfShotsInClip = 2;
				Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
				projectile.gameObject.SetActive(false);
				
				projectile.baseData.damage = 5f;
				projectile.baseData.range = 12 * (1 + UnityEngine.Random.Range(-.2f, .2f));
				projectile.baseData.speed = 30 * (1 + UnityEngine.Random.Range(-.3f, .3f));
				projectileModule.angleVariance = 10f;
				projectile.BlackPhantomDamageMultiplier = 3f;

				projectile.AdditionalScaleMultiplier = .7f;
				FakePrefab.MarkAsFakePrefab(projectile.gameObject);
				UnityEngine.Object.DontDestroyOnLoad(projectile);
				projectileModule.projectiles[0] = projectile;
				gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.SHOTGUN;
				gun.DefaultModule.projectiles[0] = projectile;
				bool flag = projectileModule != gun.DefaultModule;
				if (flag)
				{
					projectileModule.ammoCost = 0;
				}

			}

			gun.reloadTime = .7f;
			gun.SetBaseMaxAmmo(300);
			gun.CurrentAmmo = 300;
			gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(157) as Gun).muzzleFlashEffects;
			gun.quality = PickupObject.ItemQuality.A;
			gun.shellsToLaunchOnReload = 2;

			gun.shellsToLaunchOnFire = 0;
			Gun gun2 = PickupObjectDatabase.GetById(51) as Gun;
			gun.shellCasing = gun2.shellCasing;
			

			gun.gunClass = GunClass.SHOTGUN;
			gun.carryPixelOffset = new IntVector2(6, 0);
			gun.barrelOffset.transform.localPosition = new Vector3(1.25f, .8f, 0f);
			ETGMod.Databases.Items.Add(gun, null, "ANY");

			ID = gun.PickupObjectId;
		}

		public static int ID;
		public override void OnPostFired(PlayerController player, Gun gun)
		{

			AkSoundEngine.PostEvent("Play_WPN_golddoublebarrelshotgun_shot_01", base.gameObject);

			base.OnPostFired(player, gun);
		}


		public override void PostProcessProjectile(Projectile projectile)
		{
			projectile.OnHitEnemy += this.onhitenemy;
		}

        private void onhitenemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2.aiActor)
            {
				bool hassyn = false;
				if (Player.PlayerHasActiveSynergy("Until It Is Done!")) hassyn = true;
				
				if (arg2.aiActor.IsBlackPhantom)
                {
					if(hassyn) arg2.healthHaver.ApplyDamage(arg1.baseData.damage, Vector2.zero, "Rip and Tear");
					arg2.healthHaver.ApplyDamage(arg1.baseData.damage, Vector2.zero, "Rip and Tear");
                    if (arg3)
                    {
						this.gun.GainAmmo(4);
                    }
                }

                if (hassyn)
                {
                    if (UnityEngine.Random.Range(0, 20f) <= 2)
                    {
						arg2.aiActor.BecomeBlackPhantom();
                    }
                }
				
            }
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			
			InitGrapple();
			
			AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
            if (gun.IsReloading)
            {
				StartCoroutine(slightparydelay());
			}
		}
		public bool reloadDelay = false;
		private IEnumerator slightparydelay()
		{
			yield return new WaitForSeconds(.1f);
			reloadDelay = true;
		}

		private void InitGrapple()
        {
			
			this.m_grappleModule = new GrappleModule();
			this.m_grappleModule.GrapplePrefab = this.GrapplePrefab;
			this.m_grappleModule.GrappleSpeed = 40;
			this.m_grappleModule.GrappleRetractSpeed = 30f;
			this.m_grappleModule.DamageToEnemies = 10f;
			this.m_grappleModule.EnemyKnockbackForce = -40f;
			this.m_grappleModule.sourceGameObject = base.gameObject;
			GrappleModule grappleModule = this.m_grappleModule;
			grappleModule.FinishedCallback = (Action)Delegate.Combine(grappleModule.FinishedCallback, new Action(this.GrappleEndedNaturally));

		}

        private void GrappleEndedNaturally()
        {
			Once = false;
        }

        public GameObject GrapplePrefab = ((GrapplingHookItem)PickupObjectDatabase.GetById(250)).GrapplePrefab;
		private GrappleModule m_grappleModule;
		public bool setup = false;
		public bool Once = false;

        public bool HasReloaded { get; private set; }

		public override void Update()
		{
			base.Update();
			if (this.Owner != null)
			{

				
				if (!gun.IsReloading && !HasReloaded)
				{
					this.HasReloaded = true;
					reloadDelay = false;
				}

				if (gun.IsReloading && reloadDelay == true)// press R to parry and skip reload
				{
					PlayerController player = (PlayerController)this.gun.CurrentOwner;
					if (KeyDown(GungeonActions.GungeonActionType.Reload, (PlayerController)this.gun.CurrentOwner) )
					{

						if (Once == false)
						{
							AkSoundEngine.PostEvent("Play_OBJ_hook_shot_01", base.gameObject);
							m_grappleModule.Trigger(player);
							Once = true;

							tk2dSpriteAnimationClip clip = gun.spriteAnimator.GetClipByName("Doom_critical_fire");
							gun.spriteAnimator.Play(clip, 0, 2 / gun.reloadTime, true);
						}
					}

				}

			}

		}

		public bool KeyDown(GungeonActions.GungeonActionType action, PlayerController user)
		{
			if (!GameManager.Instance.IsLoadingLevel && user != null)
			{
				return BraveInput.GetInstanceForPlayer(user.PlayerIDX).ActiveActions.GetActionFromType(action).WasPressed;
			}
			else
			{
				return false;
			}

		}


	}
}



