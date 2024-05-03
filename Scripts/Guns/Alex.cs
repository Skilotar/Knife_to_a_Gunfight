
using System;
using System.Collections;
using System.Collections.Generic;
using Gungeon;
using MonoMod;
using UnityEngine;
using Dungeonator;
using ItemAPI;
using MultiplayerBasicExample;

namespace Knives
{

    class Alex : AdvancedGunBehaviour
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Library of Alexandria", "Alex");
            Game.Items.Rename("outdated_gun_mods:library_of_alexandria", "ski:library_of_alexandria");
            gun.gameObject.AddComponent<Alex>();
            gun.SetShortDescription("Throw the book at em!");
            gun.SetLongDescription("This is the saved condensed manusript of the entire library of alexandria prior to its burning. It is an staggeringly heavy book with leagues of knowledge. it would be a shame if something were to happen to it.\n\n" +
                "Throwable book that has a special trick when lit on fire." +
                "\n\n\n" +
                "- Knife_to_a_Gunfight");
           
            gun.SetupSprite(null, "Alex_idle_001", 8);
            
            gun.SetAnimationFPS(gun.shootAnimation, 5);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
           
            gun.AddProjectileModuleFrom("ak-47", true, true);
            
            gun.DefaultModule.ammoCost = 999;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Burst;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.DefaultModule.burstShotCount = 3;
            gun.GainsRateOfFireAsContinueAttack = true;
            gun.RateOfFireMultiplierAdditionPerSecond = 10f;
            gun.reloadTime = 1.7f;
            gun.DefaultModule.cooldownTime = 3f;
            gun.DefaultModule.numberOfShotsInClip = 0;
            gun.SetBaseMaxAmmo(0);
            gun.quality = PickupObject.ItemQuality.B;
            gun.encounterTrackable.EncounterGuid = "Dont set Encounter Guids";
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            
            projectile.baseData.damage *= .10f;
            projectile.baseData.speed *= 1f;
            projectile.transform.parent = gun.barrelOffset;
           
          

            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }

		private bool HasReloaded;

		public override void OnPostFired(PlayerController player, Gun gun)
        {
            
            gun.PreventNormalFireAudio = true;
          
        }
        
       
       
       
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
               


            }
        }
        int counter = 0;
        public override void Update()
        {
            if (gun.CurrentOwner)
            {
                if (gun.IsPreppedForThrow == true)
                {

                    //figure statbuff here

                }

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;

                }



            }
            
            
            bool flag = this.IsPointOnFire(gun.sprite.WorldCenter);
            if (flag)
            {
                if (counter == 600)
                {
                    CreateRewardItem();
                    UnityEngine.GameObject.Destroy(this);
                }

                counter++;
            }


        }
        private bool IsPointOnFire(Vector2 testPos)
		{
			IntVector2 Burnyoustupidbook = (testPos / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2(VectorConversions.Floor);
			if (DeadlyDeadlyGoopManager.allGoopPositionMap.ContainsKey(Burnyoustupidbook))
			{
				DeadlyDeadlyGoopManager deadlyDeadlyGoopManager = DeadlyDeadlyGoopManager.allGoopPositionMap[Burnyoustupidbook];
				return deadlyDeadlyGoopManager.IsPositionOnFire(testPos);
			}
			return false;
		}

		protected void CreateRewardItem()
		{
			PickupObject.ItemQuality itemQuality = PickupObject.ItemQuality.B;
			
			
			PickupObject itemOfTypeAndQuality = LootEngine.GetItemOfTypeAndQuality<PickupObject>(itemQuality, (UnityEngine.Random.value >= 0.5f) ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable, false);
			if (itemOfTypeAndQuality)
			{
				LootEngine.SpawnItem(itemOfTypeAndQuality.gameObject, base.transform.position, Vector2.up, 0.1f, true, false, false);
			}
		}

	
      
	}
}