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
    class Steam_overheat : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Steam_rifle_overheat", "SteamRifleO");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:steam_rifle_overheat", "ski:steam_rifle_o");

            gun.gameObject.AddComponent<Steam_overheat>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("");
            gun.SetLongDescription("" +

                "\n\n\n - Knife_to_a_Gunfight");
            gun.SetupSprite(null, "SteamRifleO_idle_001", 1);
            gun.SetAnimationFPS(gun.shootAnimation, 9);
            gun.SetAnimationFPS(gun.reloadAnimation, 7);
            gun.SetAnimationFPS(gun.introAnimation, 6);

            for (int i = 0; i < 20; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);


            }
            foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
            {
                projectileModule.ammoCost = 1;
                projectileModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
                projectileModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                projectileModule.cooldownTime = 1.4f;
                projectileModule.angleVariance = 40f;

                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                projectileModule.projectiles[0] = projectile;
                projectile.baseData.damage = 1;
                projectile.baseData.speed *= 1f;
                projectile.baseData.range = 1f;
                projectile.SuppressHitEffects = true;
                projectileModule.numberOfShotsInClip = int.MaxValue;

                projectile.SetProjectileSpriteRight("steam", 12, 12, false, tk2dBaseSprite.Anchor.MiddleCenter, 12, 12);
                PierceProjModifier stabby = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
                stabby.penetratesBreakables = true;
                stabby.penetration = 5;
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);

                gun.barrelOffset.transform.localPosition = new Vector3(2f, .5f, 0f);


                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                gun.DefaultModule.projectiles[0] = projectile;
                bool flag = projectileModule != gun.DefaultModule;
                if (flag)
                {
                    projectileModule.ammoCost = 0;
                }


            }
            gun.PreventNormalFireAudio = true;
            gun.muzzleFlashEffects = null;
            gun.gunClass = GunClass.RIFLE;
            gun.reloadTime = 0f;

            gun.DefaultModule.numberOfShotsInClip = int.MaxValue;
            gun.SetBaseMaxAmmo(300);
            gun.quality = PickupObject.ItemQuality.EXCLUDED;


            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.





            ETGMod.Databases.Items.Add(gun, null, "ANY");
            Steam_overheat.SpecialID = gun.PickupObjectId;
        }

        public static int SpecialID;

        public override void  OnPickedUpByPlayer(PlayerController player)
        {
           
            base.OnPickedUpByPlayer(player);
        }


        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_Steam_Attack", base.gameObject);
            GlobalSparksDoer.DoRandomParticleBurst(40, gun.sprite.WorldBottomLeft, gun.sprite.WorldTopRight, new Vector3(1f, 1f, 0f), 120f, 0.75f, null, null, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
            gun.PreventNormalFireAudio = true;
        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            gun.PreventNormalFireAudio = true;
            //projectile.gameObject.GetOrAddComponent<SlowingBulletsEffect>();
            base.PostProcessProjectile(projectile);

        }

        private bool HasReloaded;
        //This block of code allows us to change the reload sounds.
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
