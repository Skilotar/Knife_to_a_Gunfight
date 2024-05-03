using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using Alexandria.ItemAPI;
using MultiplayerBasicExample;
using System.Collections.Generic;

namespace Knives
{

    public class Raigun : AdvancedGunBehavior 
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("RaiGun", "Rai");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:raigun", "ski:raigun");
            gun.gameObject.AddComponent<Raigun>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Melody Of Monsoons");
            gun.SetLongDescription("The SP4RK brand Raigun causes storms with the magnitude of the mythical Raijin. Ripping clouds and scarring the earth with an endless volley of power." +
                "\n\n\n - Knife_to_a_Gunfight");

            gun.SetupSprite(null, "Rai_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 8);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);

            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).loopStart = 4;


            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(79) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 10f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 0f;
            gun.DefaultModule.cooldownTime = .02f;
            gun.DefaultModule.numberOfShotsInClip = 1800;
            gun.SetBaseMaxAmmo(1800);
            gun.ammo = 1800;

            // Here we just set the quality of the gun and the "EncounterGuid", which is used by Gungeon to identify the gun.
            gun.quality = PickupObject.ItemQuality.A;

            //This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 1.5f;
            projectile.baseData.speed = 60;

            projectile.transform.parent = gun.barrelOffset;
            projectile.AdditionalScaleMultiplier *= .5f;
            LightningProjectileComp zappy = projectile.gameObject.GetOrAddComponent<LightningProjectileComp>();

            List<string> BeamAnimPaths = new List<string>()
            {

                "Knives/Resources/BeamSprites/Sp4rk_001",
                "Knives/Resources/BeamSprites/Sp4rk_002",
                "Knives/Resources/BeamSprites/Sp4rk_003",
            };
            projectile.AddTrailToProjectile(
                 "Knives/Resources/BeamSprites/Sp4rk_001",
                new Vector2(3, 2),
                new Vector2(1, 1),
                BeamAnimPaths, 20,
                BeamAnimPaths, 20,
                -1,
                0.0001f,
                5,
                true
                );
            projectile.damageTypes = CoreDamageTypes.Electric;
            //hehe pretty glow
            gun.sprite.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.SetColor("_EmissiveColor", new Color32(234, 183, 80, 255));
            mat.SetFloat("_EmissiveColorPower", 6f);
            mat.SetFloat("_EmissivePower", 4);
            mat.SetFloat("_EmissiveThresholdSensitivity", .26f);

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



            gun.barrelOffset.transform.localPosition = new Vector3(1.7f, .7f, 0f);
            Gun muzzle = (Gun)PickupObjectDatabase.GetById(142);
            gun.muzzleFlashEffects = muzzle.muzzleFlashEffects;
            gun.gunClass = GunClass.FULLAUTO;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

        }
        bool revedup = false;
        System.Random rng = new System.Random();
        protected override void  OnPickup(GameActor owner)
        {
            PlayerController player = (PlayerController)owner;
            
            base.OnPickup(owner);
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            if (revedup == false)
            {
                //startupsound
                int sound = rng.Next(1, 2);
                switch (sound)
                {
                    case 1:
                        AkSoundEngine.PostEvent("Play_Rai_charge_001", base.gameObject);
                        break;
                    case 2:
                        AkSoundEngine.PostEvent("Play_Rai_charge_002", base.gameObject);
                        break;
                }
                
                revedup = true;
            }
        }
        

        public override void PostProcessProjectile(Projectile projectile)
        {
            int sound = rng.Next(1, 4);
            switch (sound)
            {
                case 1:
                    AkSoundEngine.PostEvent("Play_Tesla_fire_001", base.gameObject);
                   
                    break;
                case 2:
                    AkSoundEngine.PostEvent("Play_Tesla_fire_002", base.gameObject);
                   
                    break;

                case 3:
                    AkSoundEngine.PostEvent("Play_Tesla_fire_003", base.gameObject);
                   
                    break;

                case 4:
                    AkSoundEngine.PostEvent("Play_Tesla_fire_004", base.gameObject);
                   
                    break;

            }

            base.PostProcessProjectile(projectile);
        }

        private bool HasReloaded;
        //This block of code allows us to change the reload sounds.
        protected override void  Update()
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
               
            }

            if (revedup == true)
            {
                
                revedup = false;
            }

        }

    }
}

