using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using Alexandria.Misc;

namespace Knives
{

    public class Spewer : GunBehaviour
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Spewer", "Spewer");
            Game.Items.Rename("outdated_gun_mods:spewer", "ski:spewer");
            gun.gameObject.AddComponent<Spewer>();
            gun.SetShortDescription("The Goo Wizard");
            gun.SetLongDescription("shoots poison, infects you with poison.\n\n" +
                "Mad scrawling on the side of the blaster reads..\n\n" +
                "The maddness of Goo infects you! Goo the world! Spout your toxic waves amongst the masses and let the era of chaos bubble annew." +
                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "Spewer_idle_001", 4);
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 12);
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(207) as Gun, true, false);
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(208) as Gun).gunSwitchGroup;
            gun.doesScreenShake = false;
            //GUN STATS
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(151) as Gun).muzzleFlashEffects;
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.5f;
            gun.DefaultModule.cooldownTime = 0.07f;
            gun.DefaultModule.numberOfShotsInClip = 50;
            gun.barrelOffset.transform.localPosition = new Vector3(42f / 16f, 9f / 16f, 0f);
            gun.SetBaseMaxAmmo(400);
            gun.ammo = 400;
            gun.gunClass = GunClass.POISON;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "green_beam";
            gun.DefaultModule.angleVariance = 10;


            //BULLET STATS
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.AdditionalScaleMultiplier *= 1.5f;
            projectile.baseData.damage = 2f;
            projectile.baseData.speed *= 1f;
            projectile.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitBlendUber");
            projectile.sprite.renderer.material.SetFloat("_VertexColor", 1f);
            projectile.sprite.color = projectile.sprite.color.WithAlpha(0.75f);
            projectile.sprite.usesOverrideMaterial = true;
            projectile.PoisonApplyChance = .5f;
            projectile.AppliesPoison = true;
            SlowingBulletsEffect slow = projectile.gameObject.GetOrAddComponent<SlowingBulletsEffect>();
            PierceProjModifier poke = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            poke.penetration = 2;
            slow.minspeedMultPerFrame = 94f;
            slow.LifeSpan = 1.5f;
            gun.quality = PickupObject.ItemQuality.B;
            ETGMod.Databases.Items.Add(gun, null, "ANY");


            ID = gun.PickupObjectId;
        }
        public static int ID;
        public override void PostProcessProjectile(Projectile projectile)
        {
            PlayerController Player = this.gun.CurrentOwner as PlayerController;
            Player.CurrentPoisonMeterValue += .07f;
            if(Player.CurrentPoisonMeterValue > 1)
            {
                GoopDefinition poison = EasyGoopDefinitions.PoisonDef;
                poison.lifespan = 5f;
                DeadlyDeadlyGoopManager goop = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(poison);
                goop.TimedAddGoopCircle(Player.CenterPosition, 5f, 1f, true);
            }
            base.PostProcessProjectile(projectile);
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool manual)
        {
            if(this.gun.ClipShotsRemaining != this.gun.ClipCapacity)
            {
                AkSoundEngine.PostEvent("Play_OBJ_bottle_cork_01", base.gameObject);
            }
            
            base.OnReloadPressed(player, gun, manual);
        }
    }
}