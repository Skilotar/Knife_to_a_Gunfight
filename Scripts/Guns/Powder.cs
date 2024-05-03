using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using ItemAPI;
using Gungeon;
using System.Collections;
using HutongGames.PlayMaker.Actions;

namespace Knives
{
    class Powder_bag : AdvancedGunBehaviour 
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Imported Powder Bag", "PowderBag");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:imported_powder_bag", "ski:imported_powder_bag");

            gun.gameObject.AddComponent<Powder_bag>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Surplus");
            gun.SetLongDescription("One of many bags of gunpowder purchased from off planet by the gungeoneers. \n\nAfflicts enemies with [BlastBlight]. Afflicted enemies will explode after 3 instances of damage or on death. " +

                "\n\n\n - Knife_to_a_Gunfight");
            gun.SetupSprite(null, "PowderBag_idle_001", 1);
            gun.SetAnimationFPS(gun.shootAnimation, 2);
            gun.SetAnimationFPS(gun.reloadAnimation, 7);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 1;
            gun.gunClass = GunClass.EXPLOSIVE;
            gun.gunHandedness = GunHandedness.OneHanded;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 0f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.DefaultModule.cooldownTime = .8f;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.C;
            gun.UsesRechargeLikeActiveItem = true;
            gun.ActiveItemStyleRechargeAmount = 120;

            gun.PreventNormalFireAudio = true;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(.8f, .4f, 0f);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 0f;
            projectile.baseData.speed *= 2f;
            projectile.baseData.range = 0f;


            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.





            ETGMod.Databases.Items.Add(gun, null, "ANY");
            
        }
     

  


        // Token: 0x06000392 RID: 914 RVA: 0x000261A4 File Offset: 0x000243A4
        
        
        public override void OnPostFired(PlayerController player, Gun gun)
        {


            gun.PreventNormalFireAudio = true;
        }
        public override void PostProcessProjectile(Projectile proj)
        {
            PlayerController player = (PlayerController)gun.CurrentOwner;
            
            for (int i = 0; i < 8; i++)
            {
                int vari = UnityEngine.Random.Range(-20, 20);
                Projectile projectile = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + vari), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag2 = component != null;
                if (flag2)
                {
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    component.baseData.damage = 0f;
                    component.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy));
                    component.HasDefaultTint = true;
                    component.DefaultTintColor = new Color(.70f, .40f, .24f);
                    component.CurseSparks = true;
                    
                    component.gameObject.GetOrAddComponent<SlowingBulletsEffect>();

                }
               
            }
            
            base.PostProcessProjectile(projectile);

        }
        private void HandleHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2 && arg2.aiActor)
            {
                AIActor aiActor = arg2.aiActor;
                if (aiActor.IsNormalEnemy && !aiActor.IsHarmlessEnemy)
                {

                    BlastBlightedStatusController boom = aiActor.gameObject.GetOrAddComponent<BlastBlightedStatusController>();
                    boom.statused = true;

                }
            }
        }

        private bool HasReloaded;

        public override void  Update()
        {

            if (gun.CurrentOwner)
            {
                PlayerController player = this.gun.CurrentOwner as PlayerController;

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;

                }




            }
           

            base.Update();
        }
      


        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                HasReloaded = false;

                base.OnReloadPressed(player, gun, bSOMETHING);

            }
        }



    }

}