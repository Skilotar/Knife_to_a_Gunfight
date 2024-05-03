using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Collections.Generic;
using MultiplayerBasicExample;
using SaveAPI;

namespace Knives
{
    class Hippo : AdvancedGunBehaviour
    {

        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Hipp0", "hippo");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:hipp0", "ski:hipp0");
            gun.gameObject.AddComponent<Hippo>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Ammo Starved");
            gun.SetLongDescription("Hungry bullets from this gun will return 2 ammo directly to the clip when hitting another bullet and biting bullets with the reload will return 3 ammo per bullet.\n\n" +
                "" +
                "This gun was designed as an option for the common everyday working man who wanted to own a minigun. Sporting a cheaper wooden frame it costs significantly less than its Vulcan counterpart. However, the gun was so ammo inefficent that it inexplicably gained the ability to manifest a spirit in order to consume more shellings." +
                "" +
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "hippo_idle_001", 8);


            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 10);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 6;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            
            gun.GainsRateOfFireAsContinueAttack = true;
            gun.RateOfFireMultiplierAdditionPerSecond = .4f;
            
            gun.reloadTime = 1.9f;
            gun.DefaultModule.numberOfShotsInClip = 70;
            gun.DefaultModule.cooldownTime = .22f;
            gun.SetBaseMaxAmmo(500);
            gun.quality = PickupObject.ItemQuality.C;
            gun.encounterTrackable.EncounterGuid = "Hungry hippo";

            Gun shellingFlash = (Gun)PickupObjectDatabase.GetById(96);
            gun.muzzleFlashEffects = shellingFlash.muzzleFlashEffects;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(1.8f, .5f, 0f);
          
            gun.shellsToLaunchOnFire = 4;
            Gun gun2 = PickupObjectDatabase.GetById(84) as Gun;
            gun.shellCasing = gun2.shellCasing;


            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 3f;
            projectile.baseData.speed *= .5f;
            projectile.baseData.range *= 1f;

            gun.gameObject.AddComponent<GunSpecialStates>().DoesCountBlocks = true;

          

            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.SetupUnlockOnCustomFlag(CustomDungeonFlags.AMMO_STARVED, true);


            ID = gun.PickupObjectId;
        }

        public static int ID;

        public override void  OnPickedUpByPlayer(PlayerController player)
        {
           
            base.OnPickedUpByPlayer(player);
        }
        public bool fireSFXcontroller = true;
        public override void OnPostFired(PlayerController player, Gun gun)
        {

           
            gun.PreventNormalFireAudio = true;
        }
        public override void OnFinishAttack(PlayerController player, Gun gun)
        {


            base.OnFinishAttack(player, gun);
        }

        public System.Random rng = new System.Random();
        public override void PostProcessProjectile(Projectile projectile)
        {
            int chance = rng.Next(1, 13);
            if(chance == 1)
            {

                HungryProjectileModifier hunger = projectile.gameObject.GetOrAddComponent<HungryProjectileModifier>();
                hunger.HungryRadius = .2f;
                
                
            }

            if ((this.gun.CurrentOwner as PlayerController).PlayerHasActiveSynergy("Insatiable"))
            {

                HungryProjectileModifier hunger = projectile.gameObject.GetOrAddComponent<HungryProjectileModifier>();
                hunger.HungryRadius = .2f;

            }

            if (projectile.gameObject.GetComponent<HungryProjectileModifier>() != null)
            {
                projectile.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(projectile.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison));
            }

            base.PostProcessProjectile(projectile);
        }

        private void OnPreCollison(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody other, PixelCollider otherCollider)
        {
            //eat projectile
            if(other.projectile != null && other.projectile.Owner != this.Owner)
            {
                gun.GainAmmo(2);
                gun.MoveBulletsIntoClip(1);
            }
           

        }
        public bool HasReloaded = true;
        public override void  Update()
        {
            if (this.gun.CurrentOwner)
            {

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;

                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
               
                if(this.gun.GetComponent<GunSpecialStates>().successfullBlocks > 0)
                {
                      
                    gun.GainAmmo(3);

                    gun.GetComponent<GunSpecialStates>().successfullBlocks = gun.GetComponent<GunSpecialStates>().successfullBlocks - 1;
                        
                       
                }

               
               
               
            }

            base.Update();
        }
        public bool isdoingbite = false;
       
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                this.gun.PreventNormalFireAudio = true;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                if (isdoingbite == false)
                {
                    player.StartCoroutine(this.Bite(player));
                    isdoingbite = true;
                }
               
                //ETGModConsole.Log(gun.GetComponent<GunSpecialStates>().successfullBlocks.ToString());
            }

        }

        public IEnumerator Bite(PlayerController player)
        {
            yield return new WaitForSecondsRealtime(1f);


            Projectile projectile2 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
            GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag = component != null;
            if (flag)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                component.baseData.damage = 10f;

                ProjectileSlashingBehaviour slasher = component.gameObject.AddComponent<ProjectileSlashingBehaviour>();
                slasher.SlashDimensions = 60;
                slasher.SlashRange = 3.5f;
                slasher.playerKnockback = 0;
                
                slasher.soundToPlay = "Play_BOSS_bashellisk_swallow_01";
                slasher.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
                //Play_BOSS_bashellisk_swallow_01

            }
            yield return new WaitForSecondsRealtime(1f);
            isdoingbite = false;
        }
    }
}
