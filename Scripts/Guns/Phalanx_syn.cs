using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;


namespace Knives
{

    public class Phalanx_syn : AdvancedGunBehaviour
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Phalanx_syn", "Phalanx_syn");
            Game.Items.Rename("outdated_gun_mods:phalanx_syn", "ski:phalanx_syn");
            gun.gameObject.AddComponent<Phalanx>();
            gun.SetShortDescription("Hold Formation!");
            gun.SetLongDescription("Shoots Fragile Blocklets. \n\n" +
                "The perfect miliary formation perfected by the Great Bromans millennia ago. " +
                "The stance was so inspiring that the fearsome Iron Legion Swears by it; integrating it into their weapons and ships. " +
                "Good ol Phalanx! Nothing beats that! " +
                "\n\n\n - Knife_to_a_Gunfight");

            gun.SetupSprite(null, "Phalanx_syn_idle_001", 8);
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(51) as Gun).gunSwitchGroup;
            gun.SetAnimationFPS(gun.shootAnimation, 12);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);
            gun.SetAnimationFPS(gun.idleAnimation, 5);

            for (int i = 0; i < 12; i++)
            {
                gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            }
            int increment = 0;
            foreach (ProjectileModule mod in gun.Volley.projectiles)
            {

                mod.ammoCost = 1;
                mod.shootStyle = ProjectileModule.ShootStyle.Automatic;
                mod.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
                mod.cooldownTime = .35f;
                mod.angleVariance = 0f;
                mod.numberOfShotsInClip = 16;
                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(mod.projectiles[0]);
                mod.projectiles[0] = projectile;
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                projectile.baseData.damage = 3.5f;
                projectile.AdditionalScaleMultiplier *= .8f;
                BlockletsModifier block = projectile.gameObject.GetOrAddComponent<BlockletsModifier>();
                block.DieWithShield = false;
                float speedboost = 0;
                if (increment >= 3) speedboost = 0;
                if (increment <= 4 && increment >= 6) speedboost = .1f;
                if (increment <= 7 && increment >= 9) speedboost = .2f;
                if (increment <= 10 && increment >= 12) speedboost = .3f;
                projectile.baseData.speed *= .6f + speedboost;
                projectile.shouldRotate = true;
                mod.positionOffset = offsets[increment];
                mod.angleFromAim = offsetAngles[increment];

                if (mod != gun.DefaultModule)
                {
                    mod.ammoCost = 0;
                }
                increment++;
                projectile.transform.parent = gun.barrelOffset;
            }

            gun.reloadTime = 1f;
            gun.barrelOffset.transform.position = new Vector3(1.5f, .7f, 0f);
            gun.SetBaseMaxAmmo(500);
            gun.gunClass = GunClass.SHOTGUN;
            gun.quality = PickupObject.ItemQuality.EXCLUDED;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.SHOTGUN;

            ID = gun.PickupObjectId;
        }
        public override void PostProcessProjectile(Projectile projectile)
        {

            base.PostProcessProjectile(projectile);
        }
        public static int ID;

        public static Dictionary<int, Vector2> offsets = new Dictionary<int, Vector2>
        {
            { 0,new Vector2(0,0)},
            { 1,new Vector2(0,0.3f)},
            { 2,new Vector2(0,-0.3f)},
            { 3,new Vector2(0.5f,0)},
            { 4,new Vector2(0.5f,0.3f)},
            { 5,new Vector2(0.5f,-0.3f)},
            { 6,new Vector2(1f,0)},
            { 7,new Vector2(1f,0.3f)},
            { 8,new Vector2(1f,-0.3f)},
            { 9,new Vector2(1.5f,0)},
            { 10,new Vector2(1.5f,0.3f)},
            { 11,new Vector2(1.5f,-0.3f)},
        };

        public static Dictionary<int, float> offsetAngles = new Dictionary<int, float>
        {
            { 0,0},
            { 1,2},
            { 2,-2},
            { 3,0},
            { 4,2},
            { 5,-2},
            { 6,0},
            { 7,2},
            { 8,-2},
            { 9,0},
            { 10,2},
            { 11,-2},
        };


    }
}