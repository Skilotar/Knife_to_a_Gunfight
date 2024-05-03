
using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;

namespace Knives
{

    public class VectorMKTwo : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Vector 2", "vector2");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:vector_2", "ski:vector2");
            gun.gameObject.AddComponent<VectorMKTwo>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Directional Storage");
            gun.SetLongDescription("A perfection of the original vector design. This Mk 2 version recalls its shots after some time." +

                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "vector2_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 30);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(57) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 14f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.gunHandedness = GunHandedness.OneHanded;
            gun.reloadTime = 1f;
            gun.DefaultModule.cooldownTime = .07f;
            gun.DefaultModule.numberOfShotsInClip = 30;
            gun.SetBaseMaxAmmo(600);
            gun.quality = PickupObject.ItemQuality.B;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(199) as Gun).muzzleFlashEffects;

            gun.barrelOffset.transform.localPosition = new Vector3(1.25f, .8f, 0f);
            gun.carryPixelOffset = new IntVector2(5, 0);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            VectorMK2BulletsEffect floaty =projectile.gameObject.GetOrAddComponent<VectorMK2BulletsEffect>();
            PierceProjModifier stabby = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            stabby.penetration = 1;
            gun.PreventOutlines = true;
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 4f;
            projectile.baseData.force = 0;
            projectile.baseData.range *= 2f;
            projectile.baseData.speed *= 2f;
            ImprovedAfterImage trail = projectile.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            trail.shadowLifetime = .2f;
            trail.shadowTimeDelay = .0001f;
            trail.dashColor = new Color(.01f, .50f, .67f);
            trail.spawnShadows = true;
            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.FULLAUTO;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            ID = gun.PickupObjectId;

        }

        public static int ID;

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_shot_01", base.gameObject);

        }
        public override void PostProcessProjectile(Projectile projectile)
        {
            
            base.PostProcessProjectile(projectile);
        }

        private bool HasReloaded;

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

        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
                AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_reload", base.gameObject);


            }

        }

    }
}
