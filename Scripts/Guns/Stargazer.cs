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
    class Stargazer : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Stargazer", "Stargazer");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:stargazer", "ski:stargazer");
            gun.gameObject.AddComponent<Stargazer>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Seeing Stars");
            gun.SetLongDescription("Chance to Stun, Shoot a stunned enemy to knock stars off them.\n\n" +
                "A modified double barrel revolver used by the children of the sun for disputes and hunting. Custom ammunition for this revolver shines bright as it flies." +
                "\n\n\n - Knife_to_a_Gunfight");
            gun.SetupSprite(null, "Stargazer_idle_001", 3);
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 7);


            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(25) as Gun, true, false);
            gun.PreventNormalFireAudio = true;

            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            Gun gun3 = (Gun)PickupObjectDatabase.GetById(81);
            gun.muzzleFlashEffects = gun3.muzzleFlashEffects;
            gun.gunClass = GunClass.RIFLE;
            gun.reloadTime = .85f;
            gun.DefaultModule.cooldownTime = .25f;
            gun.DefaultModule.numberOfShotsInClip = 7;
            gun.SetBaseMaxAmmo(400);
            gun.quality = PickupObject.ItemQuality.B;
            gun.carryPixelOffset = new IntVector2(8, 0);

            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);

            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            gun.barrelOffset.transform.localPosition = new Vector3(2f, .5f, 0f);
            projectile.baseData.damage = 10f;
            projectile.baseData.speed *= .8f;
            projectile.baseData.range = 20f;
            projectile.StunApplyChance = .07f;
            projectile.AppliedStunDuration = 1.5f;
            projectile.AppliesStun = true;

            gun.shellsToLaunchOnFire = 1;
            Gun gun2 = PickupObjectDatabase.GetById(84) as Gun;
            gun.shellCasing = gun2.shellCasing;

            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(157) as Gun).DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            projectile2.baseData.damage = 6f;
            projectile2.baseData.speed = 20f;
            projectile2.baseData.range = 60;
            projectile2.StunApplyChance = 1f;
            projectile2.AppliedStunDuration = 2f;
            projectile2.AppliesStun = true;
            projectile2.shouldRotate = true;
            projectileStates state2 = projectile2.gameObject.GetOrAddComponent<projectileStates>();
            state2.gazer = true;
            gun.DefaultModule.usesOptionalFinalProjectile = true;
            gun.DefaultModule.numberOfFinalProjectiles = 1;
            gun.DefaultModule.finalProjectile = projectile2;
            gun.DefaultModule.finalAmmoType = GameUIAmmoType.AmmoType.BLUE_SHOTGUN;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            star = projectile2;

            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;
        }

        public static Projectile star;
        public static int ID;
        public override void OnPickedUpByPlayer(PlayerController player)
        {
          
            base.OnPickedUpByPlayer(player);
        }


        public override void OnPostFired(PlayerController player, Gun gun)
        {
            
        }


        public override void PostProcessProjectile(Projectile projectile)
        {
            projectile.OnHitEnemy += this.OnHitEnemy;
            if (projectile.gameObject.GetOrAddComponent<projectileStates>().gazer == true)
            {
                float angle = projectile.transform.eulerAngles.magnitude;
                MiscToolMethods.SpawnProjAtPosi(star, projectile.transform.TransformPoint(.9f, 0f, 0), (PlayerController)this.gun.CurrentOwner, this.gun,0,1.05f,false);
                MiscToolMethods.SpawnProjAtPosi(star, projectile.transform.TransformPoint(.4f, .5f, 0), (PlayerController)this.gun.CurrentOwner, this.gun,2,1f, false);
                MiscToolMethods.SpawnProjAtPosi(star, projectile.transform.TransformPoint(.4f, -.5f, 0), (PlayerController)this.gun.CurrentOwner, this.gun,-2,1f, false);
                MiscToolMethods.SpawnProjAtPosi(star, projectile.transform.TransformPoint(-.5f, .35f, 0), (PlayerController)this.gun.CurrentOwner, this.gun,2,.95f, false);
                MiscToolMethods.SpawnProjAtPosi(star, projectile.transform.TransformPoint(-.5f, -.35f, 0), (PlayerController)this.gun.CurrentOwner, this.gun,-2,.95f, false);
            }
            
            base.PostProcessProjectile(projectile);
        }

        private void OnHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if(arg2.aiActor != null)
            {
                if (arg2.aiActor.behaviorSpeculator.IsStunned == true)
                {
                    DoStaryBusrt(arg2.UnitCenter);
                }
            }
            
        }

        private void DoStaryBusrt(Vector2 vector2)
        {

            Projectile projectile = ((Gun)ETGMod.Databases.Items[52]).DefaultModule.chargeProjectiles[0].Projectile;
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, vector2, Quaternion.Euler(0f, 0f, 0), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = this.Owner;
                component.Shooter = this.Owner.specRigidbody;
            }

        }


        private bool HasReloaded;
        //This block of code allows us to change the reload sounds.
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