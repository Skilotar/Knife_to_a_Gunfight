using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Brave;
using Dungeonator;
using Brave.BulletScript;
using Gungeon;
using HutongGames.PlayMaker.Actions;
using ItemAPI;
using UnityEngine;

namespace Knives
{
    class ShatterBow : AdvancedGunBehaviour
    {
        public static void Add()
        {
            // Get yourself a new gun "base" first.
            // Let's just call it "Basic Gun", and use "jpxfrd" for all sprites and as "codename" All sprites must begin with the same word as the codename. For example, your firing sprite would be named "jpxfrd_fire_001".
            Gun gun = ETGMod.Databases.Items.NewGun("Shatter bow", "ShatterBow");
            // "kp:basic_gun determines how you spawn in your gun through the console. You can change this command to whatever you want, as long as it follows the "name:itemname" template.
            Game.Items.Rename("outdated_gun_mods:shatter_bow", "ski:shatter_bow");
            gun.gameObject.AddComponent<ShatterBow>();
            //These two lines determines the description of your gun, ".SetShortDescription" being the description that appears when you pick up the gun and ".SetLongDescription" being the description in the Ammonomicon entry. 
            gun.SetShortDescription("ShatterShells");
            gun.SetLongDescription("Arrows Fire on collision.\n\n" +
                "A perfectly positioned shotgun shell at the tip of these arrows causes the arrow itself to act as a firing pin. This configuration can be applied to many types of shells." +
                "\n\n\n - Knife_to_a_Gunfight");
            // This is required, unless you want to use the sprites of the base gun.
            // That, by default, is the pea shooter.
            // SetupSprite sets up the default gun sprite for the ammonomicon and the "gun get" popup.
            // WARNING: Add a copy of your default sprite to Ammonomicon Encounter Icon Collection!
            // That means, "sprites/Ammonomicon Encounter Icon Collection/defaultsprite.png" in your mod .zip. You can see an example of this with inside the mod folder.
            gun.SetupSprite(null, "ShatterBow_idle_001", 8);

            // ETGMod automatically checks which animations are available.
            // The numbers next to "shootAnimation" determine the animation fps. You can also tweak the animation fps of the reload animation and idle animation using this method.
            gun.SetAnimationFPS(gun.shootAnimation, 15);
            gun.SetAnimationFPS(gun.reloadAnimation, 6);
            gun.SetAnimationFPS(gun.chargeAnimation, 10);

            // Every modded gun has base projectile it works with that is borrowed from other guns in the game. 
            // The gun names are the names from the JSON dump! While most are the same, some guns named completely different things. If you need help finding gun names, ask a modder on the Gungeon discord.
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(8) as Gun, true, false);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 4;

            // Here we just take the default projectile module and change its settings how we want it to be.
            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 0;
            gun.gunClass = GunClass.CHARGE;
            gun.gunHandedness = GunHandedness.TwoHanded;

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = .8f;

            gun.DefaultModule.cooldownTime = .5f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.SetBaseMaxAmmo(100);

            gun.quality = PickupObject.ItemQuality.B;
            gun.gunSwitchGroup = null;
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(8) as Gun).muzzleFlashEffects;
            
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.chargeProjectiles[0].Projectile);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 14f;
            projectile.baseData.speed = 45f;
            projectile.baseData.range = 40f;
            projectile.AdditionalScaleMultiplier *= 1.1f;
            projectile.gameObject.GetOrAddComponent<PierceProjModifier>().penetration = 1;
            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(((Gun)PickupObjectDatabase.GetById(32)).DefaultModule.projectiles[0].projectile);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            
            projectile2.baseData.damage = 2f;
            projectile2.baseData.speed = 50f;
            projectile2.baseData.range = 50f;
            projectile2.AdditionalScaleMultiplier *= .5f;
            projectile2.gameObject.GetOrAddComponent<PierceProjModifier>().penetration = 1;
            BounceProjModifier bnc = projectile2.gameObject.GetOrAddComponent<BounceProjModifier>();
            bnc.numberOfBounces = 1;
            ShatterArrow arrow = projectile.gameObject.GetOrAddComponent<ShatterArrow>();
            arrow.shatterto = projectile2;


            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            projectile.SetProjectileSpriteRight("ShatterArrow", 19, 5, false, tk2dBaseSprite.Anchor.LowerLeft, 19, 5);
            projectile.shouldFlipVertically = true;
            projectile.shouldRotate = true;
            projectile.pierceMinorBreakables = true;
            gun.DefaultModule.customAmmoType = CustomClipAmmoTypeToolbox.AddCustomAmmoType("ShatterArrow", "Knives/Resources/ShatterArrow_clipfull", "Knives/Resources/ShatterArrow_clipempty");
            gun.barrelOffset.localPosition = new Vector3(1.1875f, 0.8125f, 0f);
            gun.carryPixelOffset = new IntVector2(10, 0);
            ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
            {
                Projectile = projectile,
                ChargeTime = .7f,
            };
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                item,

            };

            VFXPool HorizVFXOBj = VFXToolbox.CreateVFXPool("Arrow Tilemap VFX Horiz",
            new List<string>()
            {
                "Knives/Resources/ShatterArrow/shatter_001",
                "Knives/Resources/ShatterArrow/shatter_002",
                "Knives/Resources/ShatterArrow/shatter_003",
                "Knives/Resources/ShatterArrow/shatter_004",
            },
            4,
            new IntVector2(9, 9),
            tk2dBaseSprite.Anchor.MiddleLeft,
            false,
            0,
            true);

            projectile.hitEffects.deathTileMapHorizontal = HorizVFXOBj;
            projectile.hitEffects.tileMapHorizontal = HorizVFXOBj;

            projectile.hitEffects.deathTileMapVertical = HorizVFXOBj;
            projectile.hitEffects.tileMapVertical = HorizVFXOBj;

            MiscToolMethods.TrimAllGunSprites(gun);
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            ID = gun.PickupObjectId;
        }

        public static int ID;

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            
            this.gun.PreventNormalFireAudio = true;
        }
        public System.Random rng = new System.Random();
        public override void PostProcessProjectile(Projectile projectile)
        {
            
            base.PostProcessProjectile(projectile);
        }
       

        private bool HasReloaded;
        public bool toggle = true;

        public override void Update()
        {
            if (gun.CurrentOwner != null)
            {

                if (!gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
                if(gun.GetChargeFraction() >= .1)
                {
                    if(once == false)
                    {
                        AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                        AkSoundEngine.PostEvent("Play_WPN_woodbow_charge_01", base.gameObject);
                        once = true;
                    }
                   
                }
                else
                {
                    once = false;
                }

            }

            base.Update();
        }
        public bool once = false;
        public override void OnReload(PlayerController player, Gun gun)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
            }
            base.OnReload(player, gun);
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


