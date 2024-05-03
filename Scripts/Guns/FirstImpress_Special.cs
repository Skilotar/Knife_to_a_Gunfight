using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using System.Runtime.InteropServices;
using System.Text;
using Brave;
using Brave.BulletScript;
using Gungeon;
using HutongGames.PlayMaker.Actions;
using ItemAPI;
using UnityEngine;

namespace Knives
{
    class FirstImpression_special : AdvancedGunBehaviour
    {


        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("First Impression special", "FI");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:first_impression_special", "ski:first_impression_special");
            gun.gameObject.AddComponent<FirstImpression_special>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Make It Count");
            gun.SetLongDescription("A blade that knows how to make an entrance! Gains massive strength if it is the first attack you do in a room." + 
                "\n\n\n - Knife_to_a_Gunfight"); ;


            gun.SetupSprite(null, "FI_idle_001", 8);

            
            gun.SetAnimationFPS(gun.shootAnimation, 14);
            gun.SetAnimationFPS(gun.reloadAnimation, 14);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(59) as Gun, true, false);
           


            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.SILLY;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;

            gun.reloadTime = 2f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.DefaultModule.cooldownTime = .5f;
           


            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.EXCLUDED;
            

            //swipe
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            gun.barrelOffset.transform.localPosition = new Vector3(1.5f, 2f, 0f);
            


            



            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 9f;
            projectile.baseData.speed = 3f;
            projectile.baseData.range = 3f;
            projectile.baseData.force = 5;


            gun.sprite.usesOverrideMaterial = true;

            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 4f);
            mat.SetFloat("_EmissivePower", 3);

            MeshRenderer component = gun.GetComponent<MeshRenderer>();
            if (!component)
            {
                ETGModConsole.Log("nope");
                return;
            }
            Material[] sharedMaterials = component.sharedMaterials;
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                if (sharedMaterials[i].shader == mat)
                {
                    return;
                }
            }
            Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
            Material material = new Material(mat);
            material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
            sharedMaterials[sharedMaterials.Length - 1] = material;
            component.sharedMaterials = sharedMaterials;



            gun.InfiniteAmmo = true;
            projectile.transform.parent = gun.barrelOffset;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            MiscToolMethods.TrimAllGunSprites(gun);
            FirstImpression_special.SpecialID = gun.PickupObjectId;
        }


        public static int SpecialID;


        public override void OnPostFired(PlayerController player, Gun gun)
        {

            gun.PreventNormalFireAudio = true;

        }
        public override void PostProcessProjectile(Projectile projectile)
        {
           
            base.PostProcessProjectile(projectile);
        }
      

        private bool HasReloaded;

        public bool resist = false;
        public override void  Update()
        {

            if (gun.CurrentOwner)
            {
                PlayerController player = (PlayerController)this.gun.CurrentOwner;

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


            }

        }



    }
}