using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using System.CodeDom;

namespace Knives
{
    class Trash : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Plumbing Pipelauncher", "trash");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:plumbing_pipelauncher", "ski:plumbing_pipelauncher");
            gun.gameObject.AddComponent<Trash>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Be Unpredictable");
            gun.SetLongDescription("Charge up to 3 rockets.\n\n" +
                "" +
                "All warfare is based on deception! Gun Tzu said that! SO we must think ahead of our enemies! BUT THAT'S WHAT THEY'LL BE EXPECTING! \n" +
                "The best way to fool you're opponent is to have no idea what your doing! " +
                "" +
                "\n- Knife_to_a_Gunfight");

            gun.SetupSprite(null, "trash_idle_001", 4);
            gun.SetAnimationFPS(gun.shootAnimation, 8);
            gun.SetAnimationFPS(gun.reloadAnimation, 24);
            

            gun.AddProjectileModuleFrom((PickupObjectDatabase.GetById(39) as Gun), true, true);
            Gun gun2 = (PickupObjectDatabase.GetById(39) as Gun);
            gun.muzzleFlashEffects = gun2.muzzleFlashEffects;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 18;
            gun.gunClass = GunClass.EXPLOSIVE;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 0f;

            gun.DefaultModule.cooldownTime = 1f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.SetBaseMaxAmmo(60);

            gun.quality = PickupObject.ItemQuality.D;
            gun.carryPixelOffset = new IntVector2(7, 0);



            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 10f;
            projectile.baseData.speed *= 1f;
            projectile.BossDamageMultiplier = 1.2f;


            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            gun.DefaultModule.projectiles[0] = projectile2;
            projectile2.baseData.damage = 11f;
            projectile2.baseData.speed *= 1f;
            projectile2.BossDamageMultiplier = 1.5f;


            Projectile projectile3 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile3.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile3.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile3);
            gun.DefaultModule.projectiles[0] = projectile3;
            projectile3.baseData.damage = 12f;
            projectile3.baseData.speed *= 1f;
            projectile3.BossDamageMultiplier = 1.5f;




            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = .75f,
                AdditionalWwiseEvent = "Play_pwoomp_01"
             };
            ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile2,

                ChargeTime = 1.5f,
                AdditionalWwiseEvent = "Play_pwoomp_02"
            };
            ProjectileModule.ChargeProjectile item3 = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile3,

                ChargeTime = 2.25f,
                AdditionalWwiseEvent = "Play_pwoomp_03"
            };
            
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,
                item2,
                item3

            };
            gun.PreventNormalFireAudio = true;

            ETGMod.Databases.Items.Add(gun, null, "ANY");


        }
        System.Random rng = new System.Random();
        public override void PostProcessProjectile(Projectile projectile)
        {
            AkSoundEngine.PostEvent("Play_BOSS_tank_grenade_01", base.gameObject);
            if (projectile.GetCachedBaseDamage == 10)
            {

            }
            if (projectile.GetCachedBaseDamage == 11)
            {
                StartCoroutine(fireclip2());
               
            }
            if (projectile.GetCachedBaseDamage == 12)
            {

                StartCoroutine(fireclip3());
            }
        }
        public bool toggle = true;

        public IEnumerator fireclip2()
        {
            PlayerController player = this.gun.CurrentOwner as PlayerController;
            yield return new WaitForSecondsRealtime(.2f);
            float angle = rng.Next(-20, 20);
            int getclip = this.gun.ClipShotsRemaining;
            Projectile projectile1 = ((PickupObjectDatabase.GetById(39) as Gun)).DefaultModule.projectiles[0];
            GameObject gameObject1 = SpawnManager.SpawnProjectile(projectile1.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + angle), true);
            Projectile component1 = gameObject1.GetComponent<Projectile>();
            component1.Owner = player;
            component1.baseData.damage = 21;
            component1.Shooter = player.specRigidbody;
            component1.BossDamageMultiplier = 1.5f;
            this.gun.GainAmmo(-1);
            AkSoundEngine.PostEvent("Play_BOSS_tank_grenade_01", base.gameObject);
        }

        public IEnumerator fireclip3()
        {
            PlayerController player = this.gun.CurrentOwner as PlayerController;

            yield return new WaitForSecondsRealtime(.2f);
            float angle = rng.Next(-30, 30);
            
            Projectile projectile1 = ((PickupObjectDatabase.GetById(39) as Gun)).DefaultModule.projectiles[0];
            GameObject gameObject1 = SpawnManager.SpawnProjectile(projectile1.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + angle), true);
            Projectile component1 = gameObject1.GetComponent<Projectile>();
            component1.Owner = player;
            component1.baseData.damage = 21;
            component1.Shooter = player.specRigidbody;
            component1.BossDamageMultiplier = 1.5f;
            this.gun.GainAmmo(-1);
            AkSoundEngine.PostEvent("Play_BOSS_tank_grenade_01", base.gameObject);
            yield return new WaitForSecondsRealtime(.2f);
            float angle2 = rng.Next(-30, 30);
            
            Projectile projectile2 = ((PickupObjectDatabase.GetById(39) as Gun)).DefaultModule.projectiles[0];
            GameObject gameObject2 = SpawnManager.SpawnProjectile(projectile2.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + angle2), true);
            Projectile component2 = gameObject2.GetComponent<Projectile>();
            component2.Owner = player;
            component2.baseData.damage = 21;
            component2.Shooter = player.specRigidbody;
            component2.BossDamageMultiplier = 1.5f;
            this.gun.GainAmmo(-1);
            AkSoundEngine.PostEvent("Play_BOSS_tank_grenade_01", base.gameObject);
        }
        
        public override void OnFinishAttack(PlayerController player, Gun gun)
        {
            toggle = true;

        }
        private bool HasReloaded;

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
                /*
                if (gun.GetChargeFraction() == 1)
                {

                    BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer((this.gun.CurrentOwner as PlayerController).PlayerIDX);
                    if (instanceForPlayer.ActiveActions.ShootAction && Time.timeScale != 0 && gun.ClipShotsRemaining != 0 && !instanceForPlayer.ActiveActions.MapAction.IsPressed)
                    {
                        instanceForPlayer.ActiveActions.ShootAction.ClearInputState();
                       
                    }
                }
                */
            }

            base.Update();
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


    }
}