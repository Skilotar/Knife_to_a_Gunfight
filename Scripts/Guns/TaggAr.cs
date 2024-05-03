using System;
using System.Collections;
using Gungeon;
using MonoMod;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;
using System.Collections.Generic;
using Alexandria.BreakableAPI;
using SaveAPI;

namespace Knives
{

    public class TaggAr : AdvancedGunBehaviour
    {

        
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Tagg AR", "Tag");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:tagg_ar", "ski:tagg_ar");
            gun.gameObject.AddComponent<TaggAr>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("Anarchy Rainbow");
            gun.SetLongDescription("Chance to mark the ground with status symbols.\n\n" +
                "A spraypaint gun for the tagger on the go. Used by many skilled graffiti artist to mark a wall while still rolling by. " +

                "\n\n\n - Knife_to_a_Gunfight");


            gun.SetupSprite(null, "Tag_idle_001", 8);
            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 26);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);
            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(15) as Gun, true, false);
            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 16f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.gunHandedness = GunHandedness.OneHanded;
            gun.reloadTime = .9f;
            gun.DefaultModule.cooldownTime = .1f;
            gun.DefaultModule.numberOfShotsInClip = 15;
            gun.SetBaseMaxAmmo(1000);
            gun.quality = PickupObject.ItemQuality.B;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);
            

            gun.barrelOffset.transform.localPosition = new Vector3(1.3f, .5f, 0f);
            gun.carryPixelOffset = new IntVector2(3, 1);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 5f;
            projectile.baseData.force = .1f;
            projectile.baseData.range *= 2f;
            projectile.baseData.speed *= 2.1f;
            projectile.AdditionalScaleMultiplier *= .5f;
            projectile.objectImpactEventName = "SND_CHR_blobulin_death_01";

            gun.shellCasing = BreakableAPIToolbox.GenerateDebrisObject("Knives/Resources/Casings/Tag_mag.png", true, 2, 4, 200, 150, null, 3f, null, null, 1).gameObject;
            gun.shellsToLaunchOnFire = 0;
            gun.shellsToLaunchOnReload = 1;
            gun.shellCasingOnFireFrameDelay = 3;
            KanjiWriterProjMod paint = projectile.gameObject.GetOrAddComponent<KanjiWriterProjMod>();
            paint.Painter = true;


            
            // gross nasty multi flash setup :)
            VFXPool pool = new VFXPool();
            pool.type = VFXPoolType.SequentialGroups;
            
            VFXComplex muzzle1 =  MiscToolMethods.CreateVFXComplex("Paint Muzzleflash1",
                new List<string>()
                {
                    "Knives/Resources/Muzzleflashes/taggFlashes/Tagg_muzzleflash_001",
                    "Knives/Resources/Muzzleflashes/taggFlashes/Tagg_muzzleflash_002",
                    "Knives/Resources/Muzzleflashes/taggFlashes/Tagg_muzzleflash_003",
                },
                18, //FPS
                new IntVector2(27, 16), //Dimensions
                tk2dBaseSprite.Anchor.MiddleLeft, //Anchor
                false, //Uses a Z height off the ground
                0, //The Z height, if used
                false,
               VFXAlignment.Fixed
               );

            VFXComplex muzzle2 = MiscToolMethods.CreateVFXComplex("Paint Muzzleflash2",
                new List<string>()
                {
                    "Knives/Resources/Muzzleflashes/taggFlashes/Tagg_muzzleflash_004",
                    "Knives/Resources/Muzzleflashes/taggFlashes/Tagg_muzzleflash_005",
                    "Knives/Resources/Muzzleflashes/taggFlashes/Tagg_muzzleflash_006",
                },
                18, //FPS
                new IntVector2(27, 16), //Dimensions
                tk2dBaseSprite.Anchor.MiddleLeft, //Anchor
                false, //Uses a Z height off the ground
                0, //The Z height, if used
                false,
               VFXAlignment.Fixed
               );

            VFXComplex muzzle3 = MiscToolMethods.CreateVFXComplex("Paint Muzzleflash3",
                new List<string>()
                {
                    "Knives/Resources/Muzzleflashes/taggFlashes/Tagg_muzzleflash_007",
                    "Knives/Resources/Muzzleflashes/taggFlashes/Tagg_muzzleflash_008",
                    "Knives/Resources/Muzzleflashes/taggFlashes/Tagg_muzzleflash_009",
                },
                18, //FPS
                new IntVector2(27, 16), //Dimensions
                tk2dBaseSprite.Anchor.MiddleLeft, //Anchor
                false, //Uses a Z height off the ground
                0, //The Z height, if used
                false,
               VFXAlignment.Fixed
               );

            VFXComplex muzzle4 = MiscToolMethods.CreateVFXComplex("Paint Muzzleflash4",
                new List<string>()
                {
                    "Knives/Resources/Muzzleflashes/taggFlashes/Tagg_muzzleflash_010",
                    "Knives/Resources/Muzzleflashes/taggFlashes/Tagg_muzzleflash_011",
                    "Knives/Resources/Muzzleflashes/taggFlashes/Tagg_muzzleflash_012",
                },
                18, //FPS
                new IntVector2(27, 16), //Dimensions
                tk2dBaseSprite.Anchor.MiddleLeft, //Anchor
                false, //Uses a Z height off the ground
                0, //The Z height, if used
                false,
               VFXAlignment.Fixed
               );

            VFXComplex muzzle5 = MiscToolMethods.CreateVFXComplex("Paint Muzzleflash5",
               new List<string>()
               {
                    "Knives/Resources/Muzzleflashes/taggFlashes/Tagg_muzzleflash_013",
                    "Knives/Resources/Muzzleflashes/taggFlashes/Tagg_muzzleflash_014",
                    "Knives/Resources/Muzzleflashes/taggFlashes/Tagg_muzzleflash_015",
               },
               18, //FPS
               new IntVector2(27, 16), //Dimensions
               tk2dBaseSprite.Anchor.MiddleLeft, //Anchor
               false, //Uses a Z height off the ground
               0, //The Z height, if used
               false,
              VFXAlignment.Fixed
              );

            pool.effects = new VFXComplex[] 
            { 
                muzzle1, // blue
                muzzle2, // green
                muzzle3, // yellow/orange
                muzzle4, // red/pink
                muzzle5  // purple
                
            };

            gun.muzzleFlashEffects = pool;


            VFXPool pool2 = new VFXPool();
            pool2.type = VFXPoolType.RandomGroups;

            VFXComplex splat1 = MiscToolMethods.CreateVFXComplex("Paint splat1",
                new List<string>()
                {
                    "Knives/Resources/Splat/Tagg_Hsplat1",
                },
                18, //FPS
                new IntVector2(27, 16), //Dimensions
                tk2dBaseSprite.Anchor.MiddleCenter, //Anchor
                false, //Uses a Z height off the ground
                0, //The Z height, if used
                true,
               VFXAlignment.VelocityAligned
               );

            VFXComplex splat2 = MiscToolMethods.CreateVFXComplex("Paint splat2",
                new List<string>()
                {
                     "Knives/Resources/Splat/Tagg_Hsplat2",
                },
                18, //FPS
                new IntVector2(27, 16), //Dimensions
                tk2dBaseSprite.Anchor.MiddleCenter, //Anchor
                false, //Uses a Z height off the ground
                0, //The Z height, if used
                true,
               VFXAlignment.VelocityAligned
               );

            VFXComplex splat3 = MiscToolMethods.CreateVFXComplex("Paint splat3",
                new List<string>()
                {
                    "Knives/Resources/Splat/Tagg_Hsplat3",
                },
                18, //FPS
                new IntVector2(27, 16), //Dimensions
                tk2dBaseSprite.Anchor.MiddleCenter, //Anchor
                false, //Uses a Z height off the ground
                0, //The Z height, if used
                true,
               VFXAlignment.VelocityAligned
               );

            VFXComplex splat4 = MiscToolMethods.CreateVFXComplex("Paint splat4",
                new List<string>()
                {
                     "Knives/Resources/Splat/Tagg_Hsplat4",

                },
                18, //FPS
                new IntVector2(27, 16), //Dimensions
                tk2dBaseSprite.Anchor.MiddleCenter, //Anchor
                false, //Uses a Z height off the ground
                0, //The Z height, if used
                true,
               VFXAlignment.VelocityAligned
               );

            VFXComplex splat5 = MiscToolMethods.CreateVFXComplex("Paint splat5",
               new List<string>()
               {
                     "Knives/Resources/Splat/Tagg_Hsplat5",

               },
               18, //FPS
               new IntVector2(27, 16), //Dimensions
               tk2dBaseSprite.Anchor.MiddleCenter, //Anchor
               false, //Uses a Z height off the ground
               0, //The Z height, if used
               true,
              VFXAlignment.Fixed
              );

            pool2.effects = new VFXComplex[]
            {
                splat1,
                splat2,
                splat3,
                splat4,
                splat5
                
            };

            VFXPool pool3 = new VFXPool();
            pool3.type = VFXPoolType.RandomGroups;

            VFXComplex splatV1 = MiscToolMethods.CreateVFXComplex("Paint splatV1",
                new List<string>()
                {
                     "Knives/Resources/Splat/Tagg_Vsplat1",
                },
                18, //FPS
                new IntVector2(27, 16), //Dimensions
                tk2dBaseSprite.Anchor.MiddleCenter, //Anchor
                false, //Uses a Z height off the ground
                0, //The Z height, if used
                true,
               VFXAlignment.VelocityAligned
               );

            VFXComplex splatV2 = MiscToolMethods.CreateVFXComplex("Paint splat2",
                new List<string>()
                {
                   "Knives/Resources/Splat/Tagg_Vsplat2",
                },
                18, //FPS
                new IntVector2(27, 16), //Dimensions
                tk2dBaseSprite.Anchor.MiddleCenter, //Anchor
                false, //Uses a Z height off the ground
                0, //The Z height, if used
                true,
               VFXAlignment.VelocityAligned
               );

            VFXComplex splatV3 = MiscToolMethods.CreateVFXComplex("Paint splat3",
                new List<string>()
                {
                    "Knives/Resources/Splat/Tagg_Vsplat3",

                },
                18, //FPS
                new IntVector2(27, 16), //Dimensions
                tk2dBaseSprite.Anchor.MiddleCenter, //Anchor
                false, //Uses a Z height off the ground
                0, //The Z height, if used
                true,
               VFXAlignment.VelocityAligned
               );

            VFXComplex splatV4 = MiscToolMethods.CreateVFXComplex("Paint splat4",
                new List<string>()
                {
                    "Knives/Resources/Splat/Tagg_Vsplat4",

                },
                18, //FPS
                new IntVector2(27, 16), //Dimensions
                tk2dBaseSprite.Anchor.MiddleCenter, //Anchor
                false, //Uses a Z height off the ground
                0, //The Z height, if used
                true,
               VFXAlignment.VelocityAligned
               );

            VFXComplex splatV5 = MiscToolMethods.CreateVFXComplex("Paint splat5",
               new List<string>()
               {
                   "Knives/Resources/Splat/Tagg_Vsplat5",

               },
               18, //FPS
               new IntVector2(27, 16), //Dimensions
               tk2dBaseSprite.Anchor.MiddleCenter, //Anchor
               false, //Uses a Z height off the ground
               0, //The Z height, if used
               true,
              VFXAlignment.Fixed
              );

            pool3.effects = new VFXComplex[]
            {
                splatV1,
                splat2,
                splatV3,
                splatV4,
                splatV5

            };

            gun.muzzleFlashEffects = pool;

            projectile.hitEffects.tileMapVertical = pool2;
            projectile.hitEffects.deathTileMapVertical = pool2;

            projectile.hitEffects.tileMapHorizontal = pool3;
            projectile.hitEffects.deathTileMapHorizontal = pool3;



            projectile.transform.parent = gun.barrelOffset;
            gun.gunClass = GunClass.FULLAUTO;
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            gun.SetupUnlockOnCustomFlag(CustomDungeonFlags.SKATER_TAGGAR, true);


            ID = gun.PickupObjectId;

        }

        public static int ID;

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_WPN_earthwormgun_shot_01", base.gameObject);
        }
        public override void OnFinishAttack(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Stop_TaggAR_Fire_Background", base.gameObject);
            base.OnFinishAttack(player, gun);
        }
        public override void PostProcessProjectile(Projectile projectile)
        {

            base.PostProcessProjectile(projectile);
        }

        private bool HasReloaded;
        public bool Spray = false;
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

                if (gun.IsFiring)
                {
                    if(Spray == false)
                    {
                        AkSoundEngine.PostEvent("Play_TaggAR_Fire_Background", base.gameObject);
                        Spray = true;
                    }
                }
                else
                {
                    if (Spray == true)
                    {
                        AkSoundEngine.PostEvent("Stop_TaggAR_Fire_Background", base.gameObject);
                        Spray = false;
                    }
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
                AkSoundEngine.PostEvent("Play_TaggAR_reload_001", base.gameObject);
                AkSoundEngine.PostEvent("Play_WPN_m1911_reload_01", base.gameObject);
            }

        }

        public static int TaggARCount = 1;
    }
}
