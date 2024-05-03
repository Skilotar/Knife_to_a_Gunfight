using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Collections.Generic;
using MultiplayerBasicExample;

namespace Knives
{
    class MBSynergyForm : AdvancedGunBehaviour
    {

        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Monkey Barrel ak", "mb_ak");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:monkey_barrel_ak", "ski:monkey_barrel_ak");
            gun.gameObject.AddComponent<MBSynergyForm>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Hmm Monkey");
            gun.SetLongDescription("This barrel filled with small red monkeys is a testament to teamwork. Most regular bulletkin could beat a single monkey in fight, but this troop knows how to work together to pack a punch!" +
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "mb_ak_idle_001", 8);
            
            gun.SetAnimationFPS(gun.introAnimation, 9);
            gun.SetAnimationFPS(gun.idleAnimation, 4);
            gun.SetAnimationFPS(gun.shootAnimation, 20);
            gun.SetAnimationFPS(gun.reloadAnimation, 5);

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.idleAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.Loop;
           
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(81) as Gun, true, false);

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 4;

            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.3f;
            gun.DefaultModule.numberOfShotsInClip = 15;
            gun.DefaultModule.cooldownTime = .20f;
            gun.SetBaseMaxAmmo(500);
            gun.quality = PickupObject.ItemQuality.EXCLUDED;
            gun.clipsToLaunchOnReload = 1;
            gun.shellsToLaunchOnFire = 1;
            
            gun.shellCasing = (PickupObjectDatabase.GetById(15) as Gun).shellCasing;
            gun.clipObject = (PickupObjectDatabase.GetById(15) as Gun).clipObject;
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(15) as Gun).muzzleFlashEffects;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "banana";
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.SetProjectileSpriteRight("nanner", 5, 11, false, tk2dBaseSprite.Anchor.MiddleCenter, 5, 11);
            gun.barrelOffset.transform.localPosition = new Vector3(2.2f, .2f, 0f);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage *= .25f;
            projectile.baseData.speed *= .8f;
            projectile.baseData.range *= 1f;
            projectile.angularVelocity = 700;
            
            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            MBSynergyForm.mbakID = gun.PickupObjectId;


        }

        public static int mbakID;
        public System.Random rng = new System.Random();
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            if (player.PlayerHasActiveSynergy("Apex Predator"))
            {
                int oo = rng.Next(1, 4);
                switch (oo)
                {
                    case 1:
                        AkSoundEngine.PostEvent("Play_Monkey_shoot_001", base.gameObject);

                        break;
                    case 2:
                        AkSoundEngine.PostEvent("Play_Monkey_shoot_002", base.gameObject);

                        break;
                    case 3:
                        AkSoundEngine.PostEvent("Play_Monkey_shoot_003", base.gameObject);

                        break;
                    default: // case 4
                        AkSoundEngine.PostEvent("Play_Monkey_shoot_004", base.gameObject);

                        break;
                }

            }

        }
        public override void OnFinishAttack(PlayerController player, Gun gun)
        {


            base.OnFinishAttack(player, gun);
        }

        public override void PostProcessProjectile(Projectile projectile)
        {



            base.PostProcessProjectile(projectile);
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            this.HasReloaded = false;
            

            AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
            base.OnReloadPressed(player, gun, bSOMETHING);



        }
       
        public override void  OnPostDrop(GameActor owner)
        {
            PlayerController player = (PlayerController)owner;
            player.GunChanged -= this.OnGunChanged;
        }
        private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
        {
           
        }

        private bool HasReloaded;
        public override void  Update()
        {
            if (gun.CurrentOwner)
            {

                if (!gun.PreventNormalFireAudio)
                {
                   
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
              
            }
        }
    }
}
