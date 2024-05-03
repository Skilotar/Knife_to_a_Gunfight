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
    class Snake_Eyes : AdvancedGunBehaviour
    {

        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Mulligun", "SnakeEyes");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:mulligun", "ski:mulligun");
            gun.gameObject.AddComponent<Snake_Eyes>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Snake-Eyes");
            gun.SetLongDescription("Getting two ones in a row will automatically reload the weapon.\n\n" +
                "" +
                "A manufacting error on the custom cartridges for this gun has them stuffed with between 1 and 6 bullets. The question is... Do you feel lucky?" +
                "" +
               
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "SnakeEyes_idle_001", 2);
            gun.SetAnimationFPS(gun.shootAnimation, 12);
            gun.SetAnimationFPS(gun.reloadAnimation, 5);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SHOTGUN;
            gun.gunHandedness = GunHandedness.TwoHanded;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1f;
            gun.DefaultModule.numberOfShotsInClip = 8;
            gun.DefaultModule.cooldownTime = .45f;
            gun.SetBaseMaxAmmo(400);
            gun.quality = PickupObject.ItemQuality.B;


            gun.gunSwitchGroup = ((Gun)PickupObjectDatabase.GetById(93)).gunSwitchGroup;

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(1.3f, .6f, 0f);
            gun.carryPixelOffset = new IntVector2(5, 0);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 3f;
            projectile.baseData.speed *= 1f;
            projectile.baseData.range *= 1f;
            projectile.hitEffects.suppressMidairDeathVfx = true;
            projectile.shouldRotate = true;
            
            projectile.transform.parent = gun.barrelOffset;

            projectile.gameObject.GetOrAddComponent<projectileStates>().isDiceStarter = true;

            ETGMod.Databases.Items.Add(gun, null, "ANY");




        }

        public override void OnPickedUpByPlayer(PlayerController player)
        {

            base.OnPickedUpByPlayer(player);
        }
        int lastKnownRoll = 0;
        public override void PostProcessProjectile(Projectile mainproj)
        {
            if(mainproj.gameObject.GetOrAddComponent<projectileStates>().isDiceStarter == true)
            {
                int globalroll = UnityEngine.Random.Range(1, 7);
                Projectile projectile = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
                switch (globalroll)
                {
                    case (1):

                        if (lastKnownRoll == 1)
                        {
                            gun.GainAmmo(8);
                            gun.MoveBulletsIntoClip(9);
                            AkSoundEngine.PostEvent("Play_Stat_Up", base.gameObject);
                            lastKnownRoll = 0;
                        }

                        break;
                    case (2):

                        SpawnProjAtPosi(projectile, mainproj.transform.TransformPoint(0, .6f, 0));
                        SpawnProjAtPosi(projectile, mainproj.transform.TransformPoint(0, -.6f, 0));
                        mainproj.hitEffects.suppressMidairDeathVfx = true;
                        mainproj.transform.position = new Vector3(0, 0);
                        mainproj.DieInAir();

                        break;
                    case (3):

                        SpawnProjAtPosi(projectile, mainproj.transform.TransformPoint(0, .8f, 0));
                        SpawnProjAtPosi(projectile, mainproj.transform.TransformPoint(0, -.8f, 0));

                        break;
                    case (4):

                        SpawnProjAtPosi(projectile, mainproj.transform.TransformPoint(.5f, .5f, 0));
                        SpawnProjAtPosi(projectile, mainproj.transform.TransformPoint(.5f, -.5f, 0));
                        SpawnProjAtPosi(projectile, mainproj.transform.TransformPoint(-.5f, .5f, 0));
                        SpawnProjAtPosi(projectile, mainproj.transform.TransformPoint(-.5f, -.5f, 0));
                        mainproj.hitEffects.suppressMidairDeathVfx = true;
                        mainproj.transform.position = new Vector3(0, 0);
                        mainproj.DieInAir();

                        break;
                    case (5):

                        SpawnProjAtPosi(projectile, mainproj.transform.TransformPoint(.5f, .5f, 0));
                        SpawnProjAtPosi(projectile, mainproj.transform.TransformPoint(.5f, -.5f, 0));
                        SpawnProjAtPosi(projectile, mainproj.transform.TransformPoint(-.5f, .5f, 0));
                        SpawnProjAtPosi(projectile, mainproj.transform.TransformPoint(-.5f, -.5f, 0));


                        break;
                    case (6):
                        SpawnProjAtPosi(projectile, mainproj.transform.TransformPoint(0, .8f, 0));
                        SpawnProjAtPosi(projectile, mainproj.transform.TransformPoint(0, -.8f, 0));
                        SpawnProjAtPosi(projectile, mainproj.transform.TransformPoint(-1.5f, .8f, 0));
                        SpawnProjAtPosi(projectile, mainproj.transform.TransformPoint(-1.5f, -.8f, 0));
                        SpawnProjAtPosi(projectile, mainproj.transform.TransformPoint(-1.5f, 0, 0));

                        break;
                }


                lastKnownRoll = globalroll;

            }

            
            base.PostProcessProjectile(projectile);
        }


        public void SpawnProjAtPosi(Projectile proj, Vector2 posi)
        {


            GameObject gameObject = SpawnManager.SpawnProjectile(proj.gameObject, posi, Quaternion.Euler(0f, 0f, ((this.gun.CurrentOwner as PlayerController).CurrentGun == null) ? 0f : (this.gun.CurrentOwner as PlayerController).CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = this.gun.CurrentOwner;
                component.Shooter = this.gun.CurrentOwner.specRigidbody;

                (this.gun.CurrentOwner as PlayerController).DoPostProcessProjectile(proj);
                //component.PostprocessPlayerBullet();
            }


        }
        public override void OnFinishAttack(PlayerController player, Gun gun)
        {


            base.OnFinishAttack(player, gun);
        }

       

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            this.HasReloaded = false;

            lastKnownRoll = 0;
            AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
            base.OnReloadPressed(player, gun, bSOMETHING);
            AkSoundEngine.PostEvent("Play_WPN_yarirocketlauncher_reload_01", base.gameObject);



        }

        private bool HasReloaded;
        public override void Update()
        {
            if (gun.CurrentOwner)
            {

                
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }

            }
        }
    }
}

