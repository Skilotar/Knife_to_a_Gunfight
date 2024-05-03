using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;
using System.Collections.Generic;

namespace Knives
{

    public class StarBurst : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("StarBurst", "StarBurst");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:starburst", "ski:starburst");
            gun.gameObject.AddComponent<StarBurst>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Pepper the Heavens");
            gun.SetLongDescription("Charge to fan the hammer.\n\n" +
                "A forgotten relic of the children of the sun found in a shallow grave in the gundrominian region. Along with an ash covered journal and a rotting hat this firearm was recovered by frontiersmen to the region. \n\n" +
                "The star children once played in these fields before their childish squables turned to bloodshed." +
                "" +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "StarBurst_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(79) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.carryPixelOffset = new IntVector2(3, 0);
            gun.reloadTime = 1f;
            gun.DefaultModule.cooldownTime = .23f;
            gun.DefaultModule.numberOfShotsInClip = 6;
            gun.SetBaseMaxAmmo(300);
            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.C;
            gun.barrelOffset.transform.localPosition = new Vector3(1.3f, .5f, 0f);;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(15) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 8f;
            projectile.baseData.speed *= 1f;

            
            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(15) as Gun).DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            gun.DefaultModule.projectiles[0] = projectile2;
            projectile2.baseData.damage = 9f;
            projectile2.baseData.speed *= 1f;

            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = 0f
            };
            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                
                Projectile = projectile2,
                ChargeTime = .5f
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
                item2
            };




            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");


            ID = gun.PickupObjectId;
        }

        public static int ID;



        public override void OnPostFired(PlayerController player, Gun gun)
        {
           
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            AkSoundEngine.PostEvent("Play_WPN_beretta_shot_01", base.gameObject);

            if (projectile.GetCachedBaseDamage == 9)
            {
                
                int hammerhits = 6;
                if(gun.ClipShotsRemaining < 6)
                {
                    hammerhits = gun.ClipShotsRemaining;
                }
                
                gun.StartCoroutine(FanTheHammer(hammerhits));
                
            }

            base.PostProcessProjectile(projectile);
        }

        private IEnumerator FanTheHammer(int hammerhits)
        {
           
            for (int i = 2; i <= hammerhits; i++)
            {
                
                yield return new WaitForSeconds(.12f);
                gun.DefaultModule.angleVariance += 5f;
                
                gun.ClearCooldowns();
                gun.Attack();
            }
            gun.DefaultModule.angleVariance = 0f;
        }

        private bool HasReloaded;
       
        public override void  Update()
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
       
        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                
                AkSoundEngine.PostEvent("Play_WPN_rpg_reload_01", base.gameObject);


            }

        }

       
    }
}

