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
	class BattlingRam : AdvancedGunBehaviour
	{

		public static void Add()
		{

			Gun gun = ETGMod.Databases.Items.NewGun("Battling Ram", "BattleRam");
			Game.Items.Rename("outdated_gun_mods:battling_ram", "ski:battling_ram");
			gun.gameObject.AddComponent<BattlingRam>();
			gun.SetShortDescription("All Access Pass");
			gun.SetLongDescription("A sturdy battle rifle with a battering ram integrated. Swing at a sealed door when reloading to forcibly Unseal it. Be careful as bashing open doors will damage your ammo reserves. " +
				"" +
				"\n\n- Knife_to_a_gunfight");


			gun.SetupSprite(null, "BattleRam_idle_001", 1);
			gun.SetAnimationFPS(gun.shootAnimation, 10);
			gun.SetAnimationFPS(gun.reloadAnimation, 5);

			gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(25) as Gun, true, false);


			gun.gunHandedness = GunHandedness.TwoHanded;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.angleVariance = 0f;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Burst;
			gun.DefaultModule.burstShotCount = 2;
			gun.DefaultModule.burstCooldownTime = .2f;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.DefaultModule.cooldownTime = .7f;
			gun.DefaultModule.numberOfShotsInClip = 8;
			gun.reloadTime = 2f;
			gun.SetBaseMaxAmmo(300);
			gun.CurrentAmmo = 300;

			gun.quality = PickupObject.ItemQuality.B;

			gun.gunClass = GunClass.RIFLE;

			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);

			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			gun.barrelOffset.localPosition = new Vector3(2.5625f, 0.9375f, 0f);
			gun.shellsToLaunchOnFire = 1;
			Gun gun2 = PickupObjectDatabase.GetById(84) as Gun;
			gun.shellCasing = gun2.shellCasing;
			
			projectile.baseData.damage = 16f;
			projectile.baseData.speed *= 1.5f;
			projectile.baseData.range = 20f;
			gun.shellsToLaunchOnReload = 0;
			gun.reloadShellLaunchFrame = 6;
			gun.CanReloadNoMatterAmmo = true;
			ETGMod.Databases.Items.Add(gun, null, "ANY");


			ID = gun.PickupObjectId;


		}

		public static int ID;



		public override void PostProcessProjectile(Projectile projectile)
		{
			AkSoundEngine.PostEvent("Play_WPN_m1911_shot_01", base.gameObject);
			
		}

        private void OnPreCollison(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
			if(otherPixelCollider != null && otherRigidbody.aiActor == null)
            {

				//You cant run from a trainer battle!
				PlayerController owner = (PlayerController)this.gun.CurrentOwner;
				bool inBossFight = false;
				if (owner.CurrentRoom != null && owner.CurrentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.All))
				{
					foreach (AIActor aiactor in owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
					{
						if (aiactor != null && aiactor.healthHaver != null && aiactor.healthHaver.IsBoss)
						{
							inBossFight = true;
						}

					}
				}
				//for locks
				IPlayerInteractable nearestInteractable = owner.CurrentRoom.GetNearestInteractable(myRigidbody.sprite.WorldCenter, 3f, owner); ;
				if(nearestInteractable != null)
                {
					if (nearestInteractable is InteractableLock)
					{

						InteractableLock interactableLock = nearestInteractable as InteractableLock;
						interactableLock.ForceUnlock();
						UnityEngine.Object.Instantiate<GameObject>(EasyVFXDatabase.TeleporterPrototypeTelefragVFX, interactableLock.sprite.WorldCenter, Quaternion.identity);
						gun.LoseAmmo(50);
						gun.shellsToLaunchOnReload = 30;


					}
				}
				//for secret rooms
				if(otherRigidbody.GetComponent<MajorBreakable>() != null)
                {
					bool isSecretDoor = otherRigidbody.GetComponent<MajorBreakable>().IsSecretDoor;
					if (isSecretDoor)
					{
						gun.shellsToLaunchOnReload = 30;
						AkSoundEngine.PostEvent("Play_OBJ_boulder_crash_01", base.gameObject);
						AkSoundEngine.PostEvent("Play_OBJ_boulder_crash_01", base.gameObject);
						AkSoundEngine.PostEvent("Play_OBJ_boulder_crash_01", base.gameObject);
						otherRigidbody.GetComponent<MajorBreakable>().ApplyDamage(50, Vector2.zero, false, false, true);
						
						gun.LoseAmmo(50);
					}
				}
				//for door
				if (otherRigidbody.gameObject.GetComponentInParent<DungeonDoorController>() != null && !inBossFight )
                {

					DungeonDoorController door = otherRigidbody.gameObject.GetComponentInParent<DungeonDoorController>();
					RoomHandler room = (this.gun.CurrentOwner as PlayerController).CurrentRoom;
					
					if (door.IsSealed)
					{
						gun.shellsToLaunchOnReload = 20;

						gun.LoseAmmo(30);

                        if (owner.IsInCombat)
                        {
							GameObject roomResetter = new GameObject("room resetter");
							roomResetter.transform.position = owner.CurrentRoom.GetCenterCell().ToVector2();
							roomResetter.AddComponent<LeaveRoomResetter>().parentRoom = owner.CurrentRoom;
						}

					}
					
					door.DoUnseal(room);
					if (owner.CurrentGun.CurrentAngle > 135 || owner.CurrentGun.CurrentAngle < -45)
					{
						door.Open(true);
					}
                    else
                    {
						door.Open();

					}

                    
				}
				


			}
        }
		

		public override void OnReload(PlayerController player, Gun gun)
		{
			gun.shellsToLaunchOnReload = 0;
			AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);

			StartCoroutine(BashDelay(player));
			

			base.OnReload(player, gun);
		}

        private IEnumerator BashDelay(PlayerController player)
        {
			yield return new WaitForSeconds(1);
			
			doslashy(player);
			AkSoundEngine.PostEvent("Play_ENM_chainknight_swing_01", base.gameObject);
			
			yield return new WaitForSeconds(.02f);
			Projectile projectile = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
			GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
			Projectile component = gameObject.GetComponent<Projectile>();
			component.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(component.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison));
			bool flag2 = component != null;
			if (flag2)
			{
				component.Owner = player;
				component.Shooter = player.specRigidbody;
				component.baseData.damage = 0f;
				component.baseData.speed = 100f;
				component.baseData.range = 3f;
				component.sprite.renderer.enabled = false;
				component.SuppressHitEffects = true;
				component.hitEffects.suppressMidairDeathVfx = true;
				
			}

			

		}
		

		private void doslashy(PlayerController player)
		{
			Projectile projectile1 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
			GameObject gameObject = SpawnManager.SpawnProjectile(projectile1.gameObject, this.gun.CurrentOwner.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (this.gun.CurrentOwner.CurrentGun == null) ? 0f : this.gun.CurrentAngle), true);
			Projectile component = gameObject.GetComponent<Projectile>();
			component.AdditionalScaleMultiplier = .001f;
			component.Owner = this.gun.CurrentOwner;
			component.baseData.damage = 25;
			ProjectileSlashingBehaviour slashy = component.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
			slashy.SlashRange = 4.2f;
			slashy.DoSound = false;
			slashy.SlashDimensions = 45;
			slashy.StunApplyChance = 100;
			slashy.AppliesStun = true;
			slashy.StunTime = 1f;
			slashy.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
			slashy.SlashVFX.type = VFXPoolType.None;
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
